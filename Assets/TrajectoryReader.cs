using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
class TrajectoryPointArray
{
    public TrajectoryPoint[] point_array;
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

public class TrajectoryReader : MonoBehaviour {
    public StatePrefab pointPrefab;
    public HeightConstraintPrefab heightPrefab;
    public UprightConstraintPrefab uprightPrefab;
    private TrajectoryPoint[] points;
    private Dictionary<string, VisualConstraint> constraintsDict;
    public Vector3 robotTransform;
    public TextAsset trajectoryFile;
    public TextAsset constraintFile;

    // Use this for initialization
    void Start() {
        TrajectoryPointArray points_array = JsonUtility.FromJson<TrajectoryPointArray>(trajectoryFile.text);
        this.points = points_array.point_array;
        Constraint[] constraints = JsonUtility.FromJson<ConstraintArray>(constraintFile.text).constraints;
        CastConstraints(constraints);
        InstantiateConstraints(constraints);
        this.DrawTrajectory();
	}

    void CastConstraints(Constraint[] constraints)
    {
        for(int i = 0; i < constraints.Length; i++)
        {
            if (constraints[i].className.Equals("HeightConstraint"))
            {
                HeightConstraint c = new HeightConstraint();
                c.id = constraints[i].id;
                c.className = constraints[i].className;
                c.args = constraints[i].args;
                c.referenceHeight = constraints[i].args[0];
                c.thresholdDistance = constraints[i].args[1];
                constraints[i] = c;
            } else if (constraints[i].className.Equals("UprightConstraint"))
            {
                UprightConstraint c = new UprightConstraint();
                c.id = constraints[i].id;
                c.className = constraints[i].className;
                c.args = constraints[i].args;
                c.referenceAngle = new float[]{ constraints[i].args[0], constraints[i].args[1], constraints[i].args[2] };
                c.thresholdAngle = constraints[i].args[3];
                constraints[i] = c;
            }
        }
    }

    //create constraint objects and store them for future visualization
    void InstantiateConstraints(Constraint[] constraints)
    {
        constraintsDict = new Dictionary<string, VisualConstraint>();
        for(int i = 0; i < constraints.Length; i++)
        {
            if(constraints[i] is HeightConstraint)
            {
                HeightConstraint c = (HeightConstraint)constraints[i];
                HeightConstraintPrefab heightConstraint = Instantiate<HeightConstraintPrefab>(heightPrefab);
                heightConstraint.referenceHeight = c.referenceHeight;
                heightConstraint.thresholdDistance = c.thresholdDistance;
                heightConstraint.transform.position = new Vector3(-999, -999, 0);
                heightConstraint.GetComponent<MeshRenderer>().enabled = false;
                constraintsDict.Add(c.id + "", heightConstraint);
            } else if (constraints[i] is UprightConstraint)
            {
                UprightConstraint c = (UprightConstraint)constraints[i];
                UprightConstraintPrefab uprightConstraint = Instantiate<UprightConstraintPrefab>(uprightPrefab);
                uprightConstraint.referenceAngle = new Vector3(c.referenceAngle[0], c.referenceAngle[1], c.referenceAngle[2]);
                uprightConstraint.transform.eulerAngles = uprightConstraint.referenceAngle;
                uprightConstraint.transform.position = new Vector3(-999, -999, 0);
                uprightConstraint.thresholdAngle = c.thresholdAngle;
                uprightConstraint.GetComponent<MeshRenderer>().enabled = false;
                constraintsDict.Add(c.id + "", uprightConstraint);
            }
        }
    }
	
    public void DrawTrajectory()
    {
        int j = 0;
        Dictionary<string, VisualConstraint> constraints = new Dictionary<string, VisualConstraint>();
        for (int i = 0; i < this.points.Length; i++)
        {
            if(this.points[i].keyframe_id > j)
            {
                StatePrefab point = this.DrawSpot(points[i].robot.position[0], points[i].robot.position[1], points[i].robot.position[2], points[i].robot.orientation, points[i].applied_constraints, j);
                for(int k = 0; k < point.constraintsActive.Length; k++)
                {
                    point.constraints.Add(constraintsDict[point.constraintsActive[k] + ""]);
                }
                j++;
            }
        }
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