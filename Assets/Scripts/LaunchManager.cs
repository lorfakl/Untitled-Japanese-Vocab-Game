using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using UnityEngine.UI;
using TMPro;

public class LaunchManager : MonoBehaviour
{

    #region Public Variables
    public int secsToNextLaunch = 20;
    #endregion

    #region Private Variables
    [SerializeField]
    GameObject launchObject;

    [SerializeField]
    GameObject baselineObject;

    [SerializeField]
    float shotAngle = 115;

    [SerializeField]
    float shotForce = 40;

    [SerializeField]
    bool useRandomValues = false;

    Vector3 baselinePosition;
    WordBankManager wordBank;
    
    float maxValueY = -10;

    /// <summary>
    /// Data container for storing the start and end points of areas
    /// on the screen where StudyObjects will be spawned  betweeen so that they
    /// do not overlap. Along with the size of this area
    /// </summary>
    struct chunkRangePoints
    {
        public float start;
        public float end;
        public int size;
        public float center;
    }

    List<chunkRangePoints> currentChunks = new List<chunkRangePoints>();
    #endregion

    #region Public Methods
    /// <summary>
    /// Next Word Pressed Game Event Scriptable Object
    /// Handler
    /// </summary>
    public void NextWordButtonClickedEvent()
    {
        BeginLaunchSequence();
        HelperFunctions.Log("Best unity tip");
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        baselinePosition = baselineObject.transform.position;
        wordBank = GetComponent<WordBankManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            
        }
    }
    #endregion

    #region Private Methods
    void BeginLaunchSequence()
    {
        if(JSONWordLibrary.WordsToStudy.Count > 0)
        {
            List<JapaneseWord> words = wordBank.SetUpWordBank();
            currentChunks.Clear();
            GenerateNonOverlappedValuesForY(words.Count);
            for (int i = 0; i < words.Count; i++)
            {
                PrepNextLaunch(currentChunks[i].center);
            }
        }
        
    }

    void PrepNextLaunch(float yPos)
    {
        Vector3 newPosition = baselinePosition;
        newPosition.x = Random.Range(-20, 25);
        newPosition.y = yPos;
        GameObject newLaunchObject = GameObject.Instantiate(launchObject, newPosition, Quaternion.identity);
        Launch(newLaunchObject.GetComponent<Rigidbody>());
    }

    void GenerateNonOverlappedValuesForY(int chunkCount)
    {
        float totalSpace = maxValueY - baselinePosition.y;
        int chuckSize = (int)totalSpace / chunkCount;

        for(int i = (int)maxValueY; i > (int)baselinePosition.y; i -= chuckSize)
        {
            var chunk = new chunkRangePoints();
            chunk.start = i;
            chunk.end = i - chuckSize;
            chunk.size = chuckSize;
            chunk.center = chunk.start - (chunk.size / 2);
            currentChunks.Add(chunk);
            /*HelperFunctions.Log("Chunk Number: " + currentChunks.Count
                + "\n" + "Chunk Start: " + chunk.start + "\n" +
                "Chunk Size: " + chunk.size + "\n" +
                "Chunk End: " + chunk.end);*/
        }
    }

    void Launch(Rigidbody rb)
    {
        rb.isKinematic = false;
        if(useRandomValues)
        {
            shotAngle = Random.Range(92, 150);
            shotForce = Random.Range(50, 70);
        }
        
        Vector3 initialVelocity = HelperFunctions.RotateVector(transform.forward, shotAngle).normalized * shotForce;
        rb.AddForce(initialVelocity, ForceMode.Impulse);
    }
    #endregion
}