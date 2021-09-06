using System;
using me.cg360.spookums.core.eventsys;
using me.cg360.spookums.core.eventsys.handler;
using me.cg360.spookums.core.eventsys.type.network;
using me.cg360.spookums.core.network.netimpl.socket;
using me.cg360.spookums.core.network.packet.generic;
using System.Threading;
using me.cg360.spookums.core.network;
using me.cg360.spookums.utility;
using net.cg360.spookums.core.network;
using UnityEngine;

namespace me.cg360.spookums
{
    public class Main : MonoBehaviour
    {
        private NISocket _currentSocket; //TODO: Network Manager
        private int _responseCounter = 0;

        private PacketRegistry _packetRegistry;
        
        // Things that need to be done else it won't start
        // networking correctly:
        // - Set VanillaProtocol.CreateRegistry(); as the primary instance.
        // - Create an NI Socket and open it on a GameThread (Port 22057 is default)
        // - start that thread.
        
        private void Start()
        {
            GameThread.StartThreadChecking();
            
            EventManager eventManager = new EventManager();
            eventManager.SetAsPrimaryManager();

            Listener newListen = new Listener(this);
            eventManager.AddListener(newListen);

            _packetRegistry = VanillaProtocol.CreateRegistry();
            _packetRegistry.SetAsPrimaryInstance();

            _currentSocket = new NISocket();

            GameThread serverThread = new GameThread(() => {
                
                _currentSocket.OpenServerConnection("localhost", 22057);
                Debug.Log("Stopped Server");
                
            });
            
            serverThread.Start();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
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
            _currentSocket.SendDataPacket(new PacketInOutChatMessage($"Response #{_responseCounter}"), false);
            _responseCounter++;
        }
    }
}