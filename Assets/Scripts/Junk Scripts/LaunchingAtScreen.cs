using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderController))]
public class LaunchingAtScreen : MonoBehaviour
{



#region Public Variables
    GameObject wordPrefab;
#endregion

#region Private Variables

    LineRenderController lineRenderContrlr;
    Rigidbody rb;

    [SerializeField]
    float shotForce;
    [SerializeField]
    float shotAngle;
    


#endregion

#region Events
#endregion

#region Unity Events
#endregion

#region Public Methods

    

#endregion

#region Unity Methods

    private void Awake()
    {
        lineRenderContrlr = GetComponent<LineRenderController>();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        //Vector3 gridOrigin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        //testGrid = new Grid<string>(4, 8, 5.5f, new Vector3(-11f, -22f, 31));

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            LaunchCube();
            

        }
    }

#endregion

#region Private Methods
    void LaunchCube()
    {
        Vector3 initialVelocity = HelperFunctions.RotateVector(transform.forward, shotAngle).normalized * shotForce;
        rb.AddForce(initialVelocity, ForceMode.Impulse);
    }

    void InstantiateWordObject()
    {

    }

    IEnumerator CountDownToNextWordLaunch(int seconds)
    {
        yield return HelperFunctions.Timer(seconds);
        LaunchCube();
    }
#endregion
}