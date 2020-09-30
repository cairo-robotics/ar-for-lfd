using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class NewMenuClicker : MonoBehaviour, IInputClickHandler {

    private static int constraintToPass = -1;
    private static Dictionary<int, List<int>> appliedConstraints = new Dictionary<int, List<int>>();

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
                print("Send");
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
                print("Confirm");
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
                print("Confirm");
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
                print("Confirm");
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
        //  Remove from Trajectory -> update trajectory representation; GOTO Menu 1
        else if (menu5.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu5.SetActive(false);
                menu4.SetActive(true);
            }
            else if (thisObj.name == "ApplyButton")
            {
                menu5.SetActive(false);
                menu6.SetActive(true);
            }
            else if (thisObj.name == "ClearButton")
            {
                print("Clear");
                print(constraintToPass);
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
                menu6.SetActive(false);
                menu5.SetActive(true);
            }
            else if (thisObj.name == "ConfirmButton")
            {
                print("Confirm");
            }
        }
    }
}
