using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class ServerHandle
    {
        public static void WelcomeReceived(int clientID, Packet p)
        {
            int checkID = p.ReadInt();
            string msg = p.ReadString();
            Console.WriteLine($"{Server.clients[clientID].tcp.socket.Client.RemoteEndPoint} connected successfully as Player: {clientID} ... msg: {msg}");
            if(clientID != checkID)
            {
                Console.WriteLine($"Something has gone wrong with {clientID} ... expected id: {checkID}");
            }

            Server.clients[clientID].SendIntoGame("user");
        }

        public static void UDPTestReceived(int clientID, Packet p)
        {
            string msg = p.ReadString();
            Console.WriteLine($"Received UDP packet, message: {msg}");
        }

        public static void UDPPosition(int clientID, Packet p)
        {
            Vector3 pos = p.ReadVector();
            if (Server.clients[clientID].player != null)
            {
                Server.clients[clientID].player.position = pos;
                Server.clients[clientID].UpdatePosition();
            }
        }

        public static void UDPRotation(int clientID, Packet p)
        {
            float rot = p.ReadFloat();
            if (Server.clients[clientID].player != null) {
                Server.clients[clientID].player.rotation = rot;
                Server.clients[clientID].UpdateRotation();
            }
        }

        public static void UDPAttack(int clientID, Packet p)
        {
            int attack = p.ReadInt();
            bool facingR = p.ReadBool();
            ServerSend.SendAttack(clientID, attack, facingR);
        }

        public static void UDPSwap(int clientID, Packet p)
        {
            ServerSend.SendSwap(clientID);
        }

        public static void UDPDamage(int clientID, Packet p)
        {
            int hitID = p.ReadInt();
            float damage = p.ReadFloat();
            GameLogic.FairPlayEnforcer(damage, clientID, hitID);
        }
    }
    
}
