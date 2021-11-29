using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject ExplosionEffect;

    private void OnDestroy()
    {
        Destroy(Instantiate(ExplosionEffect, transform.position, transform.rotation), 2.0f);
    }
}
