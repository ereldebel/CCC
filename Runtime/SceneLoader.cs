using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
				if (IsSceneLoaded(scene))
				{
					_activeScenes.Add(scene);
				}
				else if (scene.loadOnStart)
				{
					LoadScene(scene);
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets all active scenes of requested type.
		/// </summary>
		public IReadOnlyCollection<SceneEntry> GetActiveScenes(SceneType type)
		{
			return _activeScenes[type];
		}

		/// <summary>
		/// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
		/// </summary>
		/// <param name="sceneEntryIndex">The index of the SceneEntry in the SceneLoader.</param>
		/// <param name="specificSceneToUnload">Optional: a non-dynamic scene to unload.</param>
		/// <param name="switchEndAction">Optional: an action to perform once the Switch ends.</param>
		public void SwitchScene(int sceneEntryIndex, SceneEntry specificSceneToUnload = null,
			Action switchEndAction = null) =>
			SwitchScene(scenes[sceneEntryIndex], specificSceneToUnload, switchEndAction);

		/// <summary>
		/// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
		/// </summary>
		/// <param name="scene">Scene to load</param>
		/// <param name="specificSceneToUnload">Optional: a non-dynamic scene to unload.</param>
		/// <param name="switchEndAction">Optional: an action to perform once the Switch ends.</param>
		public void SwitchScene(SceneEntry scene, SceneEntry specificSceneToUnload = null,
			Action switchEndAction = null)
		{
			StartCoroutine(SwitchSceneCoroutine(scene, specificSceneToUnload, switchEndAction));
		}

		/// <summary>
		/// Loads the given scene additively.
		/// </summary>
		/// <param name="sceneEntryIndex">The index of the SceneEntry in the SceneLoader.</param>
		public void LoadScene(int sceneEntryIndex) => LoadScene(scenes[sceneEntryIndex]);

		/// <summary>
		/// Loads the given scene additively.
		/// </summary>
		public void LoadScene(SceneEntry scene)
		{
			SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
			_activeScenes.Add(scene);
		}

		public void UnloadScene(int sceneEntryIndex) => UnloadScene(scenes[sceneEntryIndex]);

		/// <summary>
		/// Unloads the given scene.
		/// </summary>
		public void UnloadScene(SceneEntry scene)
		{
			SceneManager.UnloadSceneAsync(scene.sceneName);
			_activeScenes.Remove(scene);
		}

		/// <summary>
		/// Unloads active all dynamic scenes.
		/// </summary>
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

		private IEnumerator SwitchSceneCoroutine(SceneEntry newScene, SceneEntry specificSceneToUnload = null,
			Action switchEndAction = null)
		{
			yield return Coroutines.Interpolate(t => transitionScreen.alpha = t, sceneSwitchFadeDuration);
			if (specificSceneToUnload == null)
				UnloadDynamicScenes();
			else
				UnloadScene(newScene);
			if (_activeScenes[SceneType.ConstantReload].Count > 0)
				yield return ReloadScenes();
			yield return PerformSingleAsyncOperation(SceneManager.LoadSceneAsync(newScene.sceneName,
				LoadSceneMode.Additive));
			yield return Coroutines.Interpolate(t => transitionScreen.alpha = 1 - t, sceneSwitchFadeDuration);
			_activeScenes.Add(newScene);
			switchEndAction?.Invoke();
		}

		private IEnumerator ReloadScenes()
		{
			int constantReloadCount = _activeScenes[SceneType.ConstantReload].Count;
			var ops = new AsyncOperation[constantReloadCount];
			int i = 0;
			foreach (var reloadScene in _activeScenes[SceneType.ConstantReload])
			{
				ops[i++] = SceneManager.UnloadSceneAsync(reloadScene.sceneName);
			}

			yield return new WaitUntil(() => ops.All(op => op.isDone));

			i = 0;
			foreach (var reloadScene in _activeScenes[SceneType.ConstantReload])
			{
				ops[i++] = SceneManager.LoadSceneAsync(reloadScene.sceneName, LoadSceneMode.Additive);
			}

			yield return new WaitUntil(() => ops.All(op => op.isDone));
		}

		private static IEnumerator PerformSingleAsyncOperation(AsyncOperation op)
		{
			while (!op.isDone)
				yield return null;
		}

		#endregion

		#region Inner Types

		private class ActiveScenes
		{
			private readonly HashSet<SceneEntry>[] _scenes;

			/// <summary>
			/// Get a HashSet of all active scenes by type;
			/// </summary>
			public HashSet<SceneEntry> this[SceneType type] => _scenes[(ushort)type];

			internal void Add(SceneEntry scene) => this[scene.type].Add(scene);
			internal void Remove(SceneEntry scene) => this[scene.type].Remove(scene);

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
			/// <summary>
			/// The name of the scene.
			/// </summary>
			[SerializeField] public string sceneName;

			/// <summary>
			/// The type of the scene.
			/// </summary>
			[SerializeField] public SceneType type;

			/// <summary>
			/// Should this scene be loaded on SceneLoader's Awake.
			/// </summary>
			[SerializeField] public bool loadOnStart;
		}

		public enum SceneType : ushort
		{
			/// <summary>
			/// The SceneLoader's scene.
			/// </summary>
			SceneLoader = 0,

			/// <summary>
			/// A scene that stays loaded unless unloaded explicitly.
			/// </summary>
			Constant = 1,

			/// <summary>
			/// A scene that is only temporarily loaded.
			/// </summary>
			Dynamic = 2,

			/// <summary>
			/// A scene that is reloaded on each scene switch.
			/// </summary>
			ConstantReload = 3,
		}

		#endregion
	}
}