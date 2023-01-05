using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireGame : MonoBehaviour
{
    // Set up serialized variables for this mini game
    [Header("Lists for wires and possible colors")]
    [SerializeField] List<Color> wireColors;    // all colors that could be selected
    [SerializeField] List<Wires> startWires;    // the wire starting points
    [SerializeField] List<Wires> endWires;      // the wire ending points
    [SerializeField] GameObject doneButton;     // access to the button to start when the game is complete

    // public variables for use in other scripts
    public Wires mouseOverWire;                 // what wire the mouse is hovering over
    public int wiresToConnect;                  // how many wires need to be connected

    // private variables to be used in random generation of the wires
    private List<Color> availableColors;        // to keep track of what colors are available to put on the wires
    private List<int> startWireIndices;         // to keep track of possible start wire points
    private List<int> endWireIndices;           // to keep track of possible end wire points
    private bool gameDone;                      // added to keep it from continually setting the done button to active

    /// <summary>
    /// OnEnable is called when the object is activated
    /// </summary>
    void OnEnable()
    {
        // game done is true here to prevent the update from instant winning - it is set to false when wires set up
        gameDone = true;

        // setting up a delay so the wires have time to be initialized
        Invoke("SetUpWires", 0.1f);

    } // end OnEnable

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Check to see if it is time to let the player see the done button
        if ( (wiresToConnect <= 0) && !gameDone)
        {
            // Debug code - TODO: Comment out before release
            GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            Debug.Log("Wire Puzzle solved. " + gameManager.NumProblemsFixed + " puzzles solved.");

            // set up the done button amd mark this game as done as well
            doneButton.SetActive(true);
            gameDone = true;
        }
        
    } // end Update

    /// <summary>
    /// Sets up the wires for the mini game
    /// </summary>
    private void SetUpWires()
    {
        // set up the lists for the random wire generation system
        availableColors = new List<Color>(wireColors);
        startWireIndices = new List<int>();
        endWireIndices = new List<int>();

        // for end level reset the wires to connect
        doneButton.SetActive(false);
        gameDone = false;
        wiresToConnect = 0;

        // populate the wire indices lists with the indices from the wire lists (to make sure we have the correct number of indices)
        for (int index = 0; index < startWires.Count; index++)
        {
            startWireIndices.Add(index);
            wiresToConnect++;
        }

        for (int index = 0; index < endWires.Count; index++)
        {
            endWireIndices.Add(index);
        }

        // now go through the lists until all available wire indices are filled (probably only need one as we remove them together)
        while ((startWireIndices.Count > 0) && (endWireIndices.Count > 0))
        {
            // choose a random color from those available
            Color chosenColor = availableColors[Random.Range(0, availableColors.Count)];

            // randomize both ends of the wires
            int startIndex = Random.Range(0, startWireIndices.Count);
            int endIndex = Random.Range(0, endWireIndices.Count);

            // store the color in the wire start and end selected
            startWires[startWireIndices[startIndex]].SetWireColor(chosenColor);
            endWires[endWireIndices[endIndex]].SetWireColor(chosenColor);

            // remove the color and the indices
            availableColors.Remove(chosenColor);
            startWireIndices.RemoveAt(startIndex);
            endWireIndices.RemoveAt(endIndex);
        }

    } // end SetUpWires

}
