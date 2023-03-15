using System;
using UnityEngine;

[Serializable]
public class Skill {
    int id {get; set;}
    int weapon_id {get; set;}
    int parent_id {get; set;}
    string name {get; set;}
    string binding {get; set;}
    string created_at {get; set;}
    string updated_at {get; set;}

    public static Skill fromJson(string json){
        return JsonUtility.FromJson<Skill>(json);
    }

    public static string toJson(Skill skill){
        return JsonUtility.ToJson(skill);
    } 
}