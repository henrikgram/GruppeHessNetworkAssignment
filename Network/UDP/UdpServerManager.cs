using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    public class UdpServerManager : Server
    {
        private UdpClient receivingUdpClient = new UdpClient(serverPort);
        private UdpClient udpClient = new UdpClient();
        private IPEndPoint remoteIpEndPoint;
        private TimeSpan timeTillNewInvasionForce = new TimeSpan(0, 0, 2);
        private Random rnd = new Random(500);

        public string ReturnData { get; private set; }


        public UdpServerManager()
        {
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Thread receivingThread = new Thread(Recieve);
            receivingThread.IsBackground = true;
            receivingThread.Name = "Receive Thread Server";
            receivingThread.Start();
        }


        #region Methods

        /// <summary>
        /// Allows the server to send messages as bytearrays.
        /// </summary>
        public void Send(string message)
        {
            try
            {
                udpClient.Connect(inhRemoteIPEndPoint.Address, remotePort);

                Byte[] sendBytes = AesSymmetricEncryptor.Instance.EncryptString(message, AesSymmetricEncryptor.Instance.key);

                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception e)
            {
                // Writes out the exception if any errors occur.
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Method for receiving messages as byte arrays, which runs on a seperate thread. 
        /// </summary>
        private void Recieve()
        {
            Console.WriteLine("Waiting for a connection...");

            while (GameWorld.Instance.ProgramRunning)
            {
              remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    //Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref remoteIpEndPoint);
                    ReturnData = AesSymmetricEncryptor.Instance.DecryptStringFromBytes(receiveBytes, AesSymmetricEncryptor.Instance.key);

                    Console.WriteLine($"Server received: {ReturnData}");

                    HandleReturnData();
                }
                catch (Exception e)
                {
                    // Writes out the exception if any errors occur.
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Determines what functionality to run depending of the returndata.
        /// </summary>
        public void HandleReturnData()
        {
            if (GameWorld.Instance.IsServer == true && ReturnData != null && GameWorld.Instance.Instantiated)
            {

                if (ReturnData.Contains("New|Laser"))
                {
                    AddLasersAccordingToClient();
                }

                if (ReturnData.Contains("Destroy"))
                {
                    DeleteObjectsAccordingToClient();
                }

                if (ReturnData.Contains("Update|Player"))
                {
                    string[] inputParameters = ReturnData.Split('|');

                    float playPosX = float.Parse(inputParameters[2]);
                    float playposY = float.Parse(inputParameters[3]);

                    GameWorld.Instance.PlayerClient.Position = new Vector2(playPosX, playposY);
                }
            }
        }

        /// <summary>
        /// Deletes objects that have been deleted from the client
        /// </summary>
        private void DeleteObjectsAccordingToClient()
        {
            string[] inputParameters = ReturnData.Split('|');

            int tmpID = Int32.Parse(inputParameters[1]);

            GameObject destroyedObject = GameWorld.Instance.GameObjects.Find(o => o.ID == tmpID);

            if (destroyedObject != null)
            {
                GameWorld.Instance.Destroy(destroyedObject);
            }
        }

        /// <summary>
        /// Adds lasers based on input from the client
        /// </summary>
        private void AddLasersAccordingToClient()
        {
            string[] inputParameters = ReturnData.Split('|');

            int tmpID = Int32.Parse(inputParameters[2]);
            float tmpX = float.Parse(inputParameters[3]);
            float tmpY = float.Parse(inputParameters[4]);

            Laser newLaser = new Laser(new Vector2(tmpX, tmpY));
            newLaser.ID = tmpID;
            GameWorld.Instance.Instantiate(newLaser);
        }

        /// <summary>
        /// Update method for the server.
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateServer(GameTime gameTime)
        {
            // Only the server instantiates enemies.
            AddNewEnemyShipsServer();
            // The server tells the player where the enemies are.
            SendEnemyShipInfoToClient();

            if (timeTillNewInvasionForce > TimeSpan.Zero)
            {
                timeTillNewInvasionForce -= gameTime.ElapsedGameTime;
            }

            // Makes sure the server keeps sending server-players position to client.
            Send("Update|Player|" + GameWorld.Instance.PlayerServer.Position.X + "|" + GameWorld.Instance.PlayerServer.Position.Y);    
        }

        /// <summary>
        /// Prints out the highscore table.
        /// </summary>
        public void PrintHighscores()
        {
            // Shows the highscore in the console window once the players die.
            if (GameWorld.Instance.PlayerServer.IsDead && GameWorld.Instance.ShowHighscore)
            {
                Console.Clear();

                List<string> highscores = new List<string>();

                highscores = GameWorld.Instance.DBHandlerInstance.CreateHighscoreList();

                foreach (string score in highscores)
                {
                    Console.WriteLine(score);
                }

                GameWorld.Instance.ShowHighscore = false;
            }
        }

        /// <summary>
        /// Sends a wave of enemyships at set intervals of time. Also sends a message to the client to create ships at the specified
        /// positions
        /// </summary>
        private void AddNewEnemyShipsServer()
        {
            // A timer to make sure it only does it once every X-amount of time.
            if (timeTillNewInvasionForce <= TimeSpan.Zero)
            {
                for (int i = 0; i < 5; i++)
                {
                    Enemy tmpEnemy = new Enemy(new Vector2(rnd.Next(0, (int)GameWorld.Instance.ScreenSize.X - Asset.EnemySprite.Width), 0 - Asset.EnemySprite.Height)/*, enemyID*/);
                    GameWorld.Instance.NewGameObjects.Add(tmpEnemy);

                    GameWorld.Instance.ServerInstance.Send("New|Enemy|" + tmpEnemy.ID + "|" + tmpEnemy.Position.X + "|" + tmpEnemy.Position.Y);
                }
                timeTillNewInvasionForce = new TimeSpan(0, 0, 5);
            }
        }

        /// <summary>
        /// Sends information about the enemmy ships location to the client so their positions can be updated.
        /// </summary>
        private void SendEnemyShipInfoToClient()
        {
            List<GameObject> enemies = (GameWorld.Instance.GameObjects.FindAll(e => e is Enemy));

            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy currentEnemy = (Enemy)enemies[i];
                GameWorld.Instance.ServerInstance.Send("Update|Enemy|" + currentEnemy.ID + "|" + currentEnemy.Position.X + "|" + currentEnemy.Position.Y);
            }
        }

        #endregion
    }
}
