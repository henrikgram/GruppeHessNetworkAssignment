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
    public class Client
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
            receivingThread.Name = "Receive Thread Client";
            receivingThread.Start();

            Send("P");
            GameWorld.Instance.PlayerCount++;
        }

        public void Send(string message)
        {
            //while (true)
            {

                try
                {
                    udpClient.Connect("127.0.0.1", serverPort);

                    // Sends a message to the host to which you have connected.
                    //Byte[] sendBytes = Encoding.ASCII.GetBytes(message);

                    byte[] sendBytes = AesSymmetricEncryptor.Instance.EncryptString(message, AesSymmetricEncryptor.Instance.key);

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

                    //returnData = AesEncryptor.Instance.DecryptStringFromBytes(receiveBytes, key);

                    //returnData = Encoding.ASCII.GetString(receiveBytes);
                    returnData = AesSymmetricEncryptor.Instance.DecryptStringFromBytes(receiveBytes, AesSymmetricEncryptor.Instance.key);



                    Console.WriteLine("Client received: " +
                                              returnData.ToString());

                    HandleReturnData();
                    //Console.WriteLine("This message was sent from " +
                    //                            RemoteIpEndPoint.Address.ToString() +
                    //                            " on their port number " +
                    //                            RemoteIpEndPoint.Port.ToString());

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void HandleReturnData()
        {
            if (GameWorld.Instance.IsServer == false && returnData != null && GameWorld.Instance.Instantiated)
            {
                // Adds a new point once an enemy has been hit. "Point" is sent from the laser class.
                if (returnData == "Point")
                {
                    Highscore.Instance.Points++;
                }

                if (returnData == "Lose hp")
                {
                    GameWorld.Instance.PlayerServer.PlayerHealth--;
                }

                if (returnData.Contains("New|Laser"))
                {
                    AddLasersAccordingToServer();
                }

                if (returnData.Contains("New|Enemy"))
                {
                    AddNewEnemies();
                }

                if (returnData.Contains("Update|Enemy"))
                {
                    UpdateCurrentEnemies();
                }

                if (returnData.Contains("Destroy"))
                {
                    DeleteObjectsAccordingToServer();
                }

                if (returnData.Contains("Update|Player"))
                {
                    string[] inputParameters = returnData.Split('|');

                    float playPosX = float.Parse(inputParameters[2]);
                    float playposY = float.Parse(inputParameters[3]);

                    GameWorld.Instance.PlayerServer.Position = new Vector2(playPosX, playposY);
                }

            }
        }

        /// <summary>
        /// Adds new enemies on the client when they are added on the server.
        /// </summary>
        private void AddNewEnemies()
        {
            string[] inputParameters = returnData.Split('|');

            string objectType = inputParameters[1];
            int tmpID = Int32.Parse(inputParameters[2]);
            float tmpx = float.Parse(inputParameters[3]);
            float tmpy = float.Parse(inputParameters[4]);

            Enemy newEnemy = new Enemy(new Vector2(tmpx, tmpy));
            newEnemy.ID = tmpID;
            GameWorld.Instance.NewGameObjects.Add(newEnemy);
        }

        /// <summary>
        /// Updates the positions of enemies according to their position on the server.
        /// </summary>
        private void UpdateCurrentEnemies()
        {
            string[] inputParameters = returnData.Split('|');

            string objectType = inputParameters[1];
            int tmpID = Int32.Parse(inputParameters[2]);
            float tmpx = float.Parse(inputParameters[3]);
            float tmpy = float.Parse(inputParameters[4]);

            Enemy tmpEnemy = (Enemy)GameWorld.Instance.GameObjects.Find(e => e is Enemy && e.ID == tmpID);
            if (tmpEnemy != null/* && tmpEnemy.Position != new Vector2(tmpx,tmpy)*/)
            {
                tmpEnemy.Position = new Vector2(tmpx, tmpy);
            }
        }

        /// <summary>
        /// Adds lasers based on input from the client
        /// </summary>
        private void AddLasersAccordingToServer()
        {
            string[] inputParameters = returnData.Split('|');

            int tmpID = Int32.Parse(inputParameters[2]);
            float tmpX = float.Parse(inputParameters[3]);
            float tmpY = float.Parse(inputParameters[4]);

            Laser newLaser = new Laser(new Vector2(tmpX, tmpY));
            newLaser.ID = tmpID;
            GameWorld.Instance.Instantiate(newLaser);
        }

        /// <summary>
        /// Deletes objects that have been deleted on the server.
        /// </summary>
        private void DeleteObjectsAccordingToServer()
        {
            string[] inputParameters = returnData.Split('|');

            int tmpID = Int32.Parse(inputParameters[1]);

            GameObject destroyedObject = (Enemy)GameWorld.Instance.GameObjects.Find(o => o.ID == tmpID);
            if (destroyedObject != null)
            {
                GameWorld.Instance.Destroy(destroyedObject);
            }
        }
    }
}
