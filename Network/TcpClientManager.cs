using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    public class TcpClientManager : Client
    {
        private static TcpClient tcpClient;


        /// <summary>
        /// Constructs a TcpClientManager.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="portNumber"></param>
        public TcpClientManager(string ipAddress, int portNumber)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ipAddress, portNumber);

            remotePort = portNumber;

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
            remoteIPEndPoint = remoteEndPoint;

            ////Define and send streamData to the server.
            streamData = clientPort.ToString();

            streamWriter.WriteLine(streamData);

            while (tcpClient.Connected)
            {
                //Attempt to send the data to the server and read incoming stream data.
                try
                {
                    streamWriter.Flush();

                    //Incoming data from the server.
                    streamData = streamReader.ReadLine();
                }

                catch (IOException ioe)
                {
                    Console.WriteLine("The Tcp server has been closed.");
                    Thread.CurrentThread.Abort();
                }

                //Write out the data sent from the server.
                if (streamData != string.Empty)
                {
                    Console.WriteLine($"Recieved from Tcp server: {streamData}");
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
