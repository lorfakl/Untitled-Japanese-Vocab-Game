using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utilities.UserInterfaceAddOns
{
    public class LongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        [Range(0.25f, 1f)]
        float longPressThreshold;

        public UnityEvent OnLongPressRegistered
        {
            get { return onLongPressRegistered; }
        }

        public UnityEvent OnShortPressRegistered
        {
            get { return onShortPressRegistered; }
        }

        private float holdTime = 0; 
        private bool isPressed = false;
        private bool isLongPressEventFired = false;
        private UnityEvent onLongPressRegistered = new UnityEvent();
        private UnityEvent onShortPressRegistered = new UnityEvent();

        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(isPressed)
            {
                holdTime += Time.deltaTime;
                if(holdTime > longPressThreshold && !isLongPressEventFired)
                {
                    OnLongPressRegistered?.Invoke();
                    HelperFunctions.Log($"registered long press after {holdTime} seconds");
                    isLongPressEventFired = true;
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            //HelperFunctions.Log($"Pointer delta: {eventData.delta}");
            if(isPressed)
            {
                if(!isLongPressEventFired) 
                { 
                    //is a short press
                    OnShortPressRegistered?.Invoke();
                }
                else
                {
                    isLongPressEventFired = false;
                    holdTime = 0;
                }
                isPressed = false;
            }
        }
    }
}

