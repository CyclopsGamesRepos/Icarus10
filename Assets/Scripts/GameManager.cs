using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Constant values
    public const string TIMER_TEXT = "Time until Impact: ";     // the default text for the timer when updated
    // TODO: Increase to 600 seconds later when testing the game more (10 minutes)
    public const int TIME_TO_SUN = 5;                         // seconds before the ship crashes into the sun

    // Public variables
    public bool gameRunning = false;                            // lets the game know we should count down the timer

    // Serialized variables
    [Header("UI Elements to Update")]
    [SerializeField] GameObject uiMenu;                         // a link to the ui menu so we can turn it on/off
    [SerializeField] GameObject endMenu;                        // a link to the end menu so we can turn it on
    [SerializeField] TextMeshProUGUI timerText;                 // the timer text object to update

    // Private variables
    private float gameTimer = TIME_TO_SUN;                      // to keep track of the game time for end game

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        
    } // end Start

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (gameRunning)
        {
            // as long as there is time left, conintue the game
            if (gameTimer > 0)
            {
                UpdateGameTimer();
            }
            // otherwise end the game
            else
            {
                EndGame();
            }

            // update the timer value
            gameTimer -= Time.deltaTime;
        }

    } // end Update

    /// <summary>
    /// Starts the game and sets up the timer
    /// </summary>
    public void StartGame()
    {
        gameTimer = TIME_TO_SUN;
        UpdateGameTimer(); 
        gameRunning = true;

    } // end StartGame

    /// <summary>
    /// Displays the end game and restart and quit buttons - quits the game if selected or reloads the scene if not
    /// </summary>
    public void EndGame()
    {
        // let the game know it is not running any more - in a menu
        gameRunning = false;

        // turn off the UI menu and turn on the End menu
        uiMenu.SetActive(false);
        endMenu.SetActive(true);

    } // end EndGame

    /// <summary>
    /// When the restart button is clicked, restarts the game
    /// </summary>
    public void RestartGame()
    {
        // If the restart button clicked, just reload the scene to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    } // RestartGame

    /// <summary>
    /// Updates the game time and text in the UI
    /// </summary>
    private void UpdateGameTimer()
    {
        timerText.text = TIMER_TEXT + (int)gameTimer;

    } // UpdateGameTimer
}
