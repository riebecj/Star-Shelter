using UnityEngine;

namespace ch.sycoforge.Decal
{
	public class EasyDecalExtension : EasyDecal
	{
		public float AngularSpeed = 2f;

		protected override void Start()
		{
			base.Start();
			Invoke("OnBecameInvisible", 1f);
		}

		private void OnBecameVisible()
		{
			base.enabled = true;
		}

		private void OnBecameInvisible()
		{
			base.enabled = false;
		}

		protected override void Update()
		{
			base.Update();
			base.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * AngularSpeed * Time.deltaTime);
		}
	}
}
