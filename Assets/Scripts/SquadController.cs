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
    public Stack<Goal> GoalList;
    public List<GameObject> SoldiersInSquad;
    public List<GameObject> SoldiersInRange;
    public float SquadSize, ProtectExpireTimer, AverageFear, RoomLuck, Timer, WanderTimer, WanderRadius;
    public Dictionary<SoldierType, List<GameObject>> AvailableSoldierInRange;
    //[SerializeField] private GameObject squadPrefab;
    public bool IsGoalComplete, SentGoal;
    public GameObject ProtectTarget;
    public Soldier EngagingSoldier;
    public List<RoomInfo> AllRooms;
    public List<RoomInfo> UncheckedRooms;
    public Dictionary<RoomInfo.RoomType, List<RoomInfo>> KnownRooms;
    //public List<FormationInfo> formationPositions;
    public RoomInfo TargetRoom;
    public HuntOrCapture huntOrCapture;
    public Vector3 LastKnownPosition, RegroupPosition;
    public NavMeshAgent myAgent;
    #endregion

    void Start()
    {
        UncheckedRooms = new List<RoomInfo>(AllRooms);
        SentGoal = false;
        AvailableSoldierInRange = new Dictionary<SoldierType, List<GameObject>>();
        KnownRooms = new Dictionary<RoomInfo.RoomType, List<RoomInfo>>();
        for (int i = 0; i < 4; i++)
        {
            AvailableSoldierInRange.Add((SoldierType)i, new List<GameObject>());
        }
        for (int i = 0; i < 3; i++)
        {
            KnownRooms.Add((RoomInfo.RoomType)i, new List<RoomInfo>());
        }
        TargetRoom = SelectRandomRoom(false);
        GoalList = new Stack<Goal>();
        GoalList.Push(Goal.FindServer);
        myAgent.SetDestination(TargetRoom.entrance.position);
        Timer = WanderTimer;
        EvaluateSize();
    }
    void Update()
    {
        
        switch (GoalList.Peek())
        {
            case Goal.Hunt:
                if (AvailableSoldierInRange[SoldierType.Techie].Count == 0 && SoldiersInRange.Count != SoldiersInSquad.Count)
                {
                    GoalList.Push(Goal.Regroup);
                    break;
                }
                Timer += Time.deltaTime;
                if (Timer >= WanderTimer)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, WanderRadius, -1);
                    myAgent.SetDestination(newPos);
                    Timer = 0;
                }
                break;
            case Goal.FindServer: //done
                if (IsGoalComplete)
                {
                    Debug.Log("Goal complete");
                    if (KnownRooms[RoomInfo.RoomType.Server].Count > 0)
                    {
                        GoalList.Clear();
                        GoalList.Push(Goal.Hunt);
                        EvaluateSize();
                    }
                    else
                    {
                        RoomLuck += 50 / AllRooms.Count;
                        TargetRoom = SelectRandomRoom(false);
                        if (TargetRoom == null)
                        {
                            Debug.Log("hunting");
                            GoalList.Clear();
                            GoalList.Push(Goal.Hunt);
                            EvaluateSize();
                            break;
                        }
                        myAgent.SetDestination(TargetRoom.entrance.position);
                        IsGoalComplete = false;
                    }
                }
                myAgent.isStopped = false;
                if(myAgent.remainingDistance <= 0.1f)
                {
                    GoalList.Push(Goal.Search);
                    IsGoalComplete = false;
                }
                break;
            case Goal.FindArmory: //done
                if (IsGoalComplete)
                {
                    if (KnownRooms[RoomInfo.RoomType.Armory].Count > 0)
                    {
                        GoalList.Clear();
                        GoalList.Push(Goal.Hunt);
                        EvaluateSize();
                    }
                    else
                    {
                        TargetRoom = SelectRandomRoom(false);
                        if(TargetRoom == null)
                        {
                            GoalList.Clear();
                            GoalList.Push(Goal.Hunt);
                            EvaluateSize();
                            break;
                        }
                        myAgent.SetDestination(TargetRoom.entrance.position);
                        IsGoalComplete = false;
                    }
                }
                myAgent.isStopped = false;
                if (myAgent.remainingDistance <= 0.1f)
                {
                    GoalList.Push(Goal.FindArmory);
                    IsGoalComplete = false;
                }
                break;
            case Goal.Resupply:
                break;
            case Goal.Search: //done
                if (IsGoalComplete)
                {
                    Debug.Log("pop Search");
                    GoalList.Pop();
                }
                myAgent.isStopped = true;
                EvaluateSize();
                break;
            case Goal.ToTarget: //done
                myAgent.SetDestination(EngagingSoldier.LastKnownPosition);
                myAgent.isStopped = false;
                if(EngagingSoldier.EngageTimer <= 0)
                {
                    GoalList.Pop();
                }
                break;
            case Goal.Protecting: //done
                if(ProtectExpireTimer <= 0)
                {
                    GoalList.Pop();
                }
                ProtectExpireTimer -= Time.deltaTime;
                break;
            case Goal.Regroup: //done
                if ((SoldiersInRange.Count / SoldiersInSquad.Count) >= 0.75f)
                {
                    GoalList.Pop();
                }
                break;
        }
    }
    public void EvaluateSquad()
    {
        for (int i = 0; i < 4; i++)
        {
            AvailableSoldierInRange[(SoldierType)i].Clear();
        }
        for (int i = 0; i < SoldiersInRange.Count; i++)
        {
            AvailableSoldierInRange[SoldiersInRange[i].GetComponent<Soldier>().MyType].Add(SoldiersInRange[i].gameObject);
        }
        float tempFear = 0;
        for (int i = 0; i < SoldiersInSquad.Count; i++)
        {
            tempFear += SoldiersInSquad[i].GetComponent<Soldier>().Fear;
        }
        tempFear /= SoldiersInSquad.Count;
        AverageFear = tempFear;
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
                LastKnownPosition = targetPosition;
                EngagingSoldier = requesterSoldier;
                if (SoldiersInRange.Contains(requester) && GoalList.Peek() != Goal.ToTarget)
                {
                    GoalList.Push(Goal.ToTarget);
                }
                break;
            case SoldierRequest.Distracted:
                if(SoldiersInRange.Contains(requester) && AverageFear >= 20)
                {
                    LastKnownPosition = targetPosition;
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
                if (SoldiersInRange.Contains(requester) || GoalList.Peek() == Goal.Hunt)
                {
                    ProtectTarget = requester;
                    if (GoalList.Peek() != Goal.Protecting)
                    {
                        GoalList.Push(Goal.Protecting);
                    }
                    ProtectExpireTimer = 3.0f;
                }
                break;
            case SoldierRequest.ReachedRoom:
                GoalList.Push(Goal.Search);
                break;
            case SoldierRequest.SearchComplete:
                if(GoalList.Peek() == Goal.Search)
                {
                    KnownRooms[TargetRoom.myType].Add(TargetRoom);
                    Debug.Log("removing " + TargetRoom.gameObject.name);
                    UncheckedRooms.Remove(TargetRoom);
                    IsGoalComplete = true;
                }
                break;
        }
    }
    void EvaluateSize() //done, untested
    {
        SquadSize = Mathf.Clamp(SoldiersInRange.Count * 3.5f, 5f, 21f);
        transform.localScale = new Vector3(SquadSize, 3, SquadSize);
        if ((SoldiersInRange.Count / SoldiersInSquad.Count) <= 0.5f)
        {
            SentGoal = false;
            GoalList.Push(Goal.Regroup);
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
        for (int i = 0; i < SoldiersInSquad.Count; i++)
        {
            if (!SoldiersInRange.Contains(SoldiersInSquad[i]))
            {
                SoldiersInSquad[i].GetComponent<Soldier>().SquadGoal = Goal.Regroup;
            }
        }
    }
    RoomInfo SelectRandomRoom(bool includeChecked) //done, untested
    {
        if(!includeChecked && UncheckedRooms.Count > 0)
        {
            return UncheckedRooms[Random.Range(0, UncheckedRooms.Count)];
        }
        else if (includeChecked)
        {
            return AllRooms[Random.Range(0, UncheckedRooms.Count)];
        }
        return null;
    }
    RoomInfo SelectClosestRoom(Vector3 start, bool includeChecked) //done, untested
    {
        if (includeChecked)
        {
            UncheckedRooms.Sort(delegate (RoomInfo a, RoomInfo b)
            {
                return (start - a.transform.position).sqrMagnitude
                .CompareTo(
                  (start - b.transform.position).sqrMagnitude);
            }); //https://answers.unity.com/questions/341065/sort-a-list-of-gameobjects-by-distance.html
            return AllRooms[0];
        }
        else
        {
            UncheckedRooms.Sort(delegate (RoomInfo a, RoomInfo b)
            {
                return (start - a.transform.position).sqrMagnitude
                .CompareTo(
                  (start - b.transform.position).sqrMagnitude);
            }); //https://answers.unity.com/questions/341065/sort-a-list-of-gameobjects-by-distance.html
            return UncheckedRooms[0];
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
        if (!SoldiersInRange.Contains(other.gameObject) && other.CompareTag("Enemy"))
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            newSoldier.IsIsolated = false;
            newSoldier.Fear -= 5;
            SoldiersInRange.Add(other.gameObject);
            EvaluateSize();
        }
    }
    private void OnTriggerExit(Collider other) //done, untested
    {
        if (SoldiersInRange.Contains(other.gameObject)) //find path to target, if too far remove from squad and create new squad
        {
            Soldier newSoldier = other.GetComponent<Soldier>();
            NavMeshPath path = new NavMeshPath();
            if(NavMesh.CalculatePath(other.transform.position, transform.position, NavMesh.AllAreas, path))
            {
                if (GetPathLength(path) > 5f)
                {
                    newSoldier.IsIsolated = true;
                    newSoldier.Fear += 10;
                }
            }
            SoldiersInRange.Remove(other.gameObject);
            EvaluateSize();
        }
    }
}
