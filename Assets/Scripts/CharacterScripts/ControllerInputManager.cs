using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ControllerInputManager : MonoBehaviour
{
    // declare references;
    private InputControl inputControl;
    private CharacterControllerRB characterController;
    private CollisionManager collisionManager;

    // movement
    public float currentMovement;
    public bool isMovementPressed;
    public bool isRunPressed;
    public bool isJumpPressed;
    public bool isCrouchPressed;

    public float currentAim;
    public bool isAimPressed;

    //-----------------------------------------------
    public void Awake()
    {
        // initially set reference variables;
        inputControl = new InputControl();
        characterController = GetComponent<CharacterControllerRB>();
        collisionManager = GetComponent<CollisionManager>();

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

        // callback function for: Crouch input;
        inputControl.CharacterControls.Crouch.started += onCrouchInput;
        inputControl.CharacterControls.Crouch.canceled += onCrouchInput;

        // callback function for: Sitting input;
        inputControl.CharacterControls.Interact.started += onInteractInput;
        inputControl.CharacterControls.Interact.canceled += onInteractInput;


        // callback function for: aim input;
        inputControl.CharacterControls.Aim.started += onAimInput;
        inputControl.CharacterControls.Aim.canceled += onAimInput;
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
    private void onCrouchInput(InputAction.CallbackContext context)
    {
        // increment toggle variable on release
        if (context.canceled)
        {
             characterController.crouchToggle += 1;
        }

    }   // returns when 'c' is pressed (Toggle Action)


    //-----------------------------------------------
    private void onInteractInput(InputAction.CallbackContext context)
    {
        // increment toggle variable on release
        if (context.canceled)
        {
            if (collisionManager.interactionAllowed)
            {
                characterController.interactToggle += 1;
            }
        }

    }   // returns when 'e' is pressed (Toggle Action)


    //-----------------------------------------------
    private void onAimInput(InputAction.CallbackContext context)
    {

        // assign movement input to variable / Walk;
        currentAim = context.ReadValue<float>();

        // isMovementPressed is true if current movement does not equal 0
        isAimPressed = currentAim != 0;


    }   // returns a -1 or +1 value from keyboard ('a' or 'd')

    //-----------------------------------------------
    private void onJumpInput(InputAction.CallbackContext context)
    {   

        // trigger jump when button pressed
        if (context.started) { isJumpPressed = true; }            
           
        // release jump if button held
        if (context.performed) { isJumpPressed = false; }
 
        // stop jump if button released
        if (context.canceled) { isJumpPressed = false; }


    }   // returns when 'space bar pressed'. (Hold Action)




    //-----------------------------------------------
    //------------Enable / Disable Input-------------
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

