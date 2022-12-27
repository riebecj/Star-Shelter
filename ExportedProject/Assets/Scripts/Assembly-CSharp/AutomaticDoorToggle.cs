using UnityEngine;
using UnityEngine.Events;

public class AutomaticDoorToggle : MonoBehaviour
{
	public UnityEvent OnToggle;

	public GameObject[] Objects;

	public bool On;

	public void OnTrigger()
	{
		if (On)
		{
			On = false;
			for (int i = 0; i < Objects.Length; i++)
			{
				if (Objects[i].activeSelf)
				{
					Objects[i].SetActive(false);
				}
				else
				{
					Objects[i].SetActive(true);
				}
			}
		}
		else
		{
			On = true;
			for (int j = 0; j < Objects.Length; j++)
			{
				if (Objects[j].activeSelf)
				{
					Objects[j].SetActive(false);
				}
				else
				{
					Objects[j].SetActive(true);
				}
			}
		}
		OnToggle.Invoke();
	}

	public void SetState(bool _On)
	{
		if (_On)
		{
			On = true;
			for (int i = 0; i < Objects.Length; i++)
			{
				if (Objects[i].activeSelf)
				{
					Objects[i].SetActive(false);
				}
				else
				{
					Objects[i].SetActive(true);
				}
			}
		}
		else
		{
			On = false;
		}
	}
}
