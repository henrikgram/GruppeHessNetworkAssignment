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

        private string returnData;

        //public int Port { get => port; }
        public string ReturnData { get => returnData; set => returnData = value; }

        private IPEndPoint remoteIpEndPoint;


        public UdpServerManager()
        {
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Thread receivingThread = new Thread(Recieve);
            receivingThread.IsBackground = true;
            receivingThread.Name = "Receive Thread Server";
            receivingThread.Start();
        }

        /// <summary>
        /// This send information to the players (clients).
        /// </summary>
        public void Send(string message)
        {
            try
            {
                udpClient.Connect(inhRemoteIPEndPoint.Address, remotePort);

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

            Console.WriteLine("Waiting for a connection...");

            while (GameWorld.Instance.ProgramRunning)
            {
              remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    //Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref remoteIpEndPoint);

                    returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine($"Server received: {returnData}");

                    HandleOtherPlayer();
                    //if (returnData.Contains("Update|Player"))
                    //{
                    //    string[] inputParameters = returnData.Split('|');

                    //    float playPosX = float.Parse(inputParameters[2]);
                    //    float playposY = float.Parse(inputParameters[3]);

                    //    GameWorld.Instance.PlayerClient.Position = new Vector2(playPosX, playposY);
                    //}
                }
                catch (Exception e)
                {
                    // Writes out the exception if any errors occur.
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Deletes objects that have been deleted from the client
        /// </summary>
        private void DeleteObjectsAccordingToClient()
        {
            string[] inputParameters = returnData.Split('|');

            int tmpID = Int32.Parse(inputParameters[1]);

            GameObject destroyedObject = (Enemy)GameWorld.Instance.GameObjects.Find(o => o.ID == tmpID);

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
            string[] inputParameters = returnData.Split('|');

            int tmpID = Int32.Parse(inputParameters[2]);
            float tmpX = float.Parse(inputParameters[3]);
            float tmpY = float.Parse(inputParameters[4]);

            Laser newLaser = new Laser(new Vector2(tmpX, tmpY));
            newLaser.ID = tmpID;
            GameWorld.Instance.Instantiate(newLaser);
        }


        public void HandleOtherPlayer()
        {
            if (GameWorld.Instance.IsServer == true && returnData != null && GameWorld.Instance.Instantiated)
            {

                if (returnData.Contains("New|Laser"))
                {
                    AddLasersAccordingToClient();
                }
                //// Adds a new point once an enemy has been hit. "Point" is sent from the laser class.
                //else if (serverInput == "Point")
                //{
                //    Highscore.Instance.Points++;
                //}
                //string serverInput = GameWorld.Instance.ServerInstance.ReturnData;

                if (returnData.Contains("Destroy"))
                {
                    DeleteObjectsAccordingToClient();
                }

                if (returnData.Contains("Update|Player"))
                {
                    string[] inputParameters = returnData.Split('|');

                    float playPosX = float.Parse(inputParameters[2]);
                    float playposY = float.Parse(inputParameters[3]);

                    GameWorld.Instance.PlayerClient.Position = new Vector2(playPosX, playposY);
                }
            }
        }
    }
}
