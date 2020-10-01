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
    public class Server
    {
        private static int port = 11000;

        private UdpClient receivingUdpClient = new UdpClient(port);
        private UdpClient udpClient = new UdpClient();
        private string returnData;

        public int Port { get => port; }
        public string ReturnData { get => returnData; }

        private IPEndPoint remoteIpEndPoint; /*= new IPEndPoint(IPAddress.Any, 0);*/
        //private IPAddress tmpIPAdress;
        //private int tmpPort;

        public Server()
        {
            Thread receivingThread = new Thread(Recieve);
            receivingThread.IsBackground = true;
            receivingThread.Start();
        }

        /// <summary>
        /// This send information to the players (clients).
        /// </summary>
        public void Send(string message)
        {
            //if (tmpIPAdress.ToString() != "0.0.0.0")
            {
                try
                {
                    //udpClient.Connect(RemoteIpEndPoint.Address, RemoteIpEndPoint.Port);
                    //udpClient.Connect(tmpIPAdress, tmpPort);
                    udpClient.Connect("127.0.0.1", 13000);

                    Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
                    udpClient.Send(sendBytes, sendBytes.Length);
                }
                catch (Exception e)
                {
                    // Writes out the exception if any errors occur.
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Receives information from players (clients).
        /// </summary>
        private void Recieve()
        {
            Console.WriteLine("Waiting for a connection...");

            // This bool becomes false when the player exits the game.
            // Makes sure the thread dies so the game can shut down properly.
            while (GameWorld.Instance.ProgramRunning)
            {
                remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //tmpIPAdress = remoteIpEndPoint.Address;
                //tmpPort = remoteIpEndPoint.Port;

                try
                {
                    //// Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref remoteIpEndPoint);

                    returnData = Encoding.ASCII.GetString(receiveBytes);

                    if (returnData.StartsWith("Destroy"))
                    {
                        DeleteEnemiesAccordingToClient();
                    }

                    //test = Convert.ToInt32(Math.Round(Convert.ToDouble(returnData)));

                    //Console.WriteLine($"Test: {test}");

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

        private void DeleteEnemiesAccordingToClient()
        {
            string[] inputParameters = returnData.Split(',');

            int tmpID = Int32.Parse(inputParameters[1]);

            GameObject destroyedObject = (Enemy)GameWorld.Instance.GameObjects.Find(o => o.ID == tmpID);
            if (destroyedObject != null)
            {
                GameWorld.Instance.Destroy(destroyedObject);
            }
        }
    }
}
