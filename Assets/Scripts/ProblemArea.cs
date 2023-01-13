using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameManager;

public class ProblemArea : MonoBehaviour
{
    // Set up serialized variables for this mini game
    [Header("Character controller")]
    [SerializeField] CharacterControllerRB characterController;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameManager.ProblemTypes problemType;

    // public variables available to other scripts
    public GameObject problemToSolve;
    public bool hasProblem = false;

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
        if (hasProblem) 
        {
            bool startPuzzle = false;

            // some things need to happen if it is a UI puzzle type
            switch(problemType)
            {
                case GameManager.ProblemTypes.FIX_WIRES:
                    if (characterController.isSolvingCablePuzzle)
                    {
                        // turn off the in game UI
                        gameManager.TurnOffUIMenu();
                        startPuzzle = true;
                    }
                    break;

                case GameManager.ProblemTypes.CODE_BREAKER:
                    if (characterController.isSolvingTerminalPuzzle)
                    {
                        // turn off the in game UI
                        gameManager.TurnOffUIMenu();
                        startPuzzle = true;
                    }
                    break;

                // default is to start the puzzle (for fire) as the player needs to walk there
                default: 
                    startPuzzle= true;
                    break;
            }

            if (startPuzzle)
            {
                // set the problem as active
                problemToSolve.SetActive(true);
                hasProblem = false;
            }
        }
        
    } // end Update
}
