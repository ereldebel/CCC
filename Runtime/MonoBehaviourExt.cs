using UnityEngine;

namespace CCC.Runtime
{
	public class MonoBehaviourExt : MonoBehaviour
	{
		#region Private Fields

		private Transform _transform;
		private GameObject _gameObject;
		private bool _transformSet;
		private bool _gameObjectSet;

		#endregion

		#region Properties

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