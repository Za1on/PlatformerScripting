using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{

    public GameObject m_particles;
    public BoxCollider m_boxcollider;
    [SerializeField]
    bool m_playerisDashing;
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
        // wanted to make it in a way that the enemy knows if the player dashes, if he did then do line under, 
        //else keep collider too damage player
        if (other.gameObject.CompareTag("Player"))
        {
            m_boxcollider.enabled = false;
            m_particles.SetActive(true);
            m_renderer.material.DOColor(Color.green, 0);
            m_renderer.material.DOColor(Color.blue, 0.5f);
            m_renderer.material.DOColor(Color.red, 1);
            transform.DOScale(new Vector3(-1f, -1f, -1f), 0.2f);
            transform.DOScale(new Vector3(2f, 2f, 2f), 1.5f);
            transform.DOScale(new Vector3(1f, 1f, 1f), 2f);
            transform.DOScale(new Vector3(3f, 3f, 3f), 2.5f);
            transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 3f);
            EnemyDie();
        }
    }
    public void EnemyDie()
    {
        Destroy(gameObject,3f);
    }


}
