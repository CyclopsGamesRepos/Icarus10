using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class SimonSays : MonoBehaviour
{
    public enum ColorValue
    {
        RED, GREEN, BLUE, YELLOW
    }

    // Constant values
    private const string NOPE_MESSAGE = "Try again";            // message if the player gets it wrong
    private const string SUCCESS_MESSAGE = "You didi it!";      // message if they get it right
    private const float TIME_TO_SHOW = 1.0f;                    // time show the color of a pattern (use invoke to reset it?)
    private const float TIME_TO_PAUSE = 0.5f;                   // time to wait before showing next color (or after user enters it)
    private const float TIME_TO_RESET = 2.5f;                   // time to wait before showing next pattern
    private const float FADED_ALPHA = 0.3f;                     // the value to set the alpha for normal display
    private const float FULL_ALPHA = 1;                         // the value to set the alpha of the color when displaying the pattern to user
    private const int STARTING_NUM_COLORS = 4;                  // the starting number of colors to show for the code break

    // Set up serialized variables for this mini game
    [Header("Images to highlight")]
    [SerializeField] Image[] ColorSquares;                      // a link to the squares to highlight in order of the enum

    [Header("UI Elements for checking and finished")]
    [SerializeField] GameObject messageArea;                    // the message area for showing when correct or failure
    [SerializeField] TMP_Text messageText;                      // a text box to let player know if they got the code or not
    [SerializeField] GameObject doneButton;                     // the button used to exit the game when it is done

    // public variables for use in other scripts
    public bool patternShown;                                   // if the pattern has been completed, let the user try
    public bool problemSolved;                                  // used to verify if the problem has been solved

    // private variables to be used in random generation of the wires
    private List<ColorValue> colorValues;                       // a list to keep the pattern generated
    private float showTimer;                                    // the fade in timer for the actual code\
    private bool waitForNextColor;                              // is the timer waiting for the next color to be shown
    private int currentPatternIndex;                            // the index in the list we are showing
    private int userChoicePatternIndex;                         // the index in the list we are showing
    private int numTimesCompleted;                              // the number of times this puzzle has been completed (used to increease difficulty)

    /// <summary>
    /// OnEnable is called before the first frame update and every time this object is enabled
    /// </summary>
    void OnEnable()
    {
        SetRandomColor();

        // reset the game buttons and initial text
        messageText.text = NOPE_MESSAGE;
        doneButton.SetActive(false);
        problemSolved = false;
        patternShown = false;

    } // end OnEnable

    /// <summary>
    /// Start is called once when the object is created/visible. Using to set up basic variables not initialized again
    /// </summary>
    private void Start()
    {
        numTimesCompleted = 0;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Only update timer etc if the problem hasn't been fixed
        if (!problemSolved)
        {
            if (!patternShown)
            {
                ShowPattern();
            }
        }

    } // end Update

    /// <summary>
    /// Checks to see if the color clicked is the correct one in the order created
    /// </summary>
    public void CheckColor(ColorValue colorAttempted)
    {
        // Highlight the chosen color and set it to reset (use time to pause as we use it to reset the pattern too)
        UpdateSquareAlpha(FULL_ALPHA, currentPatternIndex);
        userChoicePatternIndex = currentPatternIndex;
        Invoke("ResetAlpha", TIME_TO_PAUSE);

        // check the color compared to the one on the list
        if (colorAttempted == colorValues[currentPatternIndex])
        {
            // increase the index by one so we can check the next one
            currentPatternIndex++;

            // if that was the last color in the pattern, it is finished
            if (currentPatternIndex >= colorValues.Count)
            {
                numTimesCompleted++;
                messageText.text = SUCCESS_MESSAGE;
                doneButton.SetActive(true);
                problemSolved = true;

                messageArea.SetActive(true);
                Debug.Log("Simon Says Puzzle solved.");
            }
        }
        // otherwise they pressed a wrong color, time to start again
        else
        {
            messageText.text = NOPE_MESSAGE;
            Invoke("SetRandomColor", TIME_TO_RESET);
            
            // set the message window to be active
            messageArea.SetActive(true);
            Invoke("TurnOffMessage", TIME_TO_RESET);
        }

    }

    /// <summary>
    /// Updates the fade timer and shows the pattern to the user
    /// </summary>
    private void ShowPattern()
    {
        // if the show color timer isn't finished, keep waiting
        if (showTimer > 0)
        {
            showTimer -= Time.deltaTime;
        }
        // otherwise update 
        else
        {
            // if we were pausing between colors, then set the next color in the pattern to show
            if (waitForNextColor)
            {
                // if it was the last index mark us as done
                if (currentPatternIndex >= colorValues.Count)
                {
                    patternShown = true;
                    currentPatternIndex = 0;        // re-using for player to check if they got it right
                }
                // otherwise time to set up the next color
                else
                {
                    waitForNextColor = false;
                    showTimer = TIME_TO_SHOW;
                    UpdateSquareAlpha(FULL_ALPHA, currentPatternIndex);
                }
            }
            // otherwise it was a color just shown, so set it back to the non-highlighted alpha and pause
            else
            {
                UpdateSquareAlpha(FADED_ALPHA, currentPatternIndex);
                waitForNextColor = true;
                showTimer = TIME_TO_PAUSE;

                // set the next index so it will show the next color in the pattern
                currentPatternIndex++;
            }
        }

    } // end UpdateFadeTimer

    /// <summary>
    /// Grabs a random color pattern and sets up the fading for the player to see it
    /// </summary>
    private void SetRandomColor()
    {
        // create a new list to keep the pattern (discarding the old by doing so)
        colorValues = new List<ColorValue>();

        // set up the random colors based on the number for this puzzle
        for (int i = 0; i < (STARTING_NUM_COLORS + numTimesCompleted); i++) 
        {
            colorValues.Add((ColorValue)Random.Range(0, System.Enum.GetNames(typeof(ColorValue)).Length) );
        }

        // set up the fade in of the actual code
        showTimer = TIME_TO_SHOW;
        currentPatternIndex = 0;
        patternShown = false;
        waitForNextColor = false;

        UpdateSquareAlpha(FULL_ALPHA, currentPatternIndex);

    } // SetRandomCode

    /// <summary>
    /// Udpates the alpha of the square at the index of the pattern
    /// </summary>
    /// <param name="alphaUpdate">The alpha value to change it to</param>
    /// <param name="indexOfSquare">the index into the pattern array</param>
    private void UpdateSquareAlpha(float alphaUpdate, int indexOfSquare)
    {
        int squareToUpdate = (int)colorValues[indexOfSquare];
        Color updatedColor = ColorSquares[squareToUpdate].color;
        updatedColor.a = alphaUpdate;
        ColorSquares[(int)colorValues[indexOfSquare]].color = updatedColor;

    } // end UpdateSquareAlpha
 
    /// <summary>
    /// Used to reset a user's square choice
    /// </summary>
    private void ResetAlpha()
    {
        UpdateSquareAlpha(FADED_ALPHA, userChoicePatternIndex);

    } // end ResetAlpha

    /// <summary>
    /// Turns off the done message being displayed
    /// </summary>
    private void TurnOffMessage()
    {
        messageArea.SetActive(false);

    }// end TurnOffMessage
}
