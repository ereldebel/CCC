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

		[field: SerializeField] public SceneEntry[] Scenes { get; private set; }

		[SerializeField] private float sceneSwitchFadeDuration = 2;
		[SerializeField] private CanvasGroup transitionScreen;

		#endregion

		#region Private Fields

		private readonly ActiveScenes _activeScenes = new();

		private readonly SceneEntry[] _singleScene = new SceneEntry[1];

		#endregion

		#region Event Functions

		/// <summary>
		/// If the SceneLoader scene is the only loaded scene, loads all loadOnStart scenes.
		/// Otherwise, only adds the loaded scenes into the active scenes data structure.  
		/// </summary>
		private void Awake()
		{
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
		public void Reset()
		{
			var scenesToUnload = _activeScenes[SceneMask.InverseMask(SceneType.ConstantReload)].ToList();
			var scenesToLoad = Scenes.Where(scene => scene.loadOnStart && !IsSceneLoaded(scene)).ToList();
			SwitchScene(scenesToLoad, scenesToUnload);
		}

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
		/// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
		/// <param name="switchEndAction">Optional: an action to perform once the Switch ends.</param>
		public void SwitchScene(int sceneEntryIndex, IReadOnlyCollection<SceneEntry> specificScenesToUnload = null,
			Action switchEndAction = null) =>
			SwitchScene(Scenes[sceneEntryIndex], specificScenesToUnload, switchEndAction);

		/// <summary>
		/// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
		/// </summary>
		/// <param name="scene">Scene to load</param>
		/// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
		/// <param name="switchEndAction">Optional: an action to perform once the Switch ends.</param>
		public void SwitchScene(SceneEntry scene, IReadOnlyCollection<SceneEntry> specificScenesToUnload = null,
			Action switchEndAction = null)
		{
			_singleScene[0] = scene;
			SwitchScene(_singleScene, specificScenesToUnload, switchEndAction);
		}

		/// <summary>
		/// Unloads all dynamic scenes, reloads ConstantReload scenes and loads the given scene.
		/// </summary>
		/// <param name="scenes">Scenes to load</param>
		/// <param name="specificScenesToUnload">Optional: specific scenes to unload. This is instead of the dynamic scenes unload</param>
		/// <param name="switchEndAction">Optional: an action to perform once the Switch ends.</param>
		public void SwitchScene(IReadOnlyCollection<SceneEntry> scenes,
			IReadOnlyCollection<SceneEntry> specificScenesToUnload = null,
			Action switchEndAction = null)
		{
			StartCoroutine(SwitchSceneCoroutine(scenes, specificScenesToUnload, switchEndAction));
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
			var sceneEntries = _activeScenes[SceneType.Dynamic];
			foreach (var scene in sceneEntries)
				SceneManager.UnloadSceneAsync(scene.sceneName);

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

		private IEnumerator SwitchSceneCoroutine(IReadOnlyCollection<SceneEntry> newScenes,
			IReadOnlyCollection<SceneEntry> specificScenesToUnload = null,
			Action switchEndAction = null)
		{
			yield return Coroutines.InterpolateUnscaledTime(t => transitionScreen.alpha = t, sceneSwitchFadeDuration);
			if (specificScenesToUnload != null)
				yield return UnloadScenes(specificScenesToUnload);
			else
				yield return UnloadScenes(SceneType.Dynamic);
			if (_activeScenes[SceneType.ConstantReload].Count > 0)
				yield return ReloadScenes();
			yield return LoadScenes(newScenes);
			yield return Coroutines.InterpolateUnscaledTime(t => transitionScreen.alpha = 1 - t, sceneSwitchFadeDuration);
			switchEndAction?.Invoke();
		}

		private IEnumerator ReloadScenes()
		{
			var constantReloadScenes = _activeScenes[SceneType.ConstantReload];
			int constantReloadCount = constantReloadScenes.Count;
			var ops = new AsyncOperation[constantReloadCount];
			int i = 0;
			foreach (var reloadScene in constantReloadScenes)
			{
				ops[i++] = SceneManager.UnloadSceneAsync(reloadScene.sceneName);
			}

			yield return new WaitUntil(() => ops.All(op => op.isDone));

			i = 0;
			foreach (var reloadScene in constantReloadScenes)
			{
				ops[i++] = SceneManager.LoadSceneAsync(reloadScene.sceneName, LoadSceneMode.Additive);
			}

			yield return new WaitUntil(() => ops.All(op => op.isDone));
		}

		private IEnumerator UnloadScenes(IReadOnlyCollection<SceneEntry> scenes)
		{
			int sceneCount = scenes.Count;
			var ops = new AsyncOperation[sceneCount];
			int i = 0;
			foreach (var scene in scenes)
			{
				ops[i++] = SceneManager.UnloadSceneAsync(scene.sceneName);
				_activeScenes.Remove(scene);
			}

			yield return new WaitUntil(() => ops.All(op => op.isDone));
		}

		private IEnumerator UnloadScenes(SceneType type)
		{
			var scenes = _activeScenes[type];
			int sceneCount = scenes.Count;
			var ops = new AsyncOperation[sceneCount];
			int i = 0;
			foreach (var scene in scenes)
				ops[i++] = SceneManager.UnloadSceneAsync(scene.sceneName);
			_activeScenes[type].Clear();
			yield return new WaitUntil(() => ops.All(op => op.isDone));
		}

		private IEnumerator LoadScenes(IReadOnlyCollection<SceneEntry> scenes)
		{
			int sceneCount = scenes.Count;
			var ops = new AsyncOperation[sceneCount];
			int i = 0;
			foreach (var scene in scenes)
			{
				ops[i++] = SceneManager.LoadSceneAsync(scene.sceneName, LoadSceneMode.Additive);
				_activeScenes.Add(scene);
			}

			yield return new WaitUntil(() => ops.All(op => op.isDone));
		}

		#endregion

		#region Inner Types

		private class ActiveScenes
		{
			#region Private Fields

			private readonly HashSet<SceneEntry>[] _scenes;

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

		public enum SceneType
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

		public readonly struct SceneMask
		{
			#region Private Fields

			private readonly int _mask;

			#endregion

			#region Constructors

			public SceneMask(int mask) => _mask = mask;

			public SceneMask(params SceneType[] types) =>
				_mask = types.Aggregate(0, (mask, type) => mask | (int)type);

			public static SceneMask InverseMask(SceneType type) =>
				new(~type);

			public static SceneMask InverseMask(params SceneType[] types) =>
				new(~types.Aggregate(0, (mask, type) => mask | (int)type));

			#endregion

			#region Operator Overloads

			public static explicit operator SceneMask(int mask) => new(mask);

			public static explicit operator SceneMask(SceneType type) => new(1 << (int)type);

			public static SceneMask operator &(SceneMask lhs, SceneMask rhs) => new(lhs._mask & rhs._mask);

			#endregion

			#region Public Methods

			public IEnumerator<SceneType> GetEnumerator()
			{
				const int bit = 1;
				int typeMask = _mask;
				for (int type = 0; type < EnumUtils.Count<SceneType>(); ++type, typeMask >>= 1)
					if ((typeMask & bit) != 0)
						yield return (SceneType)type;
			}

			#endregion
		}

		#endregion
	}
}