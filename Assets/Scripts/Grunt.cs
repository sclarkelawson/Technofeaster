using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : MonoBehaviour, Soldier
{
    #region Soldier stuff
    [SerializeField] private int _fear;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private Transform _playerTf;
    [SerializeField] private SquadController _mySquad;
    [SerializeField] private float _maxAimDistance;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _fovPrefab;
    public SquadController mySquad { get { return _mySquad; } set { _mySquad = value; } }
    public int fear
    {
        get { return _fear; }
        set
        {
            _fear = Mathf.Clamp(value, 0, 100);
        }
    } //get returns fear, set modifies fear and clamps between 0-100
    public float engageTimer { get; set; }
    public float fireDelta { get; set; }
    public float techSkill { get; set; }
    public float maxAimDistance { get { return _maxAimDistance; } set { _maxAimDistance = value; } }
    public Transform playerTf { get { return _playerTf; } set { _playerTf = value; } }
    public PlayerController playerController { get; set; }
    public Vector3 lastKnownPosition { get; set; }
    public bool isIsolated { get; set; }
    public bool isUpgraded { get; set; }
    public bool sightOfPlayer { get; set; }
    public bool firstShot { get; set; }
    public bool targeted { get; set; }

    public bool openingDoor { get; set; }
    public Rigidbody myRb { get; set; }
    public GameObject deathEffect { get { return _deathEffect; } set { _deathEffect = value; } }
    public GameObject fovPrefab { get { return _fovPrefab; } set { _fovPrefab = value; } }
    public SquadController.SoldierType myType { get; set; }
    public SquadController.Goal squadGoal { get; set; }
    public Soldier.SoldierGoal myGoal { get; set; }
    public Soldier.SoldierGoal lastGoal { get; set; }
    public NavMeshAgent navAgent { get; set; }
    public LineRenderer aimLine { get; set; }
    public GameObject aimLineObject { get; set; }
    public GameObject bullet { get { return _bullet; } set { _bullet = value; } }
    public Transform targetTf { get; set; }
    public Transform formationTf { get; set; }
    public int layerMask { get; set; }
    #endregion

    private void Start()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        layerMask = ~LayerMask.GetMask("TransparentFX", "Shot");
        myRb = GetComponent<Rigidbody>();
        myGoal = Soldier.SoldierGoal.FollowSquad;
        myType = SquadController.SoldierType.Grunt;
        GameObject tempFov = Instantiate(fovPrefab, transform.position, Quaternion.identity);
        tempFov.GetComponent<EnemyFov>().connectedSoldierTf = transform;
        tempFov.GetComponent<EnemyFov>().connectedSoldier = this;
        firstShot = true;
        techSkill = 10;
        openingDoor = false;
        aimLineObject = new GameObject("Line");
        aimLine = aimLineObject.AddComponent<LineRenderer>();
        aimLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        aimLine.positionCount = 2;
        aimLineObject.SetActive(false);
        aimLine.startWidth = 0.1f;
        aimLine.endWidth = 0.1f;
        aimLine.startColor = Color.red;
        aimLine.endColor = Color.red;
    }
    private void Update()
    {
        if (targetTf != null)
        {
            Vector3 rayDirection = targetTf.position - transform.position;
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, maxAimDistance, layerMask))
            {
                if (hit.transform == targetTf)
                {
                    sightOfPlayer = true;
                    lastKnownPosition = targetTf.position;
                    if(myGoal != Soldier.SoldierGoal.Attack)
                    {
                        lastGoal = myGoal;
                    }
                    myGoal = Soldier.SoldierGoal.Attack;
                    if (mySquad.goalList.Peek() == SquadController.Goal.Hunt)
                    {
                        engageTimer = 10f;
                    }
                    else if(!playerController.isWounded)
                    {
                        engageTimer = 10f - (9f / (fear + 1));
                    }
                    mySquad.EvaluateRequest(gameObject, this, SquadController.SoldierRequest.Engaging, lastKnownPosition);
                }
                else
                {
                    firstShot = true;
                    sightOfPlayer = false;
                }
            }
            
        }
    }

    private void FixedUpdate()
    {
        if (targeted && fear >= 50)
        {
            myGoal = Soldier.SoldierGoal.RequestProtection;
        }
        switch (myGoal)
        {
            case Soldier.SoldierGoal.Attack:
                if (engageTimer > 0)
                {
                    Attack();
                }
                else
                {
                    aimLineObject.SetActive(false);
                    myGoal = lastGoal;
                }
                engageTimer -= Time.fixedDeltaTime;
                break;
            case Soldier.SoldierGoal.FollowSquad:
                FollowSquad();
                break;
            case Soldier.SoldierGoal.Regroup:
                MoveToSquad();
                break;
            case Soldier.SoldierGoal.RequestProtection:
                Debug.Log("requesting help");
                mySquad.EvaluateRequest(gameObject, SquadController.SoldierRequest.Protect);
                break;
        }
    }

    void FollowSquad()
    {
        //Debug.Log(currentGoal);
        switch (mySquad.goalList.Peek())
        {
            case SquadController.Goal.Hunt:
                MoveToSquad();
                break;
            case SquadController.Goal.FindServer:
                MoveToSquad();
                break;
            case SquadController.Goal.FindArmory:
                MoveToSquad();
                break;
            case SquadController.Goal.Resupply:
                MoveToSquad();
                break;
            case SquadController.Goal.Regroup:
                MoveToSquad();

                break;
            case SquadController.Goal.Search: //if not techie, wait and spin around room
                RoomDoor targetDoor = mySquad.targetRoom.door;
                if (mySquad.availableSoldierInRange[SquadController.SoldierType.Techie].Count > 0)
                {
                    myRb.velocity = Vector3.zero;
                    navAgent.isStopped = true;
                    transform.Rotate(Vector3.up, Time.deltaTime * 60.0f);
                }
                else if(!targetDoor.canOpen.Contains(gameObject))
                {
                    navAgent.SetDestination(targetDoor.connectedRoom.entrance.position);
                    navAgent.isStopped = false;
                    openingDoor = false;
                }
                else if(!openingDoor)
                {
                    myRb.velocity = Vector3.zero;
                    navAgent.isStopped = true;
                    StartCoroutine(OpenDoor(targetDoor));
                    openingDoor = true;
                }
                break;
            case SquadController.Goal.Protecting: //done
                if(mySquad.protectTarget != gameObject)
                {
                    transform.LookAt(mySquad.protectTarget.transform);
                }
                break;
            default:
                MoveToSquad();
                break;
        }
    }
    void Attack()
    {
        if (firstShot)
        {
            fireDelta = 2.0f;
            firstShot = false;
        }
        if (sightOfPlayer)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = transform.position;
            positions[1] = targetTf.position;
            aimLine.SetPositions(positions);
            aimLineObject.SetActive(true);
            if (playerController.isWounded && navAgent.remainingDistance >= 1)
            {
                navAgent.SetDestination(lastKnownPosition);
                navAgent.isStopped = true;
            }
            else if (playerController.isWounded)
            {
                navAgent.SetDestination(lastKnownPosition);
                navAgent.isStopped = false;
            }
            else
            {
                myRb.velocity = Vector3.zero;
                transform.LookAt(targetTf);
                navAgent.isStopped = true;
            }
            fireDelta -= Time.deltaTime;
            switch (mySquad.huntOrCapture)
            {
                case SquadController.HuntOrCapture.Hunt:
                    aimLine.startColor = Color.red;
                    aimLine.endColor = Color.red;
                    if (fireDelta <= 0)
                    {
                        GameObject temp = Instantiate(bullet, transform.position + (transform.forward * 2), transform.rotation);
                        temp.transform.LookAt(targetTf);
                        temp.GetComponent<ShotController>().canKill = true;
                        fireDelta = 1.5f;
                    }
                    break;
                case SquadController.HuntOrCapture.Capture:
                    aimLine.startColor = Color.blue;
                    aimLine.endColor = Color.blue;
                    if (fireDelta <= 0 && !playerController.isWounded)
                    {
                        GameObject temp = Instantiate(bullet, transform.position + (transform.forward * 2), transform.rotation);
                        temp.transform.LookAt(targetTf);
                        temp.GetComponent<ShotController>().canKill = false;
                        fireDelta = 1.5f;
                    }
                    else if (playerController.isWounded && mySquad.knownRooms[RoomInfo.RoomType.Server].Count <= 0)
                    {

                    }
                    break;
            }
        }
        else
        {
            fireDelta = 2.0f;
            aimLineObject.SetActive(false);
            navAgent.SetDestination(lastKnownPosition);
            navAgent.isStopped = false;
        }

    }
    void MoveToSquad()
    {
        navAgent.SetDestination(mySquad.transform.position);
        if (navAgent.remainingDistance >= 2)
        {
            navAgent.isStopped = false;
        }
        else
        {
            myRb.velocity = Vector3.zero;
            navAgent.isStopped = true;
        }
    }
    public void Death() //call RemoveSoldier(), destroy object, instantiate explosion
    {
        if (mySquad != null)
        {
            mySquad.soldiersInSquad.Remove(gameObject);
            if (mySquad.soldiersInRange.Contains(gameObject))
            {
                mySquad.soldiersInRange.Remove(gameObject);
            }
        }
        Destroy(Instantiate(deathEffect, transform.position, transform.rotation), 2.0f);
        Destroy(aimLineObject);
        Destroy(gameObject);

    }

    public IEnumerator OpenDoor(Door targetDoor)
    {
        while(!targetDoor.isOpen)
        {
            targetDoor.Open(techSkill, mySquad);
            yield return new WaitForSeconds(1f);
        }
        mySquad.EvaluateRequest(gameObject, SquadController.SoldierRequest.SearchComplete);
    }
}
