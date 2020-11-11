using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    Renderer m_renderer;
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
            m_renderer.material.DOColor(Color.green, 0);
            m_renderer.material.DOColor(Color.blue, 0.5f);
            m_renderer.material.DOColor(Color.red, 1);
            
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject,3f);
    }


}
