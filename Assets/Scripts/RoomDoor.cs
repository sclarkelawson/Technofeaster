using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour, Door
{
    public float percentOpen { get; set; }
    public List<GameObject> currentlyOpening { get; set; }
    public Animator animator { get; set; }
    public RoomInfo connectedRoom;
    void Start() //Start closed
    {
        percentOpen = 0;
        animator = gameObject.GetComponent<Animator>();
    }
    public void Open(float value, bool isPlayer)
    {
        if(percentOpen >= 100)
        {
            animator.Play("open");
            if (isPlayer)
            {
                connectedRoom.EvaluateRoom();
            }
        }
        else
        {
            percentOpen += value;
        }
        
    }
    public void Close()
    {
        animator.Play("close");
    }
}
