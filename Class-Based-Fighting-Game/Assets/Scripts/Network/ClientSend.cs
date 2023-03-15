using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet){
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet){
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    public static void WelcomeReceived(){
        using (Packet p = new Packet((int)ClientPackets.welcomeReceived))
        {
            p.Write(Client.instance.id);
            p.Write("Ready to play!");

            SendTCPData(p);
        }
    }

    public static void UDPTestReceived(){
        using(Packet p = new Packet((int)ClientPackets.udpTestReceived))
        {
            p.Write("UDP packet received from client");
            SendUDPData(p);
        }
    }

    public static void SendPos(Vector3 position){
        using(Packet p = new Packet((int)ClientPackets.playerPos))
        {
            p.Write(position);
            SendUDPData(p);
        }
    }

    public static void SendRot(float rotation){
        using(Packet p = new Packet((int)ClientPackets.playerRot))
        {
            p.Write(rotation);
            SendUDPData(p);
        }
    }

    public static void Attack(int attack, bool facingR){
        using(Packet p = new Packet((int)ClientPackets.playerAttack))
        {
            p.Write(attack);
            p.Write(facingR);
            SendUDPData(p);
        }
    }

    public static void Swap(){
        using(Packet p = new Packet((int)ClientPackets.swapWeapon))
        {
            SendUDPData(p);
        }
    }

    public static void SendDamage(int id, float damage){
        using(Packet p = new Packet((int)ClientPackets.damage))
        {
            p.Write(id);
            p.Write(damage);
            SendUDPData(p);
        }
    }
    
}
