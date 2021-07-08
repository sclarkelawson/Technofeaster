using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearRadius : MonoBehaviour
{
    public int fear;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Soldier>() != null)
        {
            Soldier targetSoldier = other.GetComponent<Soldier>();
            targetSoldier.fear += fear;
        }
    }
}
