using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROSBridgeLib
{

    public class ROSConnector : MonoBehaviour
    {
        private ROSBridgeWebSocketConnection ros = null;
        public string rosbridgeAddress;
        public int rosbridgePort;

        class ConstraintAddingSubscriber : ROSBridgeSubscriber
        {
            public static string addConstraintTopic = "/Unity/AddConstraint";
            public new static string GetMessageTopic()
            {
                return addConstraintTopic;
            }

            public new static string GetMessageType()
            {
                return "std_msgs/StringMsg";
            }

            public new static ROSBridgeMsg ParseMessage(SimpleJSON.JSONNode msg)
            {
                return new std_msgs.StringMsg(msg);
            }

            public new static void CallBack(ROSBridgeMsg msg)
            {
                //message should be JSON of a Constraint object
                GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
                DynamicPoints.DynamicTrajectoryReader pointHandler = scriptHolder.GetComponent<DynamicPoints.DynamicTrajectoryReader>();
                pointHandler.AddConstraint(((std_msgs.StringMsg)msg).GetData());
            }
        }

        class ConstraintDeletingSubscriber : ROSBridgeSubscriber
        {
            public static string deleteConstraintTopic = "/Unity/DeleteConstraint";
            public new static string GetMessageTopic()
            {
                return deleteConstraintTopic;
            }

            public new static string GetMessageType()
            {
                return "std_msgs/StringMsg";
            }

            public new static ROSBridgeMsg ParseMessage(SimpleJSON.JSONNode msg)
            {
                return new std_msgs.StringMsg(msg);
            }

            public new static void CallBack(ROSBridgeMsg msg)
            {
                //msg should be ID of constraint to be deleted
                GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
                DynamicPoints.DynamicTrajectoryReader pointHandler = scriptHolder.GetComponent<DynamicPoints.DynamicTrajectoryReader>();
                pointHandler.DeleteConstraint(((std_msgs.StringMsg)msg).GetData());
            }
        }

        class PointAddingSubscriber : ROSBridgeSubscriber
        {
            public static string addPointTopic = "/Unity/AddPoint";
            public new static string GetMessageTopic()
            {
                return addPointTopic;
            }

            public new static string GetMessageType()
            {
                return "std_msgs/StringMsg";
            }

            public new static ROSBridgeMsg ParseMessage(SimpleJSON.JSONNode msg)
            {
                return new std_msgs.StringMsg(msg);
            }

            public new static void CallBack(ROSBridgeMsg msg)
            {
                //msg should be JSON of a TrajectoryPoint object
                GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
                DynamicPoints.DynamicTrajectoryReader pointHandler = scriptHolder.GetComponent<DynamicPoints.DynamicTrajectoryReader>();
                pointHandler.AddPoint(((std_msgs.StringMsg)msg).GetData());
            }
        }

        class PointDeletingSubscriber : ROSBridgeSubscriber
        {
            public static string deletePointTopic = "/Unity/DeletePoint";
            public new static string GetMessageTopic()
            {
                return deletePointTopic;
            }

            public new static string GetMessageType()
            {
                return "std_msgs/StringMsg";
            }

            public new static ROSBridgeMsg ParseMessage(SimpleJSON.JSONNode msg)
            {
                return new std_msgs.StringMsg(msg);
            }

            public new static void CallBack(ROSBridgeMsg msg)
            {
                //message should be keyframe_id of point to be deleted
                GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
                DynamicPoints.DynamicTrajectoryReader pointHandler = scriptHolder.GetComponent<DynamicPoints.DynamicTrajectoryReader>();
                pointHandler.DeletePoint(((std_msgs.StringMsg)msg).GetData());
            }
        }

        void Start()
        {
            // Where the rosbridge instance is running, could be localhost, or some external IP
            ros = new ROSBridgeWebSocketConnection(rosbridgeAddress, rosbridgePort);

            // Add subscribers and publishers (if any)
            ros.AddSubscriber(typeof(ConstraintAddingSubscriber));
            ros.AddSubscriber(typeof(ConstraintDeletingSubscriber));
            ros.AddSubscriber(typeof(PointAddingSubscriber));
            ros.AddSubscriber(typeof(PointDeletingSubscriber));

            // Fire up the subscriber(s) and publisher(s)
            ros.Connect();
        }

        // Extremely important to disconnect from ROS. Otherwise packets continue to flow
        void OnApplicationQuit()
        {
            if (ros != null)
            {
                ros.Disconnect();
            }
        }
        // Update is called once per frame in Unity
        void Update()
        {
            ros.Render();
        }
    }
}
