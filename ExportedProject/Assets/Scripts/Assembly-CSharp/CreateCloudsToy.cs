using System.Collections;
using UnityEngine;

public class CreateCloudsToy : MonoBehaviour
{
	public bool createNewCloudsToy = true;

	[Space(10f)]
	[Tooltip("if createNewCloudsToy is true, goCloud will be this transform.\nif not goCloud will be the gameobject where CloudsToy is located (some CloudsToy Mngr).")]
	public GameObject goCloud;

	[SerializeField]
	[Tooltip("if createNewCloudsToy is true, this var will contain the new Component of CloudsToy script.\nif not here will be assigned the CloudsToy script (contained in the GoCloud).")]
	private CloudsToy _cloudsToy;

	[Space(10f)]
	public Shader realisticShader;

	public Shader brightShader;

	public Material projectorMaterial;

	[Space(10f)]
	public Texture2D[] CloudsTextAdd = new Texture2D[6];

	public Texture2D[] CloudsTextBlended = new Texture2D[6];

	[Space(10f)]
	public Color MainColor = Color.blue;

	private bool isInitialized;

	private IEnumerator Start()
	{
		if (createNewCloudsToy)
		{
			goCloud = base.gameObject;
			_cloudsToy = goCloud.AddComponent<CloudsToy>();
			_cloudsToy.enabled = true;
			_cloudsToy.realisticShader = realisticShader;
			_cloudsToy.brightShader = brightShader;
			_cloudsToy.projectorMaterial = projectorMaterial;
			for (int i = 0; i < CloudsTextAdd.Length; i++)
			{
				_cloudsToy.CloudsTextAdd[i] = CloudsTextAdd[i];
			}
			for (int j = 0; j < CloudsTextBlended.Length; j++)
			{
				_cloudsToy.CloudsTextBlended[j] = CloudsTextBlended[j];
			}
			_cloudsToy.SetPresetStormy();
		}
		else
		{
			if (!goCloud.activeSelf)
			{
				goCloud.SetActive(true);
			}
			_cloudsToy = goCloud.GetComponent<CloudsToy>();
		}
		yield return new WaitForSeconds(1f);
		_cloudsToy.SoftClouds = false;
		_cloudsToy.NumberClouds = 170;
		_cloudsToy.SizeFactorPart = 1f;
		_cloudsToy.DisappearMultiplier = 3f;
		_cloudsToy.VelocityMultipier = 0f;
		_cloudsToy.PaintType = CloudsToy.TypePaintDistr.Below;
		_cloudsToy.CloudDetail = CloudsToy.TypeDetail.High;
		_cloudsToy.CloudColor = MainColor;
		_cloudsToy.MainColor = Color.grey;
		_cloudsToy.SecondColor = Color.gray * 0.5f;
		_cloudsToy.TintStrength = 80;
		_cloudsToy.offset = 0.8f;
		_cloudsToy.PT1ScaleWidth = 55f;
		_cloudsToy.PT1ScaleHeight = 5f;
		_cloudsToy.MaxDepthCloud = 130;
		_cloudsToy.IsAnimate = true;
		_cloudsToy.AnimationVelocity = 0.6f;
		_cloudsToy.NumberOfShadows = CloudsToy.TypeShadow.Some;
		_cloudsToy.EditorRepaintClouds();
		isInitialized = true;
	}

	private void Update()
	{
		if (isInitialized && _cloudsToy.CloudColor != MainColor)
		{
			_cloudsToy.CloudColor = MainColor;
		}
	}
}
