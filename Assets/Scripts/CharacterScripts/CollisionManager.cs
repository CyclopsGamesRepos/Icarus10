using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // serialized variables
    [SerializeField] GameManager gameManager;                   // a link to the game manager to set problem as solved

    public bool inCockpitZone;
    public bool inCablePuzzleZone;
    public bool inTerminalPuzzleZone;
    public bool inFireZone;

    public bool interactionAllowed;

    //--------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // wrapping the non-fire zone coniditions with the mini game active as they should not go in unless the fires are out
        if (!gameManager.onFire)
        {
            //--------------------------------------
            if (other.gameObject.tag == "PilotZone")
            {
                Debug.Log("Entered: " + other.gameObject.tag + " | Press " + "'" + "e" + "'" + " to interact.");

                interactionAllowed = true;
                inCockpitZone = true;
            }

            //--------------------------------------
            if (other.gameObject.tag == "CablePuzzleZone")
            {
                Debug.Log("Entered: " + other.gameObject.tag + " | Press " + "'" + "e" + "'" + " to interact.");

                interactionAllowed = true;
                inCablePuzzleZone = true;
            }

            //--------------------------------------
            if (other.gameObject.tag == "TerminalPuzzleZone")
            {
                Debug.Log("Entered: " + other.gameObject.tag + " | Press " + "'" + "e" + "'" + " to interact.");

                interactionAllowed = true;
                inTerminalPuzzleZone = true;
            }
        }
        else
        {
            inCockpitZone = false;
            inCablePuzzleZone = false;
            inTerminalPuzzleZone = false;
        }

        //--------------------------------------
        if (other.gameObject.tag == "FireZone")
        {
            Debug.Log("Entered: " + other.gameObject.tag + " | Press " + "'" + "e" + "'" + " to interact.");

            interactionAllowed = true;
            inFireZone = true;
        }
    }

    //--------------------------------------
    private void OnTriggerExit(Collider other)
    {
        // wrapping the non-fire zone coniditions with the mini game active as they should not go out unless the fires are out
        if (!gameManager.onFire)
        {
            //--------------------------------------
            if (other.gameObject.tag == "PilotZone")
            {
                Debug.Log("Exited: " + other.gameObject.tag);

                interactionAllowed = false;
                inCockpitZone = false;
            }

            //--------------------------------------
            if (other.gameObject.tag == "CablePuzzleZone")
            {
                Debug.Log("Exited: " + other.gameObject.tag);

                interactionAllowed = false;
                inCablePuzzleZone = false;
            }

            //--------------------------------------
            if (other.gameObject.tag == "TerminalPuzzleZone")
            {
                Debug.Log("Exited: " + other.gameObject.tag);

                interactionAllowed = false;
                inTerminalPuzzleZone = false;
            }
        }
        else
        {
            inCockpitZone = false;
            inCablePuzzleZone = false;
            inTerminalPuzzleZone = false;
        }

        //--------------------------------------
        if (other.gameObject.tag == "FireZone")
        {
            Debug.Log("Exited: " + other.gameObject.tag);

            interactionAllowed = false;
            inFireZone = false;
        }
    }


}
