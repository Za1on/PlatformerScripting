using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float hp_m;
    [SerializeField]
    float speed_m;
    [SerializeField]
    float distance_m;
    [SerializeField]
    bool movingRight_m = true;

    private void Update()
    {
        transform.Translate(Vector2.right * -speed_m * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }


}
