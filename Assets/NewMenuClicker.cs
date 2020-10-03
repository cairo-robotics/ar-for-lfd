using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class NewMenuClicker : MonoBehaviour, IInputClickHandler {

    private static int constraintToPass = -1;
    private static Dictionary<int, List<int>> appliedConstraints = new Dictionary<int, List<int>>();

    // For testing constraint application packets
    //private static Dictionary<int,List<int>> appliedConstraints = new Dictionary<int, List<int>> { { 1, new List<int>() { } }, { 3, new List<int>() { } }, { 5, new List<int>() { } },
    //{ 7, new List<int>() { } }, { 9, new List<int>() { } }, { 11, new List<int>() { } }, { 12, new List<int>() { } }, { 13, new List<int>() { } }, { 26, new List<int>() { } },
    //{ 29, new List<int>() { } }, { 32, new List<int>() { } }, { 39, new List<int>() {13,14,15 } }};

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    // Upon menu button click
    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        // Initializing menu objects
        GameObject thisObj = gameObject;
        GameObject menu1holder = GameObject.Find("Menu1Holder");
        GameObject menu2holder = GameObject.Find("Menu2Holder");
        GameObject menu3Aholder = GameObject.Find("Menu3AHolder");
        GameObject menu3Bholder = GameObject.Find("Menu3BHolder");
        GameObject menu3Cholder = GameObject.Find("Menu3CHolder");
        GameObject menu4holder = GameObject.Find("Menu4Holder");
        GameObject menu5holder = GameObject.Find("Menu5Holder");
        GameObject menu6holder = GameObject.Find("Menu6Holder");
        GameObject menu1 = menu1holder.transform.GetChild(0).gameObject;
        GameObject menu2 = menu2holder.transform.GetChild(0).gameObject;
        GameObject menu3A = menu3Aholder.transform.GetChild(0).gameObject;
        GameObject menu3B = menu3Bholder.transform.GetChild(0).gameObject;
        GameObject menu3C = menu3Cholder.transform.GetChild(0).gameObject;
        GameObject menu4 = menu4holder.transform.GetChild(0).gameObject;
        GameObject menu5 = menu5holder.transform.GetChild(0).gameObject;
        GameObject menu6 = menu6holder.transform.GetChild(0).gameObject;

        //MENU 1
        //  Edit Constraint Params -> GOTO Menu 2
        //  Apply Constraint -> GOTO Menu 4
        //  Send to Robot -> fire publisher; display confirmation message
        if (menu1.activeInHierarchy)
        {
            if (thisObj.name == "EditButton")
            {
                menu1.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "ApplyButton")
            {
                menu1.SetActive(false);
                menu4.SetActive(true);
            }
            else if (thisObj.name == "SendButton")
            {
                string jsonified = jsonify(appliedConstraints);
                GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
                ROSBridgeLib.ROSConnector rosHandler = scriptHolder.GetComponent<ROSBridgeLib.ROSConnector>();
                rosHandler.PublishThis(jsonified);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraints Sent";
            }
        }

        //MENU 2 "Edit Constraint Parameters"
        //  Go Back -> GOTO Menu 1
        //  Height Constraint 1 -> set as active; clear trajectory viz; show height viz; GOTO Menu 3A
        //  Height Constraint 2 -> set as active; clear trajectory viz; show height viz; GOTO Menu 3A
        //  Orientation Constraint 1 -> set as active; clear trajectory viz; show orientation viz; GOTO Menu 3B
        //  Orientation Constraint 2 -> set as active; clear trajectory viz; show orientation viz; GOTO Menu 3B
        //  Over/Under Constraint 1 -> set as active; clear trajectory viz; show over/under viz; GOTO Menu 3C
        //  Over/Under Constraint 2 -> set as active; clear trajectory viz; show over/under viz; GOTO Menu 3C
        else if (menu2.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                menu2.SetActive(false);
                menu1.SetActive(true);
            }
            else if (thisObj.name == "HeightButton1")
            {
                constraintToPass = 1;
                menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Height Constraint 1";
                menu2.SetActive(false);
                menu3A.SetActive(true);
            }
            else if (thisObj.name == "HeightButton2")
            {
                constraintToPass = 2;
                menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Height Constraint 2";
                menu2.SetActive(false);
                menu3A.SetActive(true);
            }
            else if (thisObj.name == "OrientationButton1")
            {
                constraintToPass = 3;
                menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Orientation Constraint 1";
                menu2.SetActive(false);
                menu3B.SetActive(true);
            }
            else if (thisObj.name == "OrientationButton2")
            {
                constraintToPass = 4;
                menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Orientation Constraint 2";
                menu2.SetActive(false);
                menu3B.SetActive(true);
            }
            else if (thisObj.name == "OverUnderButton1")
            {
                constraintToPass = 5;
                menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Over/Under Constraint 1";
                menu2.SetActive(false);
                menu3C.SetActive(true);
            }
            else if (thisObj.name == "OverUnderButton2")
            {
                constraintToPass = 6;
                menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Over/Under Constraint 2";
                menu2.SetActive(false);
                menu3C.SetActive(true);
            }
        }

        //Menu3A "Editing Height Constraint"
        //  Go Back -> clear height viz; show trajectory viz; GOTO Menu 2
        //  Edit Height (Xbox controller) -> update constraint viz
        //  Confirm Params -> update constraint representation; clear height viz; show trajectory viz; GOTO Menu 1
        else if (menu3A.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu3A.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "ConfirmButton")
            {
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                menu3A.SetActive(false);
                menu1.SetActive(true);
            }
        }

        //Menu3B "Editing Orientation Constraint"
        //  Go Back -> clear height viz; show trajectory viz; GOTO Menu 2
        //  Edit Roll (Xbox controller) -> update constraint viz
        //  Edit Pitch (Xbox controller) -> update constraint viz
        //  Edit Yaw (Xbox controller) -> update constraint viz
        //  Edit Affordance (Xbox controller) -> update constraint viz
        //  Confirm Params -> update constraint representation; clear height viz; show trajectory viz; GOTO Menu 1
        else if (menu3B.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu3B.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "ConfirmButton")
            {
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                menu3B.SetActive(false);
                menu1.SetActive(true);
            }
        }

        //Menu3C "Editing Over/Under Constraint"
        //  Go Back -> clear height viz; show trajectory viz; GOTO Menu 2
        //  Edit X (Xbox controller) -> update constraint viz
        //  Edit Y (Xbox controller) -> update constraint viz
        //  Edit Z (Xbox controller) -> update constraint viz
        //  Edit Radius (Xbox controller) -> update constraint viz
        //  Confirm Params -> update constraint representation; clear height viz; show trajectory viz; GOTO Menu 1
        else if (menu3C.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu3C.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "ConfirmButton")
            {
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                menu3C.SetActive(false);
                menu1.SetActive(true);
            }
        }

        //MENU 4 "Apply Constraint"
        //  Go Back -> GOTO Menu 1
        //  Height Constraint 1 -> set as active; GOTO Menu 5
        //  Height Constraint 2 -> set as active; GOTO Menu 5
        //  Orientation Constraint 1 -> set as active; GOTO Menu 5
        //  Orientation Constraint 2 -> set as active; GOTO Menu 5
        //  Over/Under Constraint 1 -> set as active; GOTO Menu 5
        //  Over/Under Constraint 2 -> set as active; GOTO Menu 5
        else if (menu4.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                menu4.SetActive(false);
                menu1.SetActive(true);
            }
            else if (thisObj.name == "HeightButton1")
            {
                constraintToPass = 1;
                menu5.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Apply Height Constraint 1";
                menu4.SetActive(false);
                menu5.SetActive(true);
            }
            else if (thisObj.name == "HeightButton2")
            {
                constraintToPass = 2;
                menu5.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Apply Height Constraint 2";
                menu4.SetActive(false);
                menu5.SetActive(true);
            }
            else if (thisObj.name == "OrientationButton1")
            {
                constraintToPass = 3;
                menu5.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Apply Orientation Constraint 1";
                menu4.SetActive(false);
                menu5.SetActive(true);
            }
            else if (thisObj.name == "OrientationButton2")
            {
                constraintToPass = 4;
                menu5.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Apply Orientation Constraint 2";
                menu4.SetActive(false);
                menu5.SetActive(true);
            }
            else if (thisObj.name == "OverUnderButton1")
            {
                constraintToPass = 5;
                menu5.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Apply Over/Under Constraint 1";
                menu4.SetActive(false);
                menu5.SetActive(true);
            }
            else if (thisObj.name == "OverUnderButton2")
            {
                constraintToPass = 6;
                menu5.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Apply Over/Under Constraint 2";
                menu4.SetActive(false);
                menu5.SetActive(true);
            }
        }

        //MENU 5 "Apply <Constraint Name>"
        //  Go Back -> GOTO Menu 4
        //  Add Constraint to Keyframes -> GOTO Menu 6
        //  Remove from Trajectory -> update trajectory representation; display confirmation message
        else if (menu5.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu5.SetActive(false);
                menu4.SetActive(true);
            }
            else if (thisObj.name == "ApplyButton")
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
                    // Color all keyframes in violation red
                    if (constraint_violated)
                    {
                        ball.GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(2).GetComponent<MeshRenderer>().material.color = Color.red;
                        ball.transform.GetChild(0).transform.GetChild(3).GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                    // Color all other keyframes according to a white -> green gradient
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
                menu6.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Select Starting Keyframe";
                menu5.SetActive(false);
                menu6.SetActive(true);
            }
            else if (thisObj.name == "ClearButton")
            {
                foreach (List<int> value in appliedConstraints.Values)
                {
                    if (value.Contains(constraintToPass))
                    {
                        value.Remove(constraintToPass);
                    }
                }
                revisualizeClear(constraintToPass);
                menu5.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Constraint Cleared from Model";
            }
        }

        //MENU 6 "Select Starting Keyframe"
        //  Go Back -> GOTO Menu 5
        //  Confirm Application -> IF valid, update trajectory representation; GOTO Menu 1, ELSE display error message; GOTO Menu 5
        //  <User clicks on starting keyframe> -> mark choice; color selection; change text: "Select Ending Keyframe"
        //  <User clicks on ending keyframe> -> mark choice; color selection; change text: "Selection Received"
        else if (menu6.activeInHierarchy)
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
                menu6.SetActive(false);
                menu5.SetActive(true);

                // Reset constraint application
                GlobalHolder.endID = -1;
                GlobalHolder.startID = -1;
            }
            else if (thisObj.name == "ConfirmButton")
            {
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
                    menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                    menu6.SetActive(false);
                    menu1.SetActive(true);
                    revisualize();

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

                    menu6.SetActive(false);
                    menu5.SetActive(true);
                    menu5.transform.GetChild(0).GetComponent<TextMesh>().text = "Invalid constraint application. Try again.";
                }
            }
        }
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
    string jsonify(Dictionary<int, List<int>> dict)
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
