using HoloToolkit.Unity.InputModule;
using UnityEngine;


public class Clicker2 : MonoBehaviour, IInputClickHandler
{
    int i = 0;
    Color Select = Color.yellow;
    Color Previous;

    private void Start()
    {
        Collider collider = GetComponentInChildren<Collider>();
        if (collider == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
    {
        UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
        int length = objs.Length;

        foreach (UnityEngine.GameObject ball in objs)
        {
            if (ball.GetComponent<StatePrefab>().order == i)
            {
                Previous = ball.GetComponent<MeshRenderer>().material.color;
                ball.GetComponent<MeshRenderer>().material.color = Select;
                ball.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Select;
                Debug.Log(i);
            }
            //init = Time.realtimeSinceStartup;
            //while ((Time.realtimeSinceStartup - init) <= 0.01) { }
            //Debug.Log(i);
            if (Previous != null)
            {
                if (ball.GetComponent<StatePrefab>().order == (i - 1))
                {
                    ball.GetComponent<MeshRenderer>().material.color = Previous;
                    ball.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Previous;
                }
            }
        }
        i += 1;
        if (i == length) { i = 0; }

    }
}