using Microsoft.Xna.Framework;
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
    public class Client
    {
        private static int port = 13000;
        private int serverPort;
        private string returnData;

        private UdpClient recievingUdpClient = new UdpClient(port);
        private UdpClient udpClient = new UdpClient(/*port*/);


        public Client(int port)
        {
            serverPort = port;

            Thread receivingThread = new Thread(Receive);
            receivingThread.IsBackground = true;
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
                string clientInput = returnData;

                if (clientInput == "s")
                {
                    GameWorld.Instantiate(new Laser(new Vector2(GameWorld.Instance.PlayerServer.Position.X + Asset.playerSprite.Width / 2 - 5, GameWorld.Instance.PlayerServer.Position.Y - 30)));
                }
                // Adds a new point once an enemy has been hit. "Point" is sent from the laser class.
                else if (clientInput == "Point")
                {
                    Highscore.Instance.Points++;
                }
                else if (clientInput == "Lose hp")
                {
                    GameWorld.Instance.PlayerServer.PlayerHealth--;
                }
                else
                {
                    int test = Convert.ToInt32(Math.Round(Convert.ToDouble(returnData)));

                    GameWorld.Instance.PlayerServer.Position = new Vector2(test, GameWorld.Instance.PlayerServer.Position.Y);
                }
            }
        }
    }
}