using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerAbilitiesController : MonoBehaviour
{
    public enum TargetType { Enemy, AccessPoint, Door, Interactable }
    WaitForFixedUpdate wait;
    public CinemachineVirtualCamera telefragCam;
    public CinemachineTrackedDolly telefragDolly;
    public CinemachineFreeLook tpCam;
    public Transform playerCamTf;
    public float telefragCompletion, accessSpeed;
    public PlayerController playerController;
    public GameObject explosionEffect, chargingEffect;
    public bool bufferComplete;
    public float raycastBuffer;
    public TargetType telefragTargetType;
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //List<RaycastHit> hits = new List<RaycastHit>(Physics.RaycastAll(ray, 100f, telefragMask));
            //RaycastHit[] hits = Physics.RaycastAll(ray, 100f, telefragMask);
            //for (int i = 0; i < hits.Length; i++)
            //{
            //    hit = hits[i];
            //    Transform targetTf = hit.transform;
            //    string targetTag = hit.transform.gameObject.tag;
            //    float telefragModifier;
            //}
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, telefragMask))
            {
                Transform targetTf = hit.transform;
                string targetTag = hit.transform.gameObject.tag;
                float telefragModifier;
                switch (targetTag)
                {
                    case "Enemy":
                        telefragTargetType = TargetType.Enemy;
                        telefragModifier = targetTf.gameObject.GetComponent<Soldier>().fear;
                        StartCoroutine(Telefragging(targetTf, telefragTargetType, hit, telefragModifier));
                        break;
                    case "Access Point":
                        telefragTargetType = TargetType.AccessPoint;
                        telefragModifier = accessSpeed;
                        StartCoroutine(Telefragging(targetTf, telefragTargetType, hit, telefragModifier));
                        break;
                    case "Door":
                        telefragTargetType = TargetType.Door;
                        telefragModifier = accessSpeed / 2;
                        StartCoroutine(Telefragging(targetTf, telefragTargetType, hit, telefragModifier));
                        break;
                    case "Interactable":
                        telefragTargetType = TargetType.Interactable;
                        telefragModifier = accessSpeed;
                        StartCoroutine(Telefragging(targetTf, telefragTargetType, hit, telefragModifier));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    IEnumerator Telefragging(Transform targetTf, TargetType telefragTargetType, RaycastHit hit, float telefragModifier)
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
        telefragCam.m_LookAt = targetTf.transform;
        telefragCam.gameObject.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        StartCoroutine(RaycastBufferTime());
        //Play charging sound
        //GameObject chargingEffectInstance = Instantiate(chargingEffect, target.position, target.rotation);
        while (Input.GetButton("Fire1"))
        {
            if (bufferComplete)
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, telefragMask) && hit.transform.gameObject != targetTf.gameObject)
                {
                    break;
                }
            }
            if (telefragCompletion >= 100)
            {
                break;
            }
            smoothPath.m_Waypoints[1].position = targetTf.position;
            telefragDolly.m_PathPosition = Mathf.Clamp(smoothPath.MaxPos * ((telefragCompletion - 20) / 100), 0, smoothPath.MaxPos * 0.8f);
            telefragCompletion += (50 * (telefragModifier / 100)) * Time.deltaTime;
            yield return wait;
        }
        telefragCam.gameObject.SetActive(false);
        if (telefragTargetType == TargetType.Enemy && telefragCompletion >= 100)
        {
            targetTf.gameObject.GetComponent<Soldier>().Death();
            transform.position = targetTf.position;
        }
        else if (telefragTargetType == TargetType.AccessPoint && telefragCompletion >= 100)
        {
            StartCoroutine(targetTf.gameObject.GetComponent<AccessPointController>().EnterNetwork());
            //play effect on player
        }
        else if (telefragTargetType == TargetType.Door && telefragCompletion >= 100)
        {
            Door targetDoor = targetTf.gameObject.GetComponent<Door>();
            if (!targetDoor.isOpen)
            {
                targetDoor.Open();
            }
            else
            {
                targetDoor.Close();
            }
        }
        else if (telefragTargetType == TargetType.Interactable && telefragCompletion >= 100)
        {
            targetTf.gameObject.GetComponent<Interactable>().Toggle();
        }
        //Destroy(chargingEffectInstance);
        //End charging sound
        playerController.lockMove = false;
        playerController.lockLook = false;
        Destroy(smoothPath.gameObject);

    }

    IEnumerator RaycastBufferTime()
    {
        yield return new WaitForSeconds(raycastBuffer);
        bufferComplete = true;
    }

}
