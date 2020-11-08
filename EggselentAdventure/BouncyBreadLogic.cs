using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBreadLogic : MonoBehaviour
{
    public float pitchLow = 1f;
    public float pitchHigh = 1.5f;
    private AudioSource aud;
    public float minForce = 3.0f;
    public float maxForce = 10.0f;
    public Animator anim;

    private void Awake()
    {
        aud = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bouncing off a Player");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            float velocity = Mathf.Clamp(player.previousVelocity * -1f, minForce, maxForce);
            player.MakeJump(2f * (velocity * 0.08f));
        }

        else
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                Debug.Log("Launching...");
                rb.AddForce(new Vector2(0, 10f), ForceMode2D.Impulse);
            }

            else
            {
                Debug.Log("Some object has no RB?");
            }
        }

        anim.SetTrigger("onBounce");
        aud.pitch = Random.Range(pitchLow, pitchHigh);
        aud.Play();
    }
}
