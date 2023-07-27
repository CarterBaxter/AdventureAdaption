using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public ControlVariables controlVars;
    public LayerMask whatIsGround;
    public TextMeshProUGUI velocityText;

    [Header("Variables")]
    //basic movement
    private Rigidbody playerRB;
    private Vector3 moveDirection;
    private float vInput;
    private float hInput;

    private float playerMass = 80;
    private float playerHeight;
    private bool grounded;

    public float groundDrag = 2.0f;


    //Jumping
    public float jumpForce = 600.0f;
    public float jumpCD = .25f;
    public float airMoveMultiplier = .3f;
    public bool readyToJump = true;
    public float gravityModifier = 1.5f;


    //speeds
    public enum MovingStates
    {
        sprinting,
        walking,
        croching
    }
    [Header("Move Speed")]
    public float velocity;
    public MovingStates movingState = MovingStates.walking;
    public float crouchSpeed = 3.0f;
    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    private float currentMaxSpeed;



    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;
        playerHeight = GetComponent<CapsuleCollider>().height;
        Physics.gravity *= 0; //turn off gravity

    }


    private void Update()
    {
        MoveStateInput();
        SpeedControl();
        PlayerInput(); //get directional input
        grounded = GroundCheck(); //check if grounded

        //handle ground drag
        if (grounded)
        {
            playerRB.drag = groundDrag; //drag on ground
        }
        else 
        {
            playerRB.drag = 0; //no drag in air
        }
        velocity = playerRB.velocity.x * playerRB.velocity.x + playerRB.velocity.z * playerRB.velocity.z;
        velocity = Mathf.Sqrt(velocity);
        velocityText.text = $"Velocity: {Mathf.Round(velocity)}";
    }

    void FixedUpdate() 
    {
        DirectionalMove(); //move character based on PlayerInput in Update
        GravityAcceleration(); //add a graivty accelerate force
                               //(physics gravity isnt accelerating but constant based on my understanding)
    }




    void PlayerInput()
    {
        //get directional input
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");

        //get jump input
        if (Input.GetKey(controlVars.jumpKey) && grounded && readyToJump)
        { 
            readyToJump = false;
            Jump();
            Invoke("ResetJump", jumpCD); //reset ready to jump after cooldown
        }
    }

    void DirectionalMove() 
    {
        //get new move direction from transform and input
        moveDirection = transform.forward * vInput + transform.right * hInput;
        
        //add speed in that direction
        if (grounded)
        {
            playerRB.AddForce(moveDirection.normalized * currentMaxSpeed * 10f * playerMass, ForceMode.Force);
        }
        else //not on ground (harder to change directions in air)
        {
            playerRB.AddForce(moveDirection.normalized * currentMaxSpeed * 10f * playerMass * airMoveMultiplier, ForceMode.Force);
        }
        

    }

    bool GroundCheck()
    {
        float raycastLength = playerHeight / 2 + .2f; //half the height, plus a little.

        //Raycast( Vector3 Origin, Vector3 Position, float MaxDistance, int LayerMask)
        //Returns true if hits collider with certain mask
        return Physics.Raycast(transform.position, Vector3.down, raycastLength, whatIsGround);
    }

    private void MoveStateInput() //sets currentmaxspeed according to moving state determined by inputs
    {
        if (Input.GetKey(controlVars.crouchKey)) 
        {
            movingState = MovingStates.croching;
            currentMaxSpeed = crouchSpeed;
        }
        else if (Input.GetKey(controlVars.sprintKey))
        {
            movingState = MovingStates.sprinting;
            currentMaxSpeed = sprintSpeed;
            //TO DO STAMINA
        }
        else
        {
            movingState = MovingStates.walking;
            currentMaxSpeed = walkSpeed;
        }
    }

    void SpeedControl()
    {
        Vector3 currVelocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        if (currVelocity.magnitude > currentMaxSpeed)
        {
            Vector3 ctrlVelocity = currVelocity.normalized * currentMaxSpeed;
            playerRB.velocity = new Vector3(ctrlVelocity.x, playerRB.velocity.y, ctrlVelocity.z);
            //retain y verlocity but control x and z velocity to retain max speed.
        }
    }

    void Jump()
    {
        //reset Y velocity so jumps stay consistent
        playerRB.velocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        playerRB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }


    void GravityAcceleration()
    {
        float gravity = 9.81f * gravityModifier;
        playerRB.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }
}
