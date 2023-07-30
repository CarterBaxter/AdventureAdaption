using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Slider StaminaSlider;
    public ControlVariables controlVars;
    private GameManager gameManager;
    public LayerMask whatIsGround;
    public TextMeshProUGUI velocityText;
    private CapsuleCollider capsuleCollider;

    [Header("Variables")]
    //basic movement
    public Rigidbody playerRB;
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
    private float currentMaxSpeed = 5.0f;


    public float maxStamina = 10f;
    public float currStamina = 10f;
    private Coroutine startStamina;
    private Coroutine regenStamina;

    private float startYScale;
    private float crouchYScale = 0.4f;
    private float startColliderRadius;
    private float crouchColliderRadius = .4f; //so can fit under 1 unit areas


    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerHeight = capsuleCollider.height;
        Physics.gravity *= 0; //turn off gravity
        startYScale = transform.localScale.y;
        startColliderRadius = capsuleCollider.radius;
    }


    private void Update()
    {
        MoveStateInput();
        SpeedControl();
        UpdateUI();
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
        if (!gameManager.paused) 
        { 
            DirectionalMove(); //move character based on PlayerInput in Update
            GravityAcceleration(); //add a graivty accelerate force
        }                       //(physics gravity isnt accelerating but constant based on my understanding)
    }

    void UpdateUI() 
    {
        velocity = playerRB.velocity.x * playerRB.velocity.x + playerRB.velocity.z * playerRB.velocity.z;
        velocity = Mathf.Sqrt(velocity);
        velocityText.text = $"Velocity: {Mathf.Round(velocity)}";
        StaminaSlider.value = currStamina;
    }


    void PlayerInput()
    {
        //get directional input
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");

        //get jump input
        if (Input.GetKey(controlVars.jumpKey) && grounded && readyToJump 
            && !gameManager.paused && movingState != MovingStates.croching)
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

    bool CanStandCheck()
    {
        float raycastLength = playerHeight / 2; //half the start height looking above

        //Raycast( Vector3 Origin, Vector3 Position, float MaxDistance, int LayerMask)
        //Returns true if hits collider with certain mask
        return !Physics.Raycast(transform.position, Vector3.up, raycastLength);
    }




    private void MoveStateInput() //sets currentmaxspeed according to moving state determined by inputs
    {

        if (Input.GetKeyDown(controlVars.crouchKey) && grounded) //so cant force down while in air
        {
            movingState = MovingStates.croching;
            currentMaxSpeed = crouchSpeed;

            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            playerRB.AddForce(Vector3.down * 200f, ForceMode.Impulse);
            capsuleCollider.radius = crouchColliderRadius;
        }
        else if (Input.GetKeyUp(controlVars.crouchKey) && CanStandCheck())
        {
            movingState = MovingStates.walking;
            currentMaxSpeed = walkSpeed;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            capsuleCollider.radius = startColliderRadius;
        }
        else if (Input.GetKeyDown(controlVars.sprintKey) && currStamina > 0)
        {
            movingState = MovingStates.sprinting;
            currentMaxSpeed = sprintSpeed;

            if (regenStamina != null)
            { StopCoroutine(regenStamina); }
            startStamina = StartCoroutine("StaminaCoroutine");
        }
        else if (Input.GetKeyUp(controlVars.sprintKey))
        {
            movingState = MovingStates.walking;
            currentMaxSpeed = walkSpeed;

            if (startStamina != null)
            { StopCoroutine(startStamina); }
            regenStamina = StartCoroutine("RegenStaminaCoroutine");
        }
        else if (currStamina <= 0)
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

    IEnumerator StaminaCoroutine()
    {
        while (currStamina > 0)
        {
            if (!gameManager.paused) 
                currStamina -= Time.deltaTime;
            yield return null;
        }
        currStamina = 0;
    }
    IEnumerator RegenStaminaCoroutine()
    {
        while (currStamina < maxStamina)
        {
            if (!gameManager.paused)
                currStamina += Time.deltaTime;
            yield return null;
        }       
        currStamina = maxStamina;

    }


}
/*
while (timeElapsed < lerpDuration)
{
    valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
    timeElapsed += Time.deltaTime;
    yield return null;
}
valueToLerp = endValue;
    
*/