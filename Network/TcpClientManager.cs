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

        string password;


        /// <summary>
        /// Constructs a TcpClientManager.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="portNumber"></param>
        public TcpClientManager(string ipAddress, int portNumber, string password)
        {
            this.password = password;

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
                while (MD5Manager.PasswordsAreEqual != true)
                {
                    //Encode password before sending.
                    byte[] tmpPasswordEncoding = MD5Manager.EncodePassword(password);
                    //streamData = MD5.EncodePassword(password);

                    //Convert byte array to a string before sending.
                    streamData = MD5Manager.ByteArrayToString(tmpPasswordEncoding);

                    streamWriter.WriteLine(streamData);

                    //Attempt to send the data to the server and read incoming stream data.
                    try
                    {
                        streamWriter.Flush();
                    }

                    catch (IOException ioe)
                    {
                        Console.WriteLine("The Tcp server has been closed.");
                        Thread.CurrentThread.Abort();
                    }

                    try
                    {
                        streamData = streamReader.ReadLine();

                        if (streamData == "Incorrect password.")
                        {
                            Console.WriteLine("Incorrect password.");
                        }

                        else if (streamData != "Incorrect password.")
                        {
                            break;
                        }
                    }

                    catch (IOException ioe)
                    {
                        Console.WriteLine("The Tcp server has been closed.");
                        Thread.CurrentThread.Abort();
                    }
                }


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
