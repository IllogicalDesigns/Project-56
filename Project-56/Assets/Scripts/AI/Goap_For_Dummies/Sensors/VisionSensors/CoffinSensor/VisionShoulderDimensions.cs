using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/VisionShoulderDimensions", order = 2)]
public class VisionShoulderDimensions : ScriptableObject {
    public float closeWidth = 2f;
    public float closeDist = 0f;

    public float farWidth = 2f;
    public float farDist = -1;

    bool isDebug;

    public bool CheckShoulderShape(Vector3 candidatePosition, bool isFlipped, UnityEngine.Transform transform, bool _isDebug) {
        var _closeWidth = isFlipped ? closeWidth : -closeWidth;  //bool shoulder = CheckShoulderShape(shoulderZone.closeWidth, shoulderZone.closeDist, shoulderZone.farWidth, shoulderZone.farDist, candidatePosition);
        var _farWidth = isFlipped ? farWidth : -farWidth;  //if (!shoulder) shoulder = CheckShoulderShape(-shoulderZone.closeWidth, shoulderZone.closeDist, -shoulderZone.farWidth, shoulderZone.farDist, candidatePosition);

        isDebug = _isDebug;
        var closeVec1 = (transform.position + transform.forward * closeDist) + transform.right * -_closeWidth;
        var closeVec2 = (transform.position + transform.forward * closeDist) + transform.right * (-_closeWidth * 0.5f);
        var farVec1 = (transform.position + transform.forward * farDist) + transform.right * -_farWidth;
        var farVec2 = (transform.position + transform.forward * farDist) + transform.right * (-_farWidth * 0.5f);

        bool pointInSpace = CheckTrapezoid(closeVec1, closeVec2, farVec1, farVec2, candidatePosition);
        if (!pointInSpace) pointInSpace = CheckTriangle(closeVec2, farVec2, transform.position, candidatePosition);

        return pointInSpace;
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
            Color clr = !isInTriangle ? Color.red : Color.green;
            UnityEngine.Debug.DrawLine(a, b, clr);
            UnityEngine.Debug.DrawLine(b, c, clr);
            UnityEngine.Debug.DrawLine(c, a, clr);
        }

        return isInTriangle;
    }
}

