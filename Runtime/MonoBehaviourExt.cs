using UnityEngine;

namespace CCC.Runtime
{
	/// <summary>
	/// An extension of MonoBehaviour that provides cached access to commonly used components
	/// (Transform and GameObject) with lazy initialization for improved performance.
	/// </summary>
	public class MonoBehaviourExt : MonoBehaviour
	{
		#region Private Fields

		private Transform _transform;
		private GameObject _gameObject;
		private bool _transformSet;
		private bool _gameObjectSet;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the cached Transform component with lazy initialization.
		/// </summary>
		/// <value>The Transform component of this GameObject.</value>
		public new Transform transform
		{
			get
			{
				if (!_transformSet)
				{
					_transform = base.transform;
					_transformSet = true;
				}

				return _transform;
			}
		}


		/// <summary>
		/// Gets the cached GameObject with lazy initialization.
		/// </summary>
		/// <value>The GameObject this component is attached to.</value>
		public new GameObject gameObject
		{
			get
			{
				if (!_gameObjectSet)
				{
					_gameObject = base.gameObject;
					_gameObjectSet = true;
				}

				return _gameObject;
			}
		}

		#endregion
	}
}