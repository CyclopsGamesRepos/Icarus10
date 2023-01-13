using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeBreakerGame : MonoBehaviour
{
    // Constant values
    public const string NOPE_MESSAGE = "Try again";             // message if the player gets it wrong
    public const string SUCCESS_MESSAGE = "You didi it!";       // message if they get it right
    public const float FADE_INCREMENT = 0.1f;                   // the amount to increase the alpha fade of the actual code
    public const float TIME_TO_FADE = 0.5f;                     // time between fade in of actual code
    public const int MAX_CODE_VALUE = 1000;                     // used for generating the code, currently just three digits

    // Set up serialized variables for this mini game
    [Header("Code text and sliders")]
    [SerializeField] TMP_Text actualCodeText;                   // a link to the actual code text that will get updated and slowly fade in
    [SerializeField] Slider hundredsDigitSlider;                // the hundreds digit slider so we can get its value and update the handle
    [SerializeField] TMP_Text hundredsDigitText;                // a link to the hundreds digit code text that will get updated when the slider gets updated
    [SerializeField] Slider tensDigitSlider;                    // the hundreds digit slider so we can get its value and update the handle
    [SerializeField] TMP_Text tensDigitText;                    // a link to the tens digit code text that will get updated when the slider gets updated
    [SerializeField] Slider onesDigitSlider;                    // the hundreds digit slider so we can get its value and update the handle
    [SerializeField] TMP_Text onesDigitText;                    // a link to the ones digit code text that will get updated when the slider gets updated

    [Header("UI Elements for checking and finished")]
    [SerializeField] GameObject messageArea;                    // the message area for showing when correct or failure
    [SerializeField] TMP_Text messageText;                      // a text box to let player know if they got the code or not
    [SerializeField] GameObject checkCodeButton;                // the button used to check the code - need to disable when the code is correct
    [SerializeField] GameObject doneButton;                     // the button used to exit the game when it is done

    // public variables for use in other scripts

    // private variables to be used in random generation of the wires
    private float fadeTimer;                                    // the fade in timer for the actual code
    private float fadeValue;                                    // the value for the fade in
    private bool fadedIn;                                       // if the code is faded in, time to reset it
    private bool problemSolved;                                 // used to verify if the problem has been solved
    private int randomCode;                                     // the random code the user is trying to input
    private int hundredsDigit;                                  // the hundreds digit of the attemtped code - linked to a slider
    private int tensDigit;                                      // the tens digit of the attemtped code - linked to a slider
    private int onesDigit;                                      // the ones digit of the attemtped code - linked to a slider
    private int timesAttempted;                                 // keep track of the times attempted to increase time for fade in to make it easier over time

    /// <summary>
    /// OnEnable is called before the first frame update and every time this object is enabled
    /// </summary>
    void OnEnable()
    {
        SetRandomCode();

        // set up initial slider elements
        UpdateHundredsValue();
        UpdateTensValue();
        UpdateOnesValue();

        // reset the game buttons and initial text
        messageText.text = NOPE_MESSAGE;
        checkCodeButton.SetActive(true);
        doneButton.SetActive(false);
        problemSolved = false;
        timesAttempted = 0;

    } // end OnEnable

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Only update timer etc if the problem hasn't been fixed
        if (!problemSolved)
        {
            UpdateFadeTimer();
        }

    } // end Update

    /// <summary>
    /// Checks to see if the code is acurate and shows the done button if so, stopping timer etc.
    /// </summary>
    public void CheckCode()
    {
        // check each digit to see if the value mathes the code
        int userCode = (hundredsDigit * 100) + (tensDigit * 10) + onesDigit;

        // if they are the same, game over
        if (userCode == randomCode)
        {
            messageText.text = SUCCESS_MESSAGE;
            messageArea.SetActive(true);
            checkCodeButton.SetActive(false);
            doneButton.SetActive(true);
            problemSolved = true;

            Debug.Log("Code Puzzle solved.");
        }
        // otherwise let them no to continue trying
        else
        {
            messageText.text = NOPE_MESSAGE;
            messageArea.SetActive(true);
            Invoke("TurnOffMessage", 2);
        }
    }

    /// <summary>
    /// Updates the hundreds digit of the attempted code
    /// </summary>
    public void UpdateHundredsValue()
    {
        hundredsDigit = (int)hundredsDigitSlider.value;
        hundredsDigitText.text = hundredsDigit.ToString();

    } // UpdateHundredsValue

    /// <summary>
    /// Updates the tens digit of the attempted code
    /// </summary>
    public void UpdateTensValue()
    {
        tensDigit = (int)tensDigitSlider.value;
        tensDigitText.text = tensDigit.ToString();

    } // UpdateTensValue

    /// <summary>
    /// Updates the Ones digit of the attempted code
    /// </summary>
    public void UpdateOnesValue()
    {
        onesDigit = (int)onesDigitSlider.value;
        onesDigitText.text = onesDigit.ToString();

    } // UpdateOnesValue

    /// <summary>
    /// Updates the fade timer and shows more of the actual code as it continues
    /// </summary>
    private void UpdateFadeTimer()
    {
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
        }
        else
        {
            // update the code if the fade in has happened and timer is up again
            if (fadedIn)
            {
                SetRandomCode();
                timesAttempted++;
            }
            // otherwise continue fading in the code
            else
            {
                fadeTimer = TIME_TO_FADE + (timesAttempted * FADE_INCREMENT);
                fadeValue += FADE_INCREMENT;
                UpdateActualCodeFade();

                if (fadeValue >= 1)
                {
                    fadedIn = true;
                }
            }
        }

    } // end UpdateFadeTimer

    /// <summary>
    /// Grabs a random code and sets up the fading for the player to see it
    /// </summary>
    private void SetRandomCode()
    {
        // set up the random code between 0 and 999 right now
        randomCode = Random.Range(0, MAX_CODE_VALUE);

        // update the code value - since we want to show all digits, we may need to adjust the string
        string codeToShow = randomCode.ToString();

        // add a zero for the hundreds digit if the code is just two digits
        if (randomCode < 100)
        {
            codeToShow = "0" + codeToShow;
        }

        // add a zero for the tens digit if the code is a single digit
        if (randomCode < 10)
        {
            codeToShow = "0" + codeToShow;

        }

        actualCodeText.text = codeToShow;

        // set up the fade in of the actual code
        fadeTimer = TIME_TO_FADE;
        fadeValue = 0;
        fadedIn = false;

        UpdateActualCodeFade();

    } // SetRandomCode

    /// <summary>
    /// Updates the fade on screen of the actual code
    /// </summary>
    private void UpdateActualCodeFade()
    {

        Color updatedColor = actualCodeText.color;
        updatedColor.a = Mathf.Clamp(fadeValue, 0, 1);
        actualCodeText.color = updatedColor;

    } 
    
    // end UpdateSquareToFade
    /// <summary>
    /// Turns off the done message being displayed
    /// </summary>
    private void TurnOffMessage()
    {
        messageArea.SetActive(false);

    }// end TurnOffMessage

}
