using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using System;

public class PlayerAbilitiesController : MonoBehaviour
{
    public enum TargetTag { Enemy, AccessPoint, Door, Interactable }
    public CinemachineFreeLook ThirdPersonCamera;
    WaitForFixedUpdate wait;
    [SerializeField]
    private CinemachineVirtualCamera TelefragCam;
    [SerializeField]
    private CinemachineTrackedDolly TelefragDolly;
    [SerializeField]
    private Transform PlayerCameraTf;
    [SerializeField]
    private PlayerController PlayerController;
    [SerializeField]
    private GameObject ExplosionEffect, ChargingEffect;
    [SerializeField]
    private float RaycastBuffer, AttackDistance, AccessSpeed;
    [SerializeField]
    private TargetTag TelefragTargetType;
    private int _telefragMask, _terrainMask;



    // Start is called before the first frame update
    void Start()
    {
        TelefragDolly = TelefragCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        _telefragMask = ~LayerMask.GetMask("Ignore Raycast");
        _terrainMask = LayerMask.GetMask("Terrain");
        _targetableTags = System.Enum.GetNames(typeof(TargetTag));
        _telefragComplete += PlayerAbilitiesController__telefragComplete;
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
            GameObject target = null;
            RaycastHit[] hits = RaycastForward();
            target = FindFirstTarget(hits);
            if (target != null)
            {
                float telefragModifier;
                switch (target.tag)
                {
                    case "Enemy":
                        TelefragTargetType = TargetTag.Enemy;
                        Soldier targetSoldier = target.GetComponent<Soldier>();
                        telefragModifier = targetSoldier.Fear;
                        _targetAction = targetSoldier.Death;
                        break;
                    case "AccessPoint":
                        TelefragTargetType = TargetTag.AccessPoint;
                        telefragModifier = AccessSpeed;
                        _targetAction = () => StartCoroutine(target.GetComponent<AccessPointController>().EnterNetwork());
                        break;
                    case "Door":
                        TelefragTargetType = TargetTag.Door;
                        telefragModifier = AccessSpeed / 2;
                        _targetAction = target.GetComponent<Door>().Toggle;
                        break;
                    case "Interactable":
                        TelefragTargetType = TargetTag.Interactable;
                        telefragModifier = AccessSpeed;
                        _targetAction = target.GetComponent<Interactable>().Toggle;
                        break;
                    default:
                        telefragModifier = 0f;
                        _targetAction = null;
                        break;
                }
                StartCoroutine(Telefragging(target.transform, TelefragTargetType, telefragModifier));
            }
        }
    }

    IEnumerator Telefragging(Transform targetTf, TargetTag telefragTargetType, float telefragModifier)
    {
        PlayerController.lockMove = true;
        PlayerController.lockLook = true;
        _telefragCompletion = 0;
        CinemachineSmoothPath smoothPath = CreateNewSmoothPath();
        PrepareDollyTrackAndCamera(smoothPath, targetTf);
        StartCoroutine(RaycastBufferTime());
        //Play charging sound
        //GameObject chargingEffectInstance = Instantiate(chargingEffect, target.position, target.rotation);
        while (Input.GetButton("Fire1"))
        {
            if (_bufferComplete)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, _terrainMask))
                {
                    if((hit.point.magnitude - transform.position.magnitude) < (targetTf.position.magnitude - transform.position.magnitude))
                    {
                        break;
                    }
                }
            }
            if (_telefragCompletion >= 100)
            {
                break;
            }
            smoothPath.m_Waypoints[1].position = targetTf.position;
            TelefragDolly.m_PathPosition = Mathf.Clamp(smoothPath.MaxPos * ((_telefragCompletion - 20) / 100), 0, smoothPath.MaxPos * 0.8f);
            _telefragCompletion += (50 * (telefragModifier / 100)) * Time.deltaTime;
            yield return wait;
        }
        TelefragCam.gameObject.SetActive(false);
        if(_telefragCompletion >= 100)
        {
            _telefragComplete?.Invoke(this, EventArgs.Empty);
        }
        //Destroy(chargingEffectInstance);
        //End charging sound
        PlayerController.lockMove = false;
        PlayerController.lockLook = false;
        Destroy(smoothPath.gameObject);

    }

    private void PlayerAbilitiesController__telefragComplete(object sender, EventArgs e)
    {
        _targetAction();
    }

    private RaycastHit[] RaycastForward()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, AttackDistance, _telefragMask);
        return hits.OrderBy(c => c.distance).ToArray();
    }
    private GameObject FindFirstTarget(RaycastHit[] hits)
    {
        foreach (RaycastHit hit in hits)
        {
            Transform hitTf = hit.transform;
            if (_targetableTags.Contains(hitTf.tag))
            {
                return hitTf.gameObject;
            }
        }
        return null;
    }
    CinemachineSmoothPath CreateNewSmoothPath()
    {
        CinemachineSmoothPath smoothPath = new GameObject("DollyTrack").AddComponent<CinemachineSmoothPath>();
        smoothPath.m_Waypoints = new CinemachineSmoothPath.Waypoint[2];
        smoothPath.m_Waypoints[0] = new CinemachineSmoothPath.Waypoint();
        smoothPath.m_Waypoints[0].position = PlayerCameraTf.position;
        smoothPath.m_Waypoints[1] = new CinemachineSmoothPath.Waypoint();
        return smoothPath;
    }
    void PrepareDollyTrackAndCamera(CinemachineSmoothPath smoothPath, Transform target)
    {
        TelefragDolly.m_Path = smoothPath;
        TelefragDolly.m_PathPosition = 0;
        TelefragDolly.m_AutoDolly.m_Enabled = false;
        TelefragCam.m_LookAt = target;
        TelefragCam.gameObject.SetActive(true);
    }

    IEnumerator RaycastBufferTime()
    {
        yield return new WaitForSeconds(RaycastBuffer);
        _bufferComplete = true;
    }

    private float _telefragCompletion;
    private bool _bufferComplete;
    private string[] _targetableTags;
    private event EventHandler _telefragComplete;
    private Action _targetAction;
}
