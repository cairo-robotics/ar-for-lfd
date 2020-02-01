using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverUnderConstraintPrefab : VisualConstraint
{
    public Vector3 constraintPosition;
    public float thresholdDistance;
    void Start()
    {
    }

    void Update()
    {
    }

    public override Vector3 GetPosition(Vector3 referencePosition)
    {
        return constraintPosition;
    }

    public override bool IsInViolation(Vector3 referencePosition, Vector3 referenceEulerAngles)
    {
        //Find distance to center
        if(Mathf.Sqrt((referencePosition.x - constraintPosition.x)*(referencePosition.x - constraintPosition.x) + (referencePosition.z - constraintPosition.z) * (referencePosition.z - constraintPosition.z)) <= thresholdDistance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}