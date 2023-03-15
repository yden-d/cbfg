using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public string username;
    public TMP_Text welcomeText;
    // Start is called before the first frame update
    void Start()
    {
        var playerInfo = new PlayerInfo();
        string playerInfoString = PlayerPrefs.GetString("playerInfo");
        playerInfo = JsonUtility.FromJson<PlayerInfo>(playerInfoString);
        Debug.Log(playerInfo.loadout_primary_id);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayGame(){
        Debug.Log(Client.instance);
        Client.instance.ServerConnect();
        SceneManager.LoadSceneAsync("GameScene");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ViewSkillTree() {
        SceneManager.LoadSceneAsync("SkillTree");
    }

}
