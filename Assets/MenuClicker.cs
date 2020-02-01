using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MenuClicker : MonoBehaviour, IInputClickHandler {

    private static string constraintToPass = null;
    private static string currentTask = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        GameObject thisObj = gameObject;
        GameObject menu0holder = GameObject.Find("Menu0Holder");
        GameObject menu1holder = GameObject.Find("Menu1Holder");
        GameObject menu2holder = GameObject.Find("Menu2Holder");
        GameObject menu3holder = GameObject.Find("Menu3Holder");
        GameObject menu4holder = GameObject.Find("Menu4Holder");
        GameObject menu0 = menu0holder.transform.GetChild(0).gameObject;
        GameObject menu1 = menu1holder.transform.GetChild(0).gameObject;
        GameObject menu2 = menu2holder.transform.GetChild(0).gameObject;
        GameObject menu3 = menu3holder.transform.GetChild(0).gameObject;
        GameObject menu4 = menu4holder.transform.GetChild(0).gameObject;

        //LEVEL 0 MENU - CONDITION 3 ONLY
        //  Pouring Task -> set active constraints to 1,2,3,4,5,6; go to Level 1 Menu; set menu text to "Pouring Task" 
        //  Target Task -> set active constraints to 7,8,9,10,11,12; go to Level 1 Menu; set menu text to "Target Task"
        //  Cubby Task -> set active constraints to 13,14,15,16,17,18; go to Level 1 Menu; set menu text to "Cubby Task"
        if (menu0.activeInHierarchy)
        {
            if (thisObj.name == "PouringButton")
            {
                //TODO: set active constraints
                menu0.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Pouring Task: Constraint Application";
                currentTask = "pouring";
                menu1.SetActive(true);
            }
            else if (thisObj.name == "TargetButton")
            {
                //TODO: set active constraints
                menu0.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Target Task: Constraint Application";
                currentTask = "target";
                menu1.SetActive(true);               
            }
            else if (thisObj.name == "CubbyButton")
            {
                //TODO: set active constraints
                menu0.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Cubby Task: Constraint Application";
                currentTask = "cubby";
                menu1.SetActive(true);
            }
        }

        //LEVEL 1 MENU - CONDITION 3 ONLY
        //  Select Constraint -> go to Level 2 Menu;
        //  Send Constraints to Robot -> create formatted JSON -> send ModelUpdateRequest to modelUpdate ROS service
        //  Select New Task -> go to Level 0 Menu;
        else if (menu1.activeInHierarchy)
        {
            if (thisObj.name == "SelectButton")
            {
                menu1.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "SendButton")
            {
                //TODO: format JSON (subroutine)
                //TODO: send ModelUpdateRequest to modelUpdate ROS service
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraints Sent";
            }
            else if (thisObj.name == "BackButton")
            {
                menu1.SetActive(false);
                menu0.SetActive(true);
            }
        }

        //LEVEL 2 MENU - CONDITION 3 ONLY
        //  Height Constraint 1 -> go to Level 3 Menu; set menu text to "Height Constraint 1"
        //  Height Constraint 2 -> go to Level 3 Menu; set menu text to "Height Constraint 2"
        //  Orientation Constraint 1 -> go to Level 3 Menu; set menu text to "Orientation Constraint 1"
        //  Orientation Constraint 2 -> go to Level 3 Menu; set menu text to "Orientation Constraint 2"
        //  Over/Under Constraint 1 -> go to Level 3 Menu; set menu text to "Over/Under Constraint 1"
        //  Over/Under Constraint 2 -> go to Level 3 Menu; set menu text to "Over/Under Constraint 2"
        //  Go Back -> go to Level 1 Menu
        else if (menu2.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu2.SetActive(false);
                menu1.SetActive(true);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraint Application";
            }
            else if (thisObj.name == "HeightButton1")
            {
                constraintToPass = "height1";
                
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Height Constraint 1";
            }
            else if (thisObj.name == "OverUnderButton1")
            {
                constraintToPass = "overunder1";
                
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Over/Under Constraint 1";
            }
            else if (thisObj.name == "OrientationButton1")
            {
                constraintToPass = "orientation1";
                
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Orientation Constraint 1";
            }
            else if (thisObj.name == "HeightButton2")
            {
                constraintToPass = "height2";

                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Height Constraint 2";
            }
            else if (thisObj.name == "OverUnderButton2")
            {
                constraintToPass = "overunder2";

                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Over/Under Constraint 2";
            }
            else if (thisObj.name == "OrientationButton2")
            {
                constraintToPass = "orientation2";

                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Orientation Constraint 2";
            }
        }

        //LEVEL 3 MENU - CONDITION 3 ONLY
        //  Apply Constraint -> go to Level 4 Menu; retag keyframes for constraint application
        //  Clear Constraint -> clear constraint; set menu text to "Constraint cleared from model"
        //  Go Back -> go to Level 2 Menu
        else if (menu3.activeInHierarchy)
        {
            if (thisObj.name == "ApplyButton")
            {
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (UnityEngine.GameObject ball in objs)
                {
                    ball.tag = "StartConstraint";
                    ball.GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.white;
                    ball.transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.white;
                }
                menu3.SetActive(false);
                menu4.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Select Starting Keyframe";
                menu4.SetActive(true);
            }
            else if (thisObj.name == "ClearButton")
            {
                //TODO: clear constraint
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Constraint cleared from model";
            }
            else if (thisObj.name == "BackButton")
            {
                menu3.SetActive(false);
                menu2.SetActive(true);
            }
        }

        //LEVEL 4 MENU - CONDITION 3 ONLY:
        //  <Select Starting Constraint -> Select Ending Constraint>
        //  Confirm -> go to Level 1 Menu
        //  Go Back -> go to Level 3 Menu
        else if (menu4.activeInHierarchy)
        {
            //TODO - revamp this description (ROS comms won't happen here, just collate on the stack)
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
