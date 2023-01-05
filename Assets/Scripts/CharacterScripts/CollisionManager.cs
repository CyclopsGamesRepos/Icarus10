using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public bool inCockpitZone;
    public bool inCablePuzzleZone;
    public bool inTerminalPuzzleZone;
    public bool inFireZone;

    public bool interactionAllowed;

    //--------------------------------------
    private void OnTriggerEnter(Collider other)
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
        //--------------------------------------
        if (other.gameObject.tag == "PilotZone")
        {
            Debug.Log("Exited: " + other.gameObject.tag);

            interactionAllowed = false;
            inCockpitZone = false;
        }

        //--------------------------------------
        if (other.gameObject.tag == "CablePuzzleZone" )
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

        //--------------------------------------
        if (other.gameObject.tag == "FireZone")
        {
            Debug.Log("Exited: " + other.gameObject.tag);

            interactionAllowed = false;
            inFireZone = false;
        }
    }


}
