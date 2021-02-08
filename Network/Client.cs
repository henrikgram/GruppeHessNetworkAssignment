using System.Net;

namespace GruppeHessNetworkAssignment.Network
{
    /// <summary>
    /// This class is shared between the Udp and Tcp clients.
    /// For values that both Tcp and Udp should share.
    /// </summary>
    public abstract class Client
    {
        //The port that the server should send to.
        protected static int clientPort = 13000;

        //The port that the client should send to.
        protected static int remotePort = 11000;

        //This value is saved during Tcp connection.
        //It can then be used to automatically connect to the same IP during the Udp connection.
        protected static IPEndPoint inhRemoteIPEndPoint;
    }
}
