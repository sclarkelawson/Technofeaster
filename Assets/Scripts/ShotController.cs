using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    public float shotSpeed;
    public Rigidbody myRb;
    void Start()
    {
        myRb = GetComponent<Rigidbody>();
        myRb.velocity = transform.forward * shotSpeed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().isWounded = true;
        }
        Destroy(gameObject);
    }
}
