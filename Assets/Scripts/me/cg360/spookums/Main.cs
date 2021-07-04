using me.cg360.spookums.core.eventsys;
using me.cg360.spookums.core.eventsys.handler;
using me.cg360.spookums.core.eventsys.type.network;
using me.cg360.spookums.core.network.netimpl.socket;
using me.cg360.spookums.core.network.packet.generic;
using System.Threading;
using me.cg360.spookums.core.network;
using net.cg360.spookums.core.network;
using UnityEngine;

namespace me.cg360.spookums
{
    public class Main : MonoBehaviour
    {
        private NISocket _currentSocket; //TODO: Network Manager
        private int _responseCounter = 0;

        private PacketRegistry _packetRegistry;
        
        private void Start()
        {
            EventManager eventManager = new EventManager();
            eventManager.SetAsPrimaryManager();

            Listener newListen = new Listener(this);
            eventManager.AddListener(newListen);

            _packetRegistry = VanillaProtocol.CreateRegistry();
            _packetRegistry.SetAsPrimaryInstance();

            _currentSocket = new NISocket();

            Thread serverThread = new Thread(() => {
                
                _currentSocket.OpenServerConnection("localhost", 22057);
                Debug.Log("Stopped Server");
            });
            
            serverThread.Start();
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