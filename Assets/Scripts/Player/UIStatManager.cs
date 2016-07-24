using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIStatManager : MonoBehaviour {

    public Image healthImage;

    public GameObject attributeContainer;

    public Text ArmorClassText;

    public Text strengthText, dexterityText, intelText, vitalityText, damageText, characterName;

    public CharacterBase playerScript;

    [SerializeField] float LerpSpeed = 4f;

    void Start()
    {

        if (healthImage != null)
            healthImage.fillAmount = 1;

        ArmorClassText = GameObject.Find("ArmorClassText").GetComponent<Text>();

        GrabReferences();

        playerScript = GetComponent<CharacterBase>();
    }

    // This is really sloppy, we can clean it up later
    void Update() {

        if (playerScript == null || healthImage == null || ArmorClassText == null || characterName == null || attributeContainer == null)
        {
            GrabReferences();
        }

        if (attributeContainer == null)
            return;

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
        if(attributeContainer == null)
        {
            attributeContainer = GameObject.Find("AttributesPanel");
        }

        if (attributeContainer == null)
            return;

        if (healthImage == null)
        {
            healthImage = GameObject.Find("HealthIcon").GetComponent<Image>();
            if(healthImage != null)
                healthImage.fillAmount = 1;
        }

        if (attributeContainer == null)
            return;

        if (ArmorClassText == null)
            ArmorClassText = GameObject.Find("ArmorClassText").GetComponent<Text>();
        if (playerScript == null)
            playerScript = GetComponent<CharacterBase>();
        if (strengthText == null)
            strengthText = attributeContainer.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        if (dexterityText == null)
            dexterityText = attributeContainer.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        if (intelText == null)
            intelText = attributeContainer.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        if (vitalityText == null)
            vitalityText = attributeContainer.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
        if(damageText == null)
            damageText = attributeContainer.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>();
        if (characterName == null)
            characterName = GameObject.Find("CharName").GetComponent<Text>();
    }
}
