using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//Takes and handles input and movement of the Player
public class PlayerController : MonoBehaviour
{
    //movement-related variables
    [SerializeField] private float movementSpeed = 1f;
    private Vector2 movement;

    //Dash variables
    [Header("Dashing")]
    [SerializeField] private float dashVelocity = 0.1f;
    [SerializeField] private float dashTime = 0.1f;
    private Vector2 dashDirection;
    private bool isDashing = false;
    private bool canDash = true;
    private bool dashInput = false;

    //Inspector References
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;
    
    //Scripts References
    public SwordAttack swordAttack;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        //Sets the movement values based on the movement axis
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        dashInput = Input.GetButtonDown("Dash");

        if(dashInput && canDash)
        {
            dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (dashDirection != Vector2.zero)
            {
                isDashing = true;
                canDash = false;
                trailRenderer.emitting = true;
            }
            StartCoroutine(StopDashing());
        }
        
    }

    private void FixedUpdate() 
    {
            // if movement is not 0, move;
            if (movement != Vector2.zero)
            {
                rb.MovePosition(rb.position + movement.normalized * movementSpeed * Time.fixedDeltaTime);
                animator.SetBool("isMoving", true);
                
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            //Set sprite direction
            if (movement.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (movement.x < 0)
            {
                spriteRenderer.flipX = true;
            }

            if (isDashing)
            {
            rb.MovePosition(rb.position + dashDirection.normalized * dashVelocity);
            return;
            }
    }


    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashTime); //requires return before starting to yield
        trailRenderer.emitting = false;
        isDashing = false;
        dashInput = false;
        canDash = true;
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
}
