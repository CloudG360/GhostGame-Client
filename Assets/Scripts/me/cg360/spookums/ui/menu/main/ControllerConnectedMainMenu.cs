using System;
using System.Collections;
using System.Collections.Generic;
using me.cg360.spookums;
using me.cg360.spookums.core.network.packet.auth;
using me.cg360.spookums.core.network.packet.game.info;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace me.cg360.spookums.ui.menu.main
{
    public class ControllerConnectedMainMenu : MonoBehaviour
    {
        public Main Main;
        public Button ButtonJoinQueue;
        public Button ButtonListGames;
        public Button ButtonSettings;

        void Start()
        {
            ButtonJoinQueue.onClick.AddListener(ClickedQueue);
            ButtonJoinQueue.interactable = true;
            
            //ButtonListGames.onClick.AddListener(ClickedListGames);
            ButtonListGames.interactable = false;

            //ButtonSettings.onClick.AddListener(ClickedSettings);
            ButtonSettings.interactable = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void ClickedQueue()
        {
            if (ButtonJoinQueue.interactable)
            {
                ButtonJoinQueue.interactable = false;
                Main.Client.NetworkInterface.SendDataPacket(new PacketOutGameQueueRequest(), true);
            }
        }
       
    }
}

