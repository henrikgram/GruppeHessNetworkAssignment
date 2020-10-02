using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    class UdpServerManager : Server
    {
        private UdpClient receivingUdpClient = new UdpClient(serverPort);
        private UdpClient udpClient = new UdpClient();

        private IPEndPoint remoteIpEndPoint;


        public UdpServerManager()
        {
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Thread receivingThread = new Thread(Recieve);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        /// <summary>
        /// This send information to the players (clients).
        /// </summary>
        public void Send(string message)
        {
            try
            {
                udpClient.Connect(remoteIpEndPoint.Address, remotePort);

                Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                // Writes out the exception if any errors occur.
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Receives information from players (clients).
        /// </summary>
        private void Recieve()
        {
            while (true)
            {
                try
                {
                    //Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref remoteIpEndPoint);

                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine($"Server received: {returnData}");
                }
                catch (Exception e)
                {
                    // Writes out the exception if any errors occur.
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
