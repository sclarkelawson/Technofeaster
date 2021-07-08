using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    public float shotSpeed;
    public Rigidbody myRb;
    public bool canKill;
    void Start()
    {
        myRb = GetComponent<Rigidbody>();
        myRb.velocity = transform.forward * shotSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController.isWounded)
            {
                //kill
            }
            playerController.isWounded = true;
        }
        Destroy(gameObject);
    }
}
