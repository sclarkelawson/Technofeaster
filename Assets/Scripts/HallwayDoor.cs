using System.Collections.Generic;
using UnityEngine;

public class HallwayDoor : MonoBehaviour, Door
{
    public float PercentOpen { get; set; }
    public bool IsOpen { get; set; }
    public List<GameObject> CanOpen { get; set; }
    public Animator Animator { get; set; }
    void Start() //Start open
    {
        PercentOpen = 100;
        Animator = gameObject.GetComponentInChildren<Animator>();
        CanOpen = new List<GameObject>();
        IsOpen = true;
    }
    public void Open()
    {
        Animator.Play("open");
        IsOpen = true;
    }
    public void Open(float value, SquadController squad)
    {
        if (PercentOpen >= 100)
        {
            Animator.Play("open");
            IsOpen = true;
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
}
