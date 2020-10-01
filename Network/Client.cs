using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment.Network
{
    class Client
    {
        private static int port = 13000;
        private int serverPort;

        private UdpClient recievingUdpClient = new UdpClient(port);
        private UdpClient udpClient = new UdpClient(/*port*/);
        private string returnData;

        public string ReturnData
        {
            get { return returnData; }
            set { returnData = value; }
        }

        public Client(int port)
        {
            serverPort = port;

            Thread receivingThread = new Thread(Receive);
            receivingThread.IsBackground = true;
            receivingThread.Start();

            //GameWorld.Instance.PlayerCount++;
            //Send("P");
        }


        public void Send(string message)
        {
            //while (true)
            {

                try
                {
                    udpClient.Connect("127.0.0.1", serverPort);

                    // Sends a message to the host to which you have connected.
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(message);

                    udpClient.Send(sendBytes, sendBytes.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void Receive()
        {
            // This bool becomes false when the player exits the game.
            // Makes sure the thread dies so the game can shut down properly.
            while (GameWorld.Instance.ProgramRunning)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = recievingUdpClient.Receive(ref RemoteIpEndPoint);

                    returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine("Client received: " +
                                              returnData.ToString());
                    //Console.WriteLine("This message was sent from " +
                    //                            RemoteIpEndPoint.Address.ToString() +
                    //                            " on their port number " +
                    //                            RemoteIpEndPoint.Port.ToString());
                    
                    if (returnData.Contains("New,Enemy"))
                    {
                        AddNewEnemies();
                    }

                    if (returnData.Contains("Update,Enemy,"))
                    {
                        UpdateCurrentEnemies();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void AddNewEnemies()
        {
            string[] inputParameters = returnData.Split(',');

            string objectType = inputParameters[1];
            int tmpID = Int32.Parse(inputParameters[2]);
            int tmpx = Int32.Parse(inputParameters[3]);
            //int tmpy = Int32.Parse(inputParameters[1]);
            //ClientInstance.ReturnData = null;

            switch (objectType)
            {
                case ("Enemy"):
                    GameWorld.Instance.NewGameObjects.Add(new Enemy(new Vector2(tmpx, 0-Asset.enemySprite.Height), tmpID));
                    break;

                    //case ("Laser"):
                    //    newGameObjects.Add(new Laser(new Vector2(tmpx, tmpy), tmpID));
                    //    break;
            }
        }

        private void UpdateCurrentEnemies()
        {
            string[] inputParameters = returnData.Split(',');

            string objectType = inputParameters[1];
            int tmpID = Int32.Parse(inputParameters[2]);
            int tmpx = Int32.Parse(inputParameters[3]);
            int tmpy = Int32.Parse(inputParameters[4]);

            Enemy tmpEnemy = (Enemy)GameWorld.Instance.GameObjects.Find(e => e.ID == tmpID);
            if (tmpEnemy != null)
            {
                tmpEnemy.Position = new Vector2(tmpx, tmpy);
                Console.WriteLine("Here!");
            }
        }
    }
}