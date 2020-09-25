using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment.Network
{
    class Server
    {
        private static UdpClient receivingUdpClient = new UdpClient(port);
        private static UdpClient udpClient = new UdpClient(port);

        private static IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        private static int port = 11000;

        public Server ()
        {
            Thread receivingThread = new Thread(Receiver);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        /// <summary>
        /// Receives information from players (clients).
        /// </summary>
        private static void Receiver()
        {
            Console.WriteLine("Waiting for a connection...");
            try
            {

            }
            catch (Exception e)
            {
                // Writes out the exception if any errors occur.
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// This send information to the players (clients).
        /// </summary>
        public void SendMethod()
        {
            IPAddress tmpIPAddress = null;
            string message = "Fuck off";
            // Makes sure the thread keeps running until the game is closed.
            while (GameWorld.Instance.ProgramRunning)
            {
                tmpIPAddress = RemoteIpEndPoint.Address;

                try
                {
                    udpClient.Connect(tmpIPAddress, port);

                    Byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                    udpClient.Send(sendBytes, sendBytes.Length);
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
