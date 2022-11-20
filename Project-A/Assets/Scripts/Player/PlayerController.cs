using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//Takes and handles input and movement of the Player
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float collisionOffset = 0.05f;
    [SerializeField] private ContactFilter2D movementFilter;
  
    Vector2 movementInput;
    bool canMove = true;

    Rigidbody2D rb;
    List<RaycastHit2D> castCollision = new List<RaycastHit2D>();
    Animator animator;
    SpriteRenderer spriteRenderer;

    public SwordAttack swordAttack;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (canMove)
        { 
            // if movement is not 0, move
            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput);
                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0));
                }
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }

                animator.SetBool("isMoving", success);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            //Set sprite direction
            if (movementInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (movementInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    bool TryMove(Vector2 direction)
    {
        //checks if there is a direction to move in
        if(direction!= Vector2.zero)
        {
            //Check for potential collisions
            int count = rb.Cast(
                direction, // X and Y values between -1 and 1 that represents the direction from the body to look for collisions
                movementFilter, //The settings that determine where a collision can occur on such as layers to collide with
                castCollision, // List of collisions to store the found collisions into after the Cast is finished
                movementSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * movementSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        } else
        {
            return false;
        }
            
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire()
    {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack()
    {
        if(spriteRenderer.flipX == true)
        {
            swordAttack.AttackLeft();
        } else if (spriteRenderer.flipX == false){
            swordAttack.AttackRight();
        }
    }

    public void EndSwordAttack()
    {
        swordAttack.StopAttack();
    }


    //locks and unlocks movement
    /*
    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }
    */

}
