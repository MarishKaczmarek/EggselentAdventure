using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HardBoiledEggBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private AudioSource audSrc;

    private bool facingRight = true;
    private bool canAttack = true;

    public float speed = 5f;
    public float waitTime = 2f;

    public float distance = 3f;
    public LayerMask walkingLayer;
    public LayerMask playerLayer;

    public int health = 2;

    public AudioClip attackSound;

    const string EGG_WALK = "vSpeed";
    const string EGG_ATTACK = "Attack";
    const string EGG_HURT = "Hurt";
    const string EGG_DEATH = "Death";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audSrc = GetComponent<AudioSource>();

    }

    private void Update()
    {
        ToggleMovementOrientation();

        if(anim != null)
        {
            anim.SetFloat(EGG_WALK, Mathf.Abs(rb.velocity.x));
        }

        if(!canAttack)
        {
            speed = 0f;
        }
    }

    private void FixedUpdate()
    {
        if(rb != null)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
    }

    private void ToggleMovementOrientation()
    {
        if(facingRight)
        {
            bool raycastResult = Physics2D.Raycast(transform.position, Vector2.right, distance, walkingLayer);
            if(raycastResult)
            {
                StartCoroutine(RoutineFlipCharacter(facingRight));
            }
        }

        else if(!facingRight)
        {
            bool raycastResult = Physics2D.Raycast(transform.position, Vector2.left, distance, walkingLayer);
            if (raycastResult)
            {
                StartCoroutine(RoutineFlipCharacter(facingRight));
            }
        }
    }

    private IEnumerator RoutineFlipCharacter(bool orientation)
    {
        float localSpeed = speed;
        speed = 0f;
        orientation = !orientation;
        facingRight = orientation;
        yield return new WaitForSeconds(waitTime);
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        speed = localSpeed;
        speed = speed * -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(canAttack)
            {
                bool raycastUp = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0, Vector2.up, 3f, playerLayer);

                if (raycastUp)
                {
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
                    if (health > 0)
                    {
                        anim.SetTrigger(EGG_HURT);
                        health--;
                    }

                    else if (health <= 0)
                    {
                        speed = 0f;
                        anim.SetTrigger(EGG_DEATH);
                        canAttack = false;
                    }
                }

                else
                {
                    bool raycastLeft = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0, Vector2.left, 1.5f, playerLayer);
                    bool raycastRight = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0, Vector2.right, 1.5f, playerLayer);
                    if (facingRight)
                    {
                        if (raycastRight)
                        {
                            anim.SetTrigger(EGG_ATTACK);

                            HealthController hc = collision.gameObject.GetComponent<HealthController>();
                            hc.OnPlayerHit();
                        }
                    }

                    else if (!facingRight)
                    {
                        if (raycastLeft)
                        {
                            anim.SetTrigger(EGG_ATTACK);
                            HealthController hc = collision.gameObject.GetComponent<HealthController>();
                            hc.OnPlayerHit();
                        }
                    }
                }
            }
        }
    }

    public IEnumerator OnDeath()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void PlayAttack()
    {
        audSrc.clip = attackSound;
        audSrc.Play();
    }
}
