import zmq
import time

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5556")

while True:
    msg = socket.recv()
    print("Received Request with Message: " + msg)
    time.sleep(1)
    socket.send("Hello From Server")
