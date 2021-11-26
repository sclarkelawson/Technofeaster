using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour, Door
{
    public bool IsOpen { get; set; }
    public float PercentOpen { get; set; }
    public List<GameObject> CanOpen { get; set; }
    public Animator Animator { get; set; }

    public RoomInfo ConnectedRoom;
    
    void Start() //Start closed
    {
        PercentOpen = 0;
        Animator = gameObject.GetComponentInChildren<Animator>();
        CanOpen = new List<GameObject>();
        IsOpen = false;
    }
    public void Open()
    {
        Debug.Log("opening");
        Animator.Play("open");
        IsOpen = true;
        if (!ConnectedRoom.evaluated)
        {
            ConnectedRoom.EvaluateRoom();
        }
    }
    public void Open(float value, SquadController squad)
    {
        if (PercentOpen >= 100)
        {
            IsOpen = true;
            Animator.Play("open");
            if (!ConnectedRoom.evaluated)
            {
                ConnectedRoom.EvaluateRoom(squad);
            }
        }
        else
        {
            PercentOpen += value;
        }

    }
    public void Close()
    {
        Animator.Play("close");
        IsOpen = false;
    }

    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !CanOpen.Contains(other.gameObject))
        {
            CanOpen.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (CanOpen.Contains(other.gameObject))
        {
            CanOpen.Remove(other.gameObject);
        }
    }
}
