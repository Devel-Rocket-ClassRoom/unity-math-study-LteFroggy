 using System;
 using UnityEngine;
 using Unity.Mathematics;
 using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    public Transform mover;
    public float duration = 5f;
    private SplineContainer splineContainer;
    private float t;

    private void Awake() {
        splineContainer = GetComponent<SplineContainer>();
    }

    private void Update() {
        t += Time.deltaTime / duration;
        t = Mathf.Repeat(t, 1f);
        
        if (!splineContainer.Evaluate(
                splineContainer.Spline, 
                t, 
                out float3 newPosition,
                out float3 tangentVector,
                out float3 upVector)) 
        {
            return;
        }
        
        mover.position = newPosition;
        if (math.length(tangentVector) > 0.0001f) {
            mover.rotation = Quaternion.LookRotation(tangentVector, upVector);
        }
    }
}
