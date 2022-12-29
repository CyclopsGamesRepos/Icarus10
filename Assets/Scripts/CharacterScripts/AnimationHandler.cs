using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    // References
    private CharacterControllerRB characterController;
    private Animator animator;

    // Parameter Hashing - Performance Optimization
    private int velocityXHash;
    private int velocityYHash;
    private int jumpHash;

    //-----------------------------------------------
    private void Awake()
    {
        // Get CharacterControllerRB Component
        characterController = GetComponent<CharacterControllerRB>();

        // Get Animator Component
        animator = GetComponent<Animator>();

        // Parameter Hashing
        velocityXHash = Animator.StringToHash("velocityX");
        velocityYHash = Animator.StringToHash("velocityY");
        jumpHash = Animator.StringToHash("jump");
    }

    //-----------------------------------------------
    private void FixedUpdate()
    {
        handleMovementAnimation();
        handleJumpAnimation();
    }

    //-----------------------------------------------
    private void handleMovementAnimation()
    {
       
        if (characterController.isGrounded)
        {
            // apply movement velocity to blend tree 
            animator.SetFloat(velocityXHash, Mathf.Abs(characterController.velocityX));  
        }
    }

    //-----------------------------------------------
    private void handleJumpAnimation()
    {
        animator.SetFloat(velocityYHash, Mathf.Abs(characterController.velocityY));
    }


}
