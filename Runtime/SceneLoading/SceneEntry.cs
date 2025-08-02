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
}