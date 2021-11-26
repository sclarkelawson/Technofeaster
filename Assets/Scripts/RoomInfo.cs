using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public enum RoomType { Server, Armory, Empty }
    public RoomType myType;
    public float specialRoomChance, searchedPercent;
    public bool evaluated;
    //public GameObject serverPrefab, armoryPrefab, basicRoomPrefab;
    public Transform entrance;
    public RoomDoor door;
    Renderer myRenderer;
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        GameObject activeSquad = GameObject.FindGameObjectWithTag("Squad");
        float distanceToSquad = Vector3.Distance(activeSquad.transform.position, transform.position);
        specialRoomChance = (100 / activeSquad.GetComponent<SquadController>().UncheckedRooms.Count) * (distanceToSquad / 100);
        Random.InitState((int)System.DateTime.Now.TimeOfDay.TotalSeconds);
        evaluated = false;
    }

    public void EvaluateRoom(SquadController squad)
    {
        if (evaluated)
        {
            return;
        }
        if (Random.Range(0, 100) >= 100 - (specialRoomChance + squad.RoomLuck))
        {
            SquadController.Goal[] tempGoalList = squad.GoalList.ToArray();
            Debug.Log(tempGoalList[tempGoalList.Length - 2]);
            switch (tempGoalList[tempGoalList.Length - 2])
            {
                case SquadController.Goal.FindServer:
                    //instantiate server prefab at room position
                    myRenderer.material.SetColor("_Color", Color.blue);
                    myType = RoomType.Server;
                    break;
                case SquadController.Goal.FindArmory:
                    //instantiate armory prefab at room position
                    myRenderer.material.SetColor("_Color", Color.red);
                    myType = RoomType.Armory;
                    break;
            }
        }
        else
        {
            myRenderer.material.SetColor("_Color", Color.grey);
            myType = RoomType.Empty;
        }
        evaluated = true;
    }
    public void EvaluateRoom()
    {
        if (evaluated)
        {
            return;
        }
        if (Random.Range(0, 100) >= specialRoomChance)
        {
            switch (Random.Range(0f, 1f))
            {
                case 0:
                    //instantiate server prefab at room position
                    myRenderer.material.SetColor("_Color", Color.blue);
                    myType = RoomType.Server;
                    break;
                case 1:
                    //instantiate armory prefab at room position
                    myRenderer.material.SetColor("_Color", Color.red);
                    myType = RoomType.Armory;
                    break;
            }
        }
        else
        {
            myRenderer.material.SetColor("_Color", Color.gray);
            myType = RoomType.Empty;
        }
        evaluated = true;
    }
}
