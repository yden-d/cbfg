using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : MonoBehaviour
{
    List<string> userIds;


    void Start()
    {
        userIds = new List<string>();
        StartCoroutine(getEquipped());
    }

    public void UserJoin(string userId){
        if(!userIds.Contains(userId)) {
            userIds.Add(userId);
            //StartCoroutine(SpawnPlayer(userId));
        }
    }

    IEnumerator getEquipped()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", "rmbanks@iastate.edu");
        form.AddField("password", "password");

        UnityWebRequest www = UnityWebRequest.Post("http://coms-402.merenze.com/login", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            www.Dispose();
        }
        else
        {
            www.Dispose();
            www = UnityWebRequest.Get("http://coms-402.merenze.com/whoami");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Player player = Player.fromJson(www.downloadHandler.text);
                Debug.Log(Player.toJson(player));
            }
            www.Dispose();
        }
    }
}
