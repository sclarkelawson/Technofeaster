using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayDoor : MonoBehaviour, Door
{
    public float percentOpen { get; set; }
    public bool isOpen { get; set; }
    public List<GameObject> canOpen { get; set; }
    public Animator animator { get; set; }
    void Start() //Start open
    {
        percentOpen = 100;
        animator = gameObject.GetComponentInChildren<Animator>();
        canOpen = new List<GameObject>();
        isOpen = true;
    }
    public void Open()
    {
        animator.Play("open");
        isOpen = true;
    }
    public void Open(float value, SquadController squad)
    {
        if (percentOpen >= 100)
        {
            animator.Play("open");
            isOpen = true;
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
}
