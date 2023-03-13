using System;
using System.Collections;
using System.Collections.Generic;
using CCC.Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CCC.Runtime
{
    public class SceneLoader : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private SceneEntry[] scenes;
        [SerializeField] private float sceneSwitchFadeDuration = 2;
        [SerializeField] private CanvasGroup transitionScreen;

        #endregion

        #region Non-Serialized Fields

        private readonly ActiveScenes _activeScenes = new();

        #endregion

        #region Event Functions

        private void Awake()
        {
            foreach (var scene in scenes)
            {
                if (scene.loadOnStart)
                    LoadScene(scene);
            }
        }

        #endregion

        #region Public Methods

        public void SwitchScene(int sceneEntryIndex) =>
            SwitchScene(scenes[sceneEntryIndex]);

        public void SwitchScene(SceneEntry scene, SceneEntry specificSceneToUnload = null)
        {
            StartCoroutine(SwitchSceneCoroutine(scene, specificSceneToUnload));
        }

        public void LoadScene(int sceneEntryIndex) => LoadScene(scenes[sceneEntryIndex]);

        public void LoadScene(SceneEntry scene)
        {
            SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
            _activeScenes[scene.type].Add(scene);
        }

        public void UnloadScene(int sceneEntryIndex) => UnloadScene(scenes[sceneEntryIndex]);

        public void UnloadScene(SceneEntry scene)
        {
            SceneManager.UnloadSceneAsync(scene.sceneName);
            _activeScenes[scene.type].Remove(scene);
        }

        public void UnloadDynamicScenes()
        {
            var sceneEntries = _activeScenes[SceneType.Dynamic];
            foreach (var scene in sceneEntries)
            {
                SceneManager.UnloadSceneAsync(scene.sceneName);
            }

            sceneEntries.Clear();
        }

        #endregion

        #region Private Methods
        
        private IEnumerator SwitchSceneCoroutine(SceneEntry newScene, SceneEntry specificSceneToUnload = null)
        {
            yield return Coroutines.Interpolate(t => transitionScreen.alpha = t, sceneSwitchFadeDuration);
            if (specificSceneToUnload == null)
                UnloadDynamicScenes();
            else
                UnloadScene(newScene);
            var op = SceneManager.LoadSceneAsync(newScene.sceneName, LoadSceneMode.Additive);
            while (!op.isDone)
                yield return null;
            yield return Coroutines.Interpolate(t => transitionScreen.alpha = 1-t, sceneSwitchFadeDuration);
            _activeScenes[newScene.type].Add(newScene);
        }

        #endregion

        #region Inner Types

        private class ActiveScenes
        {
            private readonly HashSet<SceneEntry>[] _scenes;

            public HashSet<SceneEntry> this[SceneType type] => _scenes[(ushort)type];

            internal ActiveScenes()
            {
                var typeCount = EnumUtils.Count<SceneType>();
                _scenes = new HashSet<SceneEntry>[typeCount];
                for (int i = 0; i < typeCount; ++i)
                    _scenes[i] = new();
            }
        }

        [Serializable]
        public class SceneEntry
        {
            [SerializeField] public string sceneName;
            [SerializeField] public SceneType type;
            [SerializeField] public bool loadOnStart;
        }

        public enum SceneType : ushort
        {
            /// <summary>
            /// The SceneLoader's scene
            /// </summary>
            SceneLoader = 0,

            /// <summary>
            /// A scene that stays loaded unless unloaded explicitly
            /// </summary>
            Constant = 1,

            /// <summary>
            /// A scene that is only temporarily loaded
            /// </summary>
            Dynamic = 2,
        }

        #endregion
    }
}