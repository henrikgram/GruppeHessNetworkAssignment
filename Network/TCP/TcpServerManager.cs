using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GruppeHessNetworkAssignment.Network
{
    /// <summary>
    /// For managing the Server side of the Tcp connection.
    /// </summary>
    public class TcpServerManager : Server
    {
        private static TcpListener tcpServer;

        //The password used to verify the client attempting to connect.
        private static string password;

        private static Thread clientThread;


        /// <summary>
        /// Constructs a TCPServerManager.
        /// </summary>
        public TcpServerManager()
        {
            StartTcpServer();
        }


        #region Methods
        /// <summary>
        /// Starts and handles a Tcp Server.
        /// </summary>
        public void StartTcpServer()
        {
            tcpServer = new TcpListener(IPAddress.Any, serverPort);
            tcpServer.Start();

            //The servers endpoint.
            IPEndPoint endPoint = (IPEndPoint)tcpServer.LocalEndpoint;

            //Get the IP so it can be shown to the user in the console.
            //This is so the server host can forward the IP to the client.
            string localIPAddress = GetLocalIP();

            //A new random password is generated when a Tcp server starts.
            //Server host must forward the password to the client.
            password = GeneratePassword();
         
            Console.WriteLine($"Server on socket: {localIPAddress}:{endPoint.Port}");
            Console.WriteLine($"Server password: {password}");
            Console.WriteLine("Waiting for a connection...");

            AcceptTcpClient();
        }


        /// <summary>
        /// Accept clients. This is used to handle clients.
        /// </summary>
        public void AcceptTcpClient()
        {
            //Instantiate a client that has been accepted to the server.
            TcpClient newTcpClient = tcpServer.AcceptTcpClient();

            Console.WriteLine("New Tcp client!");

            //Each client is running on a thread.
            clientThread = new Thread(new ParameterizedThreadStart(HandleTcpClient));
            clientThread.IsBackground = true;
            clientThread.Start(newTcpClient);
        }


        /// <summary>
        /// Manage network streams between server and client.
        /// </summary>
        /// <param name="obj">Put an instance of a Tcp client here.</param>
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
            //This is shared between Udp and Tcp servers.
            inhRemoteIPEndPoint = remoteEndPoint;

            //Encode password. This allows us to compare with the hashed password sent by the client.
            byte[] tmpPasswordEncoding = MD5Manager.EncodePassword(password);

            //Convert byte array password to string.
            string tmpPassword = MD5Manager.ByteArrayToString(tmpPasswordEncoding);

            streamData = null;

            //TODO password betyder intet lol

            //Send and recieve streams as long as the connection i present.
            while (tcpClient.Connected)
            {
                try
                {
                    streamData = streamReader.ReadLine();

                    //Compare the hashed password sent by the client with the hashed server password.
                    if (streamData != string.Empty && streamData == tmpPassword)
                    {
                        Console.WriteLine($"Recieved from Tcp client: {streamData}");

                        //What to send to the client.
                        streamData = "Correct password.";

                        streamWriter.WriteLine(streamData);
                    }

                    //Compare the hashed password sent by the client with the hashed server password.
                    else if (streamData != string.Empty && streamData != tmpPassword)
                    {
                        Console.WriteLine($"Recieved from Tcp client: {streamData}");

                        //What to send to the client.
                        streamData = "Incorrect password.";

                        streamWriter.WriteLine(streamData);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine("Client on Tcp port " + remoteEndPoint.Port.ToString() + " has left the Tcp server.");
                    Thread.CurrentThread.Abort();
                }

                //Attempt to send the data to the client.
                try
                {
                    //The data determined by the password comparison is written to the stream.
                    streamWriter.WriteLine(streamData);

                    //Flush the stream.
                    streamWriter.Flush();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Client on Tcp port " + remoteEndPoint.Port.ToString() + " has left the Tcp server.");
                    Thread.CurrentThread.Abort();
                }

                //Close the connection after data has been passed.
                tcpClient.Close();

                break;
            }

            //Stop server and thread after closing the client connection.
            if (!tcpClient.Connected)
            { 
                tcpServer.Stop();

                clientThread.Abort();

                Console.WriteLine("Connection closed.");
            }
        }


        /// <summary>
        /// Returns the local IP for the host (local computer).
        /// https://stackoverflow.com/questions/8709427/get-local-system-ip-address-using-c-sharp
        /// </summary>
        /// <returns>Returns the players IP.</returns>
        private string GetLocalIP()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addresses = ipHostEntry.AddressList;

            return addresses[addresses.Length - 1].ToString();
        }


        /// <summary>
        /// Generate and return a random password.
        /// 8 characters long, only numbers.
        /// </summary>
        /// <returns>Returns the random password.</returns>
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
        #endregion
    }
}
