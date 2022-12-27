using UnityEngine;

public class MoveBlock : MonoBehaviour
{
	public float moveYAmount = 20f;

	public float moveSpeed = 1f;

	public float waitTime = 5f;

	public float rotateSpeed = 10f;

	private float startY;

	private bool goingUp = true;

	private float stoppedUntilTime;

	private float moveUpAmount;

	protected virtual void Start()
	{
		startY = base.transform.position.y;
		moveUpAmount = Mathf.Abs(moveYAmount);
		if (moveYAmount < 0f)
		{
			startY -= moveYAmount;
			goingUp = false;
		}
		stoppedUntilTime = Time.time + waitTime;
	}

	protected virtual void Update()
	{
		if (Time.time > stoppedUntilTime)
		{
			if (goingUp)
			{
				if (base.transform.position.y < startY + moveUpAmount)
				{
					Vector3 position = base.transform.position;
					position.y += Time.deltaTime * moveSpeed;
					base.transform.position = position;
				}
				else
				{
					goingUp = false;
					stoppedUntilTime = Time.time + waitTime;
				}
			}
			else if (base.transform.position.y > startY)
			{
				Vector3 position2 = base.transform.position;
				position2.y -= Time.deltaTime * moveSpeed;
				base.transform.position = position2;
			}
			else
			{
				goingUp = true;
				stoppedUntilTime = Time.time + waitTime;
			}
		}
		base.transform.Rotate(new Vector3(0f, rotateSpeed * Time.deltaTime, 0f));
	}
}
