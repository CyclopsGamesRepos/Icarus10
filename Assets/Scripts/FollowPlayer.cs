using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Constant values

    // Serialized Fields for use in Unity
    [Header("Player information")]
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject player;
    [SerializeField] Vector3 cameraOffsetPosition;

    // Public variables

    // Private variables


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
        // Only follow the player if we are not currently solving a problem
        if (!gameManager.IsProblemActive() )
        {
            // follow the player at the given offset every frame
            transform.position = player.transform.position + cameraOffsetPosition;
        }

    } // end Update

}
