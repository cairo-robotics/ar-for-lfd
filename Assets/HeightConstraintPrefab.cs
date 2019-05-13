using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightConstraintPrefab : VisualConstraint
{
    public float referenceHeight;
    public float thresholdDistance;
    void Start()
    {
    }

    void Update()
    {
    }

    public override Vector3 GetPosition(Vector3 referencePosition)
    {
        return new Vector3(referencePosition.x, referenceHeight + thresholdDistance, referencePosition.z);
    }

    public override bool IsInViolation(Vector3 referencePosition, Vector3 referenceEulerAngles)
    {
        if(referencePosition.y > this.referenceHeight + this.thresholdDistance)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
