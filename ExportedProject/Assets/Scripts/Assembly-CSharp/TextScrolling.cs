using UnityEngine;

public class TextScrolling : MonoBehaviour
{
	public float textSpeed;

	private void Update()
	{
		base.transform.Translate(Vector3.up * Time.deltaTime * textSpeed, base.gameObject.transform);
	}
}
