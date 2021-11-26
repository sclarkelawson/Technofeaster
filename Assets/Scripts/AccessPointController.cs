using System.Collections;
using UnityEngine;

public class AccessPointController : MonoBehaviour
{
    public GameObject ConnectedCamera;
    public CameraController CameraController;
    public Transform LandingTf;

    public IEnumerator EnterNetwork()
    {
        CameraController.PlayerController.isWounded = false;
        CameraController.FadeToBlackCamera.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        CameraController.PlayerAbilitiesController.ThirdPersonCamera.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        ConnectedCamera.SetActive(true);
        CameraController.Player.SetActive(false);
        CameraController.FadeToBlackCamera.SetActive(false);
        CameraController.ActivateCams(ConnectedCamera);
    }
}
