using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Game
{
    internal class Client
    {
        //4 Mb
        public static int dataBuffersize = 4096;

        public int id;
        public Player player;
        public TCP tcp;
        public UDP udp;

        public Client(int clientID)
        {
            id = clientID;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {

            public TcpClient socket;

            private readonly int id;
            private byte[] receiveBuffer;
            private NetworkStream stream;
            private Packet receivedData;


            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBuffersize;
                socket.SendBufferSize = dataBuffersize;

                receivedData = new Packet();

                stream = socket.GetStream();
                receiveBuffer = new byte[dataBuffersize];

                //params: receive buffer, offset, size, callback method, and object state
                stream.BeginRead(receiveBuffer, 0, dataBuffersize, ReceiveCallback, null);

                Server.currPlayers++;

                ServerSend.Welcome(id, "Welcome!");

            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e}");
                }
            }

            private bool HandleData(byte[] data)
            {
                int pLength = 0;

                receivedData.SetBytes(data);

                //all packets begin with an int denoting the length of packet
                if (receivedData.UnreadLength() >= 4)
                {
                    pLength = receivedData.ReadInt();
                    if (pLength <= 0) return true;
                }

                //only executes if there is a full packet to read
                while (pLength > 0 && pLength <= receivedData.UnreadLength())
                {
                    byte[] pBytes = receivedData.ReadBytes(pLength);

                    //used to keep all work on same thread
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet p = new Packet(pBytes))
                        {
                            int pID = p.ReadInt();
                            Server.packetHandlers[pID](id, p);
                        }
                    });

                    pLength = 0;

                    if (receivedData.UnreadLength() >= 4)
                    {
                        pLength = receivedData.ReadInt();
                        if (pLength <= 0) return true;
                    }
                }

                if (pLength <= 1) return true;

                //else there is still a partial packet
                return false;
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                //try catch to avoid server crash on exception
                try
                {
                    //returns the number of bytes read
                    int byteLength = stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        //disconnect
                        Server.clients[id].Disconnect();
                        return;
                    }
                    byte[] data = new byte[byteLength];
                    Array.Copy(receiveBuffer, data, byteLength);

                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBuffersize, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    //properly disconnect
                    Server.clients[id].Disconnect();
                    Console.WriteLine($"Error, {e}");
                }

            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receiveBuffer = null;
                receivedData = null;
                socket = null;
            }
        }


        public class UDP
        {
            public IPEndPoint endPoint;
            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint ep)
            {
                endPoint = ep;
                ServerSend.UDPTest(id);
            }
            public void SendData(Packet p)
            {
                Server.SendUDPData(endPoint, p);
            }

            public void HandleData(Packet p)
            {
                int packetLength = p.ReadInt();
                byte[] packetBytes = p.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int pId = packet.ReadInt();
                        Server.packetHandlers[pId](id, packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        public void SendIntoGame(string name)
        {
            player = new Player(id, name, new Vector3(0, 0, 0));

            //spawn all current players for newly connected player
            foreach(Client c in Server.clients.Values)
            {
                if(c.player != null)
                {
                    if(c.id != id)
                    {
                        ServerSend.SpawnPlayer(id, c.player);
                    }
                }
            }

            //spawn newly connected player for all players
            foreach(Client c in Server.clients.Values)
            {
                if(c.player != null)
                {
                    ServerSend.SpawnPlayer(c.id, player);
                }
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected");

            player = null;

            Server.currPlayers--;

            tcp.Disconnect();
            udp.Disconnect();

            ServerSend.DespawnPlayer(id);
        }

        public void UpdatePosition()
        {
            ServerSend.SendPosition(player);
        }

        public void UpdateRotation()
        {
            ServerSend.SendRotation(player);
        }
    
    
    }
}
