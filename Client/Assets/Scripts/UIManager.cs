using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public GameObject ARsession;
    public GameObject record;
    public GameObject save;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
   

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ARsession.SetActive(true);
        Client.instance.ConnectToServer();
        record.SetActive(true);
        //save.SetActive(true);
    }

    public void RecordStart()
    {
        record.SetActive(false);
        save.SetActive(true);
    }

    public void RecordStop()
    {
        record.SetActive(true);
        save.SetActive(false);
    }
}
