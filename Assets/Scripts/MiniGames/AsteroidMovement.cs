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

    // laser beam variables
    private GameObject laserTurret;

    private GameObject spawnedLazer;
    [SerializeField] private GameObject LazerPrefab;
    [SerializeField] private LineRenderer linePos;

    private float rotX;
    private float rotY;
    private float rotZ;
    

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

        // laser from position
        laserTurret = GameObject.FindGameObjectWithTag("LaserTurret");

        spawnedLazer = Instantiate(LazerPrefab, laserTurret.transform) as GameObject;
        linePos = spawnedLazer.gameObject.GetComponent<LineRenderer>();

        disableLazer();

        rotX = Random.Range(-.05f, .05f);
        rotY = Random.Range(-.05f, .05f);
        rotZ = Random.Range(-.05f, .05f);

        
       gameObject.transform.localScale = new Vector3(Random.Range(.5f, 1.5f), Random.Range(.5f, 1.5f), Random.Range(.1f, 1.5f));

    } // end Start

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {

        gameObject.transform.Rotate(rotX, rotY, rotZ);

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
        enableLazer();
        spawnedLazer.transform.position = laserTurret.transform.position;
        linePos.SetPosition(0, gameObject.transform.position);
        linePos.SetPosition(1, laserTurret.transform.position);
       


        if (!beingDestroyed)
        {
            // set laser position here
            // asteroid position gameObject.transform.position
            
            shootAsteroidsScript.PlayLaserFire();
            DestroyAsteroid();
        }

    } // end OnMouseDown


    //------------------------------
    private void OnMouseUp()
    {
        //when mouse is realeased disable the laser
        disableLazer();
    }

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


    //Laser Turret functions
    //--------------------------------
    void enableLazer()
    {
        spawnedLazer.SetActive(true);
    }

    //--------------------------------
    void disableLazer()
    {
        spawnedLazer.SetActive(false);   
    }

    //--------------------------------
    void updateLazer()
    {
       // spawnedLazer.transform.position = laserTurret.transform.position;

    }
}
