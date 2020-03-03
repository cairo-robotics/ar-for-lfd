/* Creates class for dynamically loading in and deleting trajectory points/constraints
 * Meant to be used within a larger driver program that executes callbacks according to some event trigger (such as incoming ROS messages)
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DynamicPoints
{
    [System.Serializable]
    public class RobotState
    {
        //public float gripper;
        //public int id;
        //public float[] joints;
        public float[] orientation;
        public float[] position;
    }

    [System.Serializable]
    public class TrajectoryPoint
    {
        public int[] applied_constraints;
        //public int[] items;
        public int keyframe_id;
        //public string keyframe_type;
        public RobotState robot;
        //public float time;
        //public int[] triggered_constraints;
    }

    [System.Serializable]
    public class Trajectory
    {
        public TrajectoryPoint[] trajectory;
    }

    [System.Serializable]
    class Constraint
    {
        public int id;
        public string className;
        public float[] args;
    }

    class HeightConstraintBelow : Constraint
    {
        public float referenceHeight;
        public float thresholdDistance;
    }

    class HeightConstraintAbove : Constraint
    {
        public float referenceHeight;
        public float thresholdDistance;
    }

    class UprightConstraint : Constraint
    {
        public float[] referenceAngle;
        public float thresholdAngle;
    }

    class OverUnderConstraint : Constraint
    {
        public float[] constraintPosition;
        public float thresholdDistance;
    }

    [System.Serializable]
    class ConstraintArray
    {
        public Constraint[] constraints;
    }

    public class DynamicTrajectoryReader : MonoBehaviour
    {
        public StatePrefab pointPrefab;
        public HeightConstraintBelowPrefab heightBelowPrefab;
        public HeightConstraintAbovePrefab heightAbovePrefab;
        public UprightConstraintPrefab uprightPrefab;
        public OverUnderConstraintPrefab overunderPrefab;
        public static Dictionary<string, TrajectoryPoint> pointsDict;
        private Dictionary<string, VisualConstraint> constraintsDict;
        private Dictionary<string, MonoBehaviour> drawnObjectsDict;
        public Vector3 robotTransform;
        public TextAsset constraintFile;

        // Use this for initialization
        void Start()
        {
            pointsDict = new Dictionary<string, TrajectoryPoint>();
            Constraint[] constraints = JsonUtility.FromJson<ConstraintArray>(constraintFile.text).constraints;
            CastConstraints(constraints);
            this.constraintsDict = new Dictionary<string, VisualConstraint>();
            this.drawnObjectsDict = new Dictionary<string, MonoBehaviour>();
            InstantiateConstraints(constraints);
        }

        public void ManageConstraint(string message)
        {
            print(message);
            //Override message to clear all constraints (to rebuild model)
            if (String.Equals(message,"CLEAR"))
            {
                print("HERE");
                List<string> pointkeyList = new List<string>();
                // Use a separate list to prevent out-of-sync errors
                foreach (string pointkey in pointsDict.Keys)
                {
                    pointkeyList.Add(pointkey);
                }
                foreach (string pointkey in pointkeyList)
                {
                    pointsDict.Remove(pointkey);
                    drawnObjectsDict.Remove("POINT_" + pointkey);
                }
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                foreach (UnityEngine.GameObject ball in objs)
                {
                    Destroy(ball);
                }
            }
            //Message is JSON-like, represents a keyframe to be rendered
            else
            {
                Trajectory points = JsonUtility.FromJson<Trajectory>(message);
                //first make sure there is no existing TrajectoryPoint for this keyframe. If so, throw error.
                foreach (TrajectoryPoint point in points.trajectory){
                    if (pointsDict.ContainsKey(point.keyframe_id + ""))
                    {
                        throw new System.InvalidOperationException("TrajectoryPoint ID already in use! ID must be unique.");
                    }

                    pointsDict.Add(point.keyframe_id + "", point);
                    this.DrawTrajectoryPoint(point);
                }
                UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
                int max_kfid = 0;
                foreach (UnityEngine.GameObject ball in objs)
                {
                    if (ball.GetComponent<StatePrefab>().order > max_kfid)
                    {
                        max_kfid = ball.GetComponent<StatePrefab>().order;
                    }
                }
                foreach(UnityEngine.GameObject ball in objs)
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
        }

        //takes a constraint object and adds it to relevant data structures
        //public void AddConstraint(string constraintJSON)
        //{
        //    Constraint constraint = this.CastConstraint(JsonUtility.FromJson<Constraint>(constraintJSON));
        //    this.InstantiateConstraint(constraint);

        //    //go through and add drawn object to all existing trajectory points
        //    foreach (KeyValuePair<string, TrajectoryPoint> pointPair in pointsDict)
        //    {
        //        if (Array.IndexOf(pointPair.Value.applied_constraints, constraint.id) != -1)
        //        {
        //            ((StatePrefab)drawnObjectsDict["POINT_" + pointPair.Key]).constraints.Add(drawnObjectsDict["CONSTRAINT_"+constraint.id]);
        //        }
        //    }
        //}

        //public void DeleteConstraint(string constraintID)
        //{
        //    //delete from all trajectory points referencing this constraint
        //    foreach (KeyValuePair<string, TrajectoryPoint> pointPair in pointsDict)
        //    {
        //        print(pointPair.Value.applied_constraints);
        //        if (Array.IndexOf(pointPair.Value.applied_constraints, Int32.Parse(constraintID)) != -1)
        //        {
        //            ((StatePrefab)drawnObjectsDict["POINT_" + pointPair.Key]).constraints.Remove((VisualConstraint)drawnObjectsDict["CONSTRAINT_" + constraintID]);
        //        }
        //    }

        //    //delete from list of constraints
        //    if (constraintsDict.ContainsKey(constraintID))
        //    {
        //        constraintsDict.Remove(constraintID);
        //    }

        //    //delete drawn object
        //    if (drawnObjectsDict.ContainsKey("CONSTRAINT_" + constraintID))
        //    {
        //        VisualConstraint obj = (VisualConstraint)drawnObjectsDict["CONSTRAINT_" + constraintID];
        //        drawnObjectsDict.Remove("CONSTRAINT_" + constraintID);
        //        Destroy(obj.gameObject);
        //    }

        //}

        ////takes a TrajectoryPoint object and adds it to relevant data structures
        //// TODO: create callback for ROS topic that builds constraint from message and calls this function
        //public void AddPoint(string pointJSON)
        //{
        //    TrajectoryPoint point = JsonUtility.FromJson<TrajectoryPoint>(pointJSON);
        //    //first make sure there is no existing TrajectoryPoint for this keyframe. If so, throw error.
        //    if (pointsDict.ContainsKey(point.keyframe_id + ""))
        //    {
        //        throw new System.InvalidOperationException("TrajectoryPoint ID already in use! ID must be unique.");
        //    }

        //    pointsDict.Add(point.keyframe_id + "", point);
        //    this.DrawTrajectoryPoint(point);
        //}

        ////TODO: create callback for ROS topic that gets pointID (keyframe_id) from message and calls this function
        //public void DeletePoint(string pointID)
        //{
        //    if (pointID == "CLEAR")
        //    {
        //        foreach (string pointkey in pointsDict.Keys)
        //        {
        //            pointsDict.Remove(pointkey);
        //            drawnObjectsDict.Remove("POINT_" + pointkey);
        //        }
        //    }

        //    UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
        //    foreach (UnityEngine.GameObject ball in objs)
        //    {
        //        Destroy(ball);
        //    }
        //        //delete from list of points



        //        //if (pointsDict.ContainsKey(pointID))
        //        //{
        //        //    pointsDict.Remove(pointID);
        //        //}

        //        //delete drawn object
        //        //if (drawnObjectsDict.ContainsKey("POINT_" + pointID))
        //        //{
        //        //    StatePrefab obj = (StatePrefab)drawnObjectsDict["POINT_" + pointID];
        //        //    drawnObjectsDict.Remove("POINT_" + pointID);
        //        //    Destroy(obj.gameObject);          
        //        //}
        //    }

        void CastConstraints(Constraint[] constraints)
        {
            for (int i = 0; i < constraints.Length; i++)
            {
                if (constraints[i].className.Equals("HeightConstraintBelow"))
                {
                    HeightConstraintBelow c = new HeightConstraintBelow();
                    c.id = constraints[i].id;
                    c.className = constraints[i].className;
                    c.args = constraints[i].args;
                    c.referenceHeight = constraints[i].args[0];
                    c.thresholdDistance = constraints[i].args[1];
                    constraints[i] = c;
                }
                else if (constraints[i].className.Equals("HeightConstraintAbove"))
                {
                    HeightConstraintAbove c = new HeightConstraintAbove();
                    c.id = constraints[i].id;
                    c.className = constraints[i].className;
                    c.args = constraints[i].args;
                    c.referenceHeight = constraints[i].args[0];
                    c.thresholdDistance = constraints[i].args[1];
                    constraints[i] = c;
                }
                else if (constraints[i].className.Equals("UprightConstraint"))
                {
                    UprightConstraint c = new UprightConstraint();
                    c.id = constraints[i].id;
                    c.className = constraints[i].className;
                    c.args = constraints[i].args;
                    c.referenceAngle = new float[] { constraints[i].args[0], constraints[i].args[1], constraints[i].args[2], constraints[i].args[3] };
                    c.thresholdAngle = constraints[i].args[4];
                    constraints[i] = c;
                }
                else if (constraints[i].className.Equals("OverUnderConstraint"))
                {
                    OverUnderConstraint c = new OverUnderConstraint();
                    c.id = constraints[i].id;
                    c.className = constraints[i].className;
                    c.args = constraints[i].args;
                    c.constraintPosition = new float[] { constraints[i].args[0], constraints[i].args[1] + 0.5f, constraints[i].args[2] };
                    c.thresholdDistance = constraints[i].args[3];
                    constraints[i] = c;
                }
            }
        }

        //create constraint objects and store them for future visualization
        void InstantiateConstraints(Constraint[] constraints)
        {
            constraintsDict = new Dictionary<string, VisualConstraint>();
            for (int i = 0; i < constraints.Length; i++)
            {
                if (constraints[i] is HeightConstraintBelow)
                {
                    HeightConstraintBelow c = (HeightConstraintBelow)constraints[i];
                    HeightConstraintBelowPrefab heightBelowConstraint = Instantiate<HeightConstraintBelowPrefab>(heightBelowPrefab);
                    heightBelowConstraint.referenceHeight = c.referenceHeight;
                    heightBelowConstraint.thresholdDistance = c.thresholdDistance;
                    heightBelowConstraint.transform.position = new Vector3(-999, -999, 0);
                    heightBelowConstraint.GetComponent<MeshRenderer>().enabled = false;
                    constraintsDict.Add(c.id + "", heightBelowConstraint);
                    this.drawnObjectsDict["CONSTRAINT_" + c.id] = heightBelowConstraint;
                }
                else if (constraints[i] is HeightConstraintAbove)
                {
                    HeightConstraintAbove c = (HeightConstraintAbove)constraints[i];
                    HeightConstraintAbovePrefab heightAboveConstraint = Instantiate<HeightConstraintAbovePrefab>(heightAbovePrefab);
                    heightAboveConstraint.referenceHeight = c.referenceHeight;
                    heightAboveConstraint.thresholdDistance = c.thresholdDistance;
                    heightAboveConstraint.transform.position = new Vector3(-999, -999, 0);
                    heightAboveConstraint.GetComponent<MeshRenderer>().enabled = false;
                    constraintsDict.Add(c.id + "", heightAboveConstraint);
                    this.drawnObjectsDict["CONSTRAINT_" + c.id] = heightAboveConstraint;
                }
                else if (constraints[i] is UprightConstraint)
                {
                    UprightConstraint c = (UprightConstraint)constraints[i];
                    UprightConstraintPrefab uprightConstraint = Instantiate<UprightConstraintPrefab>(uprightPrefab);
                    Quaternion refAngle = new Quaternion(c.referenceAngle[0], c.referenceAngle[1], c.referenceAngle[2], c.referenceAngle[3]);
                    uprightConstraint.referenceAngle = refAngle.eulerAngles;
                    uprightConstraint.transform.eulerAngles = uprightConstraint.referenceAngle;
                    uprightConstraint.transform.position = new Vector3(-999, -999, 0);
                    uprightConstraint.thresholdAngle = c.thresholdAngle;
                    uprightConstraint.GetComponent<MeshRenderer>().enabled = false;
                    constraintsDict.Add(c.id + "", uprightConstraint);
                    this.drawnObjectsDict["CONSTRAINT_" + c.id] = uprightConstraint;
                }
                else if (constraints[i] is OverUnderConstraint)
                {
                    OverUnderConstraint c = (OverUnderConstraint)constraints[i];
                    OverUnderConstraintPrefab overUnderConstraint = Instantiate<OverUnderConstraintPrefab>(overunderPrefab);
                    overUnderConstraint.constraintPosition = new Vector3(c.constraintPosition[0], c.constraintPosition[1], c.constraintPosition[2]);
                    overUnderConstraint.thresholdDistance = c.thresholdDistance;
                    overUnderConstraint.transform.position = new Vector3(-999, -999, 0);
                    overUnderConstraint.GetComponent<MeshRenderer>().enabled = false;
                    constraintsDict.Add(c.id + "", overUnderConstraint);
                    this.drawnObjectsDict["CONSTRAINT_" + c.id] = overUnderConstraint;
                }
            }
        }

        public void DrawTrajectoryPoint(TrajectoryPoint point)
        {
            StatePrefab drawnPoint = this.DrawSpot(point.robot.position[0], point.robot.position[1], point.robot.position[2], point.robot.orientation, point.applied_constraints, point.keyframe_id);
            for (int k = 0; k < drawnPoint.constraintsActive.Length; k++)
            {
                if(drawnObjectsDict.ContainsKey("CONSTRAINT_"+drawnPoint.constraintsActive[k] + ""))
                {
                    drawnPoint.constraints.Add(drawnObjectsDict["CONSTRAINT_"+drawnPoint.constraintsActive[k] + ""]);
                }
            }
            drawnObjectsDict.Add("POINT_" + point.keyframe_id, drawnPoint);
        }

        // For revizualization - point already exists, but has new constraints
        public void DrawTrajectoryPointNoPoint(TrajectoryPoint point)
        {
            int kfid = point.keyframe_id;
            // Remove earlier keyframe to prevent duplication
            pointsDict.Remove(kfid.ToString());
            drawnObjectsDict.Remove("POINT_" + kfid);
            UnityEngine.GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
            foreach (UnityEngine.GameObject ball in objs)
            {
                // Destroy visualization of keyframe to be updated
                if(ball.GetComponent<StatePrefab>().order == kfid)
                {
                    Destroy(ball);
                }
            }
            // Make new prefab with new point
            StatePrefab drawnPoint = this.DrawSpot(point.robot.position[0], point.robot.position[1], point.robot.position[2], point.robot.orientation, point.applied_constraints, point.keyframe_id);
            // Add constraints
            for (int k = 0; k < drawnPoint.constraintsActive.Length; k++)
            {
                if (drawnObjectsDict.ContainsKey("CONSTRAINT_" + drawnPoint.constraintsActive[k] + ""))
                {
                    drawnPoint.constraints.Add(drawnObjectsDict["CONSTRAINT_" + drawnPoint.constraintsActive[k] + ""]);
                }
            }
            // Add point back to the dictionary
            pointsDict.Add(kfid.ToString(), point);
            drawnObjectsDict.Add("POINT_" + point.keyframe_id, drawnPoint);
        }

        public StatePrefab DrawSpot(float x, float y, float z, float[] orientation, int[] constraints, int order)
        {
            StatePrefab newPoint = Instantiate<StatePrefab>(pointPrefab);
            newPoint.constraintsActive = new int[constraints.Length];
            Array.Copy(constraints, newPoint.constraintsActive, constraints.Length);
            newPoint.order = order;
            newPoint.transform.position = new Vector3(x, y, z);
            newPoint.transform.eulerAngles = (new Quaternion(orientation[0], orientation[1], orientation[2], orientation[3])).eulerAngles;
            newPoint.constraints = new ArrayList();
            newPoint.toggled = false;
            return newPoint;
        }
    }
}