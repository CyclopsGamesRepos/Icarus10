using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    // constant values
    private const float MIN_IMPULSE_FORCE = 100f;               // the minimum force to apply to the asteroid
    private const float MAX__IMPULSE_FORCE = 250f;              // the maximum force to apply to the asteroid
    private const float HIT_SHIP_Z = -13.1f;                    // the time this asteroid will be around
    private const int TIME_TO_LOSE = 5;                         // the time to lose if the asteroid is missed

    // serialized variables
    [SerializeField] AudioClip hitShip;                         // the audio clip to switch to if the asteroid hits the ship

    // public variables used by other scripts
    public ShootAsteroids shootAsteroidsScript;                 // a link back to the shoot asteroid script - set up from that script

    // private variables used by this script
    private GameManager gameManager;                            // a link to the game manager to update timer if the asteroid is missed
    private AudioSource asteroidAudioOutput;
    private bool beingDestroyed = false;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Add an impulse force for this astroid aftr it points towards the ship
        transform.LookAt(shootAsteroidsScript.gameObject.transform);
        GetComponent<Rigidbody>().AddForce(Vector3.forward * Random.Range(MIN_IMPULSE_FORCE, MAX__IMPULSE_FORCE), ForceMode.Impulse);

        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        asteroidAudioOutput = GetComponent<AudioSource>();

    } // end Start

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // if the timer has run out, assume the asteroid hit the ship and take time away
        if ( (transform.position.z > HIT_SHIP_Z) && !beingDestroyed)
        {
            // remove time and change the audio clip to play
            gameManager.AdjustGameTimer(-TIME_TO_LOSE);
            asteroidAudioOutput.clip = hitShip;

            // Show particle system
            DestroyAsteroid();
        }
        
    } // end Update

    /// <summary>
    /// if the user clicks the asteroid, blow it up
    /// </summary>
    private void OnMouseDown()
    {
        if (!beingDestroyed)
        {
            shootAsteroidsScript.PlayLaserFire();
            DestroyAsteroid();
        }

    } // end OnMouseDown

    /// <summary>
    /// Destroys the asteroid with a particle effect and audio
    /// </summary>
    private void DestroyAsteroid()
    {
        // Show particle system and play audio clip
        gameObject.GetComponent<ParticleSystem>().Play();
        asteroidAudioOutput.Play();

        // destroy the asteroid
        beingDestroyed = true;
        Destroy(gameObject, 1.0f);
        shootAsteroidsScript.RemoveAsteroid();

    } // end DestroyAsteroid
}
