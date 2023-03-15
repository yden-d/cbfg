using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player{
    public int id {get; set;}
    public string username {get; set;}
    public string created_at {get; set;}
    public string updated_at {get; set;}
    public string loadout_primary_id {get; set;}
    public string loadout_secondary_id {get; set;}
    public string loadout_primary {get; set;}
    public string loadout_secondary {get; set;}
    public List<Skill> skills {get; set;}
    public List<Loadout> loadouts {get; set;}

    public static Player fromJson(string json){
        return JsonUtility.FromJson<Player>(json);
    }

    public static string toJson(Player player){
        return JsonUtility.ToJson(player);
    } 
}

