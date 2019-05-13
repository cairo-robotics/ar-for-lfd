using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;


public class Clicker : MonoBehaviour, IInputClickHandler
{
    Color[] Select = { Color.white, Color.cyan, Color.red };
    private StatePrefab thisPrefab;

    private void Start()
    {
        Collider collider = GetComponentInChildren<Collider>();
        if (collider == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        thisPrefab = gameObject.GetComponent<StatePrefab>();
    }

    /*    private void Update()
        {
            if (a)
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Select[0];
            }
        }*/

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
        bool this_toggled = thisPrefab.toggle();

        //make sure all other spots are turned off
        foreach (UnityEngine.GameObject ball in objs)
        {
            if (ball != gameObject)
            {
                bool was_toggled = ball.GetComponent<StatePrefab>().turnOff();
                if (was_toggled)
                {
                    ball.GetComponent<MeshRenderer>().material.color = Select[0];
                    ball.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select[0];
                    toggleConstraints(ball.GetComponent<StatePrefab>().constraints, false);
                }
            }
        }

        //toggle this object's constraints and text
        toggleConstraints(thisPrefab.constraints, this_toggled);
        updateText(thisPrefab.constraintsActive, this_toggled);

        //recolor this object
        if (this_toggled)
        {
            if (CheckConstraints(thisPrefab.constraints))
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Select[2];
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select[2];
            } else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Select[1];
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select[1];
            }
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Select[0];
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select[0];
        }
    }

    //turn list of constraints on or off according to setting value
    void toggleConstraints(ArrayList constraints, bool setting)
    {
        foreach(VisualConstraint constraint in constraints)
        {
            if (setting)
            {
                //if turning a constraint on, update its position according to current object
                constraint.transform.position = constraint.GetPosition(gameObject.transform.position);
            } else
            {
                constraint.transform.position = new Vector3(-999, -999, 0);
            }
            constraint.GetComponent<MeshRenderer>().enabled = setting;
        }
    }

    bool CheckConstraints(ArrayList constraints)
    {
       foreach(VisualConstraint constraint in constraints)
       {
            if (constraint.IsInViolation(gameObject.transform.position, gameObject.transform.eulerAngles)) { return true; }
       }
        return false;
    }

    //turn text on or off and fill in correct constraint IDs
    void updateText(int[] constraintsActive, bool setting)
    {
        UnityEngine.GameObject text = GameObject.FindGameObjectsWithTag("FloatingText")[0];
        if (!setting)
        {
            text.GetComponent<TextMesh>().text = "";
            return;
        }
        text.GetComponent<TextMesh>().text = "Constraints Active: ";
        for (int i = 0; i < gameObject.GetComponent<StatePrefab>().constraintsActive.Length; i++)
        {
            text.GetComponent<TextMesh>().text += gameObject.GetComponent<StatePrefab>().constraintsActive[i] + ";";
        }
    }
}