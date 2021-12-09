using System;
using System.Collections;
using System.Collections.Generic;
using me.cg360.spookums;
using me.cg360.spookums.core.network.packet.auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace me.cg360.spookums.ui.menu.main
{
    public class ControllerLoginRegisterMenu : MonoBehaviour
    {
        public Main Main;
        public Button ButtonLogin;
        public Button ButtonRegister;
        public TMP_InputField UsernameInput;
        public TMP_InputField PasswordInput;

        void Start()
        {
            ButtonLogin.onClick.AddListener(ClickedLogin);
            ButtonLogin.interactable = false;
            
            ButtonRegister.onClick.AddListener(ClickedRegister);
            ButtonRegister.interactable = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void ClickedLogin()
        {
            if (ButtonLogin.interactable)
            {
                ButtonLogin.interactable = false;
                PacketOutLogin login = new PacketOutLogin
                {
                    Mode = 0,
                    Username = UsernameInput.text.Trim(),
                    Password = PasswordInput.text.Trim()
                };
                
                FieldRewriter rewriterConnecting = Main.MainMenuController.ElementLookup["load_serverconnecting"]
                    .GetComponent<FieldRewriter>();
                rewriterConnecting.WriteField("description", "Logging In");
                Main.MainMenuController.SwitchPanel("load_serverconnecting");

                Main.NetworkInterface.SendDataPacket(login, true);
            }
        }
        
        void ClickedRegister()
        {
            if (ButtonRegister.interactable)
            {
                ButtonLogin.interactable = false;
                PacketOutUpdateAccount register = new PacketOutUpdateAccount
                {
                    IsCreatingNewAccount = true,
                    NewUsername = UsernameInput.text.Trim(),
                    NewPassword = PasswordInput.text.Trim()
                };
                
                FieldRewriter rewriterConnecting = Main.MainMenuController.ElementLookup["load_serverconnecting"]
                    .GetComponent<FieldRewriter>();
                rewriterConnecting.WriteField("description", "Logging In");
                Main.MainMenuController.SwitchPanel("load_serverconnecting");
                
                Main.NetworkInterface.SendDataPacket(register, true);
            }
        }

        private void Update()
        {
            if (UsernameInput.text.Length > 0 && PasswordInput.text.Length > 0)
            {
                ButtonLogin.interactable = true;
                ButtonRegister.interactable = true;
            }
            else
            {
                ButtonLogin.interactable = false;
                ButtonRegister.interactable = false;
            }
        }
    }
}

