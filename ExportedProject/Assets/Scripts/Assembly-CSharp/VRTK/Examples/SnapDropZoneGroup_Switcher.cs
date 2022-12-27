using UnityEngine;

namespace VRTK.Examples
{
	public class SnapDropZoneGroup_Switcher : MonoBehaviour
	{
		private GameObject cubeZone;

		private GameObject sphereZone;

		private void Start()
		{
			cubeZone = base.transform.Find("Cube_SnapDropZone").gameObject;
			sphereZone = base.transform.Find("Sphere_SnapDropZone").gameObject;
			cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += DoCubeZoneSnapped;
			cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += DoCubeZoneSnapped;
			cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += DoCubeZoneUnsnapped;
			cubeZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += DoCubeZoneUnsnapped;
			sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectEnteredSnapDropZone += DoSphereZoneSnapped;
			sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectSnappedToDropZone += DoSphereZoneSnapped;
			sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectExitedSnapDropZone += DoSphereZoneUnsnapped;
			sphereZone.GetComponent<VRTK_SnapDropZone>().ObjectUnsnappedFromDropZone += DoSphereZoneUnsnapped;
		}

		private void DoCubeZoneSnapped(object sender, SnapDropZoneEventArgs e)
		{
			sphereZone.SetActive(false);
		}

		private void DoCubeZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
		{
			sphereZone.SetActive(true);
		}

		private void DoSphereZoneSnapped(object sender, SnapDropZoneEventArgs e)
		{
			cubeZone.SetActive(false);
		}

		private void DoSphereZoneUnsnapped(object sender, SnapDropZoneEventArgs e)
		{
			cubeZone.SetActive(true);
		}
	}
}
