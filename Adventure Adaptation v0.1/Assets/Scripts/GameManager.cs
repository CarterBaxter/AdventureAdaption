using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject canvasObject;
    // Start is called before the first frame update
    void Start()
    {
        canvasObject.SetActive(true); //so I dont have to look at it in the scene view
                                       //but itll turn on when game starts
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
