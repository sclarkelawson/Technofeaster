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
    }
    public void Open(float value)
    {
        if(percentOpen >= 100)
        {
            animator.Play("open");
            isOpen = true;
            connectedRoom.EvaluateRoom();
        }
        else
        {
            percentOpen += value;
        }
    }
    public void Open(float value, SquadController squad)
    {
        if (percentOpen >= 100)
        {
            isOpen = true;
            animator.Play("open");
            connectedRoom.EvaluateRoom(squad);
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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Enemy") && !canOpen.Contains(other.gameObject))
        {
            Debug.Log("adding " + other.name);
            canOpen.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (canOpen.Contains(other.gameObject))
        {
            Debug.Log("removing " + other.name);
            canOpen.Remove(other.gameObject);
        }
    }
}
