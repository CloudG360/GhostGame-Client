using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using me.cg360.spookums.core.eventsys;
using me.cg360.spookums.core.eventsys.type.network;
using me.cg360.spookums.core.network.packet;
using me.cg360.spookums.core.network.packet.generic;
using me.cg360.spookums.utility;
using net.cg360.spookums.core.network;
using UnityEngine;

namespace me.cg360.spookums.core.network.netimpl.socket
{
    public class NISocket : NetworkInterface
    {

        protected Socket ClientSocket;
        protected Thread ListenerThread;
        protected bool IsSocketRunning;

        public NISocket()
        {
            ClientSocket = null;
            IsSocketRunning = false;
        }

        protected void StartPacketListenerThread()
        {
            ListenerThread = new Thread(PacketListenerProcess);
            ListenerThread.Start();
            ListenerThread.Join();
        }

        protected void PacketListenerProcess()
        {
            while (IsRunning())
            {
                CheckForInboundPackets();
            }
        }

        public override void OpenServerConnection(string hostname, int port)
        {
            if (!IsRunning())
            {
                
                try
                {
                    IPHostEntry entry = Dns.GetHostEntry(hostname);
                    IPAddress ip = entry.AddressList[1];
                    IPEndPoint endpoint = new IPEndPoint(ip, port);

                    foreach (IPAddress address in entry.AddressList)
                    {
                        Debug.Log(address.ToString());
                    }

                    //Probs should be UDP
                    Socket client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    try
                    {
                        client.SendBufferSize = VanillaProtocol.MAX_BUFFER_SIZE;
                        client.ReceiveBufferSize = VanillaProtocol.MAX_BUFFER_SIZE;
                        client.ReceiveTimeout = 30000;
                        client.SendTimeout = 30000;
                        client.Connect(endpoint);
                        ClientSocket = client;
                        IsSocketRunning = true;

                        StartPacketListenerThread();
                    }
                    catch (Exception err)
                    {
                        Debug.Log("Error connecting:");
                        Debug.LogException(err);
                    }

                }
                catch (Exception err)
                {
                    Debug.Log("Error Resolving:");
                    Debug.LogException(err);
                }
            }
        }

        public override List<NetworkPacket> CheckForInboundPackets()
        {
            if(IsRunning())
            {
                try
                {
                    lock (ClientSocket)
                    {
                        if (ClientSocket.Available >= 2)
                        {
                            byte[] sizeBytes = new byte[2];
                            int sizeByteCount = ClientSocket.Receive(sizeBytes, sizeBytes.Length, 0);
                            NetworkBuffer sizeBuf = NetworkBuffer.Wrap(sizeBytes);
                            
                            ushort packetSize = sizeBuf.GetUnsignedShort();
                            byte[] bodyBytes = new byte[packetSize];
                            int bodyByteCount = ClientSocket.Receive(bodyBytes, bodyBytes.Length, 0);

                            if ((packetSize > 1) && (bodyByteCount == packetSize))
                            {
                                NetworkBuffer bodyBuffer = NetworkBuffer.Wrap(bodyBytes);
                                byte id = bodyBuffer.Get();

                                if (PacketRegistry.Get().GetPacketType(id, out Type packetType))
                                {
                                    NetworkPacket packet = (NetworkPacket) Activator.CreateInstance(packetType);

                                    NetworkBuffer packetBuffer =
                                        NetworkBuffer.Wrap(new byte[sizeByteCount + bodyByteCount]);
                                    packetBuffer.Put(sizeBytes);
                                    packetBuffer.Put(bodyBytes);

                                    packet.Decode(packetBuffer);

                                    PacketEvent.Recieved pEvent = new PacketEvent.Recieved(packet);
                                    EventManager.Get().Call(pEvent);

                                    return pEvent.IsCancelled()
                                        ? new List<NetworkPacket>()
                                        : new List<NetworkPacket>(new NetworkPacket[] {packet});
                                }
                            }
                        }
                    }


                } catch(Exception err)
                {
                    Debug.LogException(err);
                }
            }

            return new List<NetworkPacket>();
        }

        public override void SendDataPacket(NetworkPacket packet, bool isUrgent)
        {
            if (IsRunning())
            {
                int length;
                NetworkBuffer data;
                PacketEvent pEvent;
                lock(packet)
                {
                    data = packet.Encode(out length);
                    pEvent = new PacketEvent.Sent(packet);
                    EventManager.Get().Call(pEvent);
                }
                
                data.Reset();
                
                if (!pEvent.IsCancelled())
                {
                    byte[] sizeData = new byte[2];
                    byte[] packetData = new byte[length - 2];
                    data.Get(sizeData);
                    data.Get(packetData);
                    
                    ClientSocket.Send(sizeData, sizeData.Length, 0);
                    ClientSocket.Send(packetData, packetData.Length, 0);
                }
            }
        }

        public override void Disconnect(PacketInOutDisconnect disconnectPacket)
        {
            if(IsRunning())
            {
                lock (ClientSocket)
                {
                    SendDataPacket(disconnectPacket, true);
                    ListenerThread.Interrupt();
                    IsSocketRunning = false;
                    ClientSocket.Shutdown(SocketShutdown.Both);
                    ClientSocket.Close();
                }
            }
        }

        public override bool IsRunning()
        {
            if (ClientSocket != null)
            {
                lock (ClientSocket)
                {
                    return IsSocketRunning && (ClientSocket != null) && ClientSocket.Connected;
                }
            }
            return false;
        }
    }
}