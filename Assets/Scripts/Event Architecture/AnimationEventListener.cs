using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


namespace Utilities.Events
{
    public enum AnimationParameters
    {
        Float = 0,
        Int = 1,
        Bool = 2, 
        Trigger = 3
    }

    public class AnimationEventListener : GameEventListener
    {
        [Tooltip("Enter the name of the Trigger. It must MATCH EXACTY what was typed in the Parameter section of the Animator")]
        [SerializeField]
        string triggerName;

        [SerializeField]
        AnimationParameters parameterType;

        Animator animationController;

        //[Tooltip("Event to register with.")]
        //public GameEvent Event;

        //[Tooltip("Response to invoke when Event is raised.")]
        //public UnityEvent Response;

        //[Tooltip("Response to invoke with required data when Event is raised.")]
        //public UnityEvent<object> ResponseWithData;


        public override void OnEventRaised()
        {
            animationController.SetTrigger(triggerName);
            HelperFunctions.Log("Set trigger called");
            Response?.Invoke();
        }

        public override void OnEventRaiseWithData(object eventData)
        {
            animationController.SetTrigger(triggerName);
            ResponseWithData?.Invoke(eventData);
        }

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }
        // Start is called before the first frame update
        void Start()
        {
            animationController = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
