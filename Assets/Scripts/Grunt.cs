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
    public float maxAimDistance { get { return _maxAimDistance; } set { _maxAimDistance = value; } }
    public Transform playerTf { get { return _playerTf; } set { _playerTf = value; } }
    public PlayerController playerController { get; set; }
    public Vector3 lastKnownPosition { get; set; }
    public bool isIsolated { get; set; }
    public bool isUpgraded { get; set; }
    public bool sightOfPlayer { get; set; }
    public bool firstShot { get; set; }
    public GameObject deathEffect { get { return _deathEffect; } set { _deathEffect = value; } }
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
        layerMask = ~LayerMask.GetMask("Transparent FX");
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
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, maxAimDistance))
            {
                if (hit.transform == targetTf)
                {
                    sightOfPlayer = true;
                    lastKnownPosition = targetTf.position;
                    myGoal = Soldier.SoldierGoal.Attack;
                    if (mySquad.currentGoal == SquadController.Goal.Hunt)
                    {
                        engageTimer = 10f;
                    }
                    else
                    {
                        engageTimer = 10f - (9f / fear + 1);
                    }
                    mySquad.EvaluateRequest(gameObject, this, SquadController.SoldierRequest.Engaging, lastKnownPosition);
                }
                else
                {
                    firstShot = true;
                    sightOfPlayer = false;
                }
            }
            else
            {
                sightOfPlayer = false;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (myGoal)
        {
            case Soldier.SoldierGoal.Attack:
                if (engageTimer > 0)
                {
                    Attack();
                }
                else
                {

                }
                engageTimer -= Time.deltaTime;
                break;
            case Soldier.SoldierGoal.FollowSquad:
                FollowSquad();
                break;
            case Soldier.SoldierGoal.Regroup:
                Regroup();
                break;
        }
    }

    void FollowSquad()
    {
        switch (squadGoal)
        {
            case SquadController.Goal.Hunt:
                break;
            case SquadController.Goal.FindServer:
                break;
            case SquadController.Goal.FindArmory:
                break;
            case SquadController.Goal.Resupply:
                break;
            case SquadController.Goal.Regroup:
                Regroup();
                break;
            case SquadController.Goal.Search:

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
            positions[1] = playerTf.position;
            Vector3 fireDirection = positions[1] - positions[0];
            aimLine.SetPositions(positions);
            aimLineObject.SetActive(true);
            navAgent.SetDestination(lastKnownPosition);
            if (playerController.isWounded)
            {
                navAgent.isStopped = false;
            }
            else
            {
                transform.forward = fireDirection;
                navAgent.isStopped = true;
            }
            if (fireDelta <= 0)
            {
                Instantiate(bullet, transform.position, Quaternion.Euler(fireDirection));
                fireDelta = 1.5f;
            }
            Debug.Log(fireDelta);
            fireDelta -= Time.deltaTime;
            switch (mySquad.huntOrCapture)
            {
                case SquadController.HuntOrCapture.Hunt:
                    aimLine.startColor = Color.red;
                    aimLine.endColor = Color.red;
                    break;
                case SquadController.HuntOrCapture.Capture:
                    aimLine.startColor = Color.blue;
                    aimLine.endColor = Color.blue;
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
    void Regroup()
    {
        navAgent.SetDestination(mySquad.transform.position);
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
}
