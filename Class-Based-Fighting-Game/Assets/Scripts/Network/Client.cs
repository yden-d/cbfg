using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
{
    //enfore singleton
    public static Client instance;

    public static int dataBufferSize = 4096;

    //currently local host implementation
    public string ip = "127.0.0.1";
    public int port = 585;
    public int id = 0;
    public TCP tcp;
    public UDP udp;
    
    private bool connected = false;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    //singleton, ensure only one instance of this
    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if(instance != this) {
            Debug.Log("enforcing singleton");
            Destroy(this);
        }
    }

    //create instance of TCP 
    private void Start() {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit(){
        Disconnect();
    }

    public void ServerConnect() {
        InitializeClientData();
        connected = true;
        tcp.Connect();
    }

    //TCP class to contain all relevant TCP data
    //
    public class TCP {
        public TcpClient socket;

        private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet receivedData;

        public void Connect() {

            //initialize TCP Client with correct buffer size
            socket = new TcpClient
            {
                //set buffer sizes for socket
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            //initialize local buffer
            receiveBuffer = new byte[dataBufferSize];

            //params: ip, port, callback, tcpclient reference
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result){
            socket.EndConnect(result);

            //ensure successful connection
            if(!socket.Connected) return;

            stream = socket.GetStream();

            receivedData = new Packet();

            //params: receive buffer, offset, data size, callback method, state
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet p){
            try{
                if(socket != null){
                    stream.BeginWrite(p.ToArray(), 0, p.Length(), null, null);
                }
            }
            catch(Exception e){
                Debug.Log($"Error: {e}");
            }
        }


        private void ReceiveCallback(IAsyncResult result){
            //try catch to avoid server crash on exception
            try
            {
                //returns the number of bytes read
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    //properly disconnect
                    instance.Disconnect();
                    return;
                }
                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception e)
            {
                //properly disconnect
                Disconnect();
                Console.WriteLine($"Error, {e}");
            }
        }

        private bool HandleData(byte[] data){
            int pLength = 0;

            receivedData.SetBytes(data);

            //all packets begin with an int denoting the length of packet
            if(receivedData.UnreadLength() >= 4){
                pLength = receivedData.ReadInt();
                if(pLength <= 0) return true;
            }

            //only executes if there is a full packet to read
            while(pLength > 0 && pLength <= receivedData.UnreadLength()){
                byte[] pBytes = receivedData.ReadBytes(pLength);

                //used to keep all work on same thread
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using(Packet p = new Packet(pBytes)){
                        int pID = p.ReadInt();
                        packetHandlers[pID](p);
                    }
                });

                pLength = 0;

                if(receivedData.UnreadLength() >= 4){
                    pLength = receivedData.ReadInt();
                    if(pLength <= 0) return true;
                }
            }

            if(pLength <= 1) return true;

            //else there is still a partial packet
            return false;
        }

        private void Disconnect(){
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    //public class to send and receive Packets via UDP
    public class UDP {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP(){
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        //establish a UDP connection
        public void Connect(int localPort){
            //Debug.Log($"in udp connect, using port: {localPort}");
            socket = new UdpClient(localPort);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            //initial connection with server and open localport
            using (Packet packet = new Packet()){
                SendData(packet);
            }
            Debug.Log("udp connect method complete, sent initial packet");
        }

        //method to send packet using UdpClient BeginSend()
        public void SendData(Packet packet){
            try{
                //sender must claim their id
                packet.InsertInt(instance.id);
                
                if(socket != null){
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            }
            catch (Exception e){
                Debug.Log($"Error in udp senddata: {e}");
            }
        }
        
        //callback method when connection is/is not made
        private void ReceiveCallback(IAsyncResult result){
            try{
                byte[] data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if(data.Length < 4){
                    instance.Disconnect();

                    return;
                }
                HandleData(data);
            }
            catch (Exception e) {
                Disconnect();
                Debug.Log($"Error in udp receivecallback: {e}");
            }
        }

        //method to handle packet data in the form of a byte[]
        private void HandleData(byte[] data){
            using(Packet p = new Packet(data)){
                int packetLength = p.ReadInt();
                data = p.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() => 
            {
                using(Packet p = new Packet(data)){
                    int packetId = p.ReadInt();
                    packetHandlers[packetId](p);
                }
            });
        }

        private void Disconnect(){
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }

    }

    //helper method to initialize the dictionary for packet
    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.Welcome},
            {(int)ServerPackets.udpTest, ClientHandle.UDPTest},
            {(int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer},
            {(int)ServerPackets.playerPos, ClientHandle.UpdatePos},
            {(int)ServerPackets.playerRot, ClientHandle.UpdateRot},
            {(int)ServerPackets.despawnPlayer, ClientHandle.DespawnPlayer},
            {(int)ServerPackets.playerAttack, ClientHandle.Attack},
            {(int)ServerPackets.swapWeapon, ClientHandle.Swap},
            {(int)ServerPackets.damage, ClientHandle.Damage}
        };

        Debug.Log("Initialized packets");
    }

    private void Disconnect(){
        if(connected){
            connected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server");
        }
    }
}
