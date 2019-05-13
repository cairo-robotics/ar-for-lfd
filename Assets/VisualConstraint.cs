using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VisualConstraint : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    public virtual Vector3 GetPosition(Vector3 referencePosition)
    {
        return referencePosition;
    }

    public virtual bool IsInViolation(Vector3 referencePosition, Vector3 referenceEulerAngles)
    {
        return false;
    }
}