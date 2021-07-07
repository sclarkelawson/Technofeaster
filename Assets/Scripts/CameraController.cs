using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public List<GameObject> cameras;
    public GameObject currentCamera, fadeToBlackCamera, player;
    public Transform playerTf;
    public PlayerAbilitiesController playerAbilitiesController;
    public PlayerController playerController;
    public bool changingCameras;
    private void Start()
    {
        //GameObject[] camerasTemp = GameObject.FindGameObjectsWithTag("Camera");
        //for(int i = 0; i < camerasTemp.Length; i++)
        //{
        //    cameras.Add(camerasTemp[i].GetComponent<CinemachineVirtualCamera>());
        //}
        this.enabled = false;
    }

    private void Update()
    {
        if(Input.GetAxis("Horizontal") != 0  && !changingCameras)
        {
            StartCoroutine(ChangeCamera(Input.GetAxis("Horizontal")));
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(ExitCamera());
        }
    }

    IEnumerator ChangeCamera(float direction)
    {
        changingCameras = true;
        Debug.Log("Direction: " + direction);
        //fadeToBlackCamera.SetActive(true);
        int currentIndex = cameras.IndexOf(currentCamera);
        int nextIndexDirection = (int)Mathf.Sign(direction);
        Debug.Log("Rounded Direction: " + nextIndexDirection);
        int nextIndex = currentIndex + nextIndexDirection;
        if (nextIndex >= cameras.Count)
        {
            nextIndex = 0;
        }
        else if (nextIndex < 0)
        {
            nextIndex = cameras.Count - 1;
        }
        Debug.Log("Next Index: " + nextIndex);
        GameObject nextCamera = cameras[nextIndex];
        nextCamera.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        currentCamera.SetActive(false);
        currentCamera = nextCamera;
        //yield return new WaitForSeconds(1.0f);
        //fadeToBlackCamera.SetActive(false);
        changingCameras = false;
    }

    IEnumerator ExitCamera()
    {
        if (currentCamera.GetComponent<CameraInfo>().connectedAccessPoint != null)
        {
            AccessPointController accessPoint = currentCamera.GetComponent<CameraInfo>().connectedAccessPoint;
            fadeToBlackCamera.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            currentCamera.SetActive(false);
            yield return new WaitForSeconds(1.0f);
            player.SetActive(true);
            player.transform.position = accessPoint.landingTf.position;
            this.enabled = false;
            fadeToBlackCamera.SetActive(false);
            playerAbilitiesController.tpCam.gameObject.SetActive(true);
        }
    }

    public void ActivateCams(GameObject newCam)
    {
        currentCamera = newCam;
        this.enabled = true;
    }
}
