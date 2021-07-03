using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Soldier
{
    public SquadController currentSquad { get; set; }
    public int fear { get; set; } //get returns fear, set modifies fear and clamps between 0-100
    public bool isIsolated { get; set; }
    public bool isUpgraded { get; set; }
    public GameObject deathEffect { get; set; }
    public SquadController.SoldierType myType { get; set; }
    public SquadController.Goal squadGoal { get; set; }
    public void Death(); //call RemoveSoldier(), destroy object, instantiate explosion
}
