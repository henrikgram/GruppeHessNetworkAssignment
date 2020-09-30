using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment
{
    public enum PacketType
    { 
      PLAYERPOSITION,
      PLAYERSHOOT,
      OBJECTUPDATE
    }


    class Packet
    {
        public PacketType packetType { get; private set; }
        public long Timestamp;


        public void Send(UdpClient client, IPEndPoint receiver)
        {

            //byte[] bytes = Encoding.ASCII.GetBytes();

        }



        
    }
}
