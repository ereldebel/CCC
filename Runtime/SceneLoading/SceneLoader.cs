using System;
using System.Collections.Generic;
using System.Linq;
using CCC.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace CCC.Runtime.SceneLoading
{
    /// <summary>
    /// A class that manages scene loading and unloading with support for different scene types,
    /// transition effects, and additive scene loading. Provides centralized scene management
    /// with automatic scene type handling and transition animations.
    /// </summary>
    public class SceneLoader : IInitializable
    {
        #region Private Fields

        private readonly SceneConfigurations _config;
        private readonly LifetimeScope _scope;
        private readonly UIGroup _transitionScreen;
        private readonly ActiveScenes _activeScenes = new();
        private readonly SceneEntry[] _singleScene = new SceneEntry[1];

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the action to perform during entry transitions (fade in).
        /// </summary>
        /// <value>Action that takes a float parameter (0-1) for transition progress.</value>
        public Action<float> EntryTransition { get; set; }

        /// <summary>
        /// Gets or sets the action to perform during exit transitions (fade out).
        /// </summary>
        /// <value>Action that takes a float parameter (0-1) for transition progress.</value>
        public Action<float> ExitTransition { get; set; }

        #endregion

        #region Constructor & Initialization

        [Inject]
        public SceneLoader(SceneConfigurations config, LifetimeScope scope, UIGroup transitionScreen)
        {
            _config = config;
            _scope = scope;
            _transitionScreen = transitionScreen;
        }

        /// <summary>
        /// If the SceneLoader scene is the only loaded scene, loads all loadOnStart scenes.
        /// Otherwise, only adds the loaded scenes into the active scenes data structure.  
        /// </summary>
        public void Initialize()
        {
            EntryTransition ??= t => _transitionScreen.Opacity = t;
            ExitTransition ??= t => _transitionScreen.Opacity = 1 - t;

            bool blankSlate = true;
            foreach (SceneEntry scene in _config.Scenes)
            {
                if (!IsSceneLoaded(scene))
                {
                    continue;
                }

                _activeScenes.Add(scene);

                if (scene.type != SceneType.SceneLoader)
                {
                    blankSlate = false;
                }
            }

            if (!blankSlate)
            {
                return;
            }

            foreach (SceneEntry scene in _config.Scenes)
            {
                if (scene.loadOnStart && scene.type != SceneType.SceneLoader)
                {
                    LoadScene(scene).Forget();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Unloads all but the SceneLoader scene and reloads all scenes marked to load on start.
        /// </summary>
        /// <returns>A UniTask representing the asynchronous reset operation.</returns>
        public UniTask ResetScenes()
        {
            List<SceneEntry> scenesToUnload = _activeScenes[SceneMask.InverseMask(SceneType.SceneLoader)].ToList();
            List<SceneEntry> scenesToLoad =
                _config.Scenes.Where(scene => scene.loadOnStart && !IsSceneLoaded(scene)).ToList();
            return SwitchScene(scenesToLoad, scenesToUnload);
        }

        /// <summary>
        /// Gets all active scenes of requested type.
        /// </summary>
        /// <param name="type">The scene type to filter by.</param>
        /// <returns>An enumerable of active scenes of the specified type.</returns>
        public IEnumerable<SceneEntry> GetActiveScenes(SceneType type)
        {
            return _activeScenes[type];
        }

        /// <summary>
        /// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
        /// </summary>
        /// <param name="sceneEntryIndex">The index of the SceneEntry in the SceneLoader.</param>
        /// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
        /// <returns>A UniTask representing the asynchronous scene switch operation.</returns>
        public UniTask SwitchScene(int sceneEntryIndex,
            IReadOnlyCollection<SceneEntry> specificScenesToUnload = null) =>
            SwitchScene(_config.Scenes[sceneEntryIndex], specificScenesToUnload);

        /// <summary>
        /// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
        /// </summary>
        /// <param name="scene">Scene to load</param>
        /// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
        /// <returns>A UniTask representing the asynchronous scene switch operation.</returns>
        public UniTask SwitchScene(SceneEntry scene, IReadOnlyCollection<SceneEntry> specificScenesToUnload = null)
        {
            _singleScene[0] = scene;
            return SwitchScene(_singleScene, specificScenesToUnload);
        }

        /// <summary>
        /// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
        /// </summary>
        /// <param name="newScenes">Scenes to load</param>
        /// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
        /// <returns>A UniTask representing the asynchronous scene switch operation.</returns>
        public async UniTask SwitchScene(IReadOnlyCollection<SceneEntry> newScenes,
            IReadOnlyCollection<SceneEntry> specificScenesToUnload = null)
        {
            await UniTaskUtils.Interpolate(EntryTransition, _config.SceneSwitchFadeDuration, ignoreTimeScale: true);
            UniTask minLoadTimer = UniTaskUtils.Delay(_config.MinLoadTime);

            if (specificScenesToUnload != null)
            {
                await UnloadScenesAsync(specificScenesToUnload);
            }
            else
            {
                await UnloadScenesAsync(SceneType.Dynamic);
            }

            if (_activeScenes[SceneType.ConstantReload].Any())
            {
                await ReloadScenesAsync();
            }

            await UniTask.WhenAll(LoadScenesAsync(newScenes), minLoadTimer);
            await UniTaskUtils.Interpolate(ExitTransition, _config.SceneSwitchFadeDuration, ignoreTimeScale: true);
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <param name="sceneEntryIndex">The index of the SceneEntry in the SceneLoader.</param>
        public UniTask LoadScene(int sceneEntryIndex) => LoadScene(_config.Scenes[sceneEntryIndex]);

        /// <summary>
        /// Unloads the given scene by index.
        /// </summary>
        /// <param name="sceneEntryIndex">The index of the SceneEntry in the SceneLoader.</param>
        public UniTask UnloadScene(int sceneEntryIndex) => UnloadScene(_config.Scenes[sceneEntryIndex]);

        /// <summary>
        /// Unloads the given scene.
        /// </summary>
        /// <param name="scene">The scene entry to unload.</param>
        public UniTask UnloadScene(SceneEntry scene)
        {
            _activeScenes.Remove(scene);
            return SceneManager.UnloadSceneAsync(scene.sceneName).ToUniTask();
        }

        /// <summary>
        /// Unloads the given scenes.
        /// </summary>
        /// <param name="scenes">The collection of scene entries to unload.</param>
        public UniTask UnloadScenes(IEnumerable<SceneEntry> scenes)
        {
            UniTask[] sceneUnloadTasks = new UniTask[scenes.Count()];

            int i = 0;
            foreach (SceneEntry scene in scenes)
            {
                sceneUnloadTasks[i++] = SceneManager.UnloadSceneAsync(scene.sceneName).ToUniTask();
                _activeScenes.Remove(scene);
            }

            return UniTask.WhenAll(sceneUnloadTasks);
        }

        /// <summary>
        /// Unloads all active dynamic scenes asynchronously.
        /// </summary>
        public UniTask UnloadDynamicScenes()
        {
            return UnloadScenesAsync(SceneType.Dynamic);
        }

        /// <summary>
        /// Unloads the given scene by SceneEntry.
        /// </summary>
        public async UniTask LoadScene(SceneEntry scene)
        {
            try
            {
                using (LifetimeScope.EnqueueParent(_scope))
                {
                    await SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to inject scene. Resolving to load scene without injection: " + e.Message);
                await SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
            }

            _activeScenes.Add(scene);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if a scene is currently loaded in the SceneManager.
        /// </summary>
        /// <param name="scene">The scene entry to check.</param>
        /// <returns>True if the scene is loaded, false otherwise.</returns>
        private bool IsSceneLoaded(SceneEntry scene)
        {
            int numOfLoadedScenes = SceneManager.sceneCount;
            for (int i = 0; i < numOfLoadedScenes; ++i)
            {
                if (SceneManager.GetSceneAt(i).name == scene.sceneName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reloads all constant reload scenes asynchronously.
        /// </summary>
        /// <returns>A UniTask representing the asynchronous reload operation.</returns>
        private async UniTask ReloadScenesAsync()
        {
            HashSet<SceneEntry> constantReloadScenes = _activeScenes[SceneType.ConstantReload];
            int constantReloadCount = constantReloadScenes.Count;
            var tasks = new UniTask[constantReloadCount];

            int i = 0;
            foreach (SceneEntry reloadScene in constantReloadScenes)
            {
                tasks[i++] = SceneManager.UnloadSceneAsync(reloadScene.sceneName).ToUniTask();
            }

            await UniTask.WhenAll(tasks);

            i = 0;
            foreach (SceneEntry reloadScene in constantReloadScenes)
            {
                tasks[i++] = LoadScene(reloadScene);
            }

            await UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// Unloads the specified scenes asynchronously.
        /// </summary>
        /// <param name="scenes">The collection of scenes to unload.</param>
        /// <returns>A UniTask representing the asynchronous unload operation.</returns>
        private async UniTask UnloadScenesAsync(IReadOnlyCollection<SceneEntry> scenes)
        {
            int sceneCount = scenes.Count;
            var tasks = new UniTask[sceneCount];

            int i = 0;
            foreach (SceneEntry scene in scenes)
            {
                tasks[i++] = SceneManager.UnloadSceneAsync(scene.sceneName)
                    .ToUniTask()
                    .ContinueWith(() => _activeScenes.Remove(scene));
            }

            await UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// Unloads all scenes of the specified type asynchronously.
        /// </summary>
        /// <param name="type">The scene type to unload.</param>
        /// <returns>A UniTask representing the asynchronous unload operation.</returns>
        private UniTask UnloadScenesAsync(SceneType type) => UnloadScenesAsync(_activeScenes[type]);

        /// <summary>
        /// Loads the specified scenes asynchronously.
        /// </summary>
        /// <param name="scenes">The collection of scenes to load.</param>
        /// <returns>A UniTask representing the asynchronous load operation.</returns>
        private async UniTask LoadScenesAsync(IReadOnlyCollection<SceneEntry> scenes)
        {
            int sceneCount = scenes.Count;
            var tasks = new UniTask[sceneCount];

            int i = 0;
            foreach (SceneEntry scene in scenes)
            {
                tasks[i++] = LoadScene(scene);
            }

            await UniTask.WhenAll(tasks);
        }

        #endregion
    }
}
