using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public List<GameObject> Cameras;
    public GameObject CurrentCamera, FadeToBlackCamera, Player;
    public Transform PlayerTf;
    public PlayerAbilitiesController PlayerAbilitiesController;
    public PlayerController PlayerController;
    public bool ChangingCameras;
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
        if(Input.GetAxis("Horizontal") != 0  && !ChangingCameras)
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
        ChangingCameras = true;
        //fadeToBlackCamera.SetActive(true);
        int currentIndex = Cameras.IndexOf(CurrentCamera);
        int nextIndexDirection = (int)Mathf.Sign(direction);
        int nextIndex = currentIndex + nextIndexDirection;
        if (nextIndex >= Cameras.Count)
        {
            nextIndex = 0;
        }
        else if (nextIndex < 0)
        {
            nextIndex = Cameras.Count - 1;
        }
        GameObject nextCamera = Cameras[nextIndex];
        nextCamera.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        CurrentCamera.SetActive(false);
        CurrentCamera = nextCamera;
        //yield return new WaitForSeconds(1.0f);
        //fadeToBlackCamera.SetActive(false);
        ChangingCameras = false;
    }

    IEnumerator ExitCamera()
    {
        if (CurrentCamera.GetComponent<CameraInfo>().connectedAccessPoint != null)
        {
            AccessPointController accessPoint = CurrentCamera.GetComponent<CameraInfo>().connectedAccessPoint;
            FadeToBlackCamera.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            CurrentCamera.SetActive(false);
            yield return new WaitForSeconds(1.0f);
            Player.SetActive(true);
            Player.transform.position = accessPoint.LandingTf.position;
            this.enabled = false;
            FadeToBlackCamera.SetActive(false);
            PlayerAbilitiesController.ThirdPersonCamera.gameObject.SetActive(true);
        }
    }

    public void ActivateCams(GameObject newCam)
    {
        CurrentCamera = newCam;
        this.enabled = true;
    }
}
