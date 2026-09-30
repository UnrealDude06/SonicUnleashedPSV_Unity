using UnityEngine;
using PathCreation; // Ensure you have the PathCreator package

[ExecuteInEditMode]
public class RailSplineGenerater : MonoBehaviour {

    [Header("Spline Options")]
    public bool closedLoop = false;
    public Transform childIf; // Optional: use if rail points are inside a container
    public float controlSpacing = 0.2f; // How tightly control points are auto-calculated
    public float normalsAngle = 90f; // Optional tilt for rails

    void Start()
    {
        PathCreator pathCreator = GetComponent<PathCreator>();
        Transform container = childIf != null ? childIf : transform;

        int childCount = container.childCount;
        if (childCount < 2) return; // Need at least 2 points to form a path

        Vector3[] localPoints = new Vector3[childCount];
        for (int i = 0; i < childCount; i++)
        {
            localPoints[i] = pathCreator.transform.InverseTransformPoint(container.GetChild(i).position);
        }

        // Build the bezier path
        BezierPath bezierPath = new BezierPath(localPoints, closedLoop, PathSpace.xyz)
        {
            GlobalNormalsAngle = normalsAngle,
            AutoControlLength = controlSpacing
        };

        // Assign to path creator
        pathCreator.bezierPath = bezierPath;

         this.enabled = false; // disable script after baking
    }
}