using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFov : MonoBehaviour
{
    public Soldier connectedSoldier;
    public Transform connectedSoldierTf;
    public bool playerInRange;
    private void Start()
    {
        connectedSoldier = connectedSoldierTf.gameObject.GetComponent<Soldier>();
    }
    private void Update()
    {
        transform.position = connectedSoldierTf.position + (connectedSoldierTf.forward * 6);
        transform.rotation = connectedSoldierTf.rotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            connectedSoldier.TargetTf = other.transform;
            playerInRange = true;
        }
        else if((other.gameObject.CompareTag("Decoy") && connectedSoldier.Fear >= 20) && (!playerInRange || connectedSoldier.Fear >= 70))
        {
            connectedSoldier.TargetTf = other.transform;
        }
        else if(connectedSoldier.MyGoal == Soldier.SoldierGoal.Regroup || connectedSoldier.MySquad.GoalList.Peek() == SquadController.Goal.Regroup)
        {

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
