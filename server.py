import zmq
import time

context = zmq.Context()
requestSocket = context.socket(zmq.REQ)
requestSocket.connect("tcp://localhost:5558")

while True:
    
    poller = zmq.Poller()
    poller.register(requestSocket, zmq.POLLIN)
    
    # Send On
    msg = "on"
    requestSocket.send_string("on")
    print(msg)
    
    # Receive Acknowledgement
    ack = dict(poller.poll(2000))
    if ack:
        if ack.get(requestSocket) == zmq.POLLIN:
            try:
                recv = requestSocket.recv(zmq.NOBLOCK)
                print(recv)
            except zmq.Again as e:
                print("No response")

    time.sleep(1)

    # Then Send Off
    msg = "off"
    requestSocket.send_string(msg)
    print(msg)

    # Receive Acknowledgement
    ack = dict(poller.poll(2000))
    if ack:
        if ack.get(requestSocket) == zmq.POLLIN:
            try:
                recv = requestSocket.recv(zmq.NOBLOCK)
                print(recv)
            except zmq.Again as e:
                print("No response")
    time.sleep(1)
