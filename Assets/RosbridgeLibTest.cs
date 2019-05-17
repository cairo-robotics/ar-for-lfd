using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using SimpleJSON;

namespace LFDBridge
{

    // Test subscriber:
    public class TestSubscriber : ROSBridgeSubscriber
    {
        static string message;

        // These two are important
        public new static string GetMessageTopic()
        {
            return "/rosbridgelib_sub_test";
        }

        public new static string GetMessageType()
        {
            return "std_msgs/String";
        }

        // Important function (I think.. Converts json to PoseMsg)
        public new static ROSBridgeMsg ParseMessage(JSONNode msg)
        {
            return new StringMsg(msg);
        }

        // This function should fire on each received ros message
        public new static void CallBack(ROSBridgeMsg msg)
        {
            StringMsg str_msg = (StringMsg)msg;
            message = str_msg.GetData();
            Debug.Log(message);
        }
    }

    // TestPublisher
    public class TestPublisher : ROSBridgePublisher
    {
        public static string GetMessageTopic()
        {
            return "/rosbridgelib_pub_test";
        }

        public static string GetMessageType()
        {
            return "std_msgs/String";
        }

        public static string ToYAMLString(StringMsg msg)
        {
            return msg.ToYAMLString();
        }

        public new static ROSBridgeMsg ParseMessage(JSONNode msg)
        {
            return new StringMsg(msg);
        }
    }

    public class TestBridge : MonoBehaviour
    {
        private ROSBridgeWebSocketConnection ros = null;

        void Start()
        {
            // Where the rosbridge instance is running, could be localhost, or some external IP
            ros = new ROSBridgeWebSocketConnection("ws://192.168.50.109", 9090);

            // Add subscribers and publishers (if any)
            ros.AddSubscriber(typeof(TestSubscriber));
            ros.AddPublisher(typeof(TestPublisher));

            // Fire up the subscriber(s) and publisher(s)
            ros.Connect();
        }

        public ROSBridgeWebSocketConnection GetRos()
        {
            return ros;
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