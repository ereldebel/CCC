using System;
using UnityEngine;

namespace CCC.Runtime.SceneLoading
{
	/// <summary>
	/// A <see cref="SceneLoader"/> scene entry.
	/// </summary>
	[Serializable]
	public class SceneEntry
	{
		/// <summary>
		/// The name of the scene.
		/// </summary>
		[SerializeField, Tooltip("The name of the scene file (without .unity extension)")]
		public string sceneName;

		/// <summary>
		/// The type of the scene.
		/// </summary>
		[SerializeField, Tooltip("The behavior type of this scene (how it should be managed)")]
		public SceneType type;

		/// <summary>
		/// Should this scene be loaded on SceneLoader's Awake.
		/// </summary>
		[SerializeField, Tooltip("Whether this scene should be loaded automatically when the SceneLoader starts")]
		public bool loadOnStart;
	}
}