using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the player's movement and animations.
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; //Horizontal speed of the player.
    public float jumpForce = 10f; //Jump force of the player.

    private Rigidbody2D rb; //Rigidbody component of the player.
    private Animator animator; //Animator component of the player.
    private bool isGrounded; //Check if the player is on the ground.
    private float lastMoveDirection; //Last move direction of the player.

    private AudioManager audioManager; //AudioManager instance to play sound effects when the player walks or jumps.

    void Awake()
    {
        // Assign references
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
        lastMoveDirection = 1f;
    }

    void Start()
    {
        // Track level start (if applicable)
        AnalyticsManager.Instance.TrackLevelStart(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    void Update()
    {
        if (rb == null || animator == null || audioManager == null) return;

        //If any of the UI is open (menu, inventory, or puzzle) or a dialogue is active, stop the player from moving and disable the animations.
        if (MenuController.IsMenuOpen || InventoryUIController.IsInventoryOpen || PuzzleController.IsPuzzleOpen || NPC.IsDialogueActive)
        {
            rb.velocity = Vector2.zero; //Stop the player from moving.
            animator.SetBool("WalkLeft", false);
            animator.SetBool("WalkRight", false);
            animator.SetBool("JumpLeft", false);
            animator.SetBool("JumpRight", false);
            return;
        }

        float move = 0f;
        //Get the horizontal input from the user.
        if (UserInput.instance.MoveLeftInput)
        {
            move = -1f; //Move left.
        }
        else if (UserInput.instance.MoveRightInput)
        {
            move = 1f; //Move right.
        }

        //Set the velocity of the player based on the input.
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        //Set the animator parameters based on the input.
        if (move < 0)
        {
            animator.SetBool("WalkLeft", true);
            animator.SetBool("WalkRight", false);
        }
        else if (move > 0)
        {
            animator.SetBool("WalkRight", true);
            animator.SetBool("WalkLeft", false);
        }
        else
        {
            animator.SetBool("WalkLeft", false);
            animator.SetBool("WalkRight", false);
        }

        //Play footstep sound if the player is moving and on the ground.
        if (move != 0 && isGrounded)
        {
            if (!audioManager.sfxSource.isPlaying)
            {
                audioManager.PlaySFX(audioManager.footstepSound);
            }
            lastMoveDirection = move;
        }

        //Handle jump movement and animations.
        if (UserInput.instance.JumpInput && isGrounded)
        {
            //Apply upward force to the player and set the ground to state false.
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;

            //Play jump sound effect.
            audioManager.PlaySFX(audioManager.jumpSound);

            //Set the animator parameters based on the input.
            if (move < 0)
            {
                animator.SetBool("JumpLeft", true);
                animator.SetBool("JumpRight", false);
            }
            else if (move > 0)
            {
                animator.SetBool("JumpRight", true);
                animator.SetBool("JumpLeft", false);
            }
            else
            {
                if (lastMoveDirection < 0)
                {
                    animator.SetBool("JumpLeft", true);
                    animator.SetBool("JumpRight", false);
                }
                else
                {
                    animator.SetBool("JumpRight", true);
                    animator.SetBool("JumpLeft", false);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check if the player is colliding with the ground.
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    ResetJumpAnimations(); //Reset jump animations when the player lands on the ground.
                    break;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Maintain the grounded state while the player is in contact with the ground.
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    return;
                }
            }
            isGrounded = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Set the grounded state to false when the player exits the collision with the ground.
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void ResetJumpAnimations()
    {
        //Reset jump animations when the player lands on the ground.
        animator.SetBool("JumpLeft", false);
        animator.SetBool("JumpRight", false);
    }
}