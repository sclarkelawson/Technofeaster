using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayDoor : MonoBehaviour, Door
{
    public float percentOpen { get; set; }
    public List<GameObject> currentlyOpening { get; set; }
    public Animator animator { get; set; }
    void Start() //Start open
    {
        percentOpen = 100;
        animator = gameObject.GetComponent<Animator>();
    }
    public void Open(float value, bool isPlayer)
    {
        if (percentOpen >= 100)
        {
            animator.Play("open");
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
