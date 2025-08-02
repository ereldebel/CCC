namespace CCC.Runtime.SceneLoading
{
	/// <summary>
	/// A type of scene.
	/// </summary>
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
}