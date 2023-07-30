using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject canvasObject;
    public GameObject canvasStats;
    public GameObject pauseMenu;
    private PlayerController playerController;
    private Vector3 velocitySustained;
    // Start is called before the first frame update

    public bool paused;
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        canvasObject.SetActive(true); //so I dont have to look at it in the scene view
                                      //but itll turn on when game starts
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        PauseGame();
    }


    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.P) && !paused)
        {
            velocitySustained = playerController.playerRB.velocity;
            playerController.playerRB.velocity = Vector3.zero;
            paused = true;
            canvasStats.SetActive(false);
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.P) && paused)
        {
            playerController.playerRB.velocity = velocitySustained;
            paused = false;
            canvasStats.SetActive(true);
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
