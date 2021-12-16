using System;
using me.cg360.spookums.core.eventsys;
using me.cg360.spookums.core.eventsys.handler;
using me.cg360.spookums.core.eventsys.type.network;
using me.cg360.spookums.core.network;
using me.cg360.spookums.core.network.netimpl.socket;
using me.cg360.spookums.core.network.packet.auth;
using me.cg360.spookums.core.network.packet.game.entity;
using me.cg360.spookums.core.network.packet.info;
using me.cg360.spookums.core.scheduler;
using me.cg360.spookums.core.scheduler.task;
using me.cg360.spookums.ui.menu;
using me.cg360.spookums.utility;
using UnityEngine;

namespace me.cg360.spookums
{
    public class Main : MonoBehaviour
    {

        public MenuPanelController MainMenuController = null;
        

        public static Main Client { get; private set; }
        
        public PacketRegistry PacketRegistry { get; private set; }
        public EventManager EventManager { get; private set; }
        public CommandingScheduler SchedulerBrain { get; private set; }
        public Scheduler MainScheduler { get; private set; }
        public NISocket NetworkInterface { get; private set; }



        public void StartServerClient(string addr, int port)
        {
            GameThread serverThread = new GameThread(() =>
            {
                NetworkInterface = new NISocket();
                NetworkInterface.OpenServerConnection(addr, port);
            });
            serverThread.Start();
        }

        public void ResetServerClient()
        {
            EventManager = new EventManager().SetAsPrimaryManager(); //TODO: Probably gonna cause a memory leak? Cleanup the old managers.
            SchedulerBrain.StopScheduler();
            MainScheduler.StopScheduler();
            
            SchedulerBrain.StartScheduler();
            MainScheduler.StartScheduler();
            
            Listener newListen = new Listener(this);
            EventManager.AddListener(newListen);
        }


        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Client = this;
            GameThread.StartThreadChecking();
            
            PacketRegistry = VanillaProtocol.CreateRegistry().SetAsPrimaryInstance();
            SchedulerBrain = new CommandingScheduler().SetAsPrimaryInstance();
            MainScheduler = new Scheduler(1);

            ResetServerClient();
        }

        
        private void OnDisable()
        {
            GameThread.StopThreadChecking();
        }


        public void FixedUpdate()
        {
            SchedulerBrain.RunSchedulerTick();
        }


        [GEventHandler]
        public void onPacketSend(PacketEvent.Sent e)
        {
            Debug.Log("OUT << " + e.Packet.ToHeaderString());
        }
        
        [GEventHandler]
        public void onPacketRecieve(PacketEvent.Recieved e)
        {
            Debug.Log("IN >> " + e.Packet.ToHeaderString());

            switch (e.Packet.PacketID)
            {
                case VanillaProtocol.PACKET_PROTOCOL_ERROR:
                    PacketInProtocolError pProtocolError = (PacketInProtocolError)e.Packet;
                    FailServerConnection("Out of date!", $"The server requires you to be on version {pProtocolError.RequiredClientVersionInfo}.");
                    break;
                
                
                case VanillaProtocol.PACKET_PROTOCOL_SUCCESS:
                    PacketInProtocolSuccess pProtocolSuccess = (PacketInProtocolSuccess)e.Packet;
                    MainScheduler.PrepareTask(() =>
                    {
                        if (MainMenuController.enabled && MainMenuController.CurrentPanel == "load_serverconnecting")
                        {
                            FieldRewriter rewriter = MainMenuController.ElementLookup["load_serverconnecting"]
                                .GetComponent<FieldRewriter>();
                            rewriter.WriteField("description", "Verified Version");
                            
                            FieldRewriter loginRewriter = MainMenuController.ElementLookup["load_loginregister"]
                                .GetComponent<FieldRewriter>();
                            loginRewriter.WriteField("error_text", "");
                            
                            MainMenuController.SwitchPanel("load_loginregister");
                        }
                    }).Schedule();
                    
                    break;
                
                
                case VanillaProtocol.PACKET_LOGIN_RESPONSE:
                    MainScheduler.PrepareTask(() =>
                    {
                        PacketInLoginResponse pLoginResponse = (PacketInLoginResponse)e.Packet;
                        Debug.Log(
                            $"Code: {PacketInLoginResponse.CodeToStatus(pLoginResponse.StatusCode).ToString()} | " +
                            $"Username: {pLoginResponse.Username} | Token: {pLoginResponse.Token}"
                        );

                        string errorText = "";

                        switch (PacketInLoginResponse.CodeToStatus(pLoginResponse.StatusCode))
                        {
                            case PacketInLoginResponse.Status.SUCCESS:
                                string username = pLoginResponse.Username;
                                
                                FieldRewriter connectedMenuRewriter = MainMenuController.ElementLookup["connected_mainmenu"]
                                    .GetComponent<FieldRewriter>();
                                connectedMenuRewriter.WriteField("title", "You're logged in as: "+username);
                                
                                MainMenuController.SwitchPanel("connected_mainmenu");
                                return;
                            
                            case PacketInLoginResponse.Status.INVALID_CREDENTIALS:
                                errorText = "Invalid Username/Password";
                                break;
                            case PacketInLoginResponse.Status.ALREADY_LOGGED_IN:
                                errorText = "You're already logged in on this server!";
                                break;
                            case PacketInLoginResponse.Status.GENERAL_LOGIN_ERROR:
                                errorText = "Something went wrong! Try again soon. (Technical Server Error)";
                                break;

                            case PacketInLoginResponse.Status.TAKEN_USERNAME:
                                errorText = "That username is already taken! Try another one.";
                                break;
                            case PacketInLoginResponse.Status.GENERAL_REGISTER_ERROR:
                                errorText = "Unable to create an account! Try again soon.";
                                break;

                            case PacketInLoginResponse.Status.TECHNICAL_SERVER_ERROR:
                                errorText = "Something went wrong! Try again soon. (Technical Server Error)";
                                break;

                            case PacketInLoginResponse.Status.INVALID_TOKEN:
                                errorText = "";
                                break;

                            default:
                                errorText = "Uh oh! Something went wrong...";
                                break;
                        }

                        FieldRewriter rewriter = MainMenuController.ElementLookup["load_loginregister"]
                            .GetComponent<FieldRewriter>();
                        rewriter.WriteField("error_text", errorText);
                        MainMenuController.SwitchPanel("load_loginregister");
                    }).Schedule();

                    break;
                
                
                case VanillaProtocol.PACKET_ENTITY_ADD:
                    PacketInAddEntity pEntityAdd = (PacketInAddEntity) e.Packet;
                    Debug.Log(
                        $"Entity | rID = {pEntityAdd.EntityRuntimeID} | type = {pEntityAdd.EntityTypeId} | position = {pEntityAdd.Position} | " +
                        $"Floor = {pEntityAdd.FloorNumber} | JSON = {pEntityAdd.PropertiesJSON} | JSON Length = {pEntityAdd.PropertiesJSON.Length}"
                    );
                    break;
            }
        }

        [GEventHandler]
        public void onServerConnectionEstablished(ConnectionEstablishedEvent e)
        {
            e.Net.SendDataPacket(new PacketOutProtocolCheck(VanillaProtocol.PROTOCOL_ID), true);
        }
        

        [GEventHandler]
        public void onServerConnectFail(ConnectionKillEvent e)
        {
            FailServerConnection($"Failed to connect (code: {e.ExitCode})", e.Reason);
        }


        protected void FailServerConnection(string title, string desc)
        {
            MainScheduler.PrepareTask(() =>
            {
                ResetServerClient();
                MainMenuController.enabled = true;
                MainMenuController.SwitchPanel("server_error");

                FieldRewriter rewriterError= MainMenuController.ElementLookup["server_error"]
                    .GetComponent<FieldRewriter>();
                
                rewriterError.WriteField("title", title);
                rewriterError.WriteField("description", desc);
                
                
                FieldRewriter rewriterConnecting = MainMenuController.ElementLookup["load_serverconnecting"]
                    .GetComponent<FieldRewriter>();
                rewriterConnecting.WriteField("description", "");
            }).Schedule();
        }
        
        
    }
}