using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.PlayFabHelper;

public class ExplanationManager : MonoBehaviour
{
    [SerializeField]
    Canvas mainCanvas;

    [SerializeField]
    List<Explanation> explanationList;

    [SerializeField]
    GameObject emphasisIndicatorPrefab;

    private static bool isActive; 
    private static bool isExplanationComplete = false;
    private static Queue<Explanation> executionOrder =  new Queue<Explanation>();
    private LoadingIndicator emphasisSettings;
    private static GameObject emphasisIndicatorObject;
    private static int maxExplanations = 2;
    private static int currentRuns = 0;
    public static bool IsExplanationCompete { get { return isExplanationComplete; } }
    public static GameObject EmphasisObject { get; set; }

    public static void StartExplanation()
    {
        if(!isActive && currentRuns < maxExplanations)
        {
            isActive = true;
            HelperFunctions.Log("Starting Explanations");
            var explanation = executionOrder.Dequeue();
            var emphasisLocation = explanation.transform.position;
            explanation.ShowExplanation();
            UpdateEmphasisLocation(emphasisLocation);
        }
        else
        {
            isExplanationComplete = true;
        }
    }

    public static void GoNext()
    {
        if(isActive && executionOrder.Count > 0)
        {
            if (emphasisIndicatorObject.activeSelf == true)
            {
                emphasisIndicatorObject.SetActive(false);
                HelperFunctions.Log("Showing Next Explanation");
                var explanation = executionOrder.Dequeue();
                var emphasisLocation = explanation.transform.position;
                explanation.ShowExplanation();
                UpdateEmphasisLocation(emphasisLocation);
            }
        }
        else
        {
            emphasisIndicatorObject.SetActive(false);
            isExplanationComplete = true;
            isActive = false;
            currentRuns++;
        }
    }

    private void Awake()
    {
        EmphasisObject = null;
        foreach(var explanation in explanationList) 
        { 
            executionOrder.Enqueue(explanation);
        }
        isActive = false;

        if(!PlayFabController.IsAuthenticated)
        {
            PlayFabController.IsAuthedEvent += CheckIfNewAccount;
        }
        else
        {
            CheckIfNewAccount();
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        emphasisIndicatorPrefab.GetComponent<Animator>().enabled = false;
        emphasisSettings = emphasisIndicatorPrefab.GetComponent<LoadingIndicator>();
        emphasisSettings.enabled = true;

        emphasisIndicatorObject = Instantiate(emphasisIndicatorPrefab, mainCanvas.transform);
        emphasisIndicatorObject.SetActive(false);

        //if account is not new change IsExplanationCompleted to true
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckIfNewAccount()
    {
        if(!Playfab.WasUserJustCreated)
        {
            isExplanationComplete = true;
        }
        else
        {
            isExplanationComplete = false;
        }
    }

    static void UpdateEmphasisLocation(Vector3 position)
    {
        emphasisIndicatorObject.transform.position = position;
        emphasisIndicatorObject.SetActive(true);
    }
}
