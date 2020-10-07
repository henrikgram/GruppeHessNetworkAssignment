using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    /// <summary>
    /// For managing the Client side of the Tcp connection.
    /// </summary>
    public class TcpClientManager : Client
    {
        private static TcpClient tcpClient;

        //This password is used to verify the client trying to connect to the server.
        string password;


        /// <summary>
        /// Constructs a TcpClientManager.
        /// </summary>
        /// <param name="ipAddress">The IP you want to connect to.</param>
        /// <param name="password">The password to send to the Server to connect.</param>
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

            //Save the remote endpoint to the Tcp and Udp shared variable.
            inhRemoteIPEndPoint = remoteEndPoint;

            //Hash password before sending to the server.
            byte[] tmpPasswordEncoding = MD5Manager.EncodePassword(password);

            //Convert byte array to a string before sending.
            streamData = MD5Manager.ByteArrayToString(tmpPasswordEncoding);

            if (tcpClient.Connected)
            {
                try
                {
                    //Send data to the stream.
                    streamWriter.WriteLine(streamData);

                    //Flush the stream.
                    streamWriter.Flush();

                    //Read data from the incoming stream.
                    streamData = streamReader.ReadLine();
                }

                catch (IOException ioe)
                {
                    Console.WriteLine("The Tcp server has been closed.");
                    Thread.CurrentThread.Abort();
                }

                //Checks to see if the server accepted the password sent by the client.
                if (streamData != string.Empty && streamData == "Correct password.")
                {
                    Console.WriteLine($"Recieved from Tcp server: {streamData}");

                    Console.WriteLine("Correct password.");
                }

                //Checks to see if the server accepted the password sent by the client.
                else if (streamData != string.Empty && streamData == "Incorrect password.")
                {
                    Console.WriteLine($"Recieved from Tcp server: {streamData}");

                    Console.WriteLine("Incorrect password.");
                }

                //Close the client after stream data has been passed between client and server.
                tcpClient.Close();
            }

            if (!tcpClient.Connected)
            {
                Console.WriteLine("Connection closed.");
            }
        }
    }
}
