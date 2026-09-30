
using UnityEngine;

namespace MegaFiers
{
	public class MegaLookAt : MonoBehaviour
	{
		public Transform target;

		void LateUpdate()
		{
			if ( target )
			{
				transform.LookAt(target);
			}
		}
	}
}