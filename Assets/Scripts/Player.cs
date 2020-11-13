using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public GameObject m_particle;
    public Animator m_anim;
    [SerializeField]
    float m_playerHp = 3;
    public Rigidbody m_Rbd;
    public Renderer m_playerRend;
    #region PauseStuff
    [SerializeField]
    bool m_GameIsPaused;
    public GameObject m_PauzeMenuObject;
    #endregion
    #region JumpStuff
    [SerializeField]
    float m_FallingGravityRatio;
    [SerializeField]
    float m_JumpingGravityRatio;
    [SerializeField]
    float m_OnFloorGravityRatio;
    [SerializeField]
    float m_JumpCancelingGravityRatio;
    [SerializeField]
    float m_Jumpforce;
    [SerializeField]
    bool m_JumpButtonPressed;
    [SerializeField]
    bool m_OnGround;
    [SerializeField]
    bool m_MustJump;
    [SerializeField]
    bool m_IsJumping;
    [SerializeField]
    bool m_IsFalling;
    #endregion
    #region DashStuff
    [SerializeField]
    float m_Speed;
    [SerializeField]
    float m_DashForce;
    [SerializeField]
    float m_PowerfullDashForce;
    [SerializeField]
    bool m_DashButtonPressed;
    [SerializeField]
    bool m_MustDash;
    [SerializeField]
    bool m_MustPowerDash;
    [SerializeField]
    bool m_ennemyCollided;
    [SerializeField]
    bool m_MovingRight;
    [SerializeField]
    bool m_MovingLeft;
    #endregion
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_ennemyCollided = false;
        m_Rbd = GetComponent<Rigidbody>();
        m_playerRend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (m_playerHp == 0)
        {
            Die();
        }
        ManageDash();
        ManageInput();
        ManageGravityRatio();
        ManageJump();
        m_IsFalling = !m_OnGround && m_Rbd.velocity.y <= 0;
    }
    public void ManageInput()
    {
        if (Input.GetKey(KeyCode.A) && !m_MustDash)
        {
            m_MovingLeft = true;
            m_Rbd.MovePosition(transform.position + transform.right * - m_Speed);
        }
        if (Input.GetKey(KeyCode.D) && !m_MustDash)
        {
            m_MovingRight = true;
            m_Rbd.MovePosition(transform.position + transform.right * m_Speed);
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            m_MovingLeft = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            m_MovingRight = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_MustJump = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(m_ennemyCollided)
            {  
                m_MustPowerDash = true;
            }
            else
            m_MustDash = true;          
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Die()
    {
        SceneManager.LoadScene("GameScene");
    }
    #region JumpManagement
    public void ManageJump()
    {
        
        if (m_MustJump)
        {
            m_MustJump = false;
            if(m_OnGround)
            {
                m_anim.SetBool("isInTheAir", true);
                Jump();
            } 
            else
            {
                if(m_Rbd.velocity.y <= 0)
                {
                    m_MustJump = false;
                }
            }
        }
    }
    public void ManageGravityRatio()
    {
        float desiredGravityRatio = m_OnFloorGravityRatio;
        if (m_IsJumping)
        {
            if (!m_JumpButtonPressed)
            {
                desiredGravityRatio = m_JumpCancelingGravityRatio;
            }
            else
            {
                desiredGravityRatio = m_JumpingGravityRatio;
            }
        }
        else if (m_IsFalling)
        {      
          desiredGravityRatio = m_FallingGravityRatio;
        }
    }
    public void Jump()
    {
        m_Rbd.AddForce(Vector3.up * m_Jumpforce, ForceMode.Impulse);
        m_IsJumping = true;
        m_IsFalling = false;
        m_OnGround = false;
    }
    private void OnCollisionEnter(Collision other)
    {
        m_anim.SetBool("isInTheAir", false);
        m_IsJumping = false;
        m_IsFalling = false;
        m_OnGround = true;
        if(other.gameObject.CompareTag("Enemy"))
        {
            if (m_MustDash)
            {
                return;
            }
            else
                m_playerHp -= 1f;
            m_playerRend.material.DOColor(Color.red, 0.2f);
        }
    }
    #endregion

    #region DashManagament
    public void ManageDash()
    {
        if (m_MustPowerDash)
        {
            PowerDash();
        }
        if(m_MustDash)
        {
            Dash();
        }
    }
    private bool m_ResetTimer;
    private float m_Timer = 0f;
    public void Dash()
    {
        m_Timer += Time.deltaTime;

        if(m_Timer > 1.5f)
        {
            m_particle.SetActive(false);
            m_MustDash = false;
            m_playerRend.material.DOColor(Color.green, 0.1f);
            m_ResetTimer = true;
        }
        if (m_ResetTimer)
        {
            m_Timer = 0f;
            m_ResetTimer = false;
        }
        if (m_MovingRight)
        {
            m_particle.SetActive(true);
            m_Rbd.AddForce(Vector3.right * m_DashForce, ForceMode.Impulse);
        }
        if (m_MovingLeft)
        {
            m_particle.SetActive(true);
            m_Rbd.AddForce(Vector3.right * -m_DashForce, ForceMode.Impulse);
        }
        m_Rbd.velocity = new Vector3(Mathf.Clamp(m_Rbd.velocity.x, -5f, 5f), m_Rbd.velocity.y, m_Rbd.velocity.z);
    }
    public void PowerDash()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer > 1.5f)
        {
            m_particle.SetActive(false);
            m_playerRend.material.DOColor(Color.green, 0.1f);
            m_MustDash = false;
            m_ResetTimer = true;
        }
        if (m_ResetTimer)
        {
            m_Timer = 0f;
            m_ResetTimer = false;
        }
        if (m_MovingRight)
        {
            m_particle.SetActive(true);
            m_Rbd.AddForce(Vector3.up * 0.01f , ForceMode.Impulse);
            m_Rbd.AddForce(Vector3.right * m_PowerfullDashForce, ForceMode.Impulse);
            m_ennemyCollided = false;
        }
        if (m_MovingLeft)
        {
            m_particle.SetActive(true);
            m_Rbd.AddForce(Vector3.up * 0.01f, ForceMode.Impulse);
            m_Rbd.AddForce(Vector3.right * -m_PowerfullDashForce, ForceMode.Impulse);
            m_ennemyCollided = false;
        }
        m_Rbd.velocity = new Vector3(Mathf.Clamp(m_Rbd.velocity.x, -10f, 10f), m_Rbd.velocity.y, m_Rbd.velocity.z);
    
}
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Turning blue is suppose to be an indication for the PowerDash, but its not working right now.
            m_playerRend.material.DOColor(Color.blue,0.1f);
            m_ennemyCollided = true;
        }
    }
    #endregion

    #region PauseManagement
    public void Resume()
    {
        m_PauzeMenuObject.SetActive(false);
        Time.timeScale = 1f;
        m_GameIsPaused = false;
    }
    public void Restart()
    {
        SceneManager.LoadScene("GameScene");
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        m_PauzeMenuObject.SetActive(true);
        Time.timeScale = 0f;
        m_GameIsPaused = true;
    }
    public void QuitCurrentGame()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        m_PauzeMenuObject.SetActive(false);
    }
    #endregion
}

