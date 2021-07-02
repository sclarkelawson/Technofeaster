using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BBCore;
using Pada1.Xml.Serializer.Utils;

public class SquadController : MonoBehaviour
{
    public enum Goals { }
    public List<GameObject> soldiers;
    public float squadSize;

    // Start is called before the first frame update
    void Start()
    {
        if(soldiers.Count <= 1)
        {
            soldiers[0].GetComponent<Soldier>().isIsolated = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    void AddSoldier(GameObject soldier)
    {

    }

    void RemoveSoldier(GameObject soldier)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!soldiers.Contains(other.gameObject))
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            newSoldier.currentSquad = gameObject;
            AddSoldier(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (soldiers.Contains(other.gameObject)) //find path to target, if too far remove from squad and create new squad
        {

        }
    }
}
