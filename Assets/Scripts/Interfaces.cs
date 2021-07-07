using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface Soldier
{
    public SquadController mySquad { get; set; }
    public int fear { get; set; } //get returns fear, set modifies fear and clamps between 0-100
    public float engageTimer { get; set; }
    public float fireDelta { get; set; }
    public float maxAimDistance { get; set; }
    public bool isIsolated { get; set; }
    public bool isUpgraded { get; set; }
    public bool sightOfPlayer { get; set; }
    public bool firstShot { get; set; }
    public GameObject deathEffect { get; set; }
    public Transform playerTf { get; set; }
    public PlayerController playerController { get; set; }
    public Vector3 lastKnownPosition { get; set; }
    public SquadController.SoldierType myType { get; set; }
    public SquadController.Goal squadGoal { get; set; }
    public enum SoldierGoal { Attack, Regroup, FollowSquad }
    public SoldierGoal myGoal { get; set; }
    public SoldierGoal lastGoal { get; set; }
    public NavMeshAgent navAgent { get; set; }
    public LineRenderer aimLine { get; set; }
    public GameObject aimLineObject { get; set; }
    public GameObject bullet { get; set; }
    public Transform targetTf { get; set; }
    public Transform formationTf { get; set; }
    public int layerMask { get; set; }
    public void Death(); //call RemoveSoldier(), destroy object, instantiate explosion
}

public interface Door
{
    public float percentOpen { get; set; }
    public List<GameObject> currentlyOpening { get; set; }
    public Animator animator { get; set; }
    public void Open(float value, bool isPlayer);
    public void Close();
}
