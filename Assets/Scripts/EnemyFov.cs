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
