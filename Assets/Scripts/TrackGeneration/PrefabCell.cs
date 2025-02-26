using UnityEngine;
using UnityEngine.Splines;

public class PrefabCell : MonoBehaviour
{    
    [Header("Track/Spline Information")]
    [Space(10)]
    [Header("Starting Info")]
    public Vector3 trackStartPos;
    public Quaternion trackStartRotation;
    [Space(10)]
    [Header("Ending Info")]
    public Vector3 trackEndPos;
    public Quaternion trackEndRotation;

    public SplineContainer spline;

    [Header("Overall Cell Information")]
    public Bounds bounds;
    public Vector3 position;
    public Quaternion rotation;
    void Start()
    {
        init();
        setBounds();
        setTrackPos();
        
    }

    public void init()
    {
        //position = new Vector3(0, 0, 0);
        //rotation = Quaternion.identity;
    }
    private void setBounds()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            bounds = meshRenderer.bounds;
        }
        else
        {
            Debug.LogWarning("No MeshRenderer found on " + gameObject.name);
        }
    }
    public void setTrackPos()
    {
        if (spline != null)
        {
            //starting info
            trackStartPos = spline.EvaluatePosition(0f);    //time 0 = start
            Vector3 startTangent = spline.EvaluateTangent(0f); 
            trackStartRotation = Quaternion.LookRotation(startTangent);

            //ending info
            trackEndPos = spline.EvaluatePosition(1f);      //time 1 = end
            Vector3 endTangent = spline.EvaluateTangent(1f);
            trackEndRotation = Quaternion.LookRotation(endTangent);
        }
        else
        {
            Debug.LogWarning("No spline assigned to " + gameObject.name);
        }
    }
}
