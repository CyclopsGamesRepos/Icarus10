using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootAsteroids : MonoBehaviour
{
    // constant values
    private const float CAMERA_SPEED = 1.0f;                    // the speed to move the camera in
    private const float SPAWN_OFSET = 5;                        // the offset for the asteroids to spawn on screen
    private const float MIN_TIME_BETWEEN_SPAWN = 0.25f;         // the minimum time between spawns - for timer
    private const float MAX_TIME_BETWEEN_SPAWN = 1.5f;          // the maximum time between spawns - for timer
    private const int MIN_NUM_ASTERIODS = 5;                    // The minimum number of asteroids to spawn for each game
    private const int MAX_NUM_ASTERIODS = 16;                   // The maximum number of asteroids to spawn for each game

    // serialized variables
    [SerializeField] GameManager gameManager;                   // a link to the game manager to set problem as solved
    [SerializeField] GameObject cameraToMove;                   // a link to the camera so we can move it into position and back
    [SerializeField] Transform cameraPosition;                  // where to put the camera when the game starts
    [SerializeField] GameObject asteroidPrefab;                 // a link to the fire object to create 
    [SerializeField] GameObject spawnPoint;                     // the area around where the asteroids will spawn

    // private variables
    private Vector3 originalCameraPosition;                     // remember where the camera was so we can put it back
    private Quaternion originalCameraRotation;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;
    private float timer;                                        // a timer for spawning asteroids
    private bool asteroidsDone;                                 // to make sure that we don't spawn asteroids when done
    private bool cameraMoving;                                  // set the camera slide in
    private int numAsteroidsPerGame;                            // the number of asteroids to spawn for this game (counter)
    private int numAsteroidsRemaining;

    // OnEnable is called before the first frame update
    void OnEnable()
    {
        // set up the basic fire system variables
        timer = Random.Range(MIN_TIME_BETWEEN_SPAWN, MAX_TIME_BETWEEN_SPAWN);
        numAsteroidsPerGame = Random.Range(MIN_NUM_ASTERIODS, MAX_NUM_ASTERIODS);
        numAsteroidsRemaining = numAsteroidsPerGame;
        asteroidsDone = false;

        // set up for smooth camera movement
        originalCameraPosition = cameraToMove.transform.position;
        originalCameraRotation = cameraToMove.transform.rotation;

        targetCameraPosition = cameraPosition.transform.position;
        targetCameraRotation = cameraPosition.transform.rotation;
        cameraMoving = true;

        // for now just move the camera (pop it there - will need to fix)
        cameraToMove.gameObject.transform.SetPositionAndRotation(targetCameraPosition, targetCameraRotation);

        // activate the first fire prefab
        Invoke("SpawnAsteroid", 2);

    } // end OnEnable

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // want to move the camera smoothly - need to fix
        //MoveCameraToPosition();

        if (!asteroidsDone)
        {
            // only spawn asteroids if there are still some to spawn
            if (numAsteroidsPerGame > 0)
            {
                // count down the time until next fire is created
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                // otherwise it is time to create a new fire
                else
                {
                    SpawnAsteroid();
                    timer = Random.Range(MIN_TIME_BETWEEN_SPAWN, MAX_TIME_BETWEEN_SPAWN); ;
                }
            }
            else
            {
                // if all the fires are out - then stop the game
                if (numAsteroidsRemaining <= 0)
                {
                    // set up move back camera
                    targetCameraPosition = originalCameraPosition;
                    targetCameraRotation = originalCameraRotation;
                    asteroidsDone = true;
                    cameraMoving = true;

                    // pop the camera back for now
                    cameraToMove.gameObject.transform.SetPositionAndRotation(targetCameraPosition, targetCameraRotation);

                    // make sure to disable the area, or it won't start the game the second time
                    // TODO: may need to move this into the camera movement method
                    gameManager.MarkProblemSolved();
                    this.gameObject.SetActive(false);
                }
            }
        }

    } // end Update

    /// <summary>
    /// Used by asteroids to tell this game it has been destroyed
    /// </summary>
    public void RemoveAsteroid()
    {
        numAsteroidsRemaining--;

    } // end RemoveAsteroid

    /// <summary>
    /// Moves the camera from the target point to the position - may need to be IEnumerated
    /// </summary>
    private void MoveCameraToPosition()
    {
        // attempting to move the camera to the correct position over time - rotation not here will need to figure it out
        if (cameraMoving)
        {
            cameraToMove.gameObject.transform.position = Vector3.Lerp(transform.position, targetCameraPosition, CAMERA_SPEED * Time.deltaTime);
            cameraToMove.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, targetCameraRotation, CAMERA_SPEED * Time.deltaTime);

            if (cameraToMove.gameObject.transform.position == targetCameraPosition)
            {
                cameraMoving = false;

                if (asteroidsDone)
                {
                    gameManager.MarkProblemSolved();
                    this.gameObject.SetActive(false);
                }
            }
        }

    }

    /// <summary>
    /// Creates an instance of the fire based off the last fire placed and adds one to the fire cout
    /// </summary>
    private void SpawnAsteroid()
    {
        // grab the spawn position for all asteroids
        Vector3 spawnPos = spawnPoint.transform.position;

        // randomize each coordinate
        float xPos = spawnPos.x + Random.Range(-SPAWN_OFSET, SPAWN_OFSET);
        float yPos = spawnPos.y + Random.Range(-SPAWN_OFSET, SPAWN_OFSET);
        float zPos = spawnPos.z + Random.Range(-SPAWN_OFSET, SPAWN_OFSET);

        spawnPos = new Vector3(xPos, yPos, zPos);

        // Create the asteroid and have it look at the player
        GameObject asteroidCreated = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
        asteroidCreated.transform.LookAt(this.transform);
        asteroidCreated.GetComponent<AsteroidMovement>().shootAsteroidsScript = this;

        // remove the asteroids from the total count
        numAsteroidsPerGame--;

    } // end CreateFire

}