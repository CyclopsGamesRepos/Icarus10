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

    private int isGroundedHash;
    private int isJumpingHash;
    private int isFallingHash;

    //-----------------------------------------------
    private void Awake()
    {
        // Get CharacterControllerRB Component
        characterController = GetComponent<CharacterControllerRB>();

        // Get Animator Component
        animator = GetComponent<Animator>();

        // Parameter Hashing movement
        velocityXHash = Animator.StringToHash("velocityX");
        velocityYHash = Animator.StringToHash("velocityY");

        // Parameter Hashing Action
        isGroundedHash = Animator.StringToHash("isGrounded");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
    }

    //-----------------------------------------------
    private void FixedUpdate()
    {
        animator.SetBool(isGroundedHash, characterController.isGrounded);
 
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
        // Set jumping animation
        animator.SetBool(isJumpingHash, characterController.isJumping);

        //set falling animation
        animator.SetBool(isFallingHash, characterController.isFalling);

    }


}
