using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MenuClicker : MonoBehaviour, IInputClickHandler {

    private static string constraintToPass = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        GameObject thisObj = gameObject;
        GameObject menu1holder = GameObject.Find("Menu1Holder");
        GameObject menu2holder = GameObject.Find("Menu2Holder");
        GameObject menu3holder = GameObject.Find("Menu3Holder");
        GameObject menu1 = menu1holder.transform.GetChild(0).gameObject;
        GameObject menu2 = menu2holder.transform.GetChild(0).gameObject;
        GameObject menu3 = menu3holder.transform.GetChild(0).gameObject;

        // Base menu
        if (menu1.activeInHierarchy)
        {
            if (thisObj.name == "AddButton")
            {
                menu1.SetActive(false);
                menu2.SetActive(true);
            }
            else
            {
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraints Cleared";
            }
        }

        // Selection of constraint to apply
        else if (menu2.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu2.SetActive(false);
                menu1.SetActive(true);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraint Application";
            }
            else if (thisObj.name == "HeightButton")
            {
                constraintToPass = "height";
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (UnityEngine.GameObject ball in objs)
                {
                    ball.tag = "StartConstraint";
                    ball.GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.white;
                }
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Select Starting Keyframe";
            }
            else if (thisObj.name == "OverUnderButton")
            {
                constraintToPass = "overunder";
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (UnityEngine.GameObject ball in objs)
                {
                    ball.tag = "StartConstraint";
                    ball.GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.white;
                }
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Select Starting Keyframe";
            }
            else if (thisObj.name == "OrientationButton")
            {
                constraintToPass = "orientation";
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (UnityEngine.GameObject ball in objs)
                {
                    ball.tag = "StartConstraint";
                    ball.GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.white;
                }
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Select Starting Keyframe";
            }
        }

        // Selection of starting and ending keyframes for constraint application
        else if (menu3.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                
                UnityEngine.GameObject[] start_objs = GameObject.FindGameObjectsWithTag("StartConstraint");
                foreach (UnityEngine.GameObject ball in start_objs)
                {
                    ball.tag = "Respawn";
                }
                UnityEngine.GameObject[] end_objs = GameObject.FindGameObjectsWithTag("EndConstraint");
                foreach (UnityEngine.GameObject ball in end_objs)
                {
                    ball.tag = "Respawn";
                }
                menu3.SetActive(false);
                menu2.SetActive(true);

                // Reset constraint application
                constraintToPass = null;
                GlobalHolder.endID = -1;
                GlobalHolder.startID = -1;
            }
            else
            {
                // CONFIRMATION BUTTON
                // PACKAGE CONSTRAINT APPLICATION MESSAGE AND PASS IT ON HERE (AFTER CHECKING VALIDITY)
                // "constraintToPass" WILL TELL CONSTRAINT TYPE
                // "GlobalHolder.startID" + "GlobalHolder.endID" WILL TELL CONSTRAINT START + END POINTS
                //print(constraintToPass);
                //print(GlobalHolder.startID);
                //print(GlobalHolder.endID);

                // Validating that the constraint makes sense
                if (constraintToPass != null && GlobalHolder.endID >= GlobalHolder.startID && GlobalHolder.startID >= 0)
                {
                    // Send ROS message here!!! TODO
                    UnityEngine.GameObject[] start_objs = GameObject.FindGameObjectsWithTag("StartConstraint");
                    foreach (UnityEngine.GameObject ball in start_objs)
                    {
                        ball.tag = "Respawn";
                    }
                    UnityEngine.GameObject[] end_objs = GameObject.FindGameObjectsWithTag("EndConstraint");
                    foreach (UnityEngine.GameObject ball in end_objs)
                    {
                        ball.tag = "Respawn";
                    }
                    menu3.SetActive(false);
                    menu1.SetActive(true);
                    menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraint Application";

                    // Reset constraint application
                    constraintToPass = null;
                    GlobalHolder.endID = -1;
                    GlobalHolder.startID = -1;
                }

            }
        }
    }
}
