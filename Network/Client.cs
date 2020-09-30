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
    class Client
    {
        private static int port = 13000;
        private int serverPort;

        private UdpClient recievingUdpClient = new UdpClient(port);
        private UdpClient udpClient = new UdpClient(/*port*/);

        private string returnData;
        public string ReturnData { get => returnData; }

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

                    //Console.WriteLine("Client received: " + returnData.ToString());
                    //Console.WriteLine("This message was sent from " +
                    //                            RemoteIpEndPoint.Address.ToString() +
                    //                            " on their port number " +
                    //                            RemoteIpEndPoint.Port.ToString());

                    UpdateAccordingToServer();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void UpdateAccordingToServer()
        {
            ////sending position to the server
            //client.Send(player.Position.X.ToString());

            //saves the recieved message from server
            string input = ReturnData;


            //makes sure to only work with Object position strings
            if (input.Contains("OP"))
            {
                //removes the "OP" tag
                input = input.Remove(0, 2);

                //splitting the string into multiple object strings
                string[] inputObjects = input.Split('|');


                //add new object if the amount is not the same
                if (GameWorld.Instance.GameObjects.Count - 1 < inputObjects.Length - 1)
                {
                    for (int i = 0; i != inputObjects.Length - 1; i++)
                    {
                        string[] inputParameters = inputObjects[i].Split(',');

                        int tmpx =  Int32.Parse(inputParameters[0]);
                        int tmpy =  Int32.Parse(inputParameters[1]);
                        int tmpID = Int32.Parse(inputParameters[2]);
                        string objectType = inputParameters[3];

                        switch (objectType)
                        {
                            case ("Enemy"):
                                GameWorld.Instance.NewGameObjects.Add(new Enemy(new Vector2(tmpx, tmpy), tmpID));
                                break;

                                //case ("Laser"):
                                //    newGameObjects.Add(new Laser(new Vector2(tmpx, tmpy), tmpID));
                                //    break;
                        }

                    }
                }

                else
                {
                    //update object

                    for (int i = 0; i < inputObjects.Length - 1; i++)
                    {
                        string[] inputParameters = inputObjects[i].Split(',');

                        int tmpx = Int32.Parse(inputParameters[0]);
                        int tmpy = Int32.Parse(inputParameters[1]);
                        int tmpID = Int32.Parse(inputParameters[2]);

                        foreach (GameObject gameObject in GameWorld.Instance.GameObjects)
                        {
                            if (gameObject.Id == tmpID)
                            {
                                gameObject.Position = new Vector2(tmpx, tmpy);
                            }
                        }
                    }
                }

                //Console.WriteLine(inputObjects.Length);
            }
        }
    }
}