/* Handles outcomes of the user clicking on a keyframe marker
 * Toggles constraint visualization, marker recoloring, and text output
 */

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
        print("COLLIDER = " + GetComponentInChildren<Collider>());
        if (collider == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        thisPrefab = gameObject.GetComponent<StatePrefab>();
    }

    //Uncomment this to overrides the normal text printing and print FPS for performance debugging 
    /*private void Update()
    {
        UnityEngine.GameObject text = GameObject.FindGameObjectsWithTag("FloatingText")[0];
        text.GetComponent<TextMesh>().text = "FPS: " + (int)(1f / Time.unscaledDeltaTime);
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
                    ball.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Select[0];
                    ball.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Select[0];
                    ball.transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Select[0];
                    toggleConstraints(ball.GetComponent<StatePrefab>().constraints, false);
                }
            }
        }

        // Debug prints for keyframe selection
        //print(gameObject.GetComponent<StatePrefab>().order);
        //print(gameObject.tag);

        //toggle this object's constraints and text
        if (gameObject.tag == "Respawn")
        {
            toggleConstraints(thisPrefab.constraints, this_toggled);
            updateText(thisPrefab.constraintsActive, this_toggled);
        }
        // Push the logic along + save the keyframe
        else if (gameObject.tag == "StartConstraint")
        {
            GlobalHolder.startID = gameObject.GetComponent<StatePrefab>().order;
            UnityEngine.GameObject[] start_objs = GameObject.FindGameObjectsWithTag("StartConstraint");
            foreach (UnityEngine.GameObject keyframe in start_objs)
            {
                keyframe.tag = "EndConstraint";
            }

            GameObject menu4holder = GameObject.Find("Menu4Holder");
            menu4holder.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = "Select Ending Keyframe";
        }
        else if (gameObject.tag == "EndConstraint")
        {
            GlobalHolder.endID = gameObject.GetComponent<StatePrefab>().order;
            UnityEngine.GameObject[] end_objs = GameObject.FindGameObjectsWithTag("EndConstraint");
            foreach (UnityEngine.GameObject keyframe in end_objs)
            {
                keyframe.tag = "Respawn";
            }

            GameObject menu4holder = GameObject.Find("Menu4Holder");
            menu4holder.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = "Keyframes Selected";
        }

        //recolor this object
        if (this_toggled)
        {
            if (CheckConstraints(thisPrefab.constraints) && gameObject.tag == "Respawn")
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Select[2];
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select[2];
                gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Select[2];
                gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Select[2];
                gameObject.transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Select[2];
            } else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Select[1];
                gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select[1];
                gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Select[1];
                gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Select[1];
                gameObject.transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Select[1];
            }
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Select[0];
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select[0];
            gameObject.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Select[0];
            gameObject.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Select[0];
            gameObject.transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Select[0];
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