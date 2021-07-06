using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public enum RoomType { Server, Armory, Empty }
    public float specialRoomChance, searchedPercent;
    public GameObject serverPrefab, armoryPrefab, basicRoomPrefab;
    public Transform entrance, roomPosition;

    void Start()
    {
        GameObject activeSquad = GameObject.FindGameObjectWithTag("Squad");
        float distanceToSquad = Vector3.Distance(activeSquad.transform.position, transform.position);
        specialRoomChance = (100 / activeSquad.GetComponent<SquadController>().uncheckedRooms.Count) * (distanceToSquad / 10);
        Random.InitState((int)System.DateTime.Now.TimeOfDay.TotalSeconds);
    }

    public void EvaluateRoom(SquadController squad)
    {
        if (Random.Range(0, 100) >= specialRoomChance + squad.roomLuck)
        {
            switch (squad.previousGoal) //currentGoal should be Search
            {
                case SquadController.Goal.FindServer:
                    //instantiate server prefab at room position
                    break;
                case SquadController.Goal.FindArmory:
                    //instantiate armory prefab at room position
                    break;
            }
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
                    break;
                case 1:
                    //instantiate armory prefab at room position
                    break;
            }
        }

    }
}
