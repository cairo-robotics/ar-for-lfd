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

    public override Vector3 GetScale(Vector3 referenceScale)
    {
        return new Vector3(2.0f * thresholdDistance, 0.5f, 2.0f * thresholdDistance);
    }

    public override bool IsInViolation(Vector3 referencePosition, Vector3 referenceEulerAngles)
    {
        //Find distance to center & verify that we're above the constraint base
        if((Mathf.Sqrt((referencePosition.x - constraintPosition.x)*(referencePosition.x - constraintPosition.x) + (referencePosition.z - constraintPosition.z) * (referencePosition.z - constraintPosition.z)) <= thresholdDistance) && (referencePosition.y >= (constraintPosition.y - 0.5f)))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}