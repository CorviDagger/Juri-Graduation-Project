using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{

    public SpriteRenderer bodyPart;

    public List<Sprite> options = new List<Sprite>();


    private int currentOption = 0;
    public void NextOption()
    {
        currentOption++;
        if(currentOption >= options.Count)
        {
            currentOption = 0;
        }
        bodyPart.sprite = options[currentOption];
    }
}
