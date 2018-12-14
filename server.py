import zmq
import time

context = zmq.Context()
socket = context.socket(zmq.PUB)
socket.bind("tcp://*:12345")

while True:
    msg = "on"
    socket.send_string(msg)
    print(msg)
    time.sleep(1)
    msg = "off"
    socket.send_string(msg)
    print(msg)
    time.sleep(1)
