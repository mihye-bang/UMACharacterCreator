using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using UnityEngine.UI;
using System.IO;
using UMA.PoseTools;

public class CharacterCreator : MonoBehaviour
{
    public DynamicCharacterAvatar avatar;
    public Slider heightSlider;
    public Slider bellySlider;

    private Dictionary<string, DnaSetter> dna;

    public List<string> hairModelsMale = new List<string>();
    public List<string> hairModelsFemale = new List<string>();
    private int currentHairMale;
    private int currentHairFemale;

    public string myRecipe;

    public List<string> pantsModelsMale = new List<string>();
    public List<string> pantsModelsFemale = new List<string>();
    private int currentPantsMale;
    private int currentPantsFemale;

    public List<string> shirtModelsMale = new List<string>();
    public List<string> shirtModelsFemale = new List<string>();
    private int currentShirtMale;
    private int currentShirtFemale;

    
    /// ////////////////////
    
    private ExpressionPlayer expression;
    private bool connected;
    public enum Moods { Neutral, Happy, Sad };
    public Moods mood = Moods.Neutral;
    private Moods lastMood = Moods.Neutral;


    void OnEnable()
    {
        /////////////////////////////
        avatar = GetComponent<DynamicCharacterAvatar>();
        avatar.CharacterCreated.AddListener(OnCreated);


        avatar.CharacterUpdated.AddListener(Updated);
        heightSlider.onValueChanged.AddListener(HeightChange);
        bellySlider.onValueChanged.AddListener(BellyChange);
    }

    void OnDisable()
    {
        avatar.CharacterUpdated.RemoveListener(Updated);
        heightSlider.onValueChanged.RemoveListener(HeightChange);
        bellySlider.onValueChanged.RemoveListener(BellyChange);



        /////
        avatar.CharacterCreated.RemoveListener(OnCreated);
    }

    /// /////////////////////////////////////////
    public void OnCreated(UMAData data)
    {
        expression = GetComponent<ExpressionPlayer>();
        expression.enableBlinking = true;
        expression.enableSaccades = true;
        connected = true;
    }
    void Updated()
    {
        if (connected && lastMood != mood)   // if operation is changed?
        {
            lastMood = mood;
            switch (mood)
            {
                case Moods.Neutral:
                    expression.leftMouthSmile_Frown = 0;
                    expression.rightMouthSmile_Frown = 0;
                    expression.leftEyeOpen_Close = 0;
                    expression.rightEyeOpen_Close = 0;
                    expression.midBrowUp_Down = 0;
                    break;
                case Moods.Happy:
                    expression.leftMouthSmile_Frown = 1f;
                    expression.rightMouthSmile_Frown = 1f;
                    expression.leftEyeOpen_Close = -1f;
                    expression.rightEyeOpen_Close = -1f;
                    expression.midBrowUp_Down = 1f;

                    break;
                case Moods.Sad:
                    expression.leftMouthSmile_Frown = -0.7f;
                    expression.rightMouthSmile_Frown = -0.7f;
                    expression.leftEyeOpen_Close = 0.3f;
                    expression.rightEyeOpen_Close = 0.3f;
                    expression.midBrowUp_Down = -0.4f;
                    break;
                default:
                    break;
            }
        }
    }




    public void SwitchGender(bool male)
    {
        if (male && avatar.activeRace.name != "HumanMaleDCS")
        {
            avatar.ChangeRace("HumanMaleDCS");
        }
        if(!male && avatar.activeRace.name != "HumanFemaleDCS")
        {
            avatar.ChangeRace("HumanFemaleDCS");
        }
    }

    void Updated(UMAData data)
    {
        dna = avatar.GetDNA();
        heightSlider.value = dna["height"].Get();
        bellySlider.value = dna["belly"].Get();
    }

    public void HeightChange(float val)
    {
        dna["height"].Set(val);
        avatar.BuildCharacter();
    }

    public void BellyChange(float val)
    {
        dna["belly"].Set(val);
        avatar.BuildCharacter();
    }

    public void ChangeSkinColor(Color col)
    {
        avatar.SetColor("Skin", col);
        // avatar.BuildCharacter();
        avatar.UpdateColors(true); // needs to be 'true'

        Debug.Log(avatar.GetColor("Skin").color);
    }

    // Change Wardrobe too, women change
    public void ChangeHair(bool plus)
    {
        if (avatar.activeRace.name == "HumanMaleDCS")
        {
            if (plus)
            {
                currentHairMale++;
                if (currentHairMale >= hairModelsMale.Count)
                    currentHairMale = 0;
            }
            else
            {
                currentHairMale--;
                if (currentHairMale < 0)
                    currentHairMale = hairModelsMale.Count;
            }

            currentHairMale = Mathf.Clamp(currentHairMale, 0, hairModelsMale.Count - 1);
            if (hairModelsMale[currentHairMale] == "None")
                avatar.ClearSlot("Hair");
            else
                avatar.SetSlot("Hair", hairModelsMale[currentHairMale]);
        }

        if (avatar.activeRace.name == "HumanFemaleDCS")
        {
            if (plus)
            {
                currentHairFemale++;
                if (currentHairFemale >= hairModelsFemale.Count)
                    currentHairFemale = 0;
            }
            else
            {
                currentHairFemale--;
                if (currentHairFemale < 0)
                    currentHairFemale = hairModelsFemale.Count;
            }

            currentHairFemale = Mathf.Clamp(currentHairFemale, 0, hairModelsFemale.Count - 1);
            if (hairModelsFemale[currentHairFemale] == "None")
                avatar.ClearSlot("Hair");
            else
                avatar.SetSlot("Hair", hairModelsFemale[currentHairFemale]);
        }

        avatar.BuildCharacter();
        //Debug.Log(avatar.GetWardrobeItemName("Hair"));
    }

    public void SaveRecipe()
    {
        myRecipe = avatar.GetCurrentRecipe();
        File.WriteAllText(Application.persistentDataPath + "/dude.txt", myRecipe);
    }

    public void LoadRecipe()
    {
        myRecipe = File.ReadAllText(Application.persistentDataPath + "/dude.txt");
        avatar.ClearSlots();
        avatar.LoadFromRecipeString(myRecipe);
    }

    public void ChangePants(bool plus)
    {
        if (avatar.activeRace.name == "HumanMaleDCS")
        {
            if (plus)
            {
                currentPantsMale++;
                if (currentPantsMale >= pantsModelsMale.Count)
                    currentPantsMale = 0;
            }
            else
            {
                currentPantsMale--;
                if (currentPantsMale < 0)
                    currentPantsMale = pantsModelsMale.Count;
            }

            currentPantsMale = Mathf.Clamp(currentPantsMale, 0, pantsModelsMale.Count - 1);
            if (pantsModelsMale[currentPantsMale] == "None")
                avatar.ClearSlot("Legs");
            else
                avatar.SetSlot("Legs", pantsModelsMale[currentPantsMale]);
        }

        if (avatar.activeRace.name == "HumanFemaleDCS")
        {
            if (plus)
            {
                currentPantsFemale++;
                if (currentPantsFemale >= pantsModelsFemale.Count)
                    currentPantsFemale = 0;
            }
            else
            {
                currentPantsFemale--;
                if (currentPantsFemale < 0)
                    currentPantsFemale = pantsModelsFemale.Count;
            }

            currentPantsFemale = Mathf.Clamp(currentPantsFemale, 0, pantsModelsFemale.Count - 1);
            if (pantsModelsFemale[currentPantsFemale] == "None")
                avatar.ClearSlot("Legs");
            else
                avatar.SetSlot("Legs", pantsModelsFemale[currentPantsFemale]);
        }

        avatar.BuildCharacter();
    }

    public void ChangeShirt(bool plus)
    {
        if (avatar.activeRace.name == "HumanMaleDCS")
        {
            if (plus)
            {
                currentShirtMale++;
                if (currentShirtMale >= shirtModelsMale.Count)
                    currentShirtMale = 0;
            }
            else
            {
                currentShirtMale--;
                if (currentShirtMale < 0)
                    currentShirtMale = shirtModelsMale.Count;
            }

            currentShirtMale = Mathf.Clamp(currentShirtMale, 0, shirtModelsMale.Count - 1);
            if (shirtModelsMale[currentShirtMale] == "None")
                avatar.ClearSlot("Chest");
            else
                avatar.SetSlot("Chest", shirtModelsMale[currentShirtMale]);
        }

        if (avatar.activeRace.name == "HumanFemaleDCS")
        {
            if (plus)
            {
                currentShirtFemale++;
                if (currentShirtFemale >= shirtModelsFemale.Count)
                    currentShirtFemale = 0;
            }
            else
            {
                currentShirtFemale--;
                if (currentShirtFemale < 0)
                    currentShirtFemale = shirtModelsFemale.Count;
            }

            currentShirtFemale = Mathf.Clamp(currentShirtFemale, 0, shirtModelsFemale.Count - 1);
            if (shirtModelsFemale[currentShirtFemale] == "None")
                avatar.ClearSlot("Chest");
            else
                avatar.SetSlot("Chest", shirtModelsFemale[currentShirtFemale]);
        }

        avatar.BuildCharacter();
    }
}
