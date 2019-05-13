using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StatePrefab : MonoBehaviour
{
    public int[] constraintsActive;
    public int order;
    public ArrayList constraints;
    public bool toggled;
    void Start()
    {
    }

    void Update()
    {
    }

    public bool toggle()
    {
        this.toggled = !toggled;
        return toggled;
    }

    public bool turnOff()
    {
        bool old_toggled = toggled;
        toggled = false;
        return old_toggled;
    }
}