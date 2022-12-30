using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ControllerInputManager : MonoBehaviour
{
    // declare references;
    private InputControl inputControl;

    // movement
    public float currentMovement;
    public bool isMovementPressed;
    public bool isRunPressed;
    public bool isJumpPressed;

    //-----------------------------------------------
    public void Awake()
    {
        // initially set reference variables;
        inputControl = new InputControl();

        //------------------------------------------------------------
        // callback function for: movement input;
        inputControl.CharacterControls.Move.started += onMovementInput;
        inputControl.CharacterControls.Move.canceled += onMovementInput;

        // callback function for: Run input;
        inputControl.CharacterControls.Run.started += onRunInput;
        inputControl.CharacterControls.Run.canceled += onRunInput;

        // callback function for: Jump input;
        inputControl.CharacterControls.Jump.started += onJumpInput;
        inputControl.CharacterControls.Jump.performed += onJumpInput;
        inputControl.CharacterControls.Jump.canceled += onJumpInput;
        //------------------------------------------------------------

    }


    //-----------------------------------------------
    private void onMovementInput(InputAction.CallbackContext context)
    {   

        // assign movement input to variable / Walk;
        currentMovement = context.ReadValue<float>();

        // isMovementPressed is true if current movement does not equal 0
        isMovementPressed = currentMovement != 0;


    }   // returns a -1 or +1 value from keyboard ('a' or 'd')

    //-----------------------------------------------
    private void onRunInput(InputAction.CallbackContext context)
    {   

        isRunPressed = context.ReadValueAsButton();


    }   // returns when 'shift' is pressed

    //-----------------------------------------------
    private void onJumpInput(InputAction.CallbackContext context)
    {   

        // trigger jump when button pressed
        if (context.started) { isJumpPressed = true; }            
           
        // release jump if button held
        if (context.performed) { isJumpPressed = false; }
 
        // stop jump if button released
        if (context.canceled) { isJumpPressed = false; }


    }   // returns when 'space bar pressed'. Prevents Retriggering

    //-----------------------------------------------
    private void OnEnable()
    {
        inputControl.CharacterControls.Enable();
    }

    //-----------------------------------------------
    private void OnDisable()
    {
        inputControl.CharacterControls.Disable();
    }

}

