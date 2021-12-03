using System;
using System.Collections;
using System.Collections.Generic;
using me.cg360.spookums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Connect_ButtonController : MonoBehaviour
{
    public Main Main;
    public MenuPanelController MasterController;
    public Button Button;
    public TMP_InputField IPInput;
    public TMP_InputField PortInput;

    void Start()
    {
        Button.onClick.AddListener(ClickedButton);
        Button.interactable = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ClickedButton()
    {
        if (Button.interactable)
        {
            Debug.Log(IPInput.text + ":" + Int32.Parse(PortInput.text));
            Main.StartServerClient(IPInput.text, Int32.Parse(PortInput.text));
            MasterController.SwitchPanel("load_serverconnecting");


        }
    }

    private void Update()
    {
        if (IPInput.text.Length > 0 && PortInput.text.Length > 0)
        {
            Button.interactable = true;
        }
        else
        {
            Button.interactable = false;
        }
    }
}
