using System.Net;

namespace GruppeHessNetworkAssignment.Network
{
    public abstract class Server
    {
        protected static int serverPort = 11000;

        protected static int remotePort = 13000;

        protected static IPEndPoint inhRemoteIPEndPoint;
    }
}
