using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlVariables : MonoBehaviour
{
    public float sensitivity = 1; //when implementing slider remember to not allow negatives

    [Header("KEYBINDS")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C; //was control but changed because cntrol likes to try and 
                                            //do shortcuts so you would have to stop moving to uncrouch


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
