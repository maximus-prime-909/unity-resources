using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Variables
    [Header("Movement")]
    public bool airControl = true;
    public float moveSpeed = 4f;
    public float movementSmoothing = 0.05f;
    private Vector2 currentVelocity = Vector2.zero;

    [Header("Jump")]
    public float jumpForce = 12f;
    public Transform GroundDetectionPoint;
    public float GroundDetectRadius = 0.3f;
    public LayerMask whatIsGround;

    [Header("Crouch")]
    public Transform CeilingCheckPoint; 
    public float HeadCollisionRadius = 0.2f;
    public LayerMask stayInCrouchLayer;
    public Collider2D crouchCollider;
    public float crouchSpeed = 2f;
 
    private float horizontal;
    private bool isLookingRight = true;
    public bool isGrounded;
    public bool isCrouching;

    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        //Taking Input From Player
        horizontal = Input.GetAxisRaw("Horizontal"); 

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }    

        //CrouchControls
        if(Input.GetKey(KeyCode.C))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }
        Crouch();
       

    }

    private void FixedUpdate() 
    {
        //Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(GroundDetectionPoint.position, GroundDetectRadius, whatIsGround);

        Move();       
        
    }

    private void Move()
    {
        if(isGrounded || airControl) //player is grounded or air control is turned on 
        {
            float speed = moveSpeed;
            if (isCrouching == true)
            {
                speed = crouchSpeed;
            }

            Vector2 targetVelocity = new Vector2(horizontal * speed * Time.fixedDeltaTime * 100f, rb.velocity.y);
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, movementSmoothing);

            if (isLookingRight == true && horizontal < 0)
            {
                Flip();
            }
            else if (isLookingRight == false && horizontal > 0)
            {
                Flip();
            }
        }       

    }

    private void Flip()
    {
        isLookingRight = !isLookingRight;

        transform.Rotate(0f, 180f, 0f);        
    }

    private void Jump()
    {
        if(isGrounded == false) 
        {
            return;
        }

        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

    }

    private void Crouch()
    {
        //Ceiling check before standing up - if ceiling found, stay in crouch
        if (isCrouching == false) // If not holding the crouch button
        {
            if (Physics2D.OverlapCircle(CeilingCheckPoint.position, HeadCollisionRadius, stayInCrouchLayer))
            {
                //Ceiling Found so stay in crouch
                isCrouching = true;
            }

        }

        //Disable or enable Crouch Collider
        if (isCrouching)
        {
            crouchCollider.enabled = false;
        }
        else
        {
            crouchCollider.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(CeilingCheckPoint.position, HeadCollisionRadius);
        Gizmos.DrawWireSphere(GroundDetectionPoint.position, GroundDetectRadius);
    }

}