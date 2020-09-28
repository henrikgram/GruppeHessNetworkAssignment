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
        private static int port = 11000;

        private  UdpClient receivingUdpClient = new UdpClient(port);
        private  UdpClient udpClient = new UdpClient();

        public int Port { get => port; }

        //private static IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);


        public Server ()
        {
            Thread receivingThread = new Thread(Recieve);
            receivingThread.IsBackground = true;
            receivingThread.Start();

            //Thread sendingThread = new Thread(SendMethod);
            //sendingThread.IsBackground = true;
            //sendingThread.Start();
        }

        /// <summary>
        /// Receives information from players (clients).
        /// </summary>
        private void Recieve()
        {
            Console.WriteLine("Waiting for a connection...");

            while (GameWorld.Instance.ProgramRunning)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    //// Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                    string returnData = Encoding.UTF8.GetString(receiveBytes);

                    Console.WriteLine($"Server received: {returnData}");
                    //Console.WriteLine($"Message was sent from: {RemoteIpEndPoint.Address.ToString()} \nOn port number: {RemoteIpEndPoint.Port.ToString()}");
                    //Console.WriteLine();
                }
                catch (Exception e)
                {
                    // Writes out the exception if any errors occur.
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// This send information to the players (clients).
        /// </summary>
        public void Send(string message)
        {
            // Makes sure the thread keeps running until the game is closed.
            //while (GameWorld.Instance.ProgramRunning)
            {
                //tmpIPAddress = RemoteIpEndPoint.Address;

                try
                {
                    //udpClient.Connect(RemoteIpEndPoint.Address, port);
                    udpClient.Connect("127.0.0.1" , 13000);

                    //message = "Hvaså smukke pige skal du ind i mujaffas bmw?";

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
