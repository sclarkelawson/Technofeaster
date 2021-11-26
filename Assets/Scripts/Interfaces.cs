using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface Soldier
{
    public SquadController MySquad { get; set; }
    public int Fear { get; set; } //get returns fear, set modifies fear and clamps between 0-100
    public float EngageTimer { get; set; }
    public float FireDelta { get; set; }
    public float TechSkill { get; set; }
    public float MaxAimDistance { get; set; }
    public bool IsIsolated { get; set; }
    public bool IsUpgraded { get; set; }
    public bool SightOfPlayer { get; set; }
    public bool FirstShot { get; set; }
    public bool Targeted { get; set; }
    public bool OpeningDoor { get; set; }
    public Rigidbody MyRb { get; set; }
    public GameObject DeathEffect { get; set; }
    public GameObject FovPrefab { get; set; }
    public Transform PlayerTf { get; set; }
    public PlayerController PlayerController { get; set; }
    public Vector3 LastKnownPosition { get; set; }
    public SquadController.SoldierType MyType { get; set; }
    public SquadController.Goal SquadGoal { get; set; }
    public enum SoldierGoal { Attack, Regroup, FollowSquad, RequestProtection, Waiting }
    public SoldierGoal MyGoal { get; set; }
    public SoldierGoal LastGoal { get; set; }
    public NavMeshAgent NavAgent { get; set; }
    public LineRenderer AimLine { get; set; }
    public GameObject AimLineObject { get; set; }
    public GameObject Bullet { get; set; }
    public Transform TargetTf { get; set; }
    public Transform FormationTf { get; set; }
    public int LayerMask { get; set; }
    public void Death(); //call RemoveSoldier(), destroy object, instantiate explosion
    public IEnumerator OpenDoor(Door targetDoor);
}

public interface Door
{
    public bool IsOpen { get; set; }
    float PercentOpen { get; set; }
    public List<GameObject> CanOpen { get; set; }
    public Animator Animator { get; set; }
    public void Open();
    public void Open(float value, SquadController squad);
    public void Close();
    public void Toggle();
}

public interface Interactable
{
    public void Toggle();
}
