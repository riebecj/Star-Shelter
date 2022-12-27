using UnityEngine;

public class StartCaps : MonoBehaviour
{
	private void Start()
	{
		Invoke("Check", 0.3f);
	}

	private void Check()
	{
		foreach (Room allRoom in BaseLoader.instance.AllRooms)
		{
			if (GetComponent<Collider>().bounds.Intersects(allRoom.GetComponent<Collider>().bounds) && !allRoom.name.Contains("Center"))
			{
				GetComponent<Room>().OnRemove();
				base.gameObject.SetActive(false);
			}
		}
	}
}
