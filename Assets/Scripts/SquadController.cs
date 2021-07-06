using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquadController : MonoBehaviour
{
    #region enums
    public enum Goal { Hunt, Resupply, Search, FindArmory, FindServer, Regroup, ToTarget, Protecting }
    public enum SoldierRequest { Engaging, Distracted, Protect, ReachedRoom }
    public enum SoldierType { Grunt, Techie, CyberPriest, Trapper }
    public enum HuntOrCapture { Hunt, Capture }
    #endregion

    #region variables
    public Goal currentGoal, previousGoal;
    public List<GameObject> soldiersInSquad;
    public List<GameObject> soldiersInRange;
    public float squadSize, protectExpireTimer, averageFear, roomLuck;
    public Dictionary<SoldierType, List<GameObject>> availableSoldierTypes;
    [SerializeField] private GameObject squadPrefab;
    public bool isGoalComplete;
    public GameObject protectTarget;
    public List<RoomInfo> uncheckedRooms;
    public Dictionary<RoomInfo.RoomType, List<Vector3>> knownRooms;
    public RoomInfo targetRoom;
    public HuntOrCapture huntOrCapture;
    public Vector3 lastKnownPosition, regroupPosition;
    #endregion

    void Start()
    {
        EvaluateSize();
    }
    void Update()
    {
        switch (currentGoal)
        {
            case Goal.Hunt:
                
                break;
            case Goal.FindServer:
                if (isGoalComplete)
                {
                    SelectRandomRoom();
                    isGoalComplete = false;
                }
                break;
            case Goal.FindArmory:
                break;
            case Goal.Resupply:
                break;
            case Goal.Search:
                if (isGoalComplete)
                {
                    currentGoal = previousGoal;
                }
                break;
            case Goal.ToTarget:
                for (int i = 0; i < soldiersInRange.Count; i++)
                {
                    soldiersInRange[i].GetComponent<Soldier>().lastKnownPosition = lastKnownPosition;
                }
                break;
            case Goal.Protecting:
                if(protectExpireTimer <= 0)
                {
                    currentGoal = previousGoal;
                }
                protectExpireTimer -= Time.deltaTime;
                break;
        }
    }
    //public void AddSoldier(GameObject soldierObject, Soldier currentSoldier)
    //{
    //    soldiersInRange.Add(soldierObject);
    //    availableSoldierTypes[currentSoldier.myType].Add(soldierObject);
    //    if (soldiers.Count == 1)
    //    {
    //        currentSoldier.isIsolated = true;
    //    }
    //    else if(currentSoldier.isIsolated)
    //    {
    //        currentSoldier.isIsolated = false;
    //    }
    //    EvaluateSize();
    //}
    //public void RemoveSoldier(GameObject soldierObject, Soldier currentSoldier)
    //{
    //    soldiers.Remove(soldierObject);
    //    availableSoldierTypes[currentSoldier.myType].Remove(soldierObject);
    //    if (soldiers.Count == 0)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else if (soldiers.Count == 1)
    //    {
    //        soldiers[0].GetComponent<Soldier>().isIsolated = true;
    //    }
    //    EvaluateSize();
    //}
    void EvaluateGoal()
    {
        int numberOfGrunts = availableSoldierTypes[SoldierType.Grunt].Count, numberOfTechies = availableSoldierTypes[SoldierType.Techie].Count, numberOfPriests = availableSoldierTypes[SoldierType.CyberPriest].Count, numberOfTrappers = availableSoldierTypes[SoldierType.Trapper].Count, total = soldiersInSquad.Count;
        float tempFear = 0;
        for (int i = 0; i < soldiersInRange.Count; i++)
        {
            tempFear += soldiersInSquad[i].GetComponent<Soldier>().fear;
        }
        tempFear /= soldiersInSquad.Count;
        if (numberOfTechies >= 1)
        {
            huntOrCapture = HuntOrCapture.Capture;
            if(knownRooms[RoomInfo.RoomType.Server].Count == 0)
            {
                currentGoal = Goal.FindServer;
            }
            else if (knownRooms[RoomInfo.RoomType.Server].Count > 0)
            {
                
            }
            
        }
    }
    public void EvaluateRequest(GameObject requester, Soldier requesterSoldier, SoldierRequest request, Vector3 targetPosition)
    {
        switch (request)
        {
            case SoldierRequest.Engaging:
                lastKnownPosition = targetPosition;
                if (soldiersInRange.Contains(requester))
                {
                    currentGoal = Goal.ToTarget;
                }
                break;
            case SoldierRequest.Protect:
                if (soldiersInRange.Contains(requester))
                {
                    protectTarget = requester;
                    if(currentGoal != Goal.Protecting)
                    {
                        previousGoal = currentGoal;
                        currentGoal = Goal.Protecting;
                    }
                    protectExpireTimer = 3.0f;
                }
                break;
            case SoldierRequest.Distracted:
                if(soldiersInRange.Contains(requester) && averageFear >= 20)
                {
                    lastKnownPosition = targetPosition;
                }
                break;
            case SoldierRequest.ReachedRoom:
                previousGoal = currentGoal;
                currentGoal = Goal.Search;
                break;
        }
    }
    void EvaluateSize() //done, untested
    {
        squadSize = soldiersInRange.Count * 3.5f;
        transform.localScale = new Vector3(squadSize, 3, squadSize);
        if (soldiersInRange.Count / soldiersInSquad.Count <= 0.5f)
        {
            currentGoal = Goal.Regroup;
        }
    }
    void SelectRandomRoom() //done, untested
    {
        targetRoom = uncheckedRooms[Random.Range(0, uncheckedRooms.Count)];
    }
    void SelectClosestRoom() //done, untested
    {
        uncheckedRooms.Sort(delegate (RoomInfo a, RoomInfo b)
        {
            return (this.transform.position - a.transform.position).sqrMagnitude
            .CompareTo(
              (this.transform.position - b.transform.position).sqrMagnitude);
        }); //https://answers.unity.com/questions/341065/sort-a-list-of-gameobjects-by-distance.html
        targetRoom = uncheckedRooms[0];
    }
    public static float GetPathLength(NavMeshPath path)
    {
        float length = 0.0f;

        if (path.status != NavMeshPathStatus.PathInvalid)
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return length;
    } //https://forum.unity.com/threads/getting-the-distance-in-nav-mesh.315846/
    private void OnTriggerEnter(Collider other) //done, untested
    {
        if (!soldiersInRange.Contains(other.gameObject))
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            newSoldier.isIsolated = false;
            newSoldier.fear -= 5;
            soldiersInRange.Add(other.gameObject);
            EvaluateSize();
        }
    }

    private void OnTriggerExit(Collider other) //done, untested
    {
        if (soldiersInRange.Contains(other.gameObject)) //find path to target, if too far remove from squad and create new squad
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            NavMeshPath path = new NavMeshPath();
            if(NavMesh.CalculatePath(other.transform.position, transform.position, NavMesh.AllAreas, path))
            {
                if (GetPathLength(path) > 5f)
                {
                    newSoldier.isIsolated = true;
                    newSoldier.fear += 10;
                }
            }
            soldiersInRange.Remove(other.gameObject);
            EvaluateSize();
        }
    }
}
