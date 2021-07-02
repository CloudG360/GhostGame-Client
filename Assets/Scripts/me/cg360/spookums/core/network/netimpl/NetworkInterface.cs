using System;
using System.Collections.Generic;
using me.cg360.spookums.core.network.packet.generic;
using me.cg360.spookums.core.network.packet;

namespace me.cg360.spookums.core.network.netimpl {

    /**
     *  The bare minimum interface to send data
     */
    public abstract class NetworkInterface {
    
        public abstract void OpenServerConnection(string hostname, int port);

        public abstract List<NetworkPacket> CheckForInboundPackets();
    
        public abstract void SendDataPacket(NetworkPacket packet, bool isUrgent);
        
        public virtual void Disconnect() { Disconnect(new PacketInOutDisconnect(null)); }
        public abstract void Disconnect(PacketInOutDisconnect disconnectPacket); // Closes the socked
        
        public abstract bool IsRunning();

    }
}
