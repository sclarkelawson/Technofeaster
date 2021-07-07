using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFov : MonoBehaviour
{
    public Soldier connectedSoldier;
    public bool playerInRange;
    private void Start()
    {
        connectedSoldier = GetComponentInParent<Soldier>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            connectedSoldier.targetTf = other.transform;
            playerInRange = true;
        }
        else if((other.gameObject.CompareTag("Decoy") && connectedSoldier.fear >= 20) && (!playerInRange || connectedSoldier.fear >= 70))
        {
            connectedSoldier.targetTf = other.transform;
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
