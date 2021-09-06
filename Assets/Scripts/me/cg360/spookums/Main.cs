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

        public Client Client { get; private set; }

        // Things that need to be done else it won't start
        // networking correctly:
        // - Set VanillaProtocol.CreateRegistry(); as the primary instance.
        // - Create an NI Socket and open it on a GameThread (Port 22057 is default)
        // - start that thread.
        
        private void Start()
        {
            GameThread.StartThreadChecking();
            Client = new Client();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnDisable()
        {
            GameThread.StopThreadChecking();
        }
    }
}