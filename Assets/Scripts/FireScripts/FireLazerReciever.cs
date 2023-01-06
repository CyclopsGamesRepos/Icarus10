using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLazerReciever : MonoBehaviour
{
    // constant values
    private const float TIME_T0_PUT_OUT = 0.5f;     // the time it takes to put out the fire

    // public variables

    // private variables
    private FireGame fireGame;                      // used to keep track of what fire game created this fire
    private float fireOutTimer;

    /// <summary>
    /// OnEnable is called when the object is created and any time it is enabled - setup code
    /// </summary>
    private void OnEnable()
    {
        fireOutTimer = TIME_T0_PUT_OUT;

    } // end OnEnable

    /// <summary>
    /// Called when a hit is detected - will use to put out the fire
    /// </summary>
    void hitDetected()
    {
        // check to see if it time to put out this fire
        if (fireOutTimer <= 0)
        {
            fireGame.DecreaseFireCount(this.gameObject);
            Destroy(gameObject);
        }
        
        // decrease the timer
        fireOutTimer -= Time.deltaTime;
        Debug.Log("Hit Detected");
    }

    /// <summary>
    /// Sets a link to the fire game that created this fire so it can put itself out and update the game
    /// </summary>
    /// <param name="fireGameLink">A link back to the calling script</param>
    public void setFireGame(FireGame fireGameLink)
    {
        fireGame = fireGameLink;

    } // end setFireGame
}
