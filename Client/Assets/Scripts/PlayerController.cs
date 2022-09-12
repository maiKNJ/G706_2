using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    ///  Sending the camera position to the server
    /// </summary>
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        Vector3 _position = transform.position;
        Quaternion _rotation = transform.rotation;
       

        ClientSend.PlayerMovement(_position, _rotation);
    }
}
