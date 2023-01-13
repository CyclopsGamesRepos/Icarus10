using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Wires : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // serialized variables for ease of access
    [SerializeField] Canvas parentCanvas;   // the main canvas (may need to make this a sub canvas or separate one for mini games)
    [SerializeField] WireGame wireGame;     // a link back to the main wire game so we can set which wires are being touched

    // public variables
    public Image wireImage;                 // The image used by this wire
    public bool isConnected = false;        // to keep track of the wire if it is connected

    // private variables
    private LineRenderer wireLine;          // the renderer we will use for this line to show the wire being fixed
    private bool beingDragged = false;      // is the wire currently being dragged with the mouse

    /// <summary>
    /// OnEnable is called when the object is created and every time it is enabled
    /// </summary>
    private void OnEnable()
    {
        // set up the variables for this wire
        wireImage = GetComponent<Image>();
        wireLine = GetComponent<LineRenderer>();

        // reset the wire
        wireLine.SetPosition(0, Vector3.zero);
        wireLine.SetPosition(1, Vector3.zero);
        isConnected = false;

    } // end OnEnable

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (beingDragged)
        {
            // grab the mouse point position in the canvas
            Vector2 movePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform,
                Input.mousePosition, parentCanvas.worldCamera, out movePos);

            // now update render line to that point from where it started
            wireLine.SetPosition(0, transform.position);
            wireLine.SetPosition(1, parentCanvas.transform.TransformPoint(movePos));
        }
        // otherwise clear the line being rendered
        else
        {
            // only clear it if it isn't connected
            if (!isConnected)
            {
                wireLine.SetPosition(0, Vector3.zero);
                wireLine.SetPosition(1, Vector3.zero);
            }
        }

        // check to see if this wire has the mouse over it
        bool mouseOver = RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera);

        // if so, then set this as the current mouse over wire
        if (mouseOver)
        {
            wireGame.mouseOverWire = this;
        }
        
    } // end Update

    /// <summary>
    /// Sets the color of this wire to the color given
    /// </summary>
    /// <param name="newColor">The new color to make this wire</param>
    public void SetWireColor(Color newColor)
    {
        // set the wire image color to the new color
        wireImage.color = newColor;
        wireLine.startColor = newColor;
        wireLine.endColor = newColor;

    } // end SetWireColor

    /// <summary>
    /// Used for the interfaces above but not used in this program
    /// </summary>
    /// <param name="eventData">The drag event</param>
    public void OnDrag(PointerEventData eventData)
    {
    } // end OnDrag

    public void OnBeginDrag(PointerEventData eventData)
    {
        // only set the line to draw if it is a start wire and it is not already connected
        if (!isConnected)
        {
            beingDragged = true;
            //wireGame.selectedWire = this;
        }

    } // end OnBeginDrag

    public void OnEndDrag(PointerEventData eventData)
    {
        // when ending the drag, check to see if the colors match
        if ( (wireGame.mouseOverWire != null) && !isConnected)
        {
            if (wireGame.mouseOverWire.wireImage.color == wireImage.color)
            {
                // we have a connection so set both endpoints to connected
                isConnected = true;
                wireGame.mouseOverWire.isConnected = true;
                wireGame.wiresToConnect--;
            }
        }

        // clear the selected wire to make sure they don't accidentally get chosen again
        wireGame.mouseOverWire = null;
        beingDragged = false;

    } // end OnEndDrag
}
