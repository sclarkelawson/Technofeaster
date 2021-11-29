using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Techie : MonoBehaviour, Soldier
{
    #region Soldier stuff
    [SerializeField] private int _fear;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private Transform _playerTf;
    [SerializeField] private SquadController _mySquad;
    [SerializeField] private float _maxAimDistance;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _fovPrefab;
    public SquadController MySquad { get { return _mySquad; } set { _mySquad = value; } }
    public int Fear
    {
        get { return _fear; }
        set
        {
            _fear = Mathf.Clamp(value, 0, 100);
        }
    } //get returns fear, set modifies fear and clamps between 0-100
    public float EngageTimer { get; set; }
    public float FireDelta { get; set; }
    public float TechSkill { get; set; }
    public float MaxAimDistance { get { return _maxAimDistance; } set { _maxAimDistance = value; } }
    public Transform PlayerTf { get { return _playerTf; } set { _playerTf = value; } }
    public PlayerController PlayerController { get; set; }
    public Vector3 LastKnownPosition { get; set; }
    public bool IsIsolated { get; set; }
    public bool IsUpgraded { get; set; }
    public bool SightOfPlayer { get; set; }
    public bool FirstShot { get; set; }
    public bool Targeted { get; set; }

    public bool OpeningDoor { get; set; }
    public Rigidbody MyRb { get; set; }
    public GameObject DeathEffect { get { return _deathEffect; } set { _deathEffect = value; } }
    public GameObject FovPrefab { get { return _fovPrefab; } set { _fovPrefab = value; } }
    public SquadController.SoldierType MyType { get; set; }
    public SquadController.Goal SquadGoal { get; set; }
    public Soldier.SoldierGoal MyGoal { get; set; }
    public Soldier.SoldierGoal LastGoal { get; set; }
    public NavMeshAgent NavAgent { get; set; }
    public LineRenderer AimLine { get; set; }
    public GameObject AimLineObject { get; set; }
    public GameObject Bullet { get { return _bullet; } set { _bullet = value; } }
    public Transform TargetTf { get; set; }
    public Transform FormationTf { get; set; }
    public int LayerMask { get; set; }
    #endregion

    private void Start()
    {
        NavAgent = gameObject.GetComponent<NavMeshAgent>();
        PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        LayerMask = ~UnityEngine.LayerMask.GetMask("TransparentFX", "Shot");
        MyRb = GetComponent<Rigidbody>();
        MyGoal = Soldier.SoldierGoal.FollowSquad;
        MyType = SquadController.SoldierType.Grunt;
        GameObject tempFov = Instantiate(FovPrefab, transform.position, Quaternion.identity);
        tempFov.GetComponent<EnemyFov>().connectedSoldierTf = transform;
        tempFov.GetComponent<EnemyFov>().connectedSoldier = this;
        FirstShot = true;
        TechSkill = 10;
        OpeningDoor = false;
        AimLineObject = new GameObject("Line");
        AimLine = AimLineObject.AddComponent<LineRenderer>();
        AimLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        AimLine.positionCount = 2;
        AimLineObject.SetActive(false);
        AimLine.startWidth = 0.1f;
        AimLine.endWidth = 0.1f;
        AimLine.startColor = Color.red;
        AimLine.endColor = Color.red;
    }
    private void Update()
    {
        if (TargetTf != null)
        {
            Vector3 rayDirection = TargetTf.position - transform.position;
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, MaxAimDistance, LayerMask))
            {
                if (hit.transform == TargetTf)
                {
                    SightOfPlayer = true;
                    LastKnownPosition = TargetTf.position;
                    if (MyGoal != Soldier.SoldierGoal.Attack)
                    {
                        LastGoal = MyGoal;
                    }
                    MyGoal = Soldier.SoldierGoal.Attack;
                    if (MySquad.GoalList.Peek() == SquadController.Goal.Hunt)
                    {
                        EngageTimer = 10f;
                    }
                    else if (!PlayerController.IsWounded)
                    {
                        EngageTimer = 10f - (9f / (Fear + 1));
                    }
                    MySquad.EvaluateRequest(gameObject, this, SquadController.SoldierRequest.Engaging, LastKnownPosition);
                }
                else
                {
                    FirstShot = true;
                    SightOfPlayer = false;
                }
            }

        }
    }

    private void FixedUpdate()
    {
        if (Targeted && Fear >= 50)
        {
            MyGoal = Soldier.SoldierGoal.RequestProtection;
        }
        switch (MyGoal)
        {
            case Soldier.SoldierGoal.Attack:
                if (EngageTimer > 0)
                {
                    Attack();
                }
                else
                {
                    AimLineObject.SetActive(false);
                    MyGoal = LastGoal;
                }
                EngageTimer -= Time.fixedDeltaTime;
                break;
            case Soldier.SoldierGoal.FollowSquad:
                FollowSquad();
                break;
            case Soldier.SoldierGoal.Regroup:
                MoveToSquad();
                break;
            case Soldier.SoldierGoal.RequestProtection:
                Debug.Log("requesting help");
                MySquad.EvaluateRequest(gameObject, SquadController.SoldierRequest.Protect);
                break;
        }
    }

    void FollowSquad()
    {
        //Debug.Log(currentGoal);
        switch (MySquad.GoalList.Peek())
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
                RoomDoor targetDoor = MySquad.TargetRoom.door;
                if (MySquad.AvailableSoldierInRange[SquadController.SoldierType.Techie].Count > 0)
                {
                    MyRb.velocity = Vector3.zero;
                    NavAgent.isStopped = true;
                    transform.Rotate(Vector3.up, Time.deltaTime * 60.0f);
                }
                else if (!targetDoor.CanOpen.Contains(gameObject))
                {
                    NavAgent.SetDestination(targetDoor.ConnectedRoom.entrance.position);
                    NavAgent.isStopped = false;
                    OpeningDoor = false;
                }
                else if (!OpeningDoor)
                {
                    MyRb.velocity = Vector3.zero;
                    NavAgent.isStopped = true;
                    StartCoroutine(OpenDoor(targetDoor));
                    OpeningDoor = true;
                }
                break;
            case SquadController.Goal.Protecting: //done
                if (MySquad.ProtectTarget != gameObject)
                {
                    transform.LookAt(MySquad.ProtectTarget.transform);
                }
                break;
            default:
                MoveToSquad();
                break;
        }
    }
    void Attack()
    {
        if (FirstShot)
        {
            FireDelta = 2.0f;
            FirstShot = false;
        }
        if (SightOfPlayer)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = transform.position;
            positions[1] = TargetTf.position;
            AimLine.SetPositions(positions);
            AimLineObject.SetActive(true);
            if (PlayerController.IsWounded && NavAgent.remainingDistance >= 1)
            {
                NavAgent.SetDestination(LastKnownPosition);
                NavAgent.isStopped = true;
            }
            else if (PlayerController.IsWounded)
            {
                NavAgent.SetDestination(LastKnownPosition);
                NavAgent.isStopped = false;
            }
            else
            {
                MyRb.velocity = Vector3.zero;
                transform.LookAt(TargetTf);
                NavAgent.isStopped = true;
            }
            FireDelta -= Time.deltaTime;
            switch (MySquad.huntOrCapture)
            {
                case SquadController.HuntOrCapture.Hunt:
                    AimLine.startColor = Color.red;
                    AimLine.endColor = Color.red;
                    if (FireDelta <= 0)
                    {
                        GameObject temp = Instantiate(Bullet, transform.position + (transform.forward * 2), transform.rotation);
                        temp.transform.LookAt(TargetTf);
                        temp.GetComponent<ShotController>().canKill = true;
                        FireDelta = 1.5f;
                    }
                    break;
                case SquadController.HuntOrCapture.Capture:
                    AimLine.startColor = Color.blue;
                    AimLine.endColor = Color.blue;
                    if (FireDelta <= 0 && !PlayerController.IsWounded)
                    {
                        GameObject temp = Instantiate(Bullet, transform.position + (transform.forward * 2), transform.rotation);
                        temp.transform.LookAt(TargetTf);
                        temp.GetComponent<ShotController>().canKill = false;
                        FireDelta = 1.5f;
                    }
                    else if (PlayerController.IsWounded && MySquad.KnownRooms[RoomInfo.RoomType.Server].Count <= 0)
                    {

                    }
                    break;
            }
        }
        else
        {
            FireDelta = 2.0f;
            AimLineObject.SetActive(false);
            NavAgent.SetDestination(LastKnownPosition);
            NavAgent.isStopped = false;
        }

    }
    void MoveToSquad()
    {
        NavAgent.SetDestination(MySquad.transform.position);
        if (NavAgent.remainingDistance >= 2)
        {
            NavAgent.isStopped = false;
        }
        else
        {
            MyRb.velocity = Vector3.zero;
            NavAgent.isStopped = true;
        }
    }
    public void Death() //call RemoveSoldier(), destroy object, instantiate explosion
    {
        if (MySquad != null)
        {
            MySquad.SoldiersInSquad.Remove(gameObject);
            if (MySquad.SoldiersInRange.Contains(gameObject))
            {
                MySquad.SoldiersInRange.Remove(gameObject);
            }
        }
        Destroy(Instantiate(DeathEffect, transform.position, transform.rotation), 2.0f);
        Destroy(AimLineObject);
        Destroy(gameObject);

    }

    public IEnumerator OpenDoor(Door targetDoor)
    {
        while (!targetDoor.IsOpen)
        {
            Debug.Log("opening");
            targetDoor.Open(TechSkill, MySquad);
            yield return new WaitForSeconds(1f);
        }
        MySquad.EvaluateRequest(gameObject, SquadController.SoldierRequest.SearchComplete);
    }
}
