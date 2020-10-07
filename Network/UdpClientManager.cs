using Microsoft.Xna.Framework;
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

        private string returnData;

        public string ReturnData
        {
            get { return returnData; }
            set { returnData = value; }
        }


        public UdpClientManager()
        {
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Thread receivingThread = new Thread(Receive);
            receivingThread.IsBackground = true;
            receivingThread.Name = "Receive Thread Client";
            receivingThread.Start();

            Send("P");
            GameWorld.Instance.PlayerCount++;
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
            while (GameWorld.Instance.ProgramRunning)
            {
              IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    //Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = recievingUdpClient.Receive(ref remoteIpEndPoint);

                    returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine($"Client received: {returnData.ToString()}");

                    HandleReturnData();
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

            GameObject destroyedObject = GameWorld.Instance.GameObjects.Find(o => o.ID == tmpID);
            if (destroyedObject != null)
            {
                GameWorld.Instance.Destroy(destroyedObject);
            }
        }
    }
}
