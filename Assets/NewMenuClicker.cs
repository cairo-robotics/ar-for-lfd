using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class NewMenuClicker : MonoBehaviour, IInputClickHandler {

    private static int constraintToPass = -1;
    private static int[] activeConstraints = new int[] { -1, -1, -1, -1, -1, -1 };
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
        //GameObject menu2holder = GameObject.Find("Menu2Holder");
        //GameObject menu3Aholder = GameObject.Find("Menu3AHolder");
        //GameObject menu3Bholder = GameObject.Find("Menu3BHolder");
        //GameObject menu3Cholder = GameObject.Find("Menu3CHolder");
        //GameObject menu4holder = GameObject.Find("Menu4Holder");
        //GameObject menu5holder = GameObject.Find("Menu5Holder");
        //GameObject menu6holder = GameObject.Find("Menu6Holder");
        GameObject menu1 = menu1holder.transform.GetChild(0).gameObject;
        //GameObject menu2 = menu2holder.transform.GetChild(0).gameObject;
        //GameObject menu3A = menu3Aholder.transform.GetChild(0).gameObject;
        //GameObject menu3B = menu3Bholder.transform.GetChild(0).gameObject;
        //GameObject menu3C = menu3Cholder.transform.GetChild(0).gameObject;
        //GameObject menu4 = menu4holder.transform.GetChild(0).gameObject;
        //GameObject menu5 = menu5holder.transform.GetChild(0).gameObject;
        //GameObject menu6 = menu6holder.transform.GetChild(0).gameObject;

        //LEVEL 1 MENU
        //  Edit Constraint Params -> GOTO Menu 2
        //  Apply Constraint -> GOTO Menu 4
        //  Send to Robot -> fire publisher; display confirmation message
        if (menu1.activeInHierarchy)
        {
            if (thisObj.name == "EditButton")
            {
                print("Edit");
            }
            else if (thisObj.name == "ApplyButton")
            {
                print("Apply");
            }
            else if (thisObj.name == "SendButton")
            {
                print("Send");
            }
        }
    }
}
