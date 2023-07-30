using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private GameManager gameManager;
    public ControlVariables controlVars;
    public float baseSensitivityX = 300;
    public float baseSensitivityY = 300;

    public Transform PlayerOrientation;

    float xRotation;
    float yRotation;
    
    
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //lock cursor and turn invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        if (!gameManager.paused) {
            //get mouse inputs and multiply them by mouse sensitivity in both directions and deltaTime
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * baseSensitivityX * controlVars.sensitivity;
            float MouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * baseSensitivityY * controlVars.sensitivity;

            //set x and yRotation variables and clamp vertical look
            yRotation += mouseX;
            xRotation -= MouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
            //transform the head and the body
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            PlayerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
