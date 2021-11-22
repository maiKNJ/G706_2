using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }


    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    //This can be used to send the data from ARcore data
    public static void PlayerMovement(Vector3 _position, Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_position);
            /*foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }*/
            _packet.Write(_rotation);

            SendUDPData(_packet);
            //SendTCPData(_packet);
        }
    }
    #endregion
}
