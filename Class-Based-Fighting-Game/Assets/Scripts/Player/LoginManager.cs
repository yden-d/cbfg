using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    //"http://coms-402.merenze.com/ping"
    //"https://catfact.ninja/fact"
    //"http://[::1]:3000/ping"
    //"http://coms-402-sd-33.class.las.iastate.edu/ping"
    //

    private string email;
    private string password;
    private string username;
    private int gui;
    public TMP_Text ErrorMessage;
    public TMP_Text DontHave;
    public TMP_Text CreateLink;
    public TMP_Text SignInButtonText;
    public GameObject UsernameField;

    void Start() {
        gui = 1;
        email = "";
        username = "";
        password = "";
        UsernameField.SetActive(false);
    }

    public void QuitGame(){
        Application.Quit();
        UsernameField.SetActive(false);
    }

    public void VerifyEmail(string e) {
        email = e;
    }

    public void ReadPass(string p) {
        password = p;
    }

    public void ReadUser(string u) {
        username = u;
    }

    public void CreateAccountButton() {
        ErrorMessage.text = "";
        if(gui == 1) CreateAccGUI();
        else SignInGUI();
    }

    public void CreateAccGUI() {
        UsernameField.SetActive(true);
        DontHave.text = "Have One?";
        CreateLink.text = "Sign In";
        SignInButtonText.text = "Create";
        gui = 0;
    }

    public void SignInGUI() {
        UsernameField.SetActive(false);
        DontHave.text = "Don't Have One?";
        CreateLink.text = "Create Account";
        SignInButtonText.text = "Sign In";
        gui = 1;
    }

    
    public void SendCredentials(){
        ErrorMessage.text = "";
        if(email == "" || password == "") {
            ErrorMessage.text = "Empty Fields";
        }
        else if(gui == 1) {
            StartCoroutine(singIn());
        }
        else if(username == "") {
            ErrorMessage.text = "Empty Fields";
            Debug.Log("User " + username);
        }
        else{
            StartCoroutine(register());
        }
    }

    IEnumerator register() {

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("username", username);
        form.AddField("password", password);
        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("username", username);

        UnityWebRequest www = UnityWebRequest.Post("http://coms-402.merenze.com/register", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.Log(www.error);
            ErrorMessage.text = "Cannot Create Account";
        }
        else{
            Debug.Log("Successful Account Creation" + '\n' + www.downloadHandler.text);
            SceneManager.LoadSceneAsync("MainMenuV2");
        }
        www.Dispose();
    }

    IEnumerator singIn() {

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("username", username);

        UnityWebRequest www = UnityWebRequest.Post("http://coms-402.merenze.com/login", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.Log(www.error);
            ErrorMessage.text = "Invalid Credentials";
        } else {
            Debug.Log("Successful Login" + '\n' + www.downloadHandler.text);

            //Get player information: loadout, username, etc..
            UnityWebRequest userData = UnityWebRequest.Get("http://coms-402.merenze.com/whoami");
            yield return userData.SendWebRequest();
            string JsonString = userData.downloadHandler.text;
            Debug.Log(JsonString);
            var playerInfo = new PlayerInfo();
            playerInfo = JsonUtility.FromJson<PlayerInfo>(JsonString);
            string playerInfoString = JsonUtility.ToJson(playerInfo);
            PlayerPrefs.SetString("playerInfo", playerInfoString);
            
            //Get the current user's loadout information
            UnityWebRequest primaryLoadout = UnityWebRequest.Get("http://coms-402.merenze.com/loadouts/" + playerInfo.loadout_primary_id);
            yield return primaryLoadout.SendWebRequest();

            if(primaryLoadout.result != UnityWebRequest.Result.Success) {
                Debug.Log(primaryLoadout.error);
            } else {
                Debug.Log("Primary Loadout Retrieval Successful");
                string primloadoutJSon = primaryLoadout.downloadHandler.text;
                var primloadoutInfo = new LoadoutInfo();
                primloadoutInfo = JsonUtility.FromJson<LoadoutInfo>(primloadoutJSon);
                string primloadoutInfoString = JsonUtility.ToJson(primloadoutInfo);
                PlayerPrefs.SetString("primloadoutInfo", primloadoutInfoString);
            
                switch(primloadoutInfo.weapon_id) {
                    case 1:
                        PlayerPrefs.SetString("primary", "sword");
                        break;
                    case 2:
                        PlayerPrefs.SetString("primary", "shield");
                        break;
                    case 3:
                        PlayerPrefs.SetString("primary", "bow");
                        break;
                    case 4:
                        PlayerPrefs.SetString("primary", "wand");
                        break;
                    case 5:
                        PlayerPrefs.SetString("primary", "staff");
                        break;
                    default:
                        break;
                }
            }
            primaryLoadout.Dispose();

            //Now do the same for the secondary
            UnityWebRequest secondaryLoadout = UnityWebRequest.Get("http://coms-402.merenze.com/loadouts/" + playerInfo.loadout_secondary_id);
            yield return secondaryLoadout.SendWebRequest();

            if(secondaryLoadout.result != UnityWebRequest.Result.Success) {
                Debug.Log(secondaryLoadout.error);
            } else {
                Debug.Log("Secondary Loadout Retrieval Successful");
                string secloadoutJSon = secondaryLoadout.downloadHandler.text;
                var secloadoutInfo = new LoadoutInfo();
                secloadoutInfo = JsonUtility.FromJson<LoadoutInfo>(secloadoutJSon);
                string secloadoutInfoString = JsonUtility.ToJson(secloadoutInfo);
                PlayerPrefs.SetString("secloadoutInfo", secloadoutInfoString);
            
                switch(secloadoutInfo.weapon_id) {
                    case 1:
                        PlayerPrefs.SetString("secondary", "sword");
                        break;
                    case 2:
                        PlayerPrefs.SetString("secondary", "shield");
                        break;
                    case 3:
                        PlayerPrefs.SetString("secondary", "bow");
                        break;
                    case 4:
                        PlayerPrefs.SetString("secondary", "wand");
                        break;
                    case 5:
                        PlayerPrefs.SetString("secondary", "staff");
                        break;
                    default:
                        break;
                }
            }
            secondaryLoadout.Dispose(); 
            SceneManager.LoadSceneAsync("MainMenuV2");
        }
        www.Dispose();
    }
}