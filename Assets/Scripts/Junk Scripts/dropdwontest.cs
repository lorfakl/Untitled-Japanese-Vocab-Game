using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class dropdwontest : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown dropdown;
    // Start is called before the first frame update
    private void Awake()
    {
        dropdown.onValueChanged.AddListener(Test);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Test(int v)
    {
        Debug.Log("I was called with no input");
        Debug.Log(v);
    }
}
