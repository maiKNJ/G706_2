using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class ClientHandler : MonoBehaviour
{
    /// <summary>
    /// Updating data between server and client
    /// </summary>
    /// <param name="_packet"></param>
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        //((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port
    }

    //Changes these below to changes cam start pos

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }
}
