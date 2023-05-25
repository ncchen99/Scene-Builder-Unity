using UnityEngine;
using WebSocketSharp;
public class Socket : MonoBehaviour
{
    WebSocket ws;
    private void Start()
    {
    Debug.Log("Message Received from ");
        ws = new WebSocket("ws://localhost:8888");
        ws.Connect();
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message Received from "+((WebSocket)sender).Url+", Data : "+e.Data);
        };
    }
private void Update()
    {
        if(ws == null)
        {
            return;
        }
if (Input.GetKeyDown(KeyCode.Space))
        {
            ws.Send("Hello");
        }  
    }
}
