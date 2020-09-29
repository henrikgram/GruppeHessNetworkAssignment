﻿using System;
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



        public Client(int port)
        {
            serverPort = port;

            Thread receivingThread = new Thread(Receive);
            receivingThread.IsBackground = true;
            receivingThread.Start();
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
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                try
                {

                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = recievingUdpClient.Receive(ref RemoteIpEndPoint);

                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine("Client received: " +
                                              returnData.ToString());
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
    }
}