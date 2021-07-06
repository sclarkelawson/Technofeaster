using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessPointController : MonoBehaviour
{
    public GameObject connectedCamera;
    public CameraController cameraController;
    public Transform landingTf;

    public IEnumerator EnterNetwork()
    {
        cameraController.fadeToBlackCamera.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        cameraController.playerAbilitiesController.tpCam.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        connectedCamera.SetActive(true);
        cameraController.player.SetActive(false);
        cameraController.fadeToBlackCamera.SetActive(false);
        cameraController.ActivateCams(connectedCamera);
    }
}
