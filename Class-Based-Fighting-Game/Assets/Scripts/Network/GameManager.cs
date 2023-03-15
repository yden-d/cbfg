using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerController> players = new Dictionary<int, PlayerController>();
    
    //public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public CameraController camera;


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

    public void SpawnPlayer(int id, string username, Vector3 position)
    {
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = position;
        player.GetComponent<PlayerController>().id = id;

        //add each player's script to the dictionary
        players.Add(id, player.GetComponent<PlayerController>());

        camera.targets.Insert(0, player.transform);

        Debug.Log($"Added new player {id} to dictionary and targets");
    }

    public void DespawnPlayer(int id)
    {
        PlayerController player = players[id];
        players.Remove(id);
        camera.targets.Remove(player.transform);
        Destroy(player.gameObject);
        Destroy(player);
    }


    //methods to be used when handling packets sent from server
    public void UpdatePlayerPos(int id, Vector3 pos)
    {
        players[id].transform.position = pos;
    }

    public void UpdatePlayerRot(int id, float rot)
    {
        Vector3 scale = players[id].transform.localScale;
        scale.x = rot;
        players[id].transform.localScale = scale;
    }

    public void SwapWeapon(int id)
    {
        players[id].SwapWielding();
    }

    public void TakeDamage(int id, float dmg)
    {
        players[id].TakeDamage(dmg);
    }

    //LIGHT MOVES) 1: down, 2: dir, 3: up, 4: neutral
    //HEAVY MOVES) 1: down, 2: dir, 3: up, 4: neutral
    public void PlayerAttack(int id, int attack, bool facingR)
    {
        Weapon weapon = players[id].wieldedWeapon.GetComponent<Weapon>();
        switch (attack)
        {
            case 1:
                weapon.lightDown();
                break;
            case 2:
                weapon.lightDirectional(facingR);
                break;
            case 3:
                weapon.lightUp();
                break;
            case 4:
                weapon.lightNonDirectional(facingR);
                break;
            case 5:
                weapon.heavyDown();
                break;
            case 6:
                weapon.heavyDirectional(facingR);
                break;
            case 7:
                weapon.heavyUp();
                break;
            case 8:
                weapon.heavyNonDirectional(facingR);
                break;
            default:
                return;
        }
    }
}
