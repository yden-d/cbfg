using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class ServerSend
    {
        private static void SendTCPData(int tcpClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[tcpClient].tcp.SendData(packet);
        }

        private static void SendUDPData(int tcpClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[tcpClient].udp.SendData(packet);
        }

        //method to send packet using TCP to all clients in client dictionary
        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for(int i = 1; i <= Server.maxPlayers; i++)
            {
                if (Server.clients[i] != null) Server.clients[i].tcp.SendData(packet);
            }
        }

        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                if (Server.clients[i] != null) Server.clients[i].udp.SendData(packet);
            }
        }

        private static void SendUDPDataToAllExcept(int id, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                if (Server.clients[i].id == id) continue;
                if (Server.clients[i] != null) Server.clients[i].udp.SendData(packet);
            }
        }

        private static void SendTCPDataToAllExcept(int id, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                if (Server.clients[i].id == id) continue;
                if (Server.clients[i] != null) Server.clients[i].tcp.SendData(packet);
            }
        }

        //Create welcome packet to be sent to client
        public static void Welcome(int tcpClient, string msg)
        {
            //singleton packet
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(msg);
                packet.Write(tcpClient);

                SendTCPData(tcpClient, packet);
            }
        }

        public static void UDPTest(int udpClient)
        {
            using (Packet p = new Packet((int)ServerPackets.udpTest))
            {
                p.Write("Testing udp");
                SendUDPData(udpClient, p);
            }
        }

        public static void SpawnPlayer(int clientid, Player newClient)
        {
            //create spawnPlayer packet, uses Packet enum
            using (Packet p = new Packet((int)ServerPackets.spawnPlayer))
            {
                p.Write(newClient.id);
                p.Write("Username Field");
                p.Write(newClient.position);
                p.Write(newClient.rotation);

                SendTCPData(clientid, p);
            }
        }

        public static void DespawnPlayer(int id)
        {
            using (Packet p = new Packet((int)ServerPackets.despawnPlayer))
            {
                p.Write(id);

                SendTCPDataToAllExcept(id, p);
            }
        }

        public static void SendPosition(Player player)
        {
            using (Packet p = new Packet((int)ServerPackets.playerPos))
            {
                p.Write(player.id);
                p.Write(player.position);
                SendUDPDataToAllExcept(player.id, p);
            }
        }

        public static void SendRotation(Player player)
        {
            using (Packet p = new Packet((int)ServerPackets.playerRot))
            {
                p.Write(player.id);
                p.Write(player.rotation);
                SendUDPDataToAllExcept(player.id, p);
            }
        }

        public static void SendAttack(int id, int attack, bool facingR)
        {
            using (Packet p = new Packet((int)ServerPackets.playerAttack))
            {
                p.Write(id);
                p.Write(attack);
                p.Write(facingR);
                SendUDPDataToAllExcept(id, p);
            }
        }

        public static void SendSwap(int id)
        {
            using (Packet p = new Packet((int)ServerPackets.swapWeapon))
            {
                p.Write(id);
                SendUDPDataToAllExcept(id, p);
            }
        }

        public static void SendDamage(int id, float damage)
        {
            using (Packet p = new Packet((int)ServerPackets.damage))
            {
                p.Write(id);
                p.Write(damage);
                SendUDPDataToAll(p);
            }
        }

    }
}
