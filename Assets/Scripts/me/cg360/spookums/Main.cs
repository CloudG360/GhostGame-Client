using me.cg360.spookums.core.eventsys;
using me.cg360.spookums.core.eventsys.handler;
using me.cg360.spookums.core.eventsys.type.network;
using me.cg360.spookums.core.network.netimpl.socket;
using me.cg360.spookums.core.network.packet.generic;
using System.Threading;
using UnityEngine;

namespace me.cg360.spookums
{
    public class Main : MonoBehaviour
    {
        private void Start()
        {
            EventManager eventManager = new EventManager();
            eventManager.SetAsPrimaryManager();

            Listener newListen = new Listener(this);
            eventManager.AddListener(newListen);

            NISocket socket = new NISocket();
            bool sendPackets = true;

            Thread serverThread = new Thread(() => {

                
                socket.OpenServerConnection("localhost", 22057);
                Debug.Log("Stopped Server");
                sendPackets = false;
            });

            Thread packetSentThread = new Thread(() =>
            {
                int i = 0;
                while(sendPackets)
                {
                    Thread.Sleep(5000);
                    socket.SendDataPacket(new PacketInOutChatMessage($"Test Message #{i}"), false);
                    i++;
                }
            });
            
            packetSentThread.Start();
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
        }
    }
}