using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    public class TcpClientManager : Client
    {
        private static TcpClient tcpClient;

        string password;


        /// <summary>
        /// Constructs a TcpClientManager.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="portNumber"></param>
        public TcpClientManager(string ipAddress, string password)
        {
            this.password = password;

            tcpClient = new TcpClient();
            tcpClient.Connect(ipAddress, remotePort);

            Console.WriteLine("Connected!");

            HandleCommunication();
        }


        /// <summary>
        /// Handle the network streams between server and client.
        /// </summary>
        public void HandleCommunication()
        {
            //Create streamReader for reading datastreams (from server)
            //and streamWriter for writing and sending datastreams (to server).
            StreamReader streamReader = new StreamReader(tcpClient.GetStream(), Encoding.UTF8);
            StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8);

            string streamData;

            //The servers endpoint (socket).
            IPEndPoint remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;

            //Save the remote endpoint.
            inhRemoteIPEndPoint = remoteEndPoint;

            //Encode password before sending.
            byte[] tmpPasswordEncoding = MD5Manager.EncodePassword(password);

            //Convert byte array to a string before sending.
            streamData = MD5Manager.ByteArrayToString(tmpPasswordEncoding);

            streamWriter.WriteLine(streamData);


            while (tcpClient.Connected)
            {
                try
                {
                    //Attempt to send the data to the server and read incoming stream data.
                    streamWriter.Flush();

                    streamData = streamReader.ReadLine();
                }

                catch (IOException ioe)
                {
                    Console.WriteLine("The Tcp server has been closed.");
                    Thread.CurrentThread.Abort();
                }

                if (streamData != string.Empty && streamData == "Correct password.")
                {
                    Console.WriteLine($"Recieved from Tcp server: {streamData}");

                    Console.WriteLine("Correct password.");
                }

                else if (streamData != string.Empty && streamData == "Incorrect password.")
                {
                    Console.WriteLine($"Recieved from Tcp server: {streamData}");

                    Console.WriteLine("Incorrect password.");
                }

                tcpClient.Close();
            }

            if (!tcpClient.Connected)
            {
                Console.WriteLine("Connection closed.");
            }
        }
    }
}
