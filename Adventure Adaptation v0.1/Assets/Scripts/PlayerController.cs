using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRB;
    public Vector3 moveDirection;
    public float speed = 5.0f;
    public float vInput;
    public float hInput;



    
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation = true;
    }

    private void Update()
    {
        DirectionalInput();
    }

    void FixedUpdate()
    {
        DirectionalMove();  
    }

    void DirectionalInput()
    {
        //get input
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");
    }

    void DirectionalMove() 
    {
        //get new move direction from transform and input
        moveDirection = transform.forward * vInput + transform.right * hInput;
        //add speed in that direction
        playerRB.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);


    }


}
