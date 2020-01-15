using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MenuClicker : MonoBehaviour, IInputClickHandler {

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
        GameObject menu1 = menu1holder.transform.GetChild(0).gameObject;
        GameObject menu2 = menu2holder.transform.GetChild(0).gameObject;
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
        else if (menu2.activeInHierarchy)
        {
            if (thisObj.name == "BackButton")
            {
                menu2.SetActive(false);
                menu1.SetActive(true);
                menu1.transform.GetChild(2).gameObject.GetComponent<TextMesh>().text = "Constraint Application";
            }
            
        }
    }
}
