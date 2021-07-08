using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour, Door
{
    public bool isOpen { get; set; }
    public float percentOpen { get; set; }
    public List<GameObject> canOpen { get; set; }
    public Animator animator { get; set; }

    public RoomInfo connectedRoom;
    
    void Start() //Start closed
    {
        percentOpen = 0;
        animator = gameObject.GetComponentInChildren<Animator>();
        canOpen = new List<GameObject>();
        isOpen = false;
    }
    public void Open()
    {
        Debug.Log("opening");
        animator.Play("open");
        isOpen = true;
        if (!connectedRoom.evaluated)
        {
            connectedRoom.EvaluateRoom();
        }
    }
    public void Open(float value, SquadController squad)
    {
        if (percentOpen >= 100)
        {
            isOpen = true;
            animator.Play("open");
            if (!connectedRoom.evaluated)
            {
                connectedRoom.EvaluateRoom(squad);
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
        isOpen = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !canOpen.Contains(other.gameObject))
        {
            canOpen.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (canOpen.Contains(other.gameObject))
        {
            canOpen.Remove(other.gameObject);
        }
    }
}
