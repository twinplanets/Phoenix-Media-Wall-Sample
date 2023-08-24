using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NativeWebSocket;

namespace TwinPlanets
{
    public class WsClient : MonoBehaviourSingleton<WsClient>
    {
        WebSocket websocket;
        public MotionObject[] motionObjects;
        [SerializeField] Text textfield;
        [SerializeField] bool SampleDataStream;
        string wsLastMessage = "No Message";
        WsSampleTransmitter wsST;

        // Start is called before the first frame update
        async void Start()
        {
            websocket = new WebSocket("ws://localhost:8080");

            websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
            };

            websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
                if (SampleDataStream)
                {
                    wsST = gameObject.AddComponent<WsSampleTransmitter>();
                }
            };

            websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
            };

            websocket.OnMessage += (bytes) =>
            {
                // getting the message as a string
                wsLastMessage = System.Text.Encoding.UTF8.GetString(bytes);
                SendSampleData(wsLastMessage);
                //Debug.Log("OnMessage! " + wsLastMessage);
            };

            // waiting for messages
            await websocket.Connect();
        }

        void Update()
        {
    #if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
    #endif
            if(textfield != null) textfield.text = wsLastMessage;
        }

        public void SendSampleData(string data)
        {
            wsLastMessage = data;
            foreach (var item in motionObjects)
            {
                item.UpdateData(System.Text.Encoding.UTF8.GetBytes(data));
            }
        }

        private async void OnApplicationQuit()
        {
            await websocket.Close();
        }

    }

    public class WsSampleTransmitter : MonoBehaviour
    {
        public string fileName = "full-2.txt";
        private string line;
        private string[] lines;
        private int currentIndex;
    
        [SerializeField][Min(0.001f)] private float messageDelay = 0.2f;
        private WsClient wsC;

        public void Start()
        {
            wsC = GetComponent<WsClient>();
            fileName = Path.Combine(Application.streamingAssetsPath, fileName);
            // Read the text file and split it into lines
            string text = File.ReadAllText(fileName);
            lines = text.Split('\n');

            currentIndex = 0;

            StartCoroutine(SendMessage());
        }

        IEnumerator SendMessage()
        {
            while(true)
            {
                if (currentIndex < lines.Length)
                {
                    line = lines[currentIndex].Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        wsC.SendSampleData(line);
                        //Debug.Log("Sent: " + line);
                    }

                    currentIndex++;
                
                }
                else
                {
                    currentIndex = 0;
                }
                yield return new WaitForSecondsRealtime(messageDelay);
            }
        }
  
    }
}