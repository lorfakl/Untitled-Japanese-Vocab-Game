using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(LineRenderer))]
public class LineRenderController : MonoBehaviour
{
    // Start is called before the first frame update
    Transform lineEndPoint;
    LineRenderer lr;

    [SerializeField]
    int numOfPoints;

    [SerializeField]
    float timeInterval;


    void Start()
    {
        //lineEndPoint = gameObject.transform.GetChild(0);
        lr = GetComponent<LineRenderer>();
    }

    public void CalculateProjectilePath(ILineRendererObject objectInstance)
    {
        List<Vector3> points = HelperFunctions.CalculateProjectilePath(numOfPoints, timeInterval, objectInstance.GetInitialVelocity(),
            objectInstance.GetInitialPosition());
        lr.positionCount = numOfPoints;
        lr.SetPositions(points.ToArray());
        lr.enabled = true;
    }

    public Vector3 CalculateVelocity(float angle, Vector3 initialPos, Vector3 finalPos)
    {
        return HelperFunctions.CalculateProjectileVelocity2D(numOfPoints, timeInterval, angle, initialPos, finalPos);
    }

    public void SetLineRenVisibility(bool isEnabled)
    {
        lr.enabled = isEnabled;
    }

}