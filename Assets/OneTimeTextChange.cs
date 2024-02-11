using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OneTimeTextChange : MonoBehaviour
{
    [SerializeField]
    Button target;

    [SerializeField]
    TMP_Text text;

    [SerializeField]
    string replaceText;

    static int timesChanged = 0;
    // Start is called before the first frame update
    void Start()
    {
        target.onClick.AddListener(() => 
        { 
            if (timesChanged < 1) 
            {
                timesChanged++;
                text.text = replaceText;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
