using System.Collections.Concurrent;
using System.Threading;

using NetMQ;
using NetMQ.Sockets;

using UnityEngine;

public class ZMQListener{
	private readonly Thread listener;
	private bool listenerStopped;
	public delegate void MessageDelegate(string message);
	private readonly MessageDelegate msgDelegate;
	private readonly ConcurrentQueue<string> msgQueue = new ConcurrentQueue<string>();

	public ZMQListener(MessageDelegate messageDelegate){
        msgDelegate = messageDelegate;
        listener = new Thread(ListenerWorker);
    }

	private void ListenerWorker(){
		AsyncIO.ForceDotNet.Force();
		using (var subSocket = new SubscriberSocket()){
			subSocket.Options.ReceiveHighWatermark = 1000;
			subSocket.Connect("tcp://localhost:12345");
			subSocket.Subscribe("");
			while(!listenerStopped){
				string frameString;
				if(!subSocket.TryReceiveFrameString(out frameString)){
					continue;
				}
				Debug.Log(frameString);
				msgQueue.Enqueue(frameString);
			}
			subSocket.Close();
		}
		NetMQConfig.Cleanup();
	}

	public void Update(){

        while (!msgQueue.IsEmpty){
            string message;
            if (msgQueue.TryDequeue(out message))
            {
                msgDelegate(message);
            }
            else
            {
                break;
            }
        }
    }

    public void Start()
    {
        listenerStopped = false;
        listener.Start();
    }

    public void Stop()
    {
        listenerStopped = true;
        listener.Join();
    }
}

public class ClientLight : MonoBehaviour{
	public ZMQListener zmq;
	public Light light;

	void MessageHandler(string msg){
		if(string.Equals(msg, "on")){
			light.enabled = true;
		}else if(string.Equals(msg, "off")){
			light.enabled = false;
		}else{
			return;
		}
	}

	void Start(){
		zmq = new ZMQListener(MessageHandler);
		zmq.Start();
	}

	void Update(){
		zmq.Update();
	}

	void OnDestroy(){
		zmq.Stop();
	}
}