using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerAbilitiesController : MonoBehaviour
{
    public enum TargetType { Enemy, AccessPoint }
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
            if (Physics.Raycast(ray, out hit))
            {
                Transform targetTf = hit.transform; ;
                string targetTag = hit.transform.gameObject.tag;
                switch (targetTag)
                {
                    case "Enemy":
                        telefragTargetType = TargetType.Enemy;
                        StartCoroutine(Telefragging(targetTf, telefragTargetType, hit));
                        break;
                    case "Access Point":
                        telefragTargetType = TargetType.AccessPoint;
                        StartCoroutine(Telefragging(targetTf, telefragTargetType, hit));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    IEnumerator Telefragging(Transform targetTf, TargetType telefragTargetType, RaycastHit hit)
    {
        float telefragModifier = 0;
        Soldier targetSoldier = null;
        if (targetTf.gameObject.GetComponent<Soldier>() != null)
        {
            targetSoldier = targetTf.gameObject.GetComponent<Soldier>();
            telefragModifier = targetSoldier.fear;
        }
        else if (telefragTargetType == TargetType.AccessPoint)
        {
            telefragModifier = accessSpeed;
        }
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
        StartCoroutine(RaycastBufferTime());
        //GameObject chargingEffectInstance = Instantiate(chargingEffect, target.position, target.rotation);
        while (Input.GetButton("Fire1"))
        {
            if (bufferComplete)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit) && hit.transform.gameObject != targetTf.gameObject)
                {
                    break;
                }
            }
            if (telefragCompletion >= 100)
            {
                if (telefragTargetType == TargetType.Enemy)
                {
                    targetSoldier.Death();
                    transform.position = targetTf.position;
                }
                else if(telefragTargetType == TargetType.AccessPoint)
                {

                }
                break;
            }
            smoothPath.m_Waypoints[1].position = targetTf.position;
            telefragDolly.m_PathPosition = Mathf.Clamp(smoothPath.MaxPos * ((telefragCompletion - 20) / 100), 0, smoothPath.MaxPos * 0.8f);
            telefragCompletion += (50 * (telefragModifier / 100)) * Time.deltaTime;
            yield return wait;
        }
        //Destroy(chargingEffectInstance);
        telefragCam.gameObject.SetActive(false);
        playerController.lockMove = false;
        playerController.lockLook = false;
        Destroy(smoothPath.gameObject);
    }
    void EndTelefrag()
    {
        
    }

    IEnumerator RaycastBufferTime()
    {
        yield return new WaitForSeconds(raycastBuffer);
        bufferComplete = true;
    }
}
