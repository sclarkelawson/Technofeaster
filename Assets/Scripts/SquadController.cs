using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    #region enums
    public enum Goal { Hunt, Resupply, Search, FindArmory, FindServer, Capture, Regroup }
    public enum SoldierType { Grunt, Techie, CyberPriest, Trapper }
    
    #endregion

    #region variables
    public Goal currentGoal;
    public List<GameObject> soldiers;
    public float squadSize;
    public Dictionary<SoldierType, List<GameObject>> availableSoldierTypes;
    [SerializeField] private GameObject squadPrefab;
    public bool isGoalComplete;
    public List<RoomInfo> uncheckedRooms;
    public Dictionary<RoomInfo.RoomType, List<Vector3>> knownRooms;
    #endregion

    void Start()
    {
        
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
    

    public void AddSoldier(GameObject soldierObject, Soldier currentSoldier)
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
        EvaluateSize();
    }

    public void RemoveSoldier(GameObject soldierObject, Soldier currentSoldier)
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
        EvaluateSize();
    }

    void EvaluateGoal()
    {
        int numberOfGrunts = availableSoldierTypes[SoldierType.Grunt].Count, numberOfTechies = availableSoldierTypes[SoldierType.Techie].Count, numberOfPriests = availableSoldierTypes[SoldierType.CyberPriest].Count, numberOfTrappers = availableSoldierTypes[SoldierType.Trapper].Count, total = soldiers.Count;
        if(numberOfTechies >= 1 && knownRooms[RoomInfo.RoomType.Server].Count == 0)
        {
            currentGoal = Goal.FindServer;
        }
    }

    void EvaluateSize()
    {
        squadSize = soldiers.Count * 3.5f;
        transform.localScale = new Vector3(squadSize, 3, squadSize);
    }

    void FindNextRoom()
    {
        RoomInfo targetRoom = uncheckedRooms[Random.Range(0, uncheckedRooms.Count)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!soldiers.Contains(other.gameObject))
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            newSoldier.currentSquad = this;
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
            newSoldier.currentSquad = this;
            RemoveSoldier(other.gameObject, newSoldier);
        }
    }
}
