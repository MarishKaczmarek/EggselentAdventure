using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    private bool benedictAlreadyActive = false;
    private bool yolkoAlreadyActive = false;
    //This bool will determine if we've already passed through here. It will allow us to prevent checkpoints from re-loading.
    [SerializeField] private Animator cpAnimator;
    private AudioSource aud;
    public Transform location;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.name == "Benedict")
            {
                if(!benedictAlreadyActive)
                {
                    benedictAlreadyActive = true;
                    HealthController checkpointPointer = collision.gameObject.GetComponent<HealthController>();
                    checkpointPointer.checkpoint = location.position;
                    cpAnimator.SetTrigger("onTriggered");
                }
            }

            else if (collision.gameObject.name == "Yolko")
            {
                if (!yolkoAlreadyActive)
                {
                    yolkoAlreadyActive = true;
                    HealthController checkpointPointer = collision.gameObject.GetComponent<HealthController>();
                    checkpointPointer.checkpoint = location.position;
                    cpAnimator.SetTrigger("onTriggered");
                }
            }
        }
    }

    public void PlaySound()
    {
        aud.Play();
    }
}
