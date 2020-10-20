using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class NewMenuClicker : MonoBehaviour, IInputClickHandler {

    private static int constraintToPass = -1;
    private static Dictionary<int, List<int>> appliedConstraints = new Dictionary<int, List<int>>();
    private static int lastControl = 0;

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

        // Initializing constraint objects for editing
        GameObject heightconstraintholder = GameObject.Find("HeightConstraintHolder");
        GameObject heightconstraint1 = heightconstraintholder.transform.GetChild(0).gameObject;
        GameObject heightconstraint2 = heightconstraintholder.transform.GetChild(1).gameObject;
        GameObject overunderconstraintholder = GameObject.Find("OverUnderConstraintHolder");
        GameObject overunderconstraint1 = overunderconstraintholder.transform.GetChild(0).gameObject;
        GameObject overunderconstraint2 = overunderconstraintholder.transform.GetChild(1).gameObject;
        GameObject orientationconstraintholder = GameObject.Find("OrientationConstraintHolder");
        GameObject orientationconstraint1 = orientationconstraintholder.transform.GetChild(0).gameObject;
        GameObject orientationconstraint2 = orientationconstraintholder.transform.GetChild(1).gameObject;

        // Grabbing constraintsDict and drawnObjectsDict from DynamicTrajectoryReader
        Dictionary<string, VisualConstraint> constraintsDict = DynamicPoints.DynamicTrajectoryReader.constraintsDict;
        Dictionary<string, MonoBehaviour> drawnObjectsDict = DynamicPoints.DynamicTrajectoryReader.drawnObjectsDict;

        //MENU 1
        //  Edit Constraint Params -> GOTO Menu 2
        //  Apply Constraint -> GOTO Menu 4
        //  Send to Robot -> fire publisher; display confirmation message
        if (menu1.activeInHierarchy)
        {
            if (thisObj.name == "EditButton")
            {
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (GameObject ball in objs)
                {
                    ball.GetComponent<Renderer>().enabled = false;
                }
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
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (GameObject ball in objs)
                {
                    ball.GetComponent<Renderer>().enabled = true;
                }
                menu2.SetActive(false);
                menu1.SetActive(true);
            }
            else if (thisObj.name == "HeightButton1")
            {
                constraintToPass = 1;
                menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Height Constraint 1";
                menu2.SetActive(false);
                menu3A.SetActive(true);
                heightconstraint1.SetActive(true);
            }
            else if (thisObj.name == "HeightButton2")
            {
                constraintToPass = 2;
                menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Height Constraint 2";
                menu2.SetActive(false);
                menu3A.SetActive(true);
                heightconstraint2.SetActive(true);
            }
            else if (thisObj.name == "OrientationButton1")
            {
                constraintToPass = 3;
                menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Orientation Constraint 1";
                menu2.SetActive(false);
                menu3B.SetActive(true);
                orientationconstraint1.SetActive(true);
                foreach (string constraintkey in constraintsDict.Keys)
                {
                    if (constraintkey == constraintToPass.ToString())
                    {
                        UprightConstraintPrefab upright = (UprightConstraintPrefab)constraintsDict[constraintkey];
                        orientationconstraint1.transform.rotation = Quaternion.Euler(upright.referenceAngle.x,upright.referenceAngle.y,upright.referenceAngle.z);
                    }
                }
                orientationconstraintholder.transform.GetChild(2).gameObject.SetActive(true);
                orientationconstraintholder.transform.GetChild(3).gameObject.SetActive(true);
                orientationconstraintholder.transform.GetChild(4).gameObject.SetActive(true);
            }
            else if (thisObj.name == "OrientationButton2")
            {
                constraintToPass = 4;
                menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Orientation Constraint 2";
                menu2.SetActive(false);
                menu3B.SetActive(true);
                orientationconstraint2.SetActive(true);
                foreach (string constraintkey in constraintsDict.Keys)
                {
                    if (constraintkey == constraintToPass.ToString())
                    {
                        UprightConstraintPrefab upright = (UprightConstraintPrefab)constraintsDict[constraintkey];
                        orientationconstraint2.transform.rotation = Quaternion.Euler(upright.referenceAngle.x, upright.referenceAngle.y, upright.referenceAngle.z);
                    }
                }
                orientationconstraintholder.transform.GetChild(2).gameObject.SetActive(true);
                orientationconstraintholder.transform.GetChild(3).gameObject.SetActive(true);
                orientationconstraintholder.transform.GetChild(4).gameObject.SetActive(true);
            }
            else if (thisObj.name == "OverUnderButton1")
            {
                constraintToPass = 5;
                menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Over/Under Constraint 1";
                menu2.SetActive(false);
                menu3C.SetActive(true);
                overunderconstraint1.SetActive(true);
                foreach (string constraintkey in constraintsDict.Keys)
                {
                    if (constraintkey == constraintToPass.ToString())
                    {
                        OverUnderConstraintPrefab overunder = (OverUnderConstraintPrefab)constraintsDict[constraintkey];
                        overunderconstraint1.transform.position = overunder.constraintPosition;
                        float cmScale = (float)(2.0 * overunder.thresholdDistance);
                        Vector3 newScale = new Vector3(cmScale, 0.5f, cmScale);
                        overunderconstraint1.transform.localScale = newScale;
                    }
                }   
            }
            else if (thisObj.name == "OverUnderButton2")
            {
                constraintToPass = 6;
                menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Editing Over/Under Constraint 2";
                menu2.SetActive(false);
                menu3C.SetActive(true);
                overunderconstraint2.SetActive(true);
                foreach (string constraintkey in constraintsDict.Keys)
                {
                    if (constraintkey == constraintToPass.ToString())
                    {
                        OverUnderConstraintPrefab overunder = (OverUnderConstraintPrefab)constraintsDict[constraintkey];
                        overunderconstraint2.transform.position = overunder.constraintPosition;
                        float cmScale = (float)(2.0 * overunder.thresholdDistance);
                        Vector3 newScale = new Vector3(cmScale, 0.5f, cmScale);
                        overunderconstraint2.transform.localScale = newScale;
                    }
                }
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
                menu3A.transform.GetChild(5).gameObject.SetActive(false);
                menu3A.transform.GetChild(6).gameObject.SetActive(false);
                if (constraintToPass == 1)
                {
                    heightconstraint1.SetActive(false);
                }
                else if (constraintToPass == 2)
                {
                    heightconstraint2.SetActive(false);
                }
                menu3A.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "ConfirmButton")
            {
                menu3A.transform.GetChild(5).gameObject.SetActive(false);
                menu3A.transform.GetChild(6).gameObject.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                if (constraintToPass == 1)
                {
                    heightconstraint1.SetActive(false);
                }
                else if (constraintToPass == 2)
                {
                    heightconstraint2.SetActive(false);
                }
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (GameObject ball in objs)
                {
                    ball.GetComponent<Renderer>().enabled = true;
                }
                menu3A.SetActive(false);
                menu1.SetActive(true);
            }
            else if (thisObj.name == "DirectionButton")
            {
                menu3A.transform.GetChild(5).gameObject.SetActive(false);
                menu3A.transform.GetChild(6).gameObject.SetActive(false);
                if (constraintToPass == 1)
                {
                    if (heightconstraint1.transform.GetChild(0).gameObject.activeInHierarchy)
                    {
                        heightconstraint1.transform.GetChild(0).gameObject.SetActive(false);
                        heightconstraint1.transform.GetChild(1).gameObject.SetActive(true);
                        menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Valid Direction: Below Plane";
                    }
                    else
                    {
                        heightconstraint1.transform.GetChild(1).gameObject.SetActive(false);
                        heightconstraint1.transform.GetChild(0).gameObject.SetActive(true);
                        menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Valid Direction: Above Plane";
                    }
                }
                else if (constraintToPass == 2)
                {
                    if (heightconstraint2.transform.GetChild(0).gameObject.activeInHierarchy)
                    {
                        heightconstraint2.transform.GetChild(0).gameObject.SetActive(false);
                        heightconstraint2.transform.GetChild(1).gameObject.SetActive(true);
                        menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Valid Direction: Below Plane";
                    }
                    else
                    {
                        heightconstraint2.transform.GetChild(1).gameObject.SetActive(false);
                        heightconstraint2.transform.GetChild(0).gameObject.SetActive(true);
                        menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Valid Direction: Above Plane";
                    }
                }
            }
            else if (thisObj.name == "HeightButton")
            {
                if (constraintToPass == 1)
                {
                    menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Threshold Height: " + heightconstraint1.transform.position.y.ToString("0.00") + "m";
                }
                else if (constraintToPass == 2)
                {
                    menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Threshold Height: " + heightconstraint2.transform.position.y.ToString("0.00") + "m";
                }
                menu3A.transform.GetChild(5).gameObject.SetActive(true);
                menu3A.transform.GetChild(6).gameObject.SetActive(true);
            }
            else if (thisObj.name == "UpButton")
            {
                if (constraintToPass == 1)
                {
                    heightconstraint1.transform.Translate(0.0f, 0.05f, 0.0f);
                    menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Threshold Height: " + heightconstraint1.transform.position.y.ToString("0.00") + "m";
                }
                else if (constraintToPass == 2)
                {
                    heightconstraint2.transform.Translate(0.0f, 0.05f, 0.0f);
                    menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Threshold Height: " + heightconstraint2.transform.position.y.ToString("0.00") + "m";
                }
            }
            else if (thisObj.name == "DownButton")
            {
                if (constraintToPass == 1)
                {
                    heightconstraint1.transform.Translate(0.0f, -0.05f, 0.0f);
                    menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Threshold Height: " + heightconstraint1.transform.position.y.ToString("0.00") + "m";
                }
                else if (constraintToPass == 2)
                {
                    heightconstraint2.transform.Translate(0.0f, -0.05f, 0.0f);
                    menu3A.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Threshold Height: " + heightconstraint2.transform.position.y.ToString("0.00") + "m";
                }
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
                menu3B.transform.GetChild(7).gameObject.SetActive(false);
                menu3B.transform.GetChild(8).gameObject.SetActive(false);
                if (constraintToPass == 3)
                {
                    orientationconstraint1.SetActive(false);
                }
                else if (constraintToPass == 4)
                {
                    orientationconstraint2.SetActive(false);
                }
                orientationconstraintholder.transform.GetChild(2).gameObject.SetActive(false);
                orientationconstraintholder.transform.GetChild(3).gameObject.SetActive(false);
                orientationconstraintholder.transform.GetChild(4).gameObject.SetActive(false);
                menu3B.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "ConfirmButton")
            {
                menu3B.transform.GetChild(7).gameObject.SetActive(false);
                menu3B.transform.GetChild(8).gameObject.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                if (constraintToPass == 3)
                {
                    Vector3 angles = orientationconstraint1.transform.rotation.eulerAngles;
                    orientationconstraint1.SetActive(false);
                    updateConstraintOrientation(constraintToPass, angles);
                }
                else if (constraintToPass == 4)
                {
                    Vector3 angles = orientationconstraint2.transform.rotation.eulerAngles;
                    orientationconstraint2.SetActive(false);
                    updateConstraintOrientation(constraintToPass, angles);
                }
                orientationconstraintholder.transform.GetChild(2).gameObject.SetActive(false);
                orientationconstraintholder.transform.GetChild(3).gameObject.SetActive(false);
                orientationconstraintholder.transform.GetChild(4).gameObject.SetActive(false);
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (GameObject ball in objs)
                {
                    ball.GetComponent<Renderer>().enabled = true;
                }
                menu3B.SetActive(false);
                menu1.SetActive(true);
            }
            else if (thisObj.name == "RollButton")
            {
                lastControl = 1;
                if (constraintToPass == 3)
                {
                    menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Z: " + orientationconstraint1.transform.rotation.eulerAngles.z.ToString("0") + " deg";
                }
                else if (constraintToPass == 4)
                {
                    menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Z: " + orientationconstraint2.transform.rotation.eulerAngles.z.ToString("0") + " deg";
                }
                menu3B.transform.GetChild(7).gameObject.SetActive(true);
                menu3B.transform.GetChild(8).gameObject.SetActive(true);
            }
            else if (thisObj.name == "PitchButton")
            {
                lastControl = 2;
                if (constraintToPass == 3)
                {
                    menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about X: " + orientationconstraint1.transform.rotation.eulerAngles.x.ToString("0") + " deg";
                }
                else if (constraintToPass == 4)
                {
                    menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about X: " + orientationconstraint2.transform.rotation.eulerAngles.x.ToString("0") + " deg";
                }
                menu3B.transform.GetChild(7).gameObject.SetActive(true);
                menu3B.transform.GetChild(8).gameObject.SetActive(true);
            }
            else if (thisObj.name == "YawButton")
            {
                lastControl = 3;
                if (constraintToPass == 3)
                {
                    menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Y: " + orientationconstraint1.transform.rotation.eulerAngles.y.ToString("0") + " deg";
                }
                else if (constraintToPass == 4)
                {
                    menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Y: " + orientationconstraint2.transform.rotation.eulerAngles.y.ToString("0") + " deg";
                }
                menu3B.transform.GetChild(7).gameObject.SetActive(true);
                menu3B.transform.GetChild(8).gameObject.SetActive(true);
            }
            else if (thisObj.name == "AffordanceButton")
            {
                menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Affordance Angle: 15 deg (Non-functional)";
            }
            else if (thisObj.name == "UpButton")
            {
                if (constraintToPass == 3)
                {
                    if (lastControl == 1)
                    {
                        orientationconstraint1.transform.Rotate(0.0f, 0.0f, 5.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Z: " + orientationconstraint1.transform.rotation.eulerAngles.z.ToString("0") + " deg";
                    }
                    else if (lastControl == 2)
                    {
                        orientationconstraint1.transform.Rotate(5.0f, 0.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about X: " + orientationconstraint1.transform.rotation.eulerAngles.x.ToString("0") + " deg";
                    }
                    else if (lastControl == 3)
                    {
                        orientationconstraint1.transform.Rotate(0.0f, 5.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Y: " + orientationconstraint1.transform.rotation.eulerAngles.y.ToString("0") + " deg";
                    }
                }
                else if (constraintToPass == 4)
                {
                    if (lastControl == 1)
                    {
                        orientationconstraint2.transform.Rotate(0.0f, 0.0f, 5.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Z: " + orientationconstraint2.transform.rotation.eulerAngles.z.ToString("0") + " deg";
                    }
                    else if (lastControl == 2)
                    {
                        orientationconstraint2.transform.Rotate(5.0f, 0.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about X: " + orientationconstraint2.transform.rotation.eulerAngles.x.ToString("0") + " deg";
                    }
                    else if (lastControl == 3)
                    {
                        orientationconstraint2.transform.Rotate(0.0f, 5.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Y: " + orientationconstraint2.transform.rotation.eulerAngles.y.ToString("0") + " deg";
                    }
                }
            }
            else if (thisObj.name == "DownButton")
            {
                if (constraintToPass == 3)
                {
                    if (lastControl == 1)
                    {
                        orientationconstraint1.transform.Rotate(0.0f, 0.0f, -5.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Z: " + orientationconstraint1.transform.rotation.eulerAngles.z.ToString("0") + " deg";
                    }
                    else if (lastControl == 2)
                    {
                        orientationconstraint1.transform.Rotate(-5.0f, 0.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about X: " + orientationconstraint1.transform.rotation.eulerAngles.x.ToString("0") + " deg";
                    }
                    else if (lastControl == 3)
                    {
                        orientationconstraint1.transform.Rotate(0.0f, -5.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Y: " + orientationconstraint1.transform.rotation.eulerAngles.y.ToString("0") + " deg";
                    }
                }
                else if (constraintToPass == 4)
                {
                    if (lastControl == 1)
                    {
                        orientationconstraint2.transform.Rotate(0.0f, 0.0f, -5.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Z: " + orientationconstraint2.transform.rotation.eulerAngles.z.ToString("0") + " deg";
                    }
                    else if (lastControl == 2)
                    {
                        orientationconstraint2.transform.Rotate(-5.0f, 0.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about X: " + orientationconstraint2.transform.rotation.eulerAngles.x.ToString("0") + " deg";
                    }
                    else if (lastControl == 3)
                    {
                        orientationconstraint2.transform.Rotate(0.0f, -5.0f, 0.0f, Space.World);
                        menu3B.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Rotation about Y: " + orientationconstraint2.transform.rotation.eulerAngles.y.ToString("0") + " deg";
                    }
                }
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
                menu3C.transform.GetChild(7).gameObject.SetActive(false);
                menu3C.transform.GetChild(8).gameObject.SetActive(false);
                if (constraintToPass == 5)
                {
                    overunderconstraint1.SetActive(false);
                }
                else if (constraintToPass == 6)
                {
                    overunderconstraint2.SetActive(false);
                }
                menu3C.SetActive(false);
                menu2.SetActive(true);
            }
            else if (thisObj.name == "ConfirmButton")
            {
                //print("Here's the info:");
                menu3C.transform.GetChild(7).gameObject.SetActive(false);
                menu3C.transform.GetChild(8).gameObject.SetActive(false);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "ARC-LfD v1.0";
                if (constraintToPass == 5)
                {
                    Vector3 position = overunderconstraint1.transform.position;
                    float radius = (float)(overunderconstraint1.transform.localScale.x / 2.0);
                    overunderconstraint1.SetActive(false);
                    updateConstraintOverUnder(constraintToPass, position, radius);
                }
                else if (constraintToPass == 6)
                {
                    Vector3 position = overunderconstraint2.transform.position;
                    float radius = (float)(overunderconstraint2.transform.localScale.x / 2.0);
                    overunderconstraint2.SetActive(false);
                    updateConstraintOverUnder(constraintToPass, position, radius);
                }
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (GameObject ball in objs)
                {
                    ball.GetComponent<Renderer>().enabled = true;
                }
                menu3C.SetActive(false);
                menu1.SetActive(true);
            }
            else if (thisObj.name == "XButton")
            {
                lastControl = 1;
                if (constraintToPass == 5)
                {
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "X Position: " + overunderconstraint1.transform.position.x.ToString("0.00") + "m";
                }
                else if (constraintToPass == 6)
                {
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "X Position: " + overunderconstraint2.transform.position.x.ToString("0.00") + "m";
                }
                menu3C.transform.GetChild(7).gameObject.SetActive(true);
                menu3C.transform.GetChild(8).gameObject.SetActive(true);
            }
            else if (thisObj.name == "YButton")
            {
                lastControl = 2;
                if (constraintToPass == 5)
                {
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Y Position: " + overunderconstraint1.transform.position.y.ToString("0.00") + "m";
                }
                else if (constraintToPass == 6)
                {
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Y Position: " + overunderconstraint2.transform.position.y.ToString("0.00") + "m";
                }
                menu3C.transform.GetChild(7).gameObject.SetActive(true);
                menu3C.transform.GetChild(8).gameObject.SetActive(true);
            }
            else if (thisObj.name == "ZButton")
            {
                lastControl = 3;
                if (constraintToPass == 5)
                {
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Z Position: " + overunderconstraint1.transform.position.z.ToString("0.00") + "m";
                }
                else if (constraintToPass == 6)
                {
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Z Position: " + overunderconstraint2.transform.position.z.ToString("0.00") + "m";
                }
                menu3C.transform.GetChild(7).gameObject.SetActive(true);
                menu3C.transform.GetChild(8).gameObject.SetActive(true);
            }
            else if (thisObj.name == "RadiusButton")
            {
                lastControl = 4;
                if (constraintToPass == 5)
                {
                    string radius = (overunderconstraint1.transform.localScale.x * 50.0).ToString("0.0");
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Radius: " + radius + "cm";
                }
                else if (constraintToPass == 6)
                {
                    string radius = (overunderconstraint2.transform.localScale.x * 50.0).ToString("0.0");
                    menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Radius: " + radius + "cm";
                }
                menu3C.transform.GetChild(7).gameObject.SetActive(true);
                menu3C.transform.GetChild(8).gameObject.SetActive(true);
            }
            else if (thisObj.name == "UpButton")
            {
                if (constraintToPass == 5)
                {
                    if (lastControl == 1)
                    {
                        overunderconstraint1.transform.Translate(0.05f, 0.0f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "X Position: " + overunderconstraint1.transform.position.x.ToString("0.00") + "m";
                    }
                    else if (lastControl == 2)
                    {
                        overunderconstraint1.transform.Translate(0.0f, 0.05f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Y Position: " + overunderconstraint1.transform.position.y.ToString("0.00") + "m";
                    }
                    else if (lastControl == 3)
                    {
                        overunderconstraint1.transform.Translate(0.0f, 0.0f, 0.05f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Z Position: " + overunderconstraint1.transform.position.z.ToString("0.00") + "m";
                    }
                    else if (lastControl == 4)
                    {
                        Vector3 scalechange = new Vector3(0.01f, 0.0f, 0.01f);
                        overunderconstraint1.transform.localScale += scalechange;
                        string radius = (overunderconstraint1.transform.localScale.x * 50.0).ToString("0.0");
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Radius: " + radius + "cm";
                    }
                }
                else if (constraintToPass == 6)
                {
                    if (lastControl == 1)
                    {
                        overunderconstraint2.transform.Translate(0.05f, 0.0f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "X Position: " + overunderconstraint2.transform.position.x.ToString("0.00") + "m";
                    }
                    else if (lastControl == 2)
                    {
                        overunderconstraint2.transform.Translate(0.0f, 0.05f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Y Position: " + overunderconstraint2.transform.position.y.ToString("0.00") + "m";
                    }
                    else if (lastControl == 3)
                    {
                        overunderconstraint2.transform.Translate(0.0f, 0.0f, 0.05f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Z Position: " + overunderconstraint2.transform.position.z.ToString("0.00") + "m";
                    }
                    else if (lastControl == 4)
                    {
                        Vector3 scalechange = new Vector3(0.01f, 0.0f, 0.01f);
                        overunderconstraint2.transform.localScale += scalechange;
                        string radius = (overunderconstraint2.transform.localScale.x * 50.0).ToString("0.0");
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Radius: " + radius + "cm";
                    }
                }
            }
            else if (thisObj.name == "DownButton")
            {
                if (constraintToPass == 5)
                {
                    if (lastControl == 1)
                    {
                        overunderconstraint1.transform.Translate(-0.05f, 0.0f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "X Position: " + overunderconstraint1.transform.position.x.ToString("0.00") + "m";
                    }
                    else if (lastControl == 2)
                    {
                        overunderconstraint1.transform.Translate(0.0f, -0.05f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Y Position: " + overunderconstraint1.transform.position.y.ToString("0.00") + "m";
                    }
                    else if (lastControl == 3)
                    {
                        overunderconstraint1.transform.Translate(0.0f, 0.0f, -0.05f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Z Position: " + overunderconstraint1.transform.position.z.ToString("0.00") + "m";
                    }
                    else if (lastControl == 4)
                    {
                        Vector3 scalechange = new Vector3(-0.01f, 0.0f, -0.01f);
                        overunderconstraint1.transform.localScale += scalechange;
                        string radius = (overunderconstraint1.transform.localScale.x * 50.0).ToString("0.0");
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Radius: " + radius + "cm";
                    }
                }
                else if (constraintToPass == 6)
                {
                    if (lastControl == 1)
                    {
                        overunderconstraint2.transform.Translate(-0.05f, 0.0f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "X Position: " + overunderconstraint2.transform.position.x.ToString("0.00") + "m";
                    }
                    else if (lastControl == 2)
                    {
                        overunderconstraint2.transform.Translate(0.0f, -0.05f, 0.0f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Y Position: " + overunderconstraint2.transform.position.y.ToString("0.00") + "m";
                    }
                    else if (lastControl == 3)
                    {
                        overunderconstraint2.transform.Translate(0.0f, 0.0f, -0.05f);
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Z Position: " + overunderconstraint2.transform.position.z.ToString("0.00") + "m";
                    }
                    else if (lastControl == 4)
                    {
                        Vector3 scalechange = new Vector3(-0.01f, 0.0f, -0.01f);
                        overunderconstraint2.transform.localScale += scalechange;
                        string radius = (overunderconstraint2.transform.localScale.x * 50.0).ToString("0.0");
                        menu3C.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "Radius: " + radius + "cm";
                    }
                }
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

    // Update the internal representations of the user-reparametrized orientation constraint
    void updateConstraintOrientation(int constraintID, Vector3 angles)
    {
        print(constraintID);
        print(angles);
        Dictionary<string, VisualConstraint> constraintsDict = DynamicPoints.DynamicTrajectoryReader.constraintsDict;
        Dictionary<string, MonoBehaviour> drawnObjectsDict = DynamicPoints.DynamicTrajectoryReader.drawnObjectsDict;
        foreach (string constraintkey in constraintsDict.Keys)
        {
            if (constraintkey == constraintID.ToString())
            {
                print(constraintkey);
                UprightConstraintPrefab upright = (UprightConstraintPrefab)constraintsDict[constraintkey];
                print(upright.thresholdAngle);
                print(upright.referenceAngle);
                upright.referenceAngle = angles;
                print(upright.thresholdAngle);
                print(upright.referenceAngle);
                drawnObjectsDict.Remove("CONSTRAINT_" + constraintkey);
                drawnObjectsDict.Add("CONSTRAINT_" + constraintkey, upright);
            }
        }
    }

    // Update the internal representations of the user-reparametrized over/under constraint
    void updateConstraintOverUnder(int constraintID, Vector3 position, float radius)
    {
        print(constraintID);
        print(position);
        print(radius);
        Dictionary<string, VisualConstraint> constraintsDict = DynamicPoints.DynamicTrajectoryReader.constraintsDict;
        Dictionary<string, MonoBehaviour> drawnObjectsDict = DynamicPoints.DynamicTrajectoryReader.drawnObjectsDict;
        foreach (string constraintkey in constraintsDict.Keys)
        {
            if (constraintkey == constraintID.ToString())
            {
                print(constraintkey);
                OverUnderConstraintPrefab overunder = (OverUnderConstraintPrefab)constraintsDict[constraintkey];
                //print(overunder.constraintPosition.x);
                //print(overunder.constraintPosition.y);
                //print(overunder.constraintPosition.z);
                //print(overunder.thresholdDistance);
                overunder.constraintPosition = position;
                overunder.thresholdDistance = radius;
                //print(overunder.constraintPosition.x);
                //print(overunder.constraintPosition.y);
                //print(overunder.constraintPosition.z);
                //print(overunder.thresholdDistance);
                drawnObjectsDict.Remove("CONSTRAINT_" + constraintkey);
                drawnObjectsDict.Add("CONSTRAINT_" + constraintkey, overunder);
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
