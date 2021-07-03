using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public enum RoomType { Server, Armory, Empty }
    public float chanceOfServer, chanceOfArmory;
    public Vector3 entrance;


    void Start()
    {
        GameObject[] activeSquads = GameObject.FindGameObjectsWithTag("Squad");
        Random.InitState((int)System.DateTime.Now.TimeOfDay.TotalSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
