using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    public class UdpClientManager : Client
    {
        private UdpClient recievingUdpClient = new UdpClient(clientPort);
        private UdpClient udpClient = new UdpClient();

        IPEndPoint remoteIpEndPoint;


        public UdpClientManager()
        {
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Thread receivingThread = new Thread(Receive);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }


        public void Send(string message)
        {
            try
            {
                udpClient.Connect(inhRemoteIPEndPoint.Address, remotePort);

                // Sends a message to the host to which you have connected.
                Byte[] sendBytes = Encoding.ASCII.GetBytes(message);

                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive()
        {
            while (true)
            {
                try
                {
                    //Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = recievingUdpClient.Receive(ref remoteIpEndPoint);

                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine($"Client received: {returnData.ToString()}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}