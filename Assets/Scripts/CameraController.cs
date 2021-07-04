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
    public bool changingCameras;
    private void Start()
    {
        //GameObject[] camerasTemp = GameObject.FindGameObjectsWithTag("Camera");
        //for(int i = 0; i < camerasTemp.Length; i++)
        //{
        //    cameras.Add(camerasTemp[i].GetComponent<CinemachineVirtualCamera>());
        //}
    }

    private void Update()
    {
        if(Input.GetAxis("Horizontal") != 0)
        {
            StartCoroutine(ChangeCamera(Input.GetAxis("Horizontal")));
        }
    }

    IEnumerator ChangeCamera(float direction)
    {
        changingCameras = true;
        fadeToBlackCamera.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        currentCamera.SetActive(false);
        int currentIndex = cameras.IndexOf(currentCamera);
        int nextIndexDirection = (int)direction;
        int nextIndex = currentIndex + nextIndexDirection;
        if (nextIndex > cameras.Count)
        {
            nextIndex = 0;
        }
        else if(nextIndex < 0)
        {
            nextIndex = cameras.Count - 1;
        }
        currentCamera = cameras[nextIndex];
        currentCamera.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        fadeToBlackCamera.SetActive(false);
        changingCameras = false;
    }
}
