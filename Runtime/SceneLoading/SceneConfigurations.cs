using UnityEngine;

namespace CCC.Runtime.SceneLoading
{
	/// <summary>
	/// Configuration asset for SceneLoader settings and scene definitions.
	/// </summary>
	[CreateAssetMenu(fileName = "SceneConfigurations", menuName = "CCC/Scene Configurations")]
	public class SceneConfigurations : ScriptableObject
	{
		[field: SerializeField, Tooltip("Array of scene entries defining all available scenes and their properties")]
		public SceneEntry[] Scenes { get; private set; }

		[field: SerializeField, Tooltip("Duration of fade transitions in seconds")]
		public float SceneSwitchFadeDuration { get; private set; } = 0.5f;

		[field: SerializeField, Tooltip("Minimum time to show loading screen in seconds")]
		public float MinLoadTime { get; private set; } = 1f;
	}
}
