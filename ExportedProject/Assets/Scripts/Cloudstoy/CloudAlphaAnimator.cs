using UnityEngine;

public class CloudAlphaAnimator : MonoBehaviour
{
	private float alpha = 0.5f;

	public float range = 0.3f;

	public float ratio = 20f;

	public bool isAnimated;

	private float _timeStart;

	private float startAlpha;

	private float destAlpha;

	private Material _cloudMaterial;

	private Color _color;

	private float startDelay;

	private bool isFirstTime = true;

	private Color origColor;

	private float timeSinceStarted;

	private float percentageComplete;

	public void SetCloudAlphaAnimator(float _ratio, float _range, bool _animate)
	{
		ratio = _ratio;
		range = _range;
		isAnimated = _animate;
		Initialize();
	}

	public void SetNewCloudColor(Color _newColor)
	{
		_cloudMaterial = GetComponent<Renderer>().material;
		_cloudMaterial.SetColor("_TintColor", _newColor);
		origColor = _newColor;
		Initialize();
	}

	private void GetCloudMaterial()
	{
		_cloudMaterial = GetComponent<Renderer>().material;
		_color = _cloudMaterial.GetColor("_TintColor");
		if (isFirstTime)
		{
			origColor = _color;
			isFirstTime = false;
		}
		alpha = origColor.a;
		if (range > alpha)
		{
			range = alpha;
		}
	}

	private void Initialize()
	{
		GetCloudMaterial();
		startDelay = Random.Range(ratio * 0.5f, ratio * 2f);
		startAlpha = alpha;
		destAlpha = alpha - range;
	}

	private void Update()
	{
		if (isFirstTime || _cloudMaterial == null)
		{
			return;
		}
		if (!isAnimated)
		{
			if (_color.a != alpha)
			{
				alpha = origColor.a;
				_color = origColor;
				_cloudMaterial.SetColor("_TintColor", origColor);
			}
			return;
		}
		if (startDelay > 0f)
		{
			startDelay -= Time.deltaTime;
			_timeStart = Time.time;
			return;
		}
		timeSinceStarted = Time.time - _timeStart;
		percentageComplete = timeSinceStarted / ratio;
		_color.a = Mathf.Lerp(startAlpha, destAlpha, percentageComplete);
		_cloudMaterial.SetColor("_TintColor", _color);
		if (percentageComplete >= 1f)
		{
			_timeStart = Time.time;
			float num = startAlpha;
			startAlpha = destAlpha;
			destAlpha = num;
		}
	}
}
