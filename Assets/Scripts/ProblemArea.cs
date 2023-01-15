using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameManager;

public class ProblemArea : MonoBehaviour
{
    // Set up serialized variables for this mini game
    [Header("Game connections")]
    [SerializeField] CharacterControllerRB characterController;         // a connection to the character controller
    [SerializeField] GameManager gameManager;                           // a connection to the game manager

    [Header("Problem material and area")]
    [SerializeField] public Material problemMaterial;                   // the material to set up when there is a problem
    [SerializeField] ProblemTypes problemType;                          // the problem type that this is

    // public variables available to other scripts
    public GameObject problemToSolve;
    public bool hasProblem = false;

    // private variables
    private Material originalMaterial;                                  // the original material of the problem area

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (hasProblem) 
        {
            bool startPuzzle = false;

            // Set up the problem based on specific character interactions
            switch(problemType)
            {
                case ProblemTypes.FIX_WIRES:
                    if (characterController.isSolvingCablePuzzle)
                    {
                        // turn off the in game UI
                        gameManager.TurnOffUIMenu();
                        startPuzzle = true;
                    }
                    break;

                case ProblemTypes.CODE_BREAKER:
                case ProblemTypes.SIMON_SAYS:
                    if (characterController.isSolvingTerminalPuzzle)
                    {
                        // turn off the in game UI
                        gameManager.TurnOffUIMenu();
                        startPuzzle = true;
                    }
                    break;

                case ProblemTypes.ASTEROIDS:
                    if (characterController.isPiloting)
                    {
                        startPuzzle = true;
                    }
                    break;

                // default is to start the puzzle (for fire) as the player needs to walk there
                default:
                    startPuzzle = true;
                    break;
            }

            // start the puzzle if it is time
            if (startPuzzle)
            {
                // set the problem as active
                problemToSolve.SetActive(true);
                hasProblem = false;

                // set up the no follow cameara for most games (except fire)
                if (problemType != ProblemTypes.FIRE)
                {
                    gameManager.SetProblemActive(true);
                }
            }
        }
        
    } // end Update

    /// <summary>
    /// Updates the material based on the state of the problem area
    /// </summary>
    public void UpdateMaterial()
    {
        MeshRenderer rendererToUpdate;

        // some things need to happen if it is a UI puzzle type
        switch (problemType)
        {
            // code breaker and simon says are unique as they use the same game object, get their parent material
            case ProblemTypes.CODE_BREAKER:
            case ProblemTypes.SIMON_SAYS:
                rendererToUpdate = gameObject.GetComponentInParent<MeshRenderer>();
                break;

            // all others just use the game object they are attached to
            default:
                rendererToUpdate = gameObject.GetComponent<MeshRenderer>();
                break;
        }

        // if we are starting the problem, then update the material to show there is a problem with this game object
        if (hasProblem)
        {
            // the fire problem has a special floor material to turn on so do that
            if (problemType == ProblemTypes.FIRE)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }

            originalMaterial = rendererToUpdate.material;
            rendererToUpdate.material = problemMaterial;
        }
        // otherwise set it back to normal
        else
        {
            // the fire problem has a special floor material to turn off so do that
            if (problemType == ProblemTypes.FIRE)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }

            rendererToUpdate.material = originalMaterial;
        }

    } // end UpdateMaterial

}
