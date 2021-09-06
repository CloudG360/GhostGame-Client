using System;
using me.cg360.spookums.core.eventsys;
using me.cg360.spookums.core.eventsys.handler;
using me.cg360.spookums.core.eventsys.type.network;
using me.cg360.spookums.core.network;
using me.cg360.spookums.core.network.netimpl.socket;
using me.cg360.spookums.core.scheduler;
using net.cg360.spookums.core.network;
using UnityEngine;
using NetworkInterface = me.cg360.spookums.core.network.netimpl.NetworkInterface;

namespace me.cg360.spookums
{
    public sealed class Client
    {

        private static Client _primaryInstance;
        
        
        public PacketRegistry PacketRegistry { get; private set; }
        public EventManager EventManager { get; private set; }
        public CommandingScheduler Scheduler { get; private set; }
        public NISocket NetworkInterface { get; private set; }

        public Client()
        {
            
        }

        public String InitClient(String address, int port)
        {
            if (_primaryInstance != null) return "Quirky Error indeed! The client is already running";
            _primaryInstance = this;
            
            PacketRegistry = VanillaProtocol.CreateRegistry();
            EventManager = new EventManager();
            Scheduler = new CommandingScheduler();

            PacketRegistry.SetAsPrimaryInstance();
            EventManager.SetAsPrimaryManager();
            Scheduler.SetAsPrimaryInstance();

            Listener newListen = new Listener(this);
            EventManager.AddListener(newListen);

            NetworkInterface = new NISocket();
            string result = NetworkInterface.OpenServerConnection(address, port);
            
            Debug.Log(result);

            _primaryInstance = null;
            return result;
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


        public static Client get()
        {
            return _primaryInstance;
        }
    }
}