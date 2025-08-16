using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using CCC.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace CCC.Runtime.SceneLoading
{
	/// <summary>
	/// A class that manages scene loading and unloading.
	/// </summary>
	public class SceneLoader : MonoBehaviour
	{
		#region Serialized Fields

		[field: SerializeField] public SceneEntry[] Scenes { get; private set; }

		[field: SerializeField] public UIGroup TransitionScreen { get; set; }

		[SerializeField] private float sceneSwitchFadeDuration = 2;
		[SerializeField] private float minLoadTime = 1.5f;

		#endregion

		#region Private Fields

		private readonly ActiveScenes _activeScenes = new();

		private readonly SceneEntry[] _singleScene = new SceneEntry[1];

		#endregion

		#region Properties

		public Action<float> EntryTransition { get; set; }
		public Action<float> ExitTransition { get; set; }

		#endregion

		#region Event Functions

		/// <summary>
		/// If the SceneLoader scene is the only loaded scene, loads all loadOnStart scenes.
		/// Otherwise, only adds the loaded scenes into the active scenes data structure.  
		/// </summary>
		private void Awake()
		{
			EntryTransition ??= t => TransitionScreen.Opacity = t;
			ExitTransition ??= t => TransitionScreen.Opacity = 1 - t;

			var blankSlate = true;
			foreach (var scene in Scenes)
			{
				if (!IsSceneLoaded(scene)) continue;
				_activeScenes.Add(scene);
				if (scene.type != SceneType.SceneLoader)
					blankSlate = false;
			}

			if (!blankSlate)
				return;
			foreach (var scene in Scenes)
			{
				if (scene.loadOnStart && scene.type != SceneType.SceneLoader)
					LoadScene(scene);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Unloads all but the SceneLoader scene and reloads all scenes marked to load on start.
		/// </summary>
		public UniTask Reset()
		{
			var scenesToUnload = _activeScenes[SceneMask.InverseMask(SceneType.SceneLoader)].ToList();
			var scenesToLoad = Scenes.Where(scene => scene.loadOnStart && !IsSceneLoaded(scene)).ToList();
			return SwitchScene(scenesToLoad, scenesToUnload);
		}

		/// <summary>
		/// Gets all active scenes of requested type.
		/// </summary>
		public IEnumerable<SceneEntry> GetActiveScenes(SceneType type)
		{
			return _activeScenes[type];
		}

		/// <summary>
		/// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
		/// </summary>
		/// <param name="sceneEntryIndex">The index of the SceneEntry in the SceneLoader.</param>
		/// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
		public UniTask SwitchScene(int sceneEntryIndex, IReadOnlyCollection<SceneEntry> specificScenesToUnload = null) =>
			SwitchScene(Scenes[sceneEntryIndex], specificScenesToUnload);

		/// <summary>
		/// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
		/// </summary>
		/// <param name="scene">Scene to load</param>
		/// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
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
		public async UniTask SwitchScene(IReadOnlyCollection<SceneEntry> newScenes,
			IReadOnlyCollection<SceneEntry> specificScenesToUnload = null)
		{
			await UniTaskUtils.InterpolateUnscaledTime(EntryTransition, sceneSwitchFadeDuration);
			UniTask minLoadTimer = UniTaskUtils.Delay(minLoadTime);

			if (specificScenesToUnload != null)
				await UnloadScenesAsync(specificScenesToUnload);
			else
				await UnloadScenesAsync(SceneType.Dynamic);

			if (_activeScenes[SceneType.ConstantReload].Any())
				await ReloadScenesAsync();

			await UniTask.WhenAll(LoadScenesAsync(newScenes), minLoadTimer);
			await UniTaskUtils.InterpolateUnscaledTime(ExitTransition, sceneSwitchFadeDuration);
		}

		/// <summary>
		/// Loads the given scene additively.
		/// </summary>
		/// <param name="sceneEntryIndex">The index of the SceneEntry in the SceneLoader.</param>
		public void LoadScene(int sceneEntryIndex) => LoadScene(Scenes[sceneEntryIndex]);

		/// <summary>
		/// Loads the given scene additively.
		/// </summary>
		public void LoadScene(SceneEntry scene)
		{
			SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
			_activeScenes.Add(scene);
		}

		public void UnloadScene(int sceneEntryIndex) => UnloadScene(Scenes[sceneEntryIndex]);

		/// <summary>
		/// Unloads the given scene.
		/// </summary>
		public void UnloadScene(SceneEntry scene)
		{
			SceneManager.UnloadSceneAsync(scene.sceneName);
			_activeScenes.Remove(scene);
		}

		/// <summary>
		/// Unloads the given scenes.
		/// </summary>
		public void UnloadScenes(IEnumerable<SceneEntry> scenes)
		{
			foreach (var scene in scenes)
			{
				SceneManager.UnloadSceneAsync(scene.sceneName);
				_activeScenes.Remove(scene);
			}
		}

		/// <summary>
		/// Unloads all active dynamic scenes.
		/// </summary>
		public void UnloadDynamicScenes()
		{
			UnloadScenesAsync(SceneType.Dynamic).Forget();
		}

		#endregion

		#region Private Methods

		private bool IsSceneLoaded(SceneEntry scene)
		{
			var numOfLoadedScenes = SceneManager.sceneCount;
			for (int i = 0; i < numOfLoadedScenes; ++i)
			{
				if (SceneManager.GetSceneAt(i).name == scene.sceneName)
					return true;
			}

			return false;
		}

		private async UniTask ReloadScenesAsync()
		{
			var constantReloadScenes = _activeScenes[SceneType.ConstantReload];
			int constantReloadCount = constantReloadScenes.Count;
			var tasks = new UniTask[constantReloadCount];

			int i = 0;
			foreach (var reloadScene in constantReloadScenes)
			{
				tasks[i++] = SceneManager.UnloadSceneAsync(reloadScene.sceneName).ToUniTask();
			}

			await UniTask.WhenAll(tasks);

			i = 0;
			foreach (var reloadScene in constantReloadScenes)
			{
				tasks[i++] = SceneManager.LoadSceneAsync(reloadScene.sceneName, LoadSceneMode.Additive).ToUniTask();
			}

			await UniTask.WhenAll(tasks);
		}

		private async UniTask UnloadScenesAsync(IReadOnlyCollection<SceneEntry> scenes)
		{
			int sceneCount = scenes.Count;
			var tasks = new UniTask[sceneCount];

			int i = 0;
			foreach (var scene in scenes)
			{
				tasks[i++] = SceneManager.UnloadSceneAsync(scene.sceneName)
					.ToUniTask()
					.ContinueWith(() => _activeScenes.Remove(scene));
			}

			await UniTask.WhenAll(tasks);
		}

		private UniTask UnloadScenesAsync(SceneType type) => UnloadScenesAsync(_activeScenes[type]);

		private async UniTask LoadScenesAsync(IReadOnlyCollection<SceneEntry> scenes)
		{
			int sceneCount = scenes.Count;
			var tasks = new UniTask[sceneCount];

			int i = 0;
			foreach (var scene in scenes)
			{
				tasks[i++] = SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive)
					.ToUniTask()
					.ContinueWith(() => _activeScenes.Add(scene));
			}

			await UniTask.WhenAll(tasks);
		}

		#endregion

		#region Inner Types

		private class ActiveScenes
		{
			#region Private Fields

			public readonly HashSet<SceneEntry>[] _scenes;

			#endregion

			#region Properties

			/// <summary>
			/// Get a HashSet of all active scenes by type.
			/// </summary>
			internal HashSet<SceneEntry> this[SceneType type] => _scenes[(int)type];

			/// <summary>
			/// Get an enumerable of all actives scenes of the given types. 
			/// </summary>
			internal IEnumerable<SceneEntry> this[SceneMask types] => new EnumerableScenes(types, this);

			#endregion

			#region Conostructors

			internal ActiveScenes()
			{
				var typeCount = EnumUtils.Count<SceneType>();
				_scenes = new HashSet<SceneEntry>[typeCount];
				for (int i = 0; i < typeCount; ++i)
					_scenes[i] = new();
			}

			#endregion

			#region Internal Methods

			internal void Add(SceneEntry scene) => this[scene.type].Add(scene);

			internal void Append(IEnumerable<SceneEntry> scenes)
			{
				foreach (var scene in scenes)
					this[scene.type].Add(scene);
			}

			internal void Remove(SceneEntry scene) => this[scene.type].Remove(scene);

			#endregion

			#region Types

			private class EnumerableScenes : IEnumerable<SceneEntry>
			{
				private readonly SceneMask _types;
				private readonly ActiveScenes _activeScenes;

				internal EnumerableScenes(SceneMask types, ActiveScenes activeScenes)
				{
					_types = types;
					_activeScenes = activeScenes;
				}

				public IEnumerator<SceneEntry> GetEnumerator()
				{
					foreach (var type in _types)
						foreach (var entry in _activeScenes[type])
							yield return entry;
				}

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
			}

			#endregion
		}
		#endregion
	}
}