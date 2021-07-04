using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessPointController : MonoBehaviour
{
    public GameObject connectedCamera;
    public CameraController cameraController;
    

    public IEnumerator EnterNetwork()
    {
        cameraController.fadeToBlackCamera.transform.position = cameraController.playerAbilitiesController.playerCamTf.position;
        cameraController.fadeToBlackCamera.transform.rotation = cameraController.playerAbilitiesController.playerCamTf.rotation;
        cameraController.fadeToBlackCamera.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        cameraController.playerAbilitiesController.tpCam.gameObject.SetActive(false);
        cameraController.player.SetActive(false);
        connectedCamera.SetActive(true);
        cameraController.fadeToBlackCamera.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        cameraController.enabled = true;
        cameraController.currentCamera = connectedCamera;
    }
}
