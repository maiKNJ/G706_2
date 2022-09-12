using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPosRot : MonoBehaviour
{
    /// <summary>
    /// Used for debugging of changes in the username field
    /// </summary>
    Client _client;
    UIManager _uimanager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("IP" + Client.instance.ip + "usernamefield" + UIManager.instance.usernameField.text);
    }
}
