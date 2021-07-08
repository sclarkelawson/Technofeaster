using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixtureController : MonoBehaviour, Interactable
{
    public GameObject explosionEffect, fearRadius;
    public int fearVal;
    public void Toggle()
    {
        GameObject tempFearRadius = Instantiate(fearRadius, transform.position, transform.rotation);
        tempFearRadius.GetComponent<FearRadius>().fear = fearVal;
        Destroy(tempFearRadius, 0.2f);
        Destroy(Instantiate(explosionEffect, transform.position, transform.rotation), 2.0f);
        Destroy(gameObject);
    }
}
