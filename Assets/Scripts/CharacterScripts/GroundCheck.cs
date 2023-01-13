using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask platform;
    [SerializeField] float threshold = .2f;

    //------------------------------
    public bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, threshold, platform);


    }   // Returns true if player is touching the ground (using LayerMask)
}
