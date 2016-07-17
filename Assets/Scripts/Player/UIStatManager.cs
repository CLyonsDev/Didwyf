using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIStatManager : MonoBehaviour {

    public Image healthImage;

    public Text ArmorClassText;

    public Text strengthText, dexterityText, intelText, vitalityText, damageText, characterName;

    public CharacterBase playerScript;

    [SerializeField] float LerpSpeed = 4f;

	// Use this for initialization
	void Awake () {
        //healthImage = GameObject.Find("HealthIcon").GetComponent<Image>();
        //ArmorClassText = GameObject.Find("ArmorClassText").GetComponent<Text>();

        GrabReferences();

        playerScript = GetComponent<CharacterBase>();
	}

    void Start()
    {
        if(healthImage != null)
            healthImage.fillAmount = 1;

        GameObject.Find("Canvas").transform.position = Vector3.zero;
    }
	
	// This is really sloppy, we can clean it up later
	void Update () {

        GrabReferences();

        if ((healthImage.fillAmount != (playerScript.currentHealth / playerScript.maxHealth)))
        {
            if(healthImage.fillAmount != healthImage.fillAmount)
                healthImage.fillAmount = 1;

            healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, (playerScript.currentHealth / playerScript.maxHealth), LerpSpeed * Time.deltaTime);
        }

        if(playerScript != null && healthImage != null && ArmorClassText != null && characterName != null)
        {
            if (ArmorClassText.text != playerScript.armorRating.ToString())
                ArmorClassText.text = playerScript.armorRating.ToString();

            if (characterName.text != playerScript.playerName.ToString())
            {
                characterName.text = playerScript.playerName.ToString();
            }

            SetStatBlockTexts();
        }
    }

    private void SetStatBlockTexts()
    {
        if (strengthText == null || dexterityText == null || intelText == null || vitalityText == null || characterName == null)
        {
            Debug.LogError("Could not find one of the text objects!");
            return;
        }

        strengthText.text = "Strength: " + playerScript.strength.ToString();
        dexterityText.text = "Dexterity: " + playerScript.dexterity.ToString();
        intelText.text = "Intelligence: " + playerScript.intelligence.ToString();
        vitalityText.text = "Vitality: " + playerScript.vitality.ToString();
        damageText.text = "Damage: " + playerScript.totalDamageMin + " - " + playerScript.totalDamageMax + " (x" + playerScript.weaponCritModifier + ")";
    }

    private void GrabReferences()
    {
        if (healthImage == null)
        {
            healthImage = GameObject.Find("HealthIcon").GetComponent<Image>();
            healthImage.fillAmount = 1;
        }
        if (ArmorClassText == null)
            ArmorClassText = GameObject.Find("ArmorClassText").GetComponent<Text>();
        if (playerScript == null)
            playerScript = GetComponent<CharacterBase>();
        if (strengthText == null)
            strengthText = GameObject.Find("StrengthText").GetComponent<Text>();
        if (dexterityText == null)
            dexterityText = GameObject.Find("DexterityText").GetComponent<Text>();
        if (intelText == null)
            intelText = GameObject.Find("IntelligenceText").GetComponent<Text>();
        if (vitalityText == null)
            vitalityText = GameObject.Find("VitalityText").GetComponent<Text>();
        if(damageText == null)
            damageText = GameObject.Find("DamageText").GetComponent<Text>();
        if (characterName == null)
            characterName = GameObject.Find("CharName").GetComponent<Text>();
    }
}
