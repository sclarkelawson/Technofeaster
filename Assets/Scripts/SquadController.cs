using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BBCore;
using Pada1.Xml.Serializer.Utils;

public class SquadController : MonoBehaviour
{
    public enum Goal { Hunt, Resupply, Search, FindArmory, FindServer }
    public enum SoldierType { Grunt, Techie, CyberPriest, Bomber }
    public Goal currentGoal;
    public List<GameObject> soldiers;
    public float squadSize;
    public Dictionary<string, Vector3> knownLocations;
    public Dictionary<SoldierType, List<GameObject>> availableSoldierTypes;
    public GameObject squadPrefab;

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
        switch (currentGoal)
        {
            case Goal.Hunt:
                for(int i = 0; i < soldiers.Count; i++)
                {

                }
                break;
            case Goal.FindServer:
                break;
            case Goal.FindArmory:
                break;
            case Goal.Resupply:
                break;
            case Goal.Search:
                break;
        }
    }
    

    void AddSoldier(GameObject soldierObject, Soldier currentSoldier)
    {
        soldiers.Add(soldierObject);
        availableSoldierTypes[currentSoldier.myType].Add(soldierObject);
        if (soldiers.Count == 1)
        {
            currentSoldier.isIsolated = true;
        }
        else if(currentSoldier.isIsolated)
        {
            currentSoldier.isIsolated = false;
        }
    }

    void RemoveSoldier(GameObject soldierObject, Soldier currentSoldier)
    {
        soldiers.Remove(soldierObject);
        availableSoldierTypes[currentSoldier.myType].Remove(soldierObject);
        if (soldiers.Count == 0)
        {
            Destroy(gameObject);
        }
        else if (soldiers.Count == 1)
        {
            soldiers[0].GetComponent<Soldier>().isIsolated = true;
        }
    }

    void EvaluateGoal()
    {
        int numberOfGrunts = availableSoldierTypes[SoldierType.Grunt].Count, numberOfTechies = availableSoldierTypes[SoldierType.Techie].Count, numberOfPriests = availableSoldierTypes[SoldierType.CyberPriest].Count, numberOfBombers = availableSoldierTypes[SoldierType.Bomber].Count, total = soldiers.Count;
        if(numberOfTechies >= 1)
        {
            currentGoal = Goal.FindServer;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!soldiers.Contains(other.gameObject))
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            newSoldier.currentSquad = gameObject;
            AddSoldier(other.gameObject, newSoldier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (soldiers.Contains(other.gameObject)) //find path to target, if too far remove from squad and create new squad
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            SquadController newSquad = Instantiate(squadPrefab).GetComponent<SquadController>();
            newSquad.AddSoldier(other.gameObject, newSoldier);
            newSoldier.currentSquad = gameObject;
            RemoveSoldier(other.gameObject, newSoldier);
        }
    }
}
