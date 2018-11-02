using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WwwPlayground : MonoBehaviour {

    [SerializeField] bool stop;

    WebSocketSharp.Server.WebSocketServer wss;


    void Start () {
        print("Start");

        wss = new WebSocketSharp.Server.WebSocketServer(8888);
        wss.AddWebSocketService<WsServerPlayground>("/plgrd");
        wss.Start();

        //System.Net.WebClient wc = new System.Net.WebClient();

        //byte[] b = wc.DownloadData("https://www.google.com/");
        //wc.DownloadDataCompleted += (sender, eventArg) => {
        //print("Download data completed " + sender + " | " + eventArg.Result.Length);
        //print("Download data completed " + b.Length);


        //WWW www = new WWW("https://www.google.com/");
        //while (!www.isDone)
        //    print("Progress: " + www.progress * 100 + "%");
        //print("## " + www.text.Substring(0, 200));

        //using(WebSocket ws = new WebSocket("ws://google.com/")) {

        //    ws.Connect();

        //    ws.Send("ASDF");
        //}

        StartCoroutine(ClientPlauground(1, 3, 3f));
        StartCoroutine(ClientPlauground(2, 2, 2.7f));
        StartCoroutine(ClientPlauground(3, 5, 3.8f));


        //Debug.Break();
        //};

        //wc.DownloadDataAsync(new System.Uri);

    }

    void Update () {
        if (stop) {
            stop = false;
            wss.Stop();
        }
    }

    IEnumerator ClientPlauground(int clientNum, int repeatChar, float waitTime) {
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        using (WebSocket ws = new WebSocket("ws://localhost:8888/plgrd")) {
            ws.OnMessage += (sender, eventArg) => {
                print("Client " + clientNum + " received " + eventArg.Data);
            };
            ws.OnOpen += (sender, eventArg) => {
                print("Connection open!");
            };


            ws.Connect();
            while (!ws.IsConnected) yield return wait;

            while (true) {
                yield return wait;
                string s = new string((char)Random.Range('a', 'z' + 1), repeatChar);
                print("Client " + clientNum + " sending " + s);
                ws.Send(s);
            }
        }
    }

    class WsServerPlayground : WebSocketSharp.Server.WebSocketBehavior {
        protected override void OnMessage (MessageEventArgs e) {
            string s = e.Data + "###";
            print("Server received " + e.Data + " and will send back " + s);
            Send(s);
        }
    }
}
