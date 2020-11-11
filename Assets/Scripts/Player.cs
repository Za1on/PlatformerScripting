using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public Rigidbody m_Rbd;
    public Renderer playerRend_m;
    #region PauseStuff
    [SerializeField]
    bool gameIsPaused_m;
    public GameObject pauzeMenuObject_m;
    #endregion
    #region JumpStuff
    [SerializeField]
    float fallingGravityRatio_m;
    [SerializeField]
    float jumpingGravityRatio_m;
    [SerializeField]
    float onFloorGravityRatio_m;
    [SerializeField]
    float jumpCancelingGravityRatio_m;
    [SerializeField]
    float jumpforce_m;
    [SerializeField]
    bool jumpButtonPressed_m;
    [SerializeField]
    bool onGround_m;
    [SerializeField]
    bool mustJump_m;
    [SerializeField]
    bool isJumping_m;
    [SerializeField]
    bool isFalling_m;
    #endregion
    #region DashStuff
    [SerializeField]
    float speed_m;
    [SerializeField]
    float dashForce_m;
    [SerializeField]
    float powerfullDashForce_m;
    [SerializeField]
    bool dashButtonPressed_m;
    [SerializeField]
    bool mustDash_m;
    [SerializeField]
    bool ennemyCollided_m;
    [SerializeField]
    bool movingRight_m;
    [SerializeField]
    bool movingLeft_m;
    #endregion
    void Start()
    {
        ennemyCollided_m = false;
        m_Rbd = GetComponent<Rigidbody>();
        playerRend_m = GetComponent<Renderer>();
    }

    void Update()
    {
        ManageDash();
        ManageInput();
        ManageGravityRatio();
        ManageJump();
        isFalling_m = !onGround_m && m_Rbd.velocity.y <= 0;
    }
    public void ManageInput()
    {
        if (Input.GetKey(KeyCode.A) && !mustDash_m)
        {
            movingLeft_m = true;
            m_Rbd.MovePosition(transform.position + transform.right * -speed_m);
        }
        if (Input.GetKey(KeyCode.D) && !mustDash_m)
        {
            movingRight_m = true;
            m_Rbd.MovePosition(transform.position + transform.right * speed_m);
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            movingLeft_m = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            movingRight_m = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mustJump_m = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            mustDash_m = true;
            playerRend_m.material.DOColor(Color.red, 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused_m)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    #region JumpManagement
    public void ManageJump()
    {
        
        if (mustJump_m)
        {
            mustJump_m = false;
            if(onGround_m)
            {
                Jump();
            } 
            else
            {
                if(m_Rbd.velocity.y <= 0)
                {
                    isJumping_m = false;
                }
            }
        }
    }
    public void ManageGravityRatio()
    {
        float desiredGravityRatio = onFloorGravityRatio_m;
        if (isJumping_m)
        {
            if (!jumpButtonPressed_m)
            {
                desiredGravityRatio = jumpCancelingGravityRatio_m;
            }
            else
            {
                desiredGravityRatio = jumpingGravityRatio_m;
            }
        }
        else if (isFalling_m)
        {      
          desiredGravityRatio = fallingGravityRatio_m;
        }
    }
    public void Jump()
    {
        m_Rbd.AddForce(Vector3.up * jumpforce_m, ForceMode.Impulse);
        isJumping_m = true;
        isFalling_m = false;
        onGround_m = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        isJumping_m = false;
        isFalling_m = false;
        onGround_m = true;
    }
    #endregion

    #region DashManagament
    public void ManageDash()
    {
        if (mustDash_m && ennemyCollided_m)        
        {
            PowerDash();
        }
        if(mustDash_m)
        {
            Dash();
        }
    }
    private bool m_ResetTimer;
    private float m_Timer = 0f;
    public void Dash()
    {
        m_Timer += Time.deltaTime;

        if(m_Timer > 2f)
        {
            mustDash_m = false;
            playerRend_m.material.DOColor(Color.green, 0.2f);
            m_ResetTimer = true;
        }
        if(m_ResetTimer)
        {
            m_Timer = 0f;
            m_ResetTimer = false;
        }
        if (movingRight_m)
        {
            m_Rbd.AddForce(Vector3.right * dashForce_m, ForceMode.Impulse);
        }
        if (movingLeft_m)
        {
            m_Rbd.AddForce(Vector3.right * -dashForce_m, ForceMode.Impulse);
        }
        m_Rbd.velocity = new Vector3(Mathf.Clamp(m_Rbd.velocity.x, -10f, 10f), m_Rbd.velocity.y, m_Rbd.velocity.z);
    }
    public void PowerDash()
    {
        if (movingRight_m)
        {
            m_Rbd.AddForce(Vector3.right * powerfullDashForce_m, ForceMode.Impulse);
            ennemyCollided_m = false;
        }
        if (movingLeft_m)
        {
            m_Rbd.AddForce(Vector3.right * -powerfullDashForce_m, ForceMode.Impulse);
            ennemyCollided_m = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            ennemyCollided_m = true;
        }
    }
    #endregion

    #region PauseManagement
    public void Resume()
    {
        pauzeMenuObject_m.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused_m = false;
    }
    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        pauzeMenuObject_m.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused_m = true;
    }
    public void QuitCurrentGame()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        pauzeMenuObject_m.SetActive(false);
    }
    #endregion
}

