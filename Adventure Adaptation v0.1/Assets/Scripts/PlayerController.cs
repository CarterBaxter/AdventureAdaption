using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public LayerMask whatIsGround;

    [Header("Variables")] //note playerRb weight is set to 80kg
    //basic movement
    private Rigidbody playerRB;
    private Vector3 moveDirection;
    private float vInput;
    private float hInput;

    private float playerHeight;
    private bool grounded;

    public float speed = 5.0f;
    public float groundDrag = 5.0f;

    //Jumping
    public float jumpForce = 600.0f;
    public float jumpCD = .25f;
    public float airMoveMultiplier = .4f;
    public bool readyToJump = true;


    [Header("KEYBINDS")]
    public KeyCode jumpKey = KeyCode.Space;



    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;
        playerHeight = GetComponent<BoxCollider>().size.y;
    }

    private void Update()
    {
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
    }

    void FixedUpdate()
    {
        DirectionalMove(); //move character based on PlayerInput in Update
    }




    void PlayerInput()
    {
        //get directional input
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");

        //get jump input
        if (Input.GetKey(jumpKey) && grounded && readyToJump)
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
            playerRB.AddForce(moveDirection.normalized * speed * 800f, ForceMode.Force);
        }
        else //not on ground (harder to change directions in air)
        {
            playerRB.AddForce(moveDirection.normalized * speed * 800f * airMoveMultiplier, ForceMode.Force);
        }
        

    }

    bool GroundCheck()
    {
        float raycastLength = playerHeight / 2 + .2f; //half the height, plus a little.

        //Raycast( Vector3 Origin, Vector3 Position, float MaxDistance, int LayerMask)
        //Returns true if hits collider with certain mask
        return Physics.Raycast(transform.position, Vector3.down, raycastLength, whatIsGround);
    }

    void SpeedControl()
    { 
        Vector3 currVelocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);

        if (currVelocity.magnitude > speed)
        {
            Vector3 ctrlVelocity = currVelocity.normalized * speed;
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

}
