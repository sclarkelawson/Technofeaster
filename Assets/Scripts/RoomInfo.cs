using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public enum RoomType { Server, Armory, Empty }
    public RoomType myType;
    public float specialRoomChance, searchedPercent;
    //public GameObject serverPrefab, armoryPrefab, basicRoomPrefab;
    public Transform entrance;
    public RoomDoor door;
    Renderer myRenderer;
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        GameObject activeSquad = GameObject.FindGameObjectWithTag("Squad");
        float distanceToSquad = Vector3.Distance(activeSquad.transform.position, transform.position);
        specialRoomChance = (100 / activeSquad.GetComponent<SquadController>().uncheckedRooms.Count) * (distanceToSquad / 10);
        Random.InitState((int)System.DateTime.Now.TimeOfDay.TotalSeconds);
    }

    public void EvaluateRoom(SquadController squad)
    {
        if (Random.Range(0, 100) >= 100 - (specialRoomChance + squad.roomLuck))
        {
            SquadController.Goal[] tempGoalList = squad.goalList.ToArray();
            switch (tempGoalList[tempGoalList.Length - 2]) //currentGoal should be Search
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
    }
    public void EvaluateRoom()
    {
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

    }
}
