using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireGame : MonoBehaviour
{
    // constant values
    private const int TIMER_TO_EXPAND = 3;                      // the fire duplicates itself over time (may need to move this elswhere
    private const int MAX_NUM_FIRES = 10;                       // don't let the fires get too out of control

    // serialized variables
    [SerializeField] GameManager gameManager;                   // a link to the game manager to set problem as solved
    [SerializeField] GameObject firePrefab;                     // a link to the fire object to create 

    // private variables
    private List<GameObject> activeFires;
    private AudioSource fireAudio;
    private float timer;
    private bool firesOut;

    // OnEnable is called before the first frame update
    void OnEnable()
    {
        // set up the basic fire system variables
        activeFires = new List<GameObject>();
        timer = TIMER_TO_EXPAND;
        firesOut = false;
        gameManager.onFire = true;

        // activate the first fire prefab
        CreateFire();

        // start the audio for the fire
        fireAudio = GetComponent<AudioSource>();
        fireAudio.Play();

    } // end OnEnable

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (!firesOut)
        {
            // count down the time until next fire is created
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            // otherwise it is time to create a new fire
            else
            {
                CreateFire();
                timer = TIMER_TO_EXPAND;
            }

            // if all the fires are out - then stop the game
            if (activeFires.Count <= 0)
            {
                gameManager.MarkProblemSolved();
                firesOut = true;
                gameManager.onFire = false;

                // make sure to disable the area, or it won't start the game the second time
                fireAudio.Stop();
                this.gameObject.SetActive(false);
            }
        }
        
    } // end Update

    /// <summary>
    /// Removes the fire from the list
    /// </summary>
    /// <param name="fireToExtinguish">the fire game object that needs to be removed</param>
    public void DecreaseFireCount(GameObject fireToExtinguish)
    {
        activeFires.Remove(fireToExtinguish);

    } // end DecreaseFireCount

    /// <summary>
    /// Creates an instance of the fire based off the last fire placed and adds one to the fire cout
    /// </summary>
    private void CreateFire()
    {
        if (activeFires.Count < MAX_NUM_FIRES)
        {
            Vector3 spawnPos = this.transform.position;

            // get the size of the area
            Vector3 boxColliderSize = GetComponent<BoxCollider>().size;

            // randomize the y based off the bounds of the collision area
            float yPos = spawnPos.y + Random.Range(0, boxColliderSize.y);

            // randomize the z based off the bounds of the collision area
            float zPosOffset = (boxColliderSize.z / 2);
            spawnPos = new Vector3(spawnPos.x, yPos, spawnPos.z + Random.Range(-zPosOffset, zPosOffset) );

            // Add the fire to the list after instantiating it
            GameObject fireCreated = Instantiate(firePrefab, spawnPos, Quaternion.identity);
            fireCreated.GetComponent<FireLazerReciever>().setFireGame(this);
            activeFires.Add(fireCreated);
        }

    } // end CreateFire

}
