using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Soldier
{
    GameObject currentSquad { get; set; }
    int fear { get; set; } //get returns fear, set modifies fear and clamps between 0-100
    bool isIsolated { get; set; }
    bool isUpgraded { get; set; }
    SquadController.SoldierType myType { get; set; }
    SquadController.Goal squadGoal { get; set; }
    void Death(); //call RemoveSoldier(), destroy object, instantiate explosion
}
