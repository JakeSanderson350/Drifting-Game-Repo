using UnityEngine;
using UnityEngine.Splines;

public class SuperStupidTesting : MonoBehaviour
{
    [Header("current spline")]
    public Vector3 firstKnotPosC;
    public Quaternion firstKnotRotC;
    public Vector3 firstKnotTanINC;
    public Vector3 firstKnotTanOUTC;
    public Vector3 firstKnotTanC;
    public Vector3 lastKnotPosC;
    public Quaternion lastKnotRotC;
    public Vector3 lastKnotTanINC;
    public Vector3 lastKnotTanOUTC;
    public Vector3 lastKnotTanC;
    public SplineContainer currentSpline;
    [Header("previous spline")]
    public Vector3 firstKnotPosP;
    public Quaternion firstKnotRotP;
    public Vector3 firstKnotTanINP;
    public Vector3 firstKnotTanOUTP;
    public Vector3 firstKnotTanP;
    public Vector3 lastKnotPosP;
    public Quaternion lastKnotRotP;
    public Vector3 lastKnotTanINP;
    public Vector3 lastKnotTanOUTP;
    public Vector3 lastKnotTanP;
    public SplineContainer previousSpline;

    private void Start()
    {
        //current spline first knot
        var currSpline = currentSpline.Spline;
        BezierKnot currFirstKnot = currSpline[0];
        firstKnotPosC = currFirstKnot.Position;
        firstKnotRotC = currFirstKnot.Rotation;
        firstKnotTanINC = currFirstKnot.TangentIn;
        firstKnotTanOUTC = currFirstKnot.TangentOut;
        firstKnotTanC = currentSpline.Spline.EvaluateTangent(0f);
        //last knot
        BezierKnot currLastKnot = currSpline[currSpline.Count - 1];
        lastKnotPosC = currLastKnot.Position;
        lastKnotRotC = currLastKnot.Rotation;
        lastKnotTanINC = currLastKnot.TangentIn;
        lastKnotTanOUTC = currLastKnot.TangentOut;
        lastKnotTanC = currentSpline.Spline.EvaluateTangent(1f);

        //previous spline first knot
        var prevSpline = previousSpline.Spline;
        BezierKnot prevFristKnot = prevSpline[0];
        firstKnotPosP = prevFristKnot.Position;
        firstKnotRotP = prevFristKnot.Rotation;
        firstKnotTanINP = prevFristKnot.TangentIn;
        firstKnotTanOUTP = prevFristKnot.TangentOut;
        firstKnotTanP = previousSpline.Spline.EvaluateTangent(0f);
        //last knot
        BezierKnot prevLastKnot = prevSpline[prevSpline.Count - 1];
        lastKnotPosP = prevLastKnot.Position;
        lastKnotRotP = prevLastKnot.Rotation;
        lastKnotTanINP = prevLastKnot.TangentIn;
        lastKnotTanOUTP = prevLastKnot.TangentOut;
        lastKnotTanP = previousSpline.Spline.EvaluateTangent(1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            var container = previousSpline.GetComponent<SplineContainer>();
            var spline = container.Spline;
            BezierKnot lastKnot = spline[spline.Count - 1];

            //set last knot in previous spline equal to first knot in current spline
            lastKnot.Position = firstKnotPosC;
            lastKnot.Rotation = firstKnotRotC;
            lastKnot.TangentIn = firstKnotTanINC;
            lastKnot.TangentOut = firstKnotTanOUTC;

            spline.SetKnot(spline.Count - 1, lastKnot);
        }
    }
}