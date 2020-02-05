using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprightConstraintPrefab : VisualConstraint
{
    public Vector3 referenceAngle;
    public float thresholdAngle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override Vector3 GetPosition(Vector3 referencePosition)
    {
        return new Vector3(referencePosition.x, referencePosition.y, referencePosition.z);
    }

    public override bool IsInViolation(Vector3 referencePosition, Vector3 referenceEulerAngles)
    {
        float angle_diff = Quaternion.Angle(Quaternion.Euler(referenceAngle), Quaternion.Euler(referenceEulerAngles));
        if (angle_diff <= thresholdAngle)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
