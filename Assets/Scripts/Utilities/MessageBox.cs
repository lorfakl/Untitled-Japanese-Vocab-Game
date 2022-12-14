using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public enum MessageBoxType
    {
        Message,
        Error,
        Loading,
        Confirmation,
        Tooltip
    }

    public static class MessageBox
    {
        public static void Show(string msg, MessageBoxType type)
        {
            HelperFunctions.Warning(msg);
            HelperFunctions.Error("Message Box needs to be implemented");
            /*GameObject mainCanvas = GameObject.FindGameObjectWithTag("canvas");
            for(int i = 0; i < mainCanvas.transform.childCount; i++)
            {
                if(mainCanvas.transform.GetChild(i).gameObject.activeSelf)
                {
                    mainCanvas.transform.GetChild(i).gameObject.SetActive(false);   
                }
            }*/
        }
    }
}

