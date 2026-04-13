using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int coins;
    public int health = 100;
    private float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;
    public GameObject restartButton;
    public float fallThreshold = -10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public int extraJumpValue = 1;
    private int extraJumps;
    private AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip hurtClip;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        extraJumps = extraJumpValue;

        if (restartButton != null)
            restartButton.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (isGrounded)
            extraJumps = extraJumpValue;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySFX(jumpClip);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
                PlaySFX(jumpClip);
            }
        }

        SetAnimation(moveInput);
        healthImage.fillAmount = health / 100f;

        if (transform.position.y < fallThreshold)
            FallDie();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
                animator.Play("Player_Idle");
            else
                animator.Play("Player_Run");
        }
        else
        {
            if (rb.linearVelocityY > 0)
                animator.Play("Player_Jump");
            else
                animator.Play("Player_Fall");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            PlaySFX(hurtClip);
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if (health <= 0)
                Die();
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void FallDie()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (restartButton != null)
            restartButton.SetActive(true);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void PlaySFX(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}