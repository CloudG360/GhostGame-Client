using System;
using me.cg360.spookums.core.eventsys;
using me.cg360.spookums.core.eventsys.handler;
using me.cg360.spookums.core.eventsys.type.network;
using me.cg360.spookums.core.network;
using me.cg360.spookums.core.network.netimpl.socket;
using me.cg360.spookums.core.scheduler;
using me.cg360.spookums.core.scheduler.task;
using me.cg360.spookums.ui.menu.main;
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


        public void Update()
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
                // Handle incoming packets
            }
        }

        [GEventHandler]
        public void onServerConnectFail(ConnectionKillEvent e)
        {
            Debug.Log("AAAAAAAAA");
            
            MainScheduler.prepareTask(() =>
            {
                ResetServerClient();
                MainMenuController.enabled = true;
                MainMenuController.SwitchPanel("server_error");
                FieldRewriter rewriter = MainMenuController.ElementLookup["server_error"]
                    .GetComponent<FieldRewriter>();

                rewriter.WriteField("title", $"Failed to connect (code: {e.ExitCode})");
                rewriter.WriteField("description", e.Reason);
            }).Schedule();
        }
        
        
    }
}