using System.Net;

namespace GruppeHessNetworkAssignment.Network
{
    public abstract class Client
    {
        protected static int clientPort = 13000;

        protected static int remotePort;

        protected static IPEndPoint remoteIPEndPoint;
    }
}
