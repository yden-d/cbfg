using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    //all the panels containing weapons and skills
    public GameObject weaponPanel1;
    public GameObject weaponPanel2;
    public GameObject swordSkills;
    public GameObject shieldSkills;
    public GameObject wandSkills;
    public GameObject staffSkills;
    public GameObject bowSkills;
    //all the slots for equipped skills and weapons
    public Button weaponSlot1;
    public Button weaponSlot2;

    public GameObject playerModel;
    public GameObject sword;
    public GameObject shield;
    public GameObject wand;
    public GameObject staff;
    public GameObject bow;

    //checks for primary weapon equipped
    private bool swordEquipped1;
    private bool shieldEquipped1;
    private bool wandEquipped1;
    private bool staffEquipped1;
    private bool bowEquipped1;
    //checks for secondary weapon equipped
    private bool swordEquipped2;
    private bool shieldEquipped2;
    private bool wandEquipped2;
    private bool staffEquipped2;
    private bool bowEquipped2;

    private List<Button> primarySkills = new List<Button>();
    private List<Button> secondarySkills = new List<Button>();
    private int primarySkillsChosen = 0;
    private int secondarySkillsChosen = 0;

    public void Start() {
        LoadPrimaryWeapon();
        LoadSecondaryWeapon();
    }

    private void LoadPrimaryWeapon() {
        switch(PlayerPrefs.GetString("primary")) {
            case "sword":
                weaponSlot1.image.sprite = Resources.Load<Sprite>("sword");
                EquipSword();
                break;
            case "shield":
                weaponSlot1.image.sprite = Resources.Load<Sprite>("shield");
                EquipShield();
                break;
            case "bow":
                weaponSlot1.image.sprite = Resources.Load<Sprite>("bow");
                EquipBow();
                break;
            case "wand":
                weaponSlot1.image.sprite = Resources.Load<Sprite>("wand");
                EquipWand();
                break;
            case "staff":
                weaponSlot1.image.sprite = Resources.Load<Sprite>("staff");
                EquipStaff();
                break;
            default:
                break;
        }
    }

    private void LoadSecondaryWeapon() {
        switch(PlayerPrefs.GetString("secondary")) {
            case "sword":
                weaponSlot2.image.sprite = Resources.Load<Sprite>("sword");
                break;
            case "shield":
                weaponSlot2.image.sprite = Resources.Load<Sprite>("shield");
                break;
            case "bow":
                weaponSlot2.image.sprite = Resources.Load<Sprite>("bow");
                break;
            case "wand":
                weaponSlot2.image.sprite = Resources.Load<Sprite>("wand");
                break;
            case "staff":
                weaponSlot2.image.sprite = Resources.Load<Sprite>("staff");
                break;
            default:
                break;
        }
    }

    public void OpenPanelOne() {
        weaponPanel1.SetActive(true);
    }

    public void OpenPanelTwo() {
        weaponPanel2.SetActive(true);
    }

    public void OpenPrimarySkillPanel() {
        if(swordEquipped1) {
            swordSkills.SetActive(true);
        } if(shieldEquipped1) {
            shieldSkills.SetActive(true);
        } if(wandEquipped1) {
            wandSkills.SetActive(true);
        } if(staffEquipped1) {
            staffSkills.SetActive(true);
        } if(bowEquipped1) {
            bowSkills.SetActive(true);
        }
    }

    public void OpenSecondarySkillPanel() {
        if(swordEquipped2) {
            swordSkills.SetActive(true);
        } if(shieldEquipped2) {
            shieldSkills.SetActive(true);
        } if(wandEquipped2) {
            wandSkills.SetActive(true);
        } if(staffEquipped2) {
            staffSkills.SetActive(true);
        } if(bowEquipped2) {
            bowSkills.SetActive(true);
        }
    }

    public void OnPrimarySkillSelect(Button button) {
        if(primarySkills.Contains(button)) {
            return;
        }
        primarySkills.Add(button);
        primarySkillsChosen++;
        
        if(primarySkillsChosen == 3) {
            if(swordEquipped1) {
            swordSkills.SetActive(false);
            } if(shieldEquipped1) {
                shieldSkills.SetActive(false);
            } if(wandEquipped1) {
                wandSkills.SetActive(false);
            } if(staffEquipped1) {
                staffSkills.SetActive(false);
            } if(bowEquipped1) {
                bowSkills.SetActive(false);
            }
        }
    }

    public void OnSecondarySkillSelect(Button button) {
        if(secondarySkills.Contains(button)) {
            return;
        }
        
        secondarySkills.Add(button);
        secondarySkillsChosen++;
        
        if(secondarySkillsChosen == 3) {
            if(swordEquipped2) {
            swordSkills.SetActive(false);
            } if(shieldEquipped2) {
                shieldSkills.SetActive(false);
            } if(wandEquipped2) {
                wandSkills.SetActive(false);
            } if(staffEquipped2) {
                staffSkills.SetActive(false);
            } if(bowEquipped2) {
                bowSkills.SetActive(false);
            }
        }  
    }
    
    public void OnWeaponSelectOne(GameObject weapon) {
        // Use if statements to compare the "weapon" parameter to the actual game objects
        if (weapon == GameObject.Find("shield1")) {
            EquipShield();
            weaponSlot1.image.sprite = Resources.Load<Sprite>("shield");
            PlayerPrefs.SetString("primary", "shield");
            shieldEquipped1 = true;
        } if (weapon == GameObject.Find("sword1")) {
            EquipSword();
            weaponSlot1.image.sprite = Resources.Load<Sprite>("sword");
            PlayerPrefs.SetString("primary", "sword");
            swordEquipped1 = true;
        } if (weapon == GameObject.Find("wand1")) {
            EquipWand();
            weaponSlot1.image.sprite = Resources.Load<Sprite>("wand");
            PlayerPrefs.SetString("primary", "wand");
            wandEquipped1 = true;
        } if (weapon == GameObject.Find("staff1")) {
            EquipStaff();
            weaponSlot1.image.sprite = Resources.Load<Sprite>("staff");
            PlayerPrefs.SetString("primary", "staff");
            staffEquipped1 = true;
        } if (weapon == GameObject.Find("bow1")) {
            EquipBow();
            weaponSlot1.image.sprite = Resources.Load<Sprite>("bow");
            PlayerPrefs.SetString("primary", "bow");
            bowEquipped1 = true;
        }
    weaponPanel1.SetActive(false);
    OpenPrimarySkillPanel();
    }

    public void OnWeaponSelectTwo(GameObject weapon) {
        // Use if statements to compare the "weapon" parameter to the actual game objects
        if (weapon == GameObject.Find("sword2")) {
            weaponSlot2.image.sprite = Resources.Load<Sprite>("sword");
            PlayerPrefs.SetString("secondary", "sword");
            swordEquipped2 = true;
        } if (weapon == GameObject.Find("shield2")) {
            weaponSlot2.image.sprite = Resources.Load<Sprite>("shield");
            PlayerPrefs.SetString("secondary", "shield");
            shieldEquipped2 = true;
        } if (weapon == GameObject.Find("wand2")) {
            weaponSlot2.image.sprite = Resources.Load<Sprite>("wand");
            PlayerPrefs.SetString("secondary", "wand");
            wandEquipped2 = true;
        } if (weapon == GameObject.Find("staff2")) {
            weaponSlot2.image.sprite = Resources.Load<Sprite>("staff");
            PlayerPrefs.SetString("secondary", "staff");
            staffEquipped2 = true;
        } if (weapon == GameObject.Find("bow2")) {
            weaponSlot2.image.sprite = Resources.Load<Sprite>("bow");
            PlayerPrefs.SetString("secondary", "bow");
            bowEquipped2 = true;
        }
        weaponPanel2.SetActive(false);
        OpenSecondarySkillPanel();
    }

    private void EquipSword() {
        shield.SetActive(false);
        sword.SetActive(true);
        wand.SetActive(false);
        staff.SetActive(false);
        bow.SetActive(false);
    }

    private void EquipShield() {
        shield.SetActive(true);
        sword.SetActive(false);
        wand.SetActive(false);
        staff.SetActive(false);
        bow.SetActive(false);
    }

    private void EquipWand() {
        shield.SetActive(false);
        sword.SetActive(false);
        wand.SetActive(true);
        staff.SetActive(false);
        bow.SetActive(false);
    }

    private void EquipStaff() {
        shield.SetActive(false);
        sword.SetActive(false);
        wand.SetActive(false);
        staff.SetActive(true);
        bow.SetActive(false);
    }

    private void EquipBow() {
        shield.SetActive(false);
        sword.SetActive(false);
        wand.SetActive(false);
        staff.SetActive(false);
        bow.SetActive(true);
    }

    public void ReturnToMenu() {
        SceneManager.LoadSceneAsync("MainMenuV2");
    }
}
