using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldDefault : MonoBehaviour
{
    public InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<InputField>();
        inputField.text = "192.168.1.19";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
