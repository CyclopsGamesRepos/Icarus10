using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
    // references
    private Animator animator;
    private Rigidbody rb;
    private PlayerInputControl playerInput;

    private bool isWalking;
    private float horizontalInput;
    private bool isMovementPressed;
    private bool isDirection = true;
    private bool isJumping;
    private bool isForward = true;
    private float direction = 1f;
   

    //--------------------------------------------------------------------                                                      
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInputControl();

        //-----------------------------
        // callback function key pressed / released
        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;

        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;


        //playerInput.CharacterControls.Jump.started += context =>
        //{
        //    Debug.Log(context.ReadValue<float>());
        //};

        //playerInput.CharacterControls.Jump.canceled += context =>
        //{
        //    Debug.Log(context.ReadValue<float>());
        //};
    }

    //--------------------------------------------------------------------                                                      
    void OnMovementInput(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
        isMovementPressed = horizontalInput != 0;
    }

    //--------------------------------------------------------------------                                                      
    void OnJump(InputAction.CallbackContext context)
    {
        float jumpPressed = context.ReadValue<float>();
    }

    //--------------------------------------------------------------------                                                      
    void FixedUpdate()
    {

        if (Mathf.Abs(horizontalInput) > 0.01f && !isJumping)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3((float)0, 0, horizontalInput));
            rb.MovePosition(rb.position + transform.forward * 2f * Time.fixedDeltaTime);


            if (!isWalking)
            {
                isWalking = true;
                animator.SetBool("isWalking", true);
            }

        }
        else if (isWalking)
        {
            isWalking = false;
            animator.SetBool("isWalking", false);
        }

    }

    //--------------------------------------------------------------------                                                      
    void Update()
    {
        Debug.Log(isForward);
    }

    //--------------------------------------------------------------------                                                      
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    //--------------------------------------------------------------------                                                      
    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
