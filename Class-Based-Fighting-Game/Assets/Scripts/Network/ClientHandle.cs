using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

//A class that handles packets of different formats
//Each method is to handle a certain packet structure
public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet){
        //must be read in the same order that was sent
        string msg = packet.ReadString();
        int clientID = packet.ReadInt();

        Debug.Log($"Message from server using TCP: {msg}");

        //Set client ID using the welcome TCP packet
        Client.instance.id = clientID;

        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

    }

    public static void UDPTest(Packet packet){
        string msg = packet.ReadString();
        Debug.Log($"Successful udp packet received, message: {msg}");
        ClientSend.UDPTestReceived();
    }

    public static void SpawnPlayer(Packet packet){
        Debug.Log("clienthandle spawn");
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        while(GameManager.instance == null)
        {
            Debug.Log("waiting");
        }
        GameManager.instance.SpawnPlayer(id, username, position);
    }

    public static void DespawnPlayer(Packet packet){
        int id = packet.ReadInt();
        GameManager.instance.DespawnPlayer(id);
    }

    public static void UpdatePos(Packet packet){
        int id = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        GameManager.instance.UpdatePlayerPos(id, pos);
    }


    public static void UpdateRot(Packet packet){
        int id = packet.ReadInt();
        float rot = packet.ReadFloat();
        GameManager.instance.UpdatePlayerRot(id, rot);
    }

    public static void Attack(Packet packet){
        int id = packet.ReadInt();
        int attack = packet.ReadInt();
        bool facingR = packet.ReadBool();
        GameManager.instance.PlayerAttack(id, attack, facingR);
    }

    public static void Swap(Packet packet){
        int id = packet.ReadInt();
        GameManager.instance.SwapWeapon(id);
    }

    public static void Damage(Packet packet){
        int id = packet.ReadInt();
        float damage = packet.ReadFloat();
        GameManager.instance.TakeDamage(id, damage);
    }

}
