using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    public class TcpServerManager : Server
    {
        private static TcpListener tcpServer;

        private static string password;

        /// <summary>
        /// Constructs a TCPServerManager.
        /// </summary>
        public TcpServerManager()
        {
            StartTcpServer(serverPort);
        }


        /// <summary>
        /// Starts and handles a Tcp Server.
        /// </summary>
        /// <param name="port"></param>
        public void StartTcpServer(int port)
        {
            tcpServer = new TcpListener(IPAddress.Any, port);
            tcpServer.Start();

            //The servers endpoint.
            IPEndPoint endPoint = (IPEndPoint)tcpServer.LocalEndpoint;

            string localIPAddress = GetLocalIP();

            //password = GeneratePassword();
            password = "12345678";

            Console.WriteLine($"Server on socket: {localIPAddress}:{endPoint.Port}");
            Console.WriteLine($"Server password: {password}");
            Console.WriteLine("Waiting for a connection...");

            AcceptTcpClient();
        }

        /// <summary>
        /// //Loop clients. This is used to handle clients, also more than one client.
        /// </summary>
        public void AcceptTcpClient()
        {
            //Instantiate a client that has been accepted to the server.
            TcpClient newTcpClient = tcpServer.AcceptTcpClient();

            //The clients endpoint (socket).
            IPEndPoint endPoint = (IPEndPoint)newTcpClient.Client.RemoteEndPoint;

            Console.WriteLine("New Tcp client!");

            //Each client is running on a thread.
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleTcpClient));
            clientThread.Start(newTcpClient);
        }

        /// <summary>
        /// Manage network streams between server and client.
        /// </summary>
        /// <param name="obj"></param>
        public static void HandleTcpClient(object obj)
        {
            //Create the tcp client. It's the same client that was previously accepted to the server (obj parameter).
            TcpClient tcpClient = (TcpClient)obj;

            //Create streamReader for reading datastreams (from server)
            //and streamWriter for writing and sending datastreams (to server).
            StreamReader streamReader = new StreamReader(tcpClient.GetStream(), Encoding.ASCII);
            StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream(), Encoding.ASCII);

            string streamData;

            //The clients IP.
            IPEndPoint remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;

            //Save the remote endpoint.
            inhRemoteIPEndPoint = remoteEndPoint;

            //Encode password.
            byte[] tmpPasswordEncoding = MD5Manager.EncodePassword(password);

            //Convert byte array to string.
            string tmpPassword = MD5Manager.ByteArrayToString(tmpPasswordEncoding);

            streamData = null;


            //Send and recieve streams as long as the connection i present.
            while (tcpClient.Connected)
            {
                try
                {
                    streamData = streamReader.ReadLine();

                    //Compare the two encoded passwords.
                    if (streamData != string.Empty && streamData != tmpPassword)
                    {
                        Console.WriteLine($"Recieved from Tcp client: {streamData}");

                        //What to send to the client.
                        streamData = "Incorrect password.";
                        streamWriter.WriteLine(streamData);
                    }

                    else if (streamData != string.Empty && streamData == tmpPassword)
                    {
                        Console.WriteLine($"Recieved from Tcp client: {streamData}");

                        streamData = "Correct password.";

                        streamWriter.WriteLine(streamData);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine("Client on Tcp port " + remoteEndPoint.Port.ToString() + " has left the Tcp server.");
                    Thread.CurrentThread.Abort();
                }


                streamWriter.WriteLine(streamData);

                //Attempt to send the data to the client.
                try
                {
                    streamWriter.Flush();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Client on Tcp port " + remoteEndPoint.Port.ToString() + " has left the Tcp server.");
                    Thread.CurrentThread.Abort();
                }

                Thread.CurrentThread.Abort();
                tcpServer.Stop();
            }

            if (!tcpClient.Connected)
            {
                Console.WriteLine("Connection closed.");
            }
        }

        /// <summary>
        /// Returns the local IP for the host (local computer).
        /// https://stackoverflow.com/questions/8709427/get-local-system-ip-address-using-c-sharp
        /// </summary>
        /// <returns></returns>
        private string GetLocalIP()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addresses = ipHostEntry.AddressList;

            return addresses[addresses.Length - 1].ToString();
        }

        private string GeneratePassword()
        {
            int pwLength = 8;
            Random random = new Random();
            string password = null;

            for (int i = 0; i < pwLength; i++)
            {
                password = password + random.Next(0, 9).ToString();
            }

            return password;
        }
    }
}
