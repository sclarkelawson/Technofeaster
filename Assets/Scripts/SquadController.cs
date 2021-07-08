using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquadController : MonoBehaviour
{
    #region enums
    public enum Goal { Hunt, Resupply, Search, FindArmory, FindServer, Regroup, ToTarget, Protecting }
    public enum SoldierRequest { Engaging, Distracted, Protect, ReachedRoom, SearchComplete }
    public enum SoldierType { Grunt, Techie, CyberPriest, Trapper }
    public enum HuntOrCapture { Hunt, Capture }
    #endregion

    #region variables
    //public Goal currentGoal, previousGoal;
    public Stack<Goal> goalList;
    public List<GameObject> soldiersInSquad;
    public List<GameObject> soldiersInRange;
    public float squadSize, protectExpireTimer, averageFear, roomLuck, timer, wanderTimer, wanderRadius;
    public Dictionary<SoldierType, List<GameObject>> availableSoldierInRange;
    //[SerializeField] private GameObject squadPrefab;
    public bool isGoalComplete, sentGoal;
    public GameObject protectTarget;
    public Soldier engagingSoldier;
    public List<RoomInfo> allRooms;
    public List<RoomInfo> uncheckedRooms;
    public Dictionary<RoomInfo.RoomType, List<RoomInfo>> knownRooms;
    //public List<FormationInfo> formationPositions;
    public RoomInfo targetRoom;
    public HuntOrCapture huntOrCapture;
    public Vector3 lastKnownPosition, regroupPosition;
    public NavMeshAgent myAgent;
    #endregion

    void Start()
    {
        uncheckedRooms = new List<RoomInfo>(allRooms);
        sentGoal = false;
        availableSoldierInRange = new Dictionary<SoldierType, List<GameObject>>();
        knownRooms = new Dictionary<RoomInfo.RoomType, List<RoomInfo>>();
        for (int i = 0; i < 4; i++)
        {
            availableSoldierInRange.Add((SoldierType)i, new List<GameObject>());
        }
        for (int i = 0; i < 3; i++)
        {
            knownRooms.Add((RoomInfo.RoomType)i, new List<RoomInfo>());
        }
        targetRoom = SelectRandomRoom(false);
        goalList = new Stack<Goal>();
        goalList.Push(Goal.FindServer);
        myAgent.SetDestination(targetRoom.entrance.position);
        timer = wanderTimer;
        EvaluateSize();
    }
    void Update()
    {
        
        switch (goalList.Peek())
        {
            case Goal.Hunt:
                if (availableSoldierInRange[SoldierType.Techie].Count == 0 && soldiersInRange.Count != soldiersInSquad.Count)
                {
                    goalList.Push(Goal.Regroup);
                    break;
                }
                timer += Time.deltaTime;
                if (timer >= wanderTimer)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    myAgent.SetDestination(newPos);
                    timer = 0;
                }
                break;
            case Goal.FindServer: //done
                if (isGoalComplete)
                {
                    Debug.Log("Goal complete");
                    if (knownRooms[RoomInfo.RoomType.Server].Count > 0)
                    {
                        goalList.Clear();
                        goalList.Push(Goal.Hunt);
                        EvaluateSize();
                    }
                    else
                    {
                        roomLuck += 50 / allRooms.Count;
                        targetRoom = SelectRandomRoom(false);
                        if (targetRoom == null)
                        {
                            Debug.Log("hunting");
                            goalList.Clear();
                            goalList.Push(Goal.Hunt);
                            EvaluateSize();
                            break;
                        }
                        myAgent.SetDestination(targetRoom.entrance.position);
                        isGoalComplete = false;
                    }
                }
                myAgent.isStopped = false;
                if(myAgent.remainingDistance <= 0.1f)
                {
                    goalList.Push(Goal.Search);
                    isGoalComplete = false;
                }
                break;
            case Goal.FindArmory: //done
                if (isGoalComplete)
                {
                    if (knownRooms[RoomInfo.RoomType.Armory].Count > 0)
                    {
                        goalList.Clear();
                        goalList.Push(Goal.Hunt);
                        EvaluateSize();
                    }
                    else
                    {
                        targetRoom = SelectRandomRoom(false);
                        if(targetRoom == null)
                        {
                            goalList.Clear();
                            goalList.Push(Goal.Hunt);
                            EvaluateSize();
                            break;
                        }
                        myAgent.SetDestination(targetRoom.entrance.position);
                        isGoalComplete = false;
                    }
                }
                myAgent.isStopped = false;
                if (myAgent.remainingDistance <= 0.1f)
                {
                    goalList.Push(Goal.FindArmory);
                    isGoalComplete = false;
                }
                break;
            case Goal.Resupply:
                break;
            case Goal.Search: //done
                if (isGoalComplete)
                {
                    Debug.Log("pop Search");
                    goalList.Pop();
                }
                myAgent.isStopped = true;
                EvaluateSize();
                break;
            case Goal.ToTarget: //done
                myAgent.SetDestination(engagingSoldier.lastKnownPosition);
                myAgent.isStopped = false;
                if(engagingSoldier.engageTimer <= 0)
                {
                    goalList.Pop();
                }
                break;
            case Goal.Protecting: //done
                if(protectExpireTimer <= 0)
                {
                    goalList.Pop();
                }
                protectExpireTimer -= Time.deltaTime;
                break;
            case Goal.Regroup: //done
                if ((soldiersInRange.Count / soldiersInSquad.Count) >= 0.75f)
                {
                    goalList.Pop();
                }
                break;
        }
    }
    public void EvaluateSquad()
    {
        for (int i = 0; i < 4; i++)
        {
            availableSoldierInRange[(SoldierType)i].Clear();
        }
        for (int i = 0; i < soldiersInRange.Count; i++)
        {
            availableSoldierInRange[soldiersInRange[i].GetComponent<Soldier>().myType].Add(soldiersInRange[i].gameObject);
        }
        float tempFear = 0;
        for (int i = 0; i < soldiersInSquad.Count; i++)
        {
            tempFear += soldiersInSquad[i].GetComponent<Soldier>().fear;
        }
        tempFear /= soldiersInSquad.Count;
        averageFear = tempFear;
        //if (1 >= 1)
        //{
        //    huntOrCapture = HuntOrCapture.Capture;
        //    if(knownRooms[RoomInfo.RoomType.Server].Count == 0)
        //    {
        //        currentGoal = Goal.FindServer;
        //    }
        //    else if (knownRooms[RoomInfo.RoomType.Server].Count > 0)
        //    {
                
        //    }
            
        //}
    }
    public void EvaluateRequest(GameObject requester, Soldier requesterSoldier, SoldierRequest request, Vector3 targetPosition)
    {
        
        switch (request)
        {
            case SoldierRequest.Engaging:
                lastKnownPosition = targetPosition;
                engagingSoldier = requesterSoldier;
                if (soldiersInRange.Contains(requester) && goalList.Peek() != Goal.ToTarget)
                {
                    goalList.Push(Goal.ToTarget);
                }
                break;
            case SoldierRequest.Distracted:
                if(soldiersInRange.Contains(requester) && averageFear >= 20)
                {
                    lastKnownPosition = targetPosition;
                }
                break;
        }
    }
    public void EvaluateRequest(GameObject requester, SoldierRequest request)
    {
        Debug.Log("Evaluating request: " + request);
        switch (request)
        {
            case SoldierRequest.Protect: //done
                if (soldiersInRange.Contains(requester) || goalList.Peek() == Goal.Hunt)
                {
                    protectTarget = requester;
                    if (goalList.Peek() != Goal.Protecting)
                    {
                        goalList.Push(Goal.Protecting);
                    }
                    protectExpireTimer = 3.0f;
                }
                break;
            case SoldierRequest.ReachedRoom:
                goalList.Push(Goal.Search);
                break;
            case SoldierRequest.SearchComplete:
                if(goalList.Peek() == Goal.Search)
                {
                    knownRooms[targetRoom.myType].Add(targetRoom);
                    Debug.Log("removing " + targetRoom.gameObject.name);
                    uncheckedRooms.Remove(targetRoom);
                    isGoalComplete = true;
                }
                break;
        }
    }
    void EvaluateSize() //done, untested
    {
        squadSize = Mathf.Clamp(soldiersInRange.Count * 3.5f, 5f, 21f);
        transform.localScale = new Vector3(squadSize, 3, squadSize);
        if ((soldiersInRange.Count / soldiersInSquad.Count) <= 0.5f)
        {
            sentGoal = false;
            goalList.Push(Goal.Regroup);
        }
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) //https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    void SendGoal(Goal goal)
    {
        for (int i = 0; i < soldiersInSquad.Count; i++)
        {
            if (!soldiersInRange.Contains(soldiersInSquad[i]))
            {
                soldiersInSquad[i].GetComponent<Soldier>().squadGoal = Goal.Regroup;
            }
        }
    }
    RoomInfo SelectRandomRoom(bool includeChecked) //done, untested
    {
        if(!includeChecked && uncheckedRooms.Count > 0)
        {
            return uncheckedRooms[Random.Range(0, uncheckedRooms.Count)];
        }
        else if (includeChecked)
        {
            return allRooms[Random.Range(0, uncheckedRooms.Count)];
        }
        return null;
    }
    RoomInfo SelectClosestRoom(Vector3 start, bool includeChecked) //done, untested
    {
        if (includeChecked)
        {
            uncheckedRooms.Sort(delegate (RoomInfo a, RoomInfo b)
            {
                return (start - a.transform.position).sqrMagnitude
                .CompareTo(
                  (start - b.transform.position).sqrMagnitude);
            }); //https://answers.unity.com/questions/341065/sort-a-list-of-gameobjects-by-distance.html
            return allRooms[0];
        }
        else
        {
            uncheckedRooms.Sort(delegate (RoomInfo a, RoomInfo b)
            {
                return (start - a.transform.position).sqrMagnitude
                .CompareTo(
                  (start - b.transform.position).sqrMagnitude);
            }); //https://answers.unity.com/questions/341065/sort-a-list-of-gameobjects-by-distance.html
            return uncheckedRooms[0];
        }
        
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
        if (!soldiersInRange.Contains(other.gameObject) && other.CompareTag("Enemy"))
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
