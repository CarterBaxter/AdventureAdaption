using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;

    public Transform PlayerOrientation;

    float xRotation;
    float yRotation;
    
    
    void Start()
    {
        //lock cursor and turn invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        //get mouse inputs and multiply them by mouse sensitivity in both directions and deltaTime
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float MouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        //set x and yRotation variables and clamp vertical look
        yRotation += mouseX;
        xRotation -= MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        //transform the head and the body
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        PlayerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
