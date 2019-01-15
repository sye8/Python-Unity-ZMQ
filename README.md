# ZMQ Connection Between Python and Unity

## Unity Respond, Python Request

### Execute

Under `UnityResponder`, run `request.py`, then start game mode of `demo.unity`

Lighting on the sphere will flash with 1 second interval, according to the command "on" or "off" sent from the Python script `request.py`. Unity will send back acknowledgement that it has received the command, which `request.py` will print.

## Unity Request, Python Respond



## Note

For the C# code to compile, set editor to .NET 4.6

`File -> Build Settings -> Player Settings... -> Other Settings -> Scripting Runtime Version -> Experimental (.NET 4.6 Equivalent)`
