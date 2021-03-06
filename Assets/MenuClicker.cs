﻿using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MenuClicker : MonoBehaviour, IInputClickHandler {

    private static int constraintToPass = -1;
    private static string currentTask = null;
    private static int[] activeConstraints = new int[] { -1, -1, -1, -1, -1, -1 };
    private static Dictionary<int, List<int>> appliedConstraints = new Dictionary<int, List<int>>();
    // Commented out section is test dictionary
    //new Dictionary<int,List<int>> { { 1, new List<int>() { } }, { 3, new List<int>() { } }, { 5, new List<int>() { } },
    //{ 7, new List<int>() { } }, { 9, new List<int>() { } }, { 11, new List<int>() { } }, { 12, new List<int>() { } }, { 13, new List<int>() { } }, { 26, new List<int>() { } },
    //{ 29, new List<int>() { } }, { 32, new List<int>() { } }, { 39, new List<int>() {13,14,15 } }};

    // Use this for initialization
    void Start()
    {
        
    }
	// Update is called once per frame
	void Update ()
    {
		
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
                activeConstraints = new int[] { 1, 2, 3, 4, 5, 6 };
                // Grab constraint dict from DynamicTrajectoryReader
                Dictionary<string, DynamicPoints.TrajectoryPoint> pointsDict = DynamicPoints.DynamicTrajectoryReader.pointsDict;
                print(pointsDict);
                appliedConstraints.Clear();
                foreach (DynamicPoints.TrajectoryPoint value in pointsDict.Values)
                {
                    int kfid = value.keyframe_id;
                    int[] constraints = value.applied_constraints;
                    List<int> constraints_list = new List<int>();
                    foreach (int elem in constraints)
                    {
                        constraints_list.Add(elem);
                    }
                    appliedConstraints.Add(kfid, constraints_list);
                    // Only add the constraint if it doesn't already exist in the dictionary
                    //if (!appliedConstraints.ContainsKey(kfid))
                    //{
                    //  appliedConstraints.Add(kfid, constraints_list);
                    //}

                }
                menu0.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Pouring Task: Constraint Application";
                currentTask = "pouring";
                menu1.SetActive(true);
            }
            else if (thisObj.name == "TargetButton")
            {
                activeConstraints = new int[] { 7, 8, 9, 10, 11, 12 };
                // Grab constraint dict from DynamicTrajectoryReader
                Dictionary<string, DynamicPoints.TrajectoryPoint> pointsDict = DynamicPoints.DynamicTrajectoryReader.pointsDict;
                print(pointsDict);
                appliedConstraints.Clear();
                foreach (DynamicPoints.TrajectoryPoint value in pointsDict.Values)
                {
                    int kfid = value.keyframe_id;
                    int[] constraints = value.applied_constraints;
                    List<int> constraints_list = new List<int>();
                    foreach (int elem in constraints)
                    {
                        constraints_list.Add(elem);
                    }
                    appliedConstraints.Add(kfid, constraints_list);
                    // Only add the constraint if it doesn't already exist in the dictionary
                    //if (!appliedConstraints.ContainsKey(kfid))
                    //{
                    //  appliedConstraints.Add(kfid, constraints_list);
                    //}

                }
                menu0.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Target Task: Constraint Application";
                currentTask = "target";
                menu1.SetActive(true);               
            }
            else if (thisObj.name == "CubbyButton")
            {
                activeConstraints = new int[] { 13, 14, 15, 16, 17, 18 };
                // Grab constraint dict from DynamicTrajectoryReader
                Dictionary<string, DynamicPoints.TrajectoryPoint> pointsDict = DynamicPoints.DynamicTrajectoryReader.pointsDict;
                print(pointsDict);
                appliedConstraints.Clear();
                foreach (DynamicPoints.TrajectoryPoint value in pointsDict.Values)
                {
                    int kfid = value.keyframe_id;
                    int[] constraints = value.applied_constraints;
                    List<int> constraints_list = new List<int>();
                    foreach (int elem in constraints)
                    {
                        constraints_list.Add(elem);
                    }
                    appliedConstraints.Add(kfid, constraints_list);
                    // Only add the constraint if it doesn't already exist in the dictionary
                    //if (!appliedConstraints.ContainsKey(kfid))
                    //{
                    //  appliedConstraints.Add(kfid, constraints_list);
                    //}

                }
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
                print("SENDING");
                string jsonified = jsonify(appliedConstraints);
                print(jsonified);
                GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
                ROSBridgeLib.ROSConnector rosHandler = scriptHolder.GetComponent<ROSBridgeLib.ROSConnector>();
                rosHandler.PublishThis(jsonified);
                menu1.SetActive(false);
                menu0.SetActive(true);
                menu0.transform.GetChild(3).gameObject.GetComponent<TextMesh>().text = "Constraints Sent";
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
                constraintToPass = activeConstraints[0];
                
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Height Constraint 1";
            }
            else if (thisObj.name == "HeightButton2")
            {
                constraintToPass = activeConstraints[1];

                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Height Constraint 2";
            }
            else if (thisObj.name == "OrientationButton1")
            {
                constraintToPass = activeConstraints[2];

                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Orientation Constraint 1";
            }
            else if (thisObj.name == "OrientationButton2")
            {
                constraintToPass = activeConstraints[3];

                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Orientation Constraint 2";
            }
            else if (thisObj.name == "OverUnderButton1")
            {
                constraintToPass = activeConstraints[4];
                
                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Over/Under Constraint 1";
            }
            else if (thisObj.name == "OverUnderButton2")
            {
                constraintToPass = activeConstraints[5];

                menu2.SetActive(false);
                menu3.SetActive(true);
                menu3.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Over/Under Constraint 2";
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
                int max_kfid = 0;
                foreach (UnityEngine.GameObject ball in objs)
                {
                    if (ball.GetComponent<StatePrefab>().order > max_kfid)
                    {
                        max_kfid = ball.GetComponent<StatePrefab>().order;
                    }
                }
                foreach (UnityEngine.GameObject ball in objs)
                {
                    ball.tag = "StartConstraint";
                    int kfid = ball.GetComponent<StatePrefab>().order;
                    Clicker clicker = ball.GetComponent<Clicker>();
                    bool constraint_violated = clicker.CheckConstraints(ball.GetComponent<StatePrefab>().constraints);
                    if (constraint_violated)
                    {
                        ball.GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                    else
                    {
                        float factor = 255.0f / max_kfid;
                        float kcolor = (factor * kfid) / 255.0f;
                        if (kcolor > 1.0f)
                        {
                            kcolor = 1.0f;
                        }
                        ball.GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                        ball.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                        ball.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                        ball.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                        ball.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                    }
                }
                UnityEngine.GameObject[] cons = GameObject.FindGameObjectsWithTag("GameController");
                foreach (UnityEngine.GameObject constraint in cons)
                {
                    constraint.transform.position = new Vector3(-999, -999, 0);
                }
                menu3.SetActive(false);
                menu4.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Select Starting Keyframe";
                menu4.SetActive(true);
            }
            else if (thisObj.name == "ClearButton")
            {
                foreach (List<int> value in appliedConstraints.Values)
                {
                    if(value.Contains(constraintToPass))
                    {
                        value.Remove(constraintToPass);
                    }
                }
                revisualizeClear(constraintToPass);
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
                menu4.SetActive(false);
                menu3.SetActive(true);

                // Reset constraint application
                GlobalHolder.endID = -1;
                GlobalHolder.startID = -1;
            }
            else
            {
                // CONFIRMATION BUTTON
                // Package constraint data structure here to send when the user hits "finished"

                // DEBUG PRINTS
                //print(constraintToPass);
                //print(GlobalHolder.startID);
                //print(GlobalHolder.endID);

                // Validating that the constraint makes sense
                if (constraintToPass != -1 && GlobalHolder.endID >= GlobalHolder.startID && GlobalHolder.startID >= 0)
                {
                    // Look thru dictionary to add the applied constraint over the range
                    foreach (int kfid in appliedConstraints.Keys)
                    {
                        if (kfid >= GlobalHolder.startID && kfid <= GlobalHolder.endID)
                        {
                            if (!appliedConstraints[kfid].Contains(constraintToPass))
                            {
                                appliedConstraints[kfid].Add(constraintToPass);
                            }
                        }
                    }
                    //DEBUG PRINT
                    //print(appliedConstraints);

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
                    menu4.SetActive(false);
                    menu1.SetActive(true);
                    revisualize();

                    //menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraint Application";

                    // Reset constraint application
                    constraintToPass = -1;
                    GlobalHolder.endID = -1;
                    GlobalHolder.startID = -1;
                }

                // Validation pass failed - reset everything (except constraintToPass) and bump the user back a menu
                else
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
                    GlobalHolder.endID = -1;
                    GlobalHolder.startID = -1;

                    menu4.SetActive(false);
                    menu3.SetActive(true);
                    menu3.transform.GetChild(0).GetComponent<TextMesh>().text = "Invalid constraint application. Try again.";
                }

            }
        }
    }

    // Update internals of DynamicTrajectoryReader so the user sees what they should see once they've added constraint applications
    // In essence, this is the inverse of the process used to update the MenuClicker's internal dictionary
    void revisualize()
    {
        Dictionary<string, DynamicPoints.TrajectoryPoint> pointsDict = DynamicPoints.DynamicTrajectoryReader.pointsDict;
        GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
        DynamicPoints.DynamicTrajectoryReader dtr = scriptHolder.GetComponent<DynamicPoints.DynamicTrajectoryReader>();
        List<string> pointkeyList = new List<string>();
        // Use a separate list to prevent out-of-sync errors
        foreach (string pointkey in pointsDict.Keys)
        {
            pointkeyList.Add(pointkey);
        }
        // Loop over that list
        foreach (string pointkey in pointkeyList)
        {
            DynamicPoints.TrajectoryPoint point = pointsDict[pointkey];
            bool is_new = true;
            int kfid = point.keyframe_id;
            int[] constraints = point.applied_constraints;
            foreach (int new_constraint in appliedConstraints[kfid])
            {
                is_new = true;
                foreach (int old_constraint in constraints)
                {
                    if (old_constraint == new_constraint)
                    {
                        is_new = false;
                    }
                }
                // If there's a constraint that should be applied to keyframe but isn't yet
                if (is_new)
                {
                    int[] new_constraints = new int[constraints.Length + 1];
                    for (int i = 0; i < constraints.Length; i++)
                    {
                        new_constraints[i] = constraints[i];
                    }
                    // Add new constraint
                    new_constraints[constraints.Length] = new_constraint;
                    // Reset constraint array and re-draw
                    DynamicPoints.DynamicTrajectoryReader.pointsDict[pointkey].applied_constraints = new_constraints;
                    dtr.DrawTrajectoryPointNoPoint(DynamicPoints.DynamicTrajectoryReader.pointsDict[pointkey]);
                }     
            }
        }
        recolor();
    }

    // Clear a constraint from the entire trajectory
    void revisualizeClear(int constraintID)
    {
        Dictionary<string, DynamicPoints.TrajectoryPoint> pointsDict = DynamicPoints.DynamicTrajectoryReader.pointsDict;
        GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
        DynamicPoints.DynamicTrajectoryReader dtr = scriptHolder.GetComponent<DynamicPoints.DynamicTrajectoryReader>();
        print(pointsDict);
        List<string> pointkeyList = new List<string>();
        // Use a separate list to prevent out-of-sync errors
        foreach (string pointkey in pointsDict.Keys)
        {
            pointkeyList.Add(pointkey);
        }
        // Loop over that list
        foreach (string pointkey in pointkeyList)
        {
            DynamicPoints.TrajectoryPoint point = pointsDict[pointkey];
            int[] constraints = point.applied_constraints;
            foreach (int constraint in constraints)
            {
                // If a keyframe has the constraint in question, remove it from its constraint array
                if (constraintID == constraint)
                {
                    int[] new_constraints = new int[constraints.Length - 1];
                    int count = 0;
                    for (int i = 0; i < constraints.Length; i++)
                    {
                        if (constraints[i] != constraintID)
                        {
                            new_constraints[count] = constraints[i];
                            count++;
                        }
                    }
                    // Reset constraint array and re-draw
                    DynamicPoints.DynamicTrajectoryReader.pointsDict[pointkey].applied_constraints = new_constraints;
                    dtr.DrawTrajectoryPointNoPoint(DynamicPoints.DynamicTrajectoryReader.pointsDict[pointkey]);
                }
            }
        }
        recolor();
    }

    // Make sure the color gradient is preserved after revisualizations
    void recolor()
    {
        UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
        int max_kfid = 0;
        foreach (UnityEngine.GameObject ball in objs)
        {
            if (ball.GetComponent<StatePrefab>().order > max_kfid)
            {
                max_kfid = ball.GetComponent<StatePrefab>().order;
            }
        }
        foreach (UnityEngine.GameObject ball in objs)
        {
            int kfid = ball.GetComponent<StatePrefab>().order;
            Clicker clicker = ball.GetComponent<Clicker>();
            bool constraint_violated = clicker.CheckConstraints(ball.GetComponent<StatePrefab>().constraints);
            if (constraint_violated)
            {
                ball.GetComponent<MeshRenderer>().material.color = Color.red;
                ball.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
                ball.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.red;
                ball.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                ball.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else
            {
                float factor = 255.0f / max_kfid;
                float kcolor = (factor * kfid) / 255.0f;
                if (kcolor > 1.0f)
                {
                    kcolor = 1.0f;
                }
                ball.GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                ball.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                ball.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                ball.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
                ball.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.color = new Color(kcolor, 1.0f, kcolor);
            }
        }
    }

    // Turn our dictionary into a JSON string to pass over ROS network
    string jsonify (Dictionary<int,List<int>> dict)
    {
        string json_string = "{";
        foreach (int key in dict.Keys)
        {
            json_string += "\\\"" + key.ToString() + "\\\"";
            json_string += ":{\\\"applied_constraints\\\":[";
            foreach (int value in dict[key])
            {
                json_string += value.ToString();
                json_string += ",";
            }
            // Remove final comma
            if (json_string[json_string.Length - 1] == ',')
            {
                json_string = json_string.Substring(0, json_string.Length - 1);
            }
            json_string += "]},";
        }
        // Remove final comma
        if (json_string[json_string.Length - 1] == ',')
        {
            json_string = json_string.Substring(0, json_string.Length - 1);
        }
        json_string += "}";
        //print(json_string);
        return json_string;
    }
}
