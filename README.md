# Low-budget application for virtual production
The program is made in Unity 2020.3.22f1, using the plugin AR Foundation for motion tracking.

## Getting started
Follow the instructions to use the tracking system.

### Prerequisities
There are two parts of the project, the server and the client.

The client ("Client") needs to be opened in Unity and build on an Android device that is compatible with AR Core. This is because it is used through the AR Foundation for the tracking. You can see a list of compatible devices [here](https://developers.google.com/ar/devices). 
To build the client on the device make sure to choose Android inside the Unity Build Settings (See manual [here](https://docs.unity3d.com/Manual/android-BuildProcess.html))

The server ("CamServer") needs to be build and run on a computer. The computer can furthermore be connected to a projection setup or LED Wall for optimal use.

Both the server and client devices should be on the same internet connection.

### Running the program
The server should be running before trying to connect the Android device with the proper IP Address (found in the computer settings).
