using UnityEngine;

namespace Reaktion
{
	public class InjectorBase : MonoBehaviour
	{
		protected float dbLevel = -60f;

		public float DbLevel
		{
			get
			{
				return dbLevel;
			}
		}
	}
}
