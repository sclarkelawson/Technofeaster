using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : MonoBehaviour, Soldier
{
    [SerializeField] private int _fear;
    [SerializeField] private GameObject _deathEffect;
    public SquadController currentSquad { get; set; }
    public int fear 
    { 
        get { return _fear; } 
        set 
        {
            _fear = Mathf.Clamp(value, 0, 100);
        } 
    } //get returns fear, set modifies fear and clamps between 0-100

    public bool isIsolated { get; set; }
    public bool isUpgraded { get; set; }
    public GameObject deathEffect { get { return _deathEffect;  } set { _deathEffect = value;  } }
    public SquadController.SoldierType myType { get; set; }

    public SquadController.Goal squadGoal { get; set; }
    public void Death() //call RemoveSoldier(), destroy object, instantiate explosion
    {
        //currentSquad.RemoveSoldier(gameObject, this);
        Destroy(Instantiate(deathEffect, transform.position, transform.rotation), 2.0f);
        Destroy(gameObject);
    }

    private void Start()
    {

    }
}
