using System;
using UnityEngine;

[Serializable]
public class Loadout{
    int id {get; set;}
    int user_id {get; set;}
    int weapon_id {get; set;}
    int neutral_light_id {get; set;}
    int neutral_heavy_id {get; set;}
    int side_light_id {get; set;}
    int side_heavy_id {get; set;}
    int up_light_id {get; set;}
    int up_heavy_id {get; set;}
    int created_at {get; set;}
    int updated_at {get; set;}

    public static Loadout fromJson(string json){
        return JsonUtility.FromJson<Loadout>(json);
    }

    public static string toJson(Loadout loadout){
        return JsonUtility.ToJson(loadout);
    } 
}