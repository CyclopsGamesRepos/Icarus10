using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ColorSquare : MonoBehaviour
{
    // Constant values

    // Set up serialized variables for this mini game
    [SerializeField] SimonSays simonSaysGame;
    [SerializeField] SimonSays.ColorValue thisSquareColor;

    // public variables for use in other scripts

    // private variables to be used in random generation of the wires

    public void WhenClicked()
    {
        // only allow a click if the pattern is done and the problem is not solved
        if (!simonSaysGame.problemSolved && simonSaysGame.patternShown)
        {
            simonSaysGame.CheckColor(thisSquareColor);
        }
    }

}
