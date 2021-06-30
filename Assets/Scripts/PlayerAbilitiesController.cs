using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerAbilitiesController : MonoBehaviour
{
    WaitForFixedUpdate wait;
    public CinemachineVirtualCamera telefragCam;
    public CinemachineTrackedDolly telefragDolly;
    public CinemachineFreeLook tpCam;
    public Transform playerCamTf;
    public float telefragCompletion, fearVal;
    public PlayerController playerController;
    public GameObject explosionEffect, chargingEffect;
    public bool bufferComplete;
    public float raycastBuffer;
    private int telefragMask;



    // Start is called before the first frame update
    void Start()
    {
        telefragDolly = telefragCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        telefragMask = ~LayerMask.GetMask("Ignore Raycast");
    }

    // Update is called once per frame
    void Update()
    {
        Telefrag();
    }

    void Telefrag()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Telefragging());
        }
    }

    IEnumerator Telefragging()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Transform target = hit.transform;
            if (target.CompareTag("enemy"))
            {

                playerController.lockMove = true;
                playerController.lockLook = true;
                telefragCompletion = 0;
                CinemachineSmoothPath smoothPath = new GameObject("DollyTrack").AddComponent<CinemachineSmoothPath>();
                smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[2];
                smoothPath.m_Waypoints[0] = new CinemachineSmoothPath.Waypoint();
                smoothPath.m_Waypoints[0].position = playerCamTf.position;
                smoothPath.m_Waypoints[1] = new CinemachineSmoothPath.Waypoint();
                telefragDolly.m_Path = smoothPath;
                telefragDolly.m_PathPosition = 0;
                telefragDolly.m_AutoDolly.m_Enabled = false;
                telefragCam.m_LookAt = target.transform;
                telefragCam.gameObject.SetActive(true);
                StartCoroutine(RaycastBufferTime());
                //GameObject chargingEffectInstance = Instantiate(chargingEffect, target.position, target.rotation);
                while (Input.GetButton("Fire1"))
                {
                    if (bufferComplete)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit) && hit.transform.gameObject != target.gameObject)
                        {
                            break;
                        }
                    }
                    if (telefragCompletion >= 100)
                    {
                        transform.position = target.position;
                        Destroy(target.gameObject);
                        Destroy(Instantiate(explosionEffect, target.position, target.rotation), 2.0f);
                        break;
                    }
                    smoothPath.m_Waypoints[1].position = target.position;
                    telefragDolly.m_PathPosition = Mathf.Clamp(smoothPath.MaxPos * ((telefragCompletion - 20) / 100), 0, smoothPath.MaxPos * 0.8f);
                    telefragCompletion += (40 * (fearVal / 100)) * Time.deltaTime;
                    yield return wait;
                }
                //Destroy(chargingEffectInstance);
                telefragCam.gameObject.SetActive(false);
                playerController.lockMove = false;
                playerController.lockLook = false;
                Destroy(smoothPath.gameObject);
            }
        }

    }

    IEnumerator RaycastBufferTime()
    {
        yield return new WaitForSeconds(raycastBuffer);
        bufferComplete = true;
    }
}