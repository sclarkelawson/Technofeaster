using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : MonoBehaviour, Soldier
{
    public SquadController currentSquad { get; set; }
    public int fear 
    { 
        get { return fear; } 
        set 
        {
            fear = Mathf.Clamp(value, 0, 100);
        } 
    } //get returns fear, set modifies fear and clamps between 0-100

    public bool isIsolated { get; set; }
    public bool isUpgraded { get; set; }
    public GameObject deathEffect { get; set; }
    public SquadController.SoldierType myType { get; set; }

    public SquadController.Goal squadGoal { get; set; }
    public void Death() //call RemoveSoldier(), destroy object, instantiate explosion
    {
        currentSquad.RemoveSoldier(gameObject, this);
        Instantiate(deathEffect, transform.position, transform.rotation);
    }
}
