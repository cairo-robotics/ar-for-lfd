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
        public float gripper;
        public int id;
        public float[] joints;
        public float[] orientation;
        public float[] position;
    }

    [System.Serializable]
    public class TrajectoryPoint
    {
        public int[] applied_constraints;
        public int[] items;
        public int keyframe_id;
        public string keyframe_type;
        public RobotState robot;
        public float time;
        public int[] triggered_constraints;
    }

    [System.Serializable]
    class Constraint
    {
        public int id;
        public string className;
        public float[] args;
    }

    class HeightConstraint : Constraint
    {
        public float referenceHeight;
        public float thresholdDistance;
    }

    class UprightConstraint : Constraint
    {
        public float[] referenceAngle;
        public float thresholdAngle;
    }

    [System.Serializable]
    class ConstraintArray
    {
        public Constraint[] constraints;
    }

    public class DynamicTrajectoryReader : MonoBehaviour
    {
        public StatePrefab pointPrefab;
        public HeightConstraintPrefab heightPrefab;
        public UprightConstraintPrefab uprightPrefab;
        private Dictionary<string, TrajectoryPoint> pointsDict;
        private Dictionary<string, VisualConstraint> constraintsDict;
        private Dictionary<string, MonoBehaviour> drawnObjectsDict;
        public Vector3 robotTransform;

        // Use this for initialization
        void Start()
        {
            this.pointsDict = new Dictionary<string, TrajectoryPoint>();
            this.constraintsDict = new Dictionary<string, VisualConstraint>();
            this.drawnObjectsDict = new Dictionary<string, MonoBehaviour>();
        }

        //takes a constraint object and adds it to relevant data structures
        public void AddConstraint(string constraintJSON)
        {
            Constraint constraint = this.CastConstraint(JsonUtility.FromJson<Constraint>(constraintJSON));
            this.InstantiateConstraint(constraint);
        }

        public void DeleteConstraint(string constraintID)
        {
            //delete from all trajectory points referencing this constraint
            foreach (KeyValuePair<string, TrajectoryPoint> pointPair in pointsDict)
            {
                if (Array.IndexOf(pointPair.Value.applied_constraints, constraintID) != -1)
                {
                    ((StatePrefab)drawnObjectsDict["POINT_" + pointPair.Key]).constraints.Remove(constraintsDict[constraintID]);
                }
            }

            //delete from list of constraints
            if (constraintsDict.ContainsKey(constraintID))
            {
                constraintsDict.Remove(constraintID);
            }

            //delete drawn object
            if (drawnObjectsDict.ContainsKey("CONSTRAINT_" + constraintID))
            {
                Destroy(drawnObjectsDict["CONSTRAINT_" + constraintID]);
                drawnObjectsDict.Remove("CONSTRAINT_" + constraintID);
            }

        }

        //takes a TrajectoryPoint object and adds it to relevant data structures
        // TODO: create callback for ROS topic that builds constraint from message and calls this function
        public void AddPoint(string pointJSON)
        {
            TrajectoryPoint point = JsonUtility.FromJson<TrajectoryPoint>(pointJSON);
            //first make sure there is no existing TrajectoryPoint for this keyframe. If so, throw error.
            if (pointsDict.ContainsKey(point.keyframe_id + ""))
            {
                throw new System.InvalidOperationException("TrajectoryPoint ID already in use! ID must be unique.");
            }

            pointsDict.Add(point.keyframe_id + "", point);
            this.DrawTrajectoryPoint(point);
        }

        //TODO: create callback for ROS topic that gets pointID (keyframe_id) from message and calls this function
        public void DeletePoint(string pointID)
        {
            //delete from list of points
            if (pointsDict.ContainsKey(pointID))
            {
                pointsDict.Remove(pointID);
            }

            //delete drawn object
            if (drawnObjectsDict.ContainsKey("POINT_" + pointID))
            {
                Destroy(drawnObjectsDict["POINT_" + pointID]);
                drawnObjectsDict.Remove("POINT_" + pointID);
            }
        }

        Constraint CastConstraint(Constraint constraint)
        {
            if (constraint.className.Equals("HeightConstraint"))
            {
                HeightConstraint c = new HeightConstraint();
                c.id = constraint.id;
                c.className = constraint.className;
                c.args = constraint.args;
                c.referenceHeight = constraint.args[0];
                c.thresholdDistance = constraint.args[1];
                constraint = c;
            }
            else if (constraint.className.Equals("UprightConstraint"))
            {
                UprightConstraint c = new UprightConstraint();
                c.id = constraint.id;
                c.className = constraint.className;
                c.args = constraint.args;
                c.referenceAngle = new float[] { constraint.args[0], constraint.args[1], constraint.args[2] };
                c.thresholdAngle = constraint.args[3];
                constraint = c;
            }
            return constraint;
        }

        //create a constraint object and add it to constraint dictionary
        void InstantiateConstraint(Constraint constraint)
        {
            //first make sure there is no existing constraint with this constraint id. If so, throw error.
            if (constraintsDict.ContainsKey(constraint.id + ""))
            {
                throw new System.InvalidOperationException("Constraint ID already in use! ID must be unique.");
            }

            if (constraint is HeightConstraint)
            {
                HeightConstraint c = (HeightConstraint)constraint;
                HeightConstraintPrefab heightConstraint = Instantiate<HeightConstraintPrefab>(heightPrefab);
                heightConstraint.referenceHeight = c.referenceHeight;
                heightConstraint.thresholdDistance = c.thresholdDistance;
                heightConstraint.transform.position = new Vector3(-999, -999, 0);
                heightConstraint.GetComponent<MeshRenderer>().enabled = false;
                constraintsDict.Add(c.id + "", heightConstraint);
                drawnObjectsDict.Add("CONSTRAINT_" + c.id, heightConstraint);
            }
            else if (constraint is UprightConstraint)
            {
                UprightConstraint c = (UprightConstraint)constraint;
                UprightConstraintPrefab uprightConstraint = Instantiate<UprightConstraintPrefab>(uprightPrefab);
                uprightConstraint.referenceAngle = new Vector3(c.referenceAngle[0], c.referenceAngle[1], c.referenceAngle[2]);
                uprightConstraint.transform.eulerAngles = uprightConstraint.referenceAngle;
                uprightConstraint.transform.position = new Vector3(-999, -999, 0);
                uprightConstraint.thresholdAngle = c.thresholdAngle;
                uprightConstraint.GetComponent<MeshRenderer>().enabled = false;
                constraintsDict.Add(c.id + "", uprightConstraint);
                drawnObjectsDict.Add("CONSTRAINT_" + c.id, uprightConstraint);
            }
        }

        public void DrawTrajectoryPoint(TrajectoryPoint point)
        {
            StatePrefab drawnPoint = this.DrawSpot(point.robot.position[0], point.robot.position[1], point.robot.position[2], point.robot.orientation, point.applied_constraints, 0); //TODO: leaving order as 0 for now, check that this is okay
            for (int k = 0; k < drawnPoint.constraintsActive.Length; k++)
            {
                drawnPoint.constraints.Add(constraintsDict[drawnPoint.constraintsActive[k] + ""]);
            }
            drawnObjectsDict.Add("POINTS_" + point.keyframe_id, drawnPoint);
        }

        public StatePrefab DrawSpot(float x, float y, float z, float[] orientation, int[] constraints, int order)
        {
            StatePrefab newPoint = Instantiate<StatePrefab>(pointPrefab);
            newPoint.constraintsActive = constraints;
            newPoint.order = order;
            newPoint.transform.position = this.robotTransform + new Vector3(x, z, -1 * y);
            newPoint.transform.eulerAngles = (new Quaternion(orientation[0], orientation[1], orientation[2], orientation[3])).eulerAngles;
            newPoint.transform.eulerAngles = new Vector3(newPoint.transform.eulerAngles.x, newPoint.transform.eulerAngles.z, newPoint.transform.eulerAngles.y);
            newPoint.constraints = new ArrayList();
            newPoint.toggled = false;
            return newPoint;
        }
    }
}