using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // TODO: perhaps create a problem script to deal with problems and clean this code up
    public enum ProblemTypes
    {
        FIX_WIRES,
        CODE_BREAKER,
        FIRE,
        ASTEROIDS,
        SIMON_SAYS
    }

    // Constant values
    public const string TIMER_TEXT = "Time until Impact: ";             // the default text for the timer when updated
    public const int TIME_TO_SUN = 600;                                 // seconds before the ship crashes into the sun - 600 is 10 minutes
    public const int TIME_TO_PROBLEM = 10;                              // seconds before the next problem happens
    public const int NUM_PROBLEMS_TO_WIN = 13;                          // the number of problems that must be solved to beat the clock
    public const string WIN_TEXT = "You did it!\n Time to go home.";    // the text if the player solves all the problems

    // Serialized variables
    [Header("Player Data")]
    [SerializeField] CharacterControllerRB characterControllerRB;       // set up the player character controller to update animations...maybe

    [Header("UI Elements to Update")]
    [SerializeField] GameObject uiMenu;                                 // a link to the ui menu so we can turn it on/off
    [SerializeField] GameObject endMenu;                                // a link to the end menu so we can turn it on
    [SerializeField] Image endBGImage;                                  // if the player wins the endMenu BG to update
    [SerializeField] Sprite winImage;                                   // if the player wins update the endMenu BG to this image
    [SerializeField] TextMeshProUGUI endGameText;                       // the text object to update message to the player at end of the game
    [SerializeField] TextMeshProUGUI timerText;                         // the timer text object to update

    // An array to track the problems
    [Header("Problem Game Locations and material")]
    [SerializeField] GameObject[] wireProblemAreas;                     // objects that can have the wire problem spring up
    [SerializeField] GameObject[] codeProblemAreas;                     // objects that can have the code problem spring up
    [SerializeField] GameObject[] fireProblemAreas;                     // objects that can have the fire problem spring up
    [SerializeField] GameObject[] asteroidProblemAreas;                 // objects that can have the asteroid problem spring up
    [SerializeField] GameObject[] simonSaysProblemAreas;                // objects that can have the simon says problem spring up

    // Public variables
    public bool gameRunning = false;                                    // lets the game know we should count down the timer
    public bool onFire = false;                                         // to let the controller system know not to allow other actions until fire is out
    public int currentProblemType;                                      // needed to put the material back to normal
    public int currentProblemLocation;                                  

    // Private variables
    private GameObject[][] problemAreas;                                // the list of possilble places for each problem
    private float gameTimer;                                            // to keep track of the game time for end game
    private float problemTimer;                                         // keeps track of time to next problem
    private bool problemStarted = false;                                // to keep track of when problems occur and to reset timer
    private bool problemActive = false;                                 // used to stop follow camera when not in fire state
    private int numProblemsFixed = 0;                                   // keeps track of how many problems have been fixed to check on end game
    private int numProblemTypes;                                        // the total number of problem types for the problem arrays

    public int NumProblemsFixed { get => numProblemsFixed; }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // set up the overall problem area spaces (some may be only a single area)
        numProblemTypes = System.Enum.GetNames(typeof(ProblemTypes)).Length;
        problemAreas = new GameObject[numProblemTypes][];

        problemAreas[(int)ProblemTypes.FIX_WIRES]       = wireProblemAreas;
        problemAreas[(int)ProblemTypes.CODE_BREAKER]    = codeProblemAreas;
        problemAreas[(int)ProblemTypes.FIRE]            = fireProblemAreas;
        problemAreas[(int)ProblemTypes.ASTEROIDS]       = asteroidProblemAreas;
        problemAreas[(int)ProblemTypes.SIMON_SAYS]      = simonSaysProblemAreas;             

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
        GetComponent<AudioSource>().Play();

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
    /// Sets the whether the player is actively solving the problem or not
    /// </summary>
    /// <param name="activeState"></param>
    public void SetProblemActive(bool activeState)
    {
        problemActive = activeState;

    } // end SetProblemActive

    /// <summary>
    /// Returns wether the player is attempting to solve a problem
    /// </summary>
    /// <returns></returns>
    public bool IsProblemActive()
    {
        return problemActive;

    } // end IsProblemActive

    /// <summary>
    /// Marks a problem as solved and starts the next problem timer
    /// </summary>
    public void MarkProblemSolved()
    {
        numProblemsFixed++;

        // TODO add timer to launch next problem here
        problemTimer = TIME_TO_PROBLEM;
        problemStarted = false;
        SetProblemActive(false);

        // put the original material back on the object
        problemAreas[currentProblemType][currentProblemLocation].GetComponent<ProblemArea>().UpdateMaterial();

        Debug.Log(NumProblemsFixed + " problems solved.");

    } // end MarkProblemSolved

    /// <summary>
    /// Used by other scripts to turn off the UI menu when going into a menu based mini game
    /// </summary>
    public void TurnOffUIMenu()
    {
        uiMenu.SetActive(false);
    }

    /// <summary>
    /// Adjusts the game timer by the given amount
    /// </summary>
    /// <param name="numSeconds">the number of seconds to add or remove (negative)</param>
    public void AdjustGameTimer(int numSeconds)
    {
        gameTimer += numSeconds;

    } // end AdjustGameTimer

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

            // randomize the next problem type
            currentProblemType = Random.Range(0, numProblemTypes);

            // DEBUG: to test your specific problem, use the enum type here instead of the random one above (comment it out when done)
            //currentProblemType = (int)ProblemTypes.FIRE;

            // now randomize the next problem location
            currentProblemLocation = Random.Range(0, problemAreas[currentProblemType].Length);

            // mark the problem area as having a problem
            ProblemArea prblemAreaScript = problemAreas[currentProblemType][currentProblemLocation].GetComponent<ProblemArea>();
            prblemAreaScript.hasProblem = true;

            // set the object there to the problem material to signify that it needs help (store the original material for later)
            prblemAreaScript.UpdateMaterial();

        }

    } // end UpdateProblemTimer
}
