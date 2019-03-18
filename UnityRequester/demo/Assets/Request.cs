using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using NetMQ;
using NetMQ.Sockets;


[System.Serializable]
public class JSONDemo
{
	public string uuid;
	public double x;
	public double y;

	public override string ToString(){
		return "UUID: " + uuid + "; x: " + x + "; y: " + y;
	}
}
	

public class Request : MonoBehaviour {
	public Transform spawnPos;
	public GameObject sphere;
	public GameObject cube;

	private bool requesterIsStarted = false;
	private string outMsg = "";
	private string inMsg = "";


	void request(){
		AsyncIO.ForceDotNet.Force();

		var socket = new RequestSocket("tcp://localhost:5556");

		try{
			while(requesterIsStarted){
				if(!string.Equals(outMsg, "")){
					if(!string.Equals(outMsg, "sendJSON")){
						socket.SendFrame(outMsg);
						Debug.Log("Send: " + outMsg);
					}else{
						JSONDemo toSend = new JSONDemo();
						toSend.uuid = System.Guid.NewGuid().ToString();
						System.Random random = new System.Random();
						toSend.x = random.NextDouble() * 5;
						toSend.y = random.NextDouble() * 5;
						string json = JsonUtility.ToJson(toSend);
						socket.SendFrame("sendJSON " + json);
					}
					outMsg = "";

					string msg;
					if (socket.TryReceiveFrameString(TimeSpan.FromSeconds(3), out msg)) {
						inMsg = msg;
					}
				}
			}
		}finally{
			if (socket != null) {
				((IDisposable)socket).Dispose();
			}
		}
	}	

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<InputField>().onEndEdit.AddListener(InputFieldHandler);
		requesterIsStarted = true;
		Task task = new Task (async() => request());
		task.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!string.Equals (inMsg, "")) {
			int spaceIndex = inMsg.IndexOf (' ');
			string type = inMsg.Substring (0, spaceIndex);
			string args = inMsg.Substring (spaceIndex + 1);

			switch (type) {
			case "Spawn":
				switch (args) {
				case "sphere":
					Instantiate (sphere, spawnPos.position, spawnPos.rotation);
					Debug.Log ("Spawning a sphere");
					break;
				case "cube":
					Instantiate (cube, spawnPos.position, spawnPos.rotation);
					Debug.Log ("Spawning a cube");
					break;
				default:
					Debug.Log ("Spawn: Unrecognized object");
					break;
				}
				break;
			case "JSON":
				Debug.Log (JsonUtility.FromJson<JSONDemo> (args));
				break;
			case "Error":
				Debug.Log ("ResponderError: " + args);
				break;
			default:
				Debug.Log (inMsg);
				break;
			}

			inMsg = "";
		}
	}

	void OnDestroy(){
		requesterIsStarted = false;
	}

	private void InputFieldHandler(string txt){
		outMsg = txt;
	}
}
