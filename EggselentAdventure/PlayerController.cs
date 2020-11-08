using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EggselentGlobals
{
    public PlayerTypes PlayerType; //Which player type are we?

    // List of possible control setups, these must match the same ones used in INPUT manager.
    const string HORIZONTAL_A = "Horizontal_A";
    const string HORIZONTAL_B = "Horizontal_B";
    const string JUMP_A = "Jump_A";
    const string JUMP_B = "Jump_B";
    const string USE_A = "Use_A";
    const string USE_B = "Use_B";

    //List of Animator parameters, this allows for efficient change of states.
    const string EGG_SPEED = "hSpeed";
    const string EGG_JUMP = "vSpeed";
    const string EGG_JUMPING = "inAir";
    const string EGG_PUSHING = "isPushing";

    // What set of inputs are we using?
    private string horizontal;
    private string jump;
    private string use;

    //If I were smart I'd move these to a seperate script that all the other inherit from. But ain't nobody got time for that.
    public Animator anim;
    private Rigidbody2D rb;

    private bool canMove = true;

    // What do we need to adjust and control our movement?
    public float speed = 10f; //How fast do we move about?
    private float magnitude; //How much pressure are we putting into our controller?
    private bool moving = false;
    public bool facingRight = false;

    // What do we need to adjust and control our jumping?
    public float jumpForce = 5f; // The force at which we jump.
    private bool hasCollided = false; //Are we colliding with anything?
    private bool hasJumped = false; //Are we in mid-air?
    private bool jumpPressed = false;
    private Vector2 playerDimensions;
    private float rayDistance = 2f;

    public float previousVelocity;
    private float currentVelocity;

    //What are we walking on.
    private string surface;
    //Is there anything in front of us?
    private bool isPushing = false;

    //Who can we collide against?
    public LayerMask playerLayer;

    //Have we pressed the Use Key in the previous frame;
    public bool useKeyPressed;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
        
        if(anim == null)
        {
            Debug.Log("This GameObject requires Animator to function properly... Lack of Animator could cause unwanted behavior...");
        }

        rb = GetComponent<Rigidbody2D>();

        if(rb == null)
        {
            Debug.Log("This GameObject requires a 2D RigidBody to function properly... The object cannot function without one...");
        }

        // Depending on which player we are, adjust our Axis and Input buttons to the correct button.
        if (PlayerType == PlayerTypes.PlayerA)
        {
            Debug.Log("GameObject " + gameObject.name + " is " + PLAYER_A_NAME);
            horizontal = HORIZONTAL_A;
            jump = JUMP_A;
            use = USE_A;
        }

        else if(PlayerType == PlayerTypes.PlayerB)
        {
            Debug.Log("GameObject " + gameObject.name + " is " + PLAYER_B_NAME);
            horizontal = HORIZONTAL_B;
            jump = JUMP_B;
            use = USE_B;
        }

        //FlipCharacter();

    }

    private void Update()
    {
        //Handles the input communication.
        if (rb != null)
        {
            if(canMove != false)
            {
                magnitude = Input.GetAxis(horizontal);
                moving = Input.GetButton(horizontal);
                jumpPressed = Input.GetButtonDown(jump);
                useKeyPressed = Input.GetButtonDown(use);
            }

            else
            {
                Debug.Log("Can't move...");
                magnitude = 0;
                moving = false;
                jumpPressed = false;
                useKeyPressed = false;
            }

            if(magnitude > 0)
            {
                facingRight = true;
            }

            else if (magnitude < 0)
            {
                facingRight = false;
            }

            isPushing = CheckPushing();

            if (anim != null)
            {
                anim.SetFloat(EGG_SPEED, Mathf.Abs(magnitude));
                anim.SetFloat(EGG_JUMP, rb.velocity.y);
                anim.SetBool(EGG_JUMPING, hasJumped);
                anim.SetBool(EGG_PUSHING, isPushing);
            }

            previousVelocity = currentVelocity;
            currentVelocity = rb.velocity.y;
        }

        CheckSurface();
    }

    private void FixedUpdate()
    {
        //Assuming there is a rigidbody attached to this object, we allow it to control movement.
        if(rb != null)
        {
            //Only if we haven't collided against a wall can we move about, this prevents the player from pushing itself against it (which is one of the reasons
            // for sticking to it).

            //NOTE TO SELF
            // In order to achieve the slippery, we will need to detect what material we're walking on and then change our code.
            // There's two main issues right now
            // We're telling the game to move at a specific velocity each time, (magniture by speed), this is why our X velocity doesn't react to the any materials.
            // We're also saying that the characters have no friction in their material physics.
            // THis is to prevent the spaghetthi glitch where the character(s) will stick to a vertical wall for NO GOOD REASON because UNITY IS STUPID.
            // MY SOLUTION
            // At some point, we will start grabbing information, as to what the player has collided with. If it collided with an ice, we will change the calculation to check
            // if the player is moving, when they let go of the button, it will not touch the velocity at all and will auto-calculate it based off physics.
            // This will give the impression of slippery ice, without sacrificng the need to figure out how to use that at a later stage
            // Not sure if I'm a genius or just a bad programmer.
            if (!hasCollided)
            {
                //rb.velocity = new Vector2((magnitude * speed), rb.velocity.y);
                if(surface == "Ice")
                {
                    if(moving)
                    {
                        rb.velocity = new Vector2((magnitude * speed), rb.velocity.y);
                    }
                }

                else
                {
                    rb.velocity = new Vector2((magnitude * speed), rb.velocity.y);
                }
            }

            if (jumpPressed && !hasJumped)
            {
                Jump(jumpForce);
            }
            
            CheckJump();
            FlipCharacter();
            /*
            CheckForCollisions();
            UnstickPlayer();
            */
        }
    }

    private void Jump(float force)
    {
        // There seems to be some conflict in terms of input. Can't press Left Ctrl if Right Ctrl was pressed previous frame and vice versa.

        // Only if we haven't jumped already can we actually jump.
            rb.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
            hasJumped = true;
    }

    private void CheckJump()
    {
        bool raycastResult = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), rayDistance, playerLayer);

        if (raycastResult)
        {
            hasJumped = false;
        }

        else if (!raycastResult)
        {
            hasJumped = true;
        }

    }

    private void FlipCharacter()
    {
        if(facingRight == false && transform.localScale.x > 0)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        else if (facingRight == true && transform.localScale.x < 0)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }

    private void CheckSurface()
    {
        RaycastHit2D objects = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), rayDistance, playerLayer);
        if(objects.collider != null)
        {
            surface = objects.collider.gameObject.tag;
        }
        
    }

    private bool CheckPushing()
    {
        bool result = false;

        if(facingRight)
        {
            Debug.Log("We're facing right...");
            result = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), rayDistance, playerLayer);
            Debug.Log("ARE WE PUSHING?: " + result);
        }

        else if(!facingRight)
        {
            Debug.Log("We're facing left...");
            result = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.left), rayDistance, playerLayer);
            Debug.Log("ARE WE PUSHING?: " + result);
        }

        return result;
    }

    public void MakeJump(float multiplier)
    {
        Jump(jumpForce * multiplier);
    }

    public void ToggleMovement(bool state)
    {
        if(state == true)
        {
            //Enable Movement
            canMove = true;
        }

        else if (state == false)
        {
            //Disable Movement
            canMove = false;
        }
    }
}