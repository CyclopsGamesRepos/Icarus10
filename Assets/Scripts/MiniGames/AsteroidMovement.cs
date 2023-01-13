using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    // constant values
    private const float MIN_IMPULSE_FORCE = 50f;                // the minimum force to apply to the asteroid
    private const float MAX__IMPULSE_FORCE = 150f;              // the maximum force to apply to the asteroid
    private const int LIFE_TIME = 5;                            // the time this asteroid will be around
    private const int TIME_TO_LOSE = 5;                         // the time to lose if the asteroid is missed

    // serialized variables
    [SerializeField] GameManager gameManager;                   // a link to the game manager to update timer if the asteroid is missed

    // public variables used by other scripts
    public ShootAsteroids shootAsteroidsScript;

    // private variables used by this script
    private float lifeTimer;
    private bool beingDestroyed = false;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Add an impulse force for this astroid
        GetComponent<Rigidbody>().AddForce(Vector3.forward * Random.Range(MIN_IMPULSE_FORCE, MAX__IMPULSE_FORCE), ForceMode.Impulse);

        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();

        // set up a timer
        lifeTimer = LIFE_TIME;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // count down the time
        lifeTimer -= Time.deltaTime;

        // if the timer has run out, assume the asteroid hit the ship and take time away
        if ( (lifeTimer < 0) && !beingDestroyed)
        {
            gameManager.AdjustGameTimer(-TIME_TO_LOSE);

            // Show particle system
            gameObject.GetComponent<ParticleSystem>().Play();
            Destroy(gameObject, 1.0f);
            beingDestroyed = true;
            shootAsteroidsScript.RemoveAsteroid();
        }
        
    }

    /// <summary>
    /// if the user clicks the asteroid, blow it up
    /// </summary>
    private void OnMouseDown()
    {
        if (!beingDestroyed)
        {
            // Show particle system
            gameObject.GetComponent<ParticleSystem>().Play();
            beingDestroyed = true;
            Destroy(gameObject, 1.0f);
            shootAsteroidsScript.RemoveAsteroid();
        }
    }
}
