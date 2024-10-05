using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/VisionCoffinDimensions", order = 1)]
public class VisionCoffinDimensions : ScriptableObject {
    public float farWidth = 2.5f;
    public float farDist = 15f;

    public float midWidth = 4f;
    public float midDist = 5f;

    public float closeWidth = 1f;
    public float closeDist = 0f;

    public Color debugColor = Color.red;

    bool isDebug;

    public bool CheckCoffin(Vector3 candidatePosition, UnityEngine.Transform transform, bool _isDebug) {
        isDebug = _isDebug;
        Vector3 farLeft = (transform.position + transform.forward * farDist) + transform.right * -farWidth;
        Vector3 farRight = transform.position + transform.forward * farDist + transform.right * farWidth;
        Vector3 midLeft = (transform.position + transform.forward * midDist) + transform.right * -midWidth;
        Vector3 midRight = transform.position + transform.forward * midDist + transform.right * midWidth;
        Vector3 closeLeft = (transform.position + transform.forward * closeDist) + transform.right * -closeWidth;
        Vector3 closeRight = transform.position + transform.forward * closeDist + transform.right * closeWidth;
        bool isInCoffin = false;
        if (CheckTrapezoid(closeLeft, closeRight, midLeft, midRight, candidatePosition))
            isInCoffin = true;

        if (CheckTrapezoid(midLeft, midRight, farLeft, farRight, candidatePosition))
            isInCoffin = true;

        return isInCoffin;
    }

    private bool CheckTrapezoid(Vector3 closeLeft, Vector3 closeRight, Vector3 midLeft, Vector3 midRight, Vector3 candidatePosition) {
        bool isInTrapezoid = false;
        if (CheckTriangle(closeLeft, closeRight, midLeft, candidatePosition))
            isInTrapezoid = true;

        if (CheckTriangle(midLeft, midRight, closeRight, candidatePosition))
            isInTrapezoid = true;

        return isInTrapezoid;
    }



    private bool CheckTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p) {
        // Compute vectors
        var v0 = c - a;
        var v1 = b - a;
        var v2 = p - a;

        // Compute dot products
        var dot00 = Vector3.Dot(v0, v0);
        var dot01 = Vector3.Dot(v0, v1);
        var dot02 = Vector3.Dot(v0, v2);
        var dot11 = Vector3.Dot(v1, v1);
        var dot12 = Vector3.Dot(v1, v2);

        // Compute barycentric coordinates
        var invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        bool isInTriangle = (u >= 0) && (v >= 0) && (u + v < 1);

        if (isDebug) {
            Color clr = !isInTriangle ? debugColor : Color.green;
            UnityEngine.Debug.DrawLine(a, b, clr);
            UnityEngine.Debug.DrawLine(b, c, clr);
            UnityEngine.Debug.DrawLine(c, a, clr);
        }

        return isInTriangle;
    }

}
