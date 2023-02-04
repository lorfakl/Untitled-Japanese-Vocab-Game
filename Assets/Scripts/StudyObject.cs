using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Events;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TMP_Text))]
public class StudyObject : MonoBehaviour
{

#region Public Variables
    public GameEvent studyObjectSelectedEvent;

    public JapaneseWord Word
    {
        get { return word; }
    }

    public TMP_Text Text
    {
        get { return text; }
    }

    public Vector3 PositionSelected
    {
        get;
        private set;
    }

    public float TimeInFlight
    {
        get;
        private set;
    }
    public List<Vector3> Positions
    {
        get { return positions; }
    }

    #endregion

    #region Private Variables
    private JapaneseWord word;
    private Rigidbody rb;
    private TMP_Text text;
    private List<Vector3> positions = new List<Vector3>();
    private bool selectionEnabled;
    private bool shouldCountFlightTime;

    [SerializeField]
    ParticleSystem studyObjectParticleSystem;

    string _textToDisplay;
    #endregion

    #region Events
    #endregion

    #region Unity Events

    #endregion

    #region Public Methods
    public void DisableSelection()
    {
        selectionEnabled = false;
        shouldCountFlightTime = false;
    }

    public void MakeInvisible()
    {
        text.enabled = false;
        studyObjectParticleSystem.Play();
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        var entry = WordBankManager.WordBank.Dequeue();
        word = entry.Word;
        _textToDisplay = entry.DisplayText;
        rb = GetComponent<Rigidbody>();
        text = GetComponent<TMP_Text>();
        selectionEnabled = true;
        shouldCountFlightTime=true;
        
    }

    void Start()
    {
        text.text = _textToDisplay;
        TimeInFlight = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if(shouldCountFlightTime)
        {
            positions.Add(gameObject.transform.position);
            TimeInFlight += Time.fixedDeltaTime;
        }
    }

    private void OnMouseDown()
    {
        if(selectionEnabled)
        {
            //HelperFunctions.Log("Was clicked");

            if (studyObjectSelectedEvent != null)
            {
                studyObjectSelectedEvent.Raise(this);
                studyObjectSelectedEvent.Raise();
            }
            else
            {
                throw new System.Exception("Study Object(class) is missing a reference to the Study Object Selected Event Scriptable Object");
            }
        }
        
    }

    private void OnBecameInvisible()
    {
        //HelperFunctions.Log("Time in flight: " + TimeInFlight);
        //HelperFunctions.Log("Positions counted: " + Positions.Count);

        Destroy(gameObject);
    }
    #endregion

    #region Private Methods



    #endregion
}