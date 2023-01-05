using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // may not need this
    public enum Puzzles
    {
        FIX_ROBOT,
        CODE_BREAKER,
        FIRE,

    }

    // Constant values
    public const string TIMER_TEXT = "Time until Impact: ";             // the default text for the timer when updated
    public const int TIME_TO_SUN = 600;                                 // seconds before the ship crashes into the sun - 600 is 10 minutes
    public const int TIME_TO_PROBLEM = 10;                              // seconds before the next problem happens
    public const int NUM_PROBLEMS_TO_WIN = 5;                           // the number of puzzles that must be solved to beat the clock
    public const string WIN_TEXT = "You did it!\n Time to go home.";    // the text if the player solves all the problems

    // Public variables
    public bool gameRunning = false;                                    // lets the game know we should count down the timer

    // Serialized variables
    [Header("UI Elements to Update")]
    [SerializeField] GameObject uiMenu;                                 // a link to the ui menu so we can turn it on/off
    [SerializeField] GameObject endMenu;                                // a link to the end menu so we can turn it on
    [SerializeField] Image endBGImage;                                  // if the player wins the endMenu BG to update
    [SerializeField] Sprite winImage;                                   // if the player wins update the endMenu BG to this image
    [SerializeField] TextMeshProUGUI endGameText;                       // the text object to update message to the player at end of the game
    [SerializeField] TextMeshProUGUI timerText;                         // the timer text object to update

    // An array to track the problems
    // TODO: make this cleaner by using an enum and array to store the games - not all are UI based so may be tricky
    [Header("Problem Elements")]
    [SerializeField] GameObject[] problems;                             // a link to the problems as Game Objects

    // Private variables
    private float gameTimer;                                            // to keep track of the game time for end game
    private float problemTimer;                                         // keeps track of time to next problem
    private int numProblemsFixed = 0;                                   // keeps track of how many problems have been fixed to check on end game
    private bool problemStarted = false;                                // to keep track of when problems occur and to reset timer

    public int NumProblemsFixed { get => numProblemsFixed; }

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
            if ( (gameTimer > 0) && (NumProblemsFixed != NUM_PROBLEMS_TO_WIN) )
            {
                UpdateGameTimer();

                // check to see if it is time to start a new problem
                if (!problemStarted)
                {
                    UpdateProblemTimer();
                }
            }
            // otherwise end the game
            else
            {
                EndGame();
            }
        }

    } // end Update

    /// <summary>
    /// Starts the game and sets up the timer
    /// </summary>
    public void StartGame()
    {
        // set up the game timer
        gameTimer = TIME_TO_SUN;
        UpdateGameTimer();

        // set up the next problem timer
        problemTimer = TIME_TO_PROBLEM;

        // mark the game as running
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

        // set up the win image if we got here by solving enough problems
        if (NumProblemsFixed == NUM_PROBLEMS_TO_WIN)
        {
            endBGImage.sprite = winImage;
            endGameText.text = WIN_TEXT;
        }

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
    /// Marks a problem as solved and starts the next problem timer
    /// </summary>
    public void MarkProblemSolved()
    {
        numProblemsFixed++;

        // TODO add timer to launch next problem here
        problemTimer = TIME_TO_PROBLEM;
        problemStarted = false;

    } // end MarkProblemSolved

    /// <summary>
    /// Updates the game time and text in the UI
    /// </summary>
    private void UpdateGameTimer()
    {
        timerText.text = TIMER_TEXT + (int)gameTimer;

        // update the timer value
        gameTimer -= Time.deltaTime;

    } // UpdateGameTimer

    /// <summary>
    /// Updates the in game next problem timer and sets the next problem when it is up
    /// </summary>
    private void UpdateProblemTimer()
    {
        // update the timer value
        problemTimer -= Time.deltaTime;

        if (problemTimer < 0)
        {
            problemStarted = true;

            // turn off the in game UI
            uiMenu.SetActive(false);

            // randomize the next puzzle - right now only starts wire puzzle
            int randomPuzzleIndex = Random.Range(0, problems.Length);
            problems[randomPuzzleIndex].SetActive(true);
        }

    } // end UpdateProblemTimer
}
