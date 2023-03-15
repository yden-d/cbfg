using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Game
{
    internal class Server
    {

        public static int maxPlayers { get; private set; }

        public static int currPlayers;

        public static int port { get; private set; }

        //dictionary to keep track of clients ... key == clientID
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        //server needs to know which packet method to call based on packet id
        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        //method that will start the server
        //Params: maxP = max players, p = port
        public static void Start(int maxP, int p)
        {
            maxPlayers = maxP;
            port = p;

            //populate client dictionary
            InitializeServerData();

            //start server
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            //params: async callback method and null object state
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on port: {port}");
        }

        //attempt for client to connect to server
        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            //check for open spot in server
            for (int i = 1; i <= maxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);
                    Console.WriteLine($"Successful connection for player at: {client.Client.RemoteEndPoint}");
                    return;
                }
            }
            Console.WriteLine($"Server is full, could not connect player at: {client.Client.RemoteEndPoint}");
        }

        
        private static void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //sets IPEndPoint to the data source EndPoint
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                //check if we can read int
                if(data.Length < 4)
                {
                    return;

                }
                using (Packet p = new Packet(data))
                {
                    //read client id
                    int cId = p.ReadInt();
                    if (cId == 0) return;

                    //check if the connection is new
                    if (clients[cId].udp.endPoint == null)
                    {
                        Console.WriteLine("New UDP connection");
                        clients[cId].udp.Connect(clientEndPoint);
                        return;
                    }

                    //check if endPoints match before handling data
                    if (clients[cId].udp.endPoint.ToString() == clientEndPoint.ToString())
                    {
                        clients[cId].udp.HandleData(p);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
        }

        //ensures endPoint is valid before calling UDPClient BeginSend()
        public static void SendUDPData(IPEndPoint clientEP, Packet packet)
        {
            try
            {
                if(clientEP != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEP, null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending data through UDP: {e}");
            }
        }
        

        //method to initialize client objects in clients dictionary
        private static void InitializeServerData()
        {
            for (int i = 1; i <= maxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived},
                { (int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived },
                { (int)ClientPackets.playerPos, ServerHandle.UDPPosition },
                { (int)ClientPackets.playerRot, ServerHandle.UDPRotation },
                { (int)ClientPackets.playerAttack, ServerHandle.UDPAttack },
                { (int)ClientPackets.swapWeapon, ServerHandle.UDPSwap },
                { (int)ClientPackets.damage, ServerHandle.UDPDamage }
            };
        }

    }
}