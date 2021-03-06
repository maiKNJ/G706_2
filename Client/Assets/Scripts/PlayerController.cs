using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        Vector3 _position = transform.position;
        Quaternion _rotation = transform.rotation;
        //KeyCode should be changed to be input from ARcore, and should be used/sent to the server
        //bool[] _inputs = new bool[]
        //{
        //    Input.GetKey(KeyCode.W),
        //    Input.GetKey(KeyCode.S),
        //    Input.GetKey(KeyCode.A),
        //    Input.GetKey(KeyCode.D),
        //};

        ClientSend.PlayerMovement(_position, _rotation);
    }
}
