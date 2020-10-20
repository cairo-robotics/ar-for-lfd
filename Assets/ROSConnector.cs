using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROSBridgeLib
{

    public class ROSConnector : MonoBehaviour
    {
        public ROSBridgeWebSocketConnection ros = null;
        public string rosbridgeAddress;
        public int rosbridgePort;
       
        // Main subscriber - listens for either individual keyframe readouts in JSON, or a "CLEAR" command
        class ConstraintManagerSubscriber : ROSBridgeSubscriber
        {
            public static string constraintManagerTopic = "/constraint_manager";
            public new static string GetMessageTopic()
            {
                return constraintManagerTopic;
            }

            public new static string GetMessageType()
            {
                return "std_msgs/String";
            }

            public new static ROSBridgeMsg ParseMessage(SimpleJSON.JSONNode msg)
            {
                return new std_msgs.StringMsg(msg);
            }

            public static void CallBack(ROSBridgeMsg msg)
            {
                //message should either be "CLEAR" or JSON of a TrajectoryPoint object
                print("Callback activated");
                GameObject scriptHolder = GameObject.FindWithTag("PrimaryScriptHolder");
                DynamicPoints.DynamicTrajectoryReader pointHandler = scriptHolder.GetComponent <DynamicPoints.DynamicTrajectoryReader>();
                pointHandler.ManageConstraint(((std_msgs.StringMsg)msg).GetData());
            }
        }

        // Main publisher - sends JSON with lists of active constraints per keyframe
        class ModelUpdatePublisher : ROSBridgePublisher
        {
            public static string modelUpdateTopic = "/cairo_lfd/keyframe_update";
            public new static string GetMessageTopic()
            {
                return modelUpdateTopic;
            }

            public new static string GetMessageType()
            {
                return "std_msgs/String";
            }

            public static string ToYAMLString(std_msgs.StringMsg msg)
            {
                return msg.ToYAMLString();
            }

            public static ROSBridgeMsg ParseMessage(SimpleJSON.JSONNode msg)
            {
                return new std_msgs.StringMsg(msg);
            }

        }

        // Secondary publisher - sends JSON with updated constraints after editing
        class ConstraintUpdatePublisher : ROSBridgePublisher
        {
            public static string constraintUpdateTopic = "/constraint_edits";
            public new static string GetMessageTopic()
            {
                return constraintUpdateTopic;
            }

            public new static string GetMessageType()
            {
                return "std_msgs/String";
            }

            public static string ToYAMLString(std_msgs.StringMsg msg)
            {
                return msg.ToYAMLString();
            }

            public static ROSBridgeMsg ParseMessage(SimpleJSON.JSONNode msg)
            {
                return new std_msgs.StringMsg(msg);
            }

        }


        void Start()
        {
            // Where the rosbridge instance is running, could be localhost, or some external IP
            ros = new ROSBridgeWebSocketConnection(rosbridgeAddress, rosbridgePort);

            // Add subscribers and publishers (if any)
            ros.AddSubscriber(typeof(ConstraintManagerSubscriber));
            ros.AddPublisher(typeof(ModelUpdatePublisher));
            ros.AddPublisher(typeof(ConstraintUpdatePublisher));
            //ros.AddSubscriber(typeof(ConstraintAddingSubscriber));
            //ros.AddSubscriber(typeof(ConstraintDeletingSubscriber));
            //ros.AddSubscriber(typeof(PointAddingSubscriber));
            //ros.AddSubscriber(typeof(PointDeletingSubscriber));

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

        // For now this  publishes to the keyframe updater while a separate function publishes to constraint updates.
        // Ideally in the future, combine these and pass in a parameter specifying which one to publish to
        public void PublishThis(string msg)
        {
            std_msgs.StringMsg myMsg = new std_msgs.StringMsg(msg);
            print("firing keyframe publisher");
            ros.Publish(ModelUpdatePublisher.GetMessageTopic(), myMsg);
        }

        // For publishing Constraint edits. Ideally combine with above
        public void PublishConstraintEdits(string msg)
        {
            std_msgs.StringMsg myMsg = new std_msgs.StringMsg(msg);
            print("firing constraint publisher");
            ros.Publish(ConstraintUpdatePublisher.GetMessageTopic(), myMsg);
        }
    }
}
