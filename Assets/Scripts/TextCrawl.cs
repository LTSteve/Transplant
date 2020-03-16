using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCrawl : MonoBehaviour
{
    public static TextCrawl Instance;

    public Text TextElement;

    [@TextAreaAttribute(15,20)]
    public string Text;

    public float LetterDelay;

    public PlayerController Player;

    public AudioSource audio;

    public static bool WindUpOver = false;

    private float letterDelay = 0f;

    public bool dead = false;

    private void Start()
    {
        Instance = this;

        letterDelay = LetterDelay;
    }

    private void Update()
    {
        if (dead)
        {
            if (WindUpOver)
            {
                kill();
            }
            return;
        }

        letterDelay -= Time.deltaTime;

        if(letterDelay <= 0)
        {
            letterDelay = LetterDelay;

            updateTextElement();
        }
    }

    private void updateTextElement()
    {
        if(Text.Length <= 0)
        {
            dead = true;
            return;
        }

        var nextChar = Text.Substring(0, 1);
        Text = Text.Substring(1);

        TextElement.text = TextElement.text + nextChar;

        audio.Play();
    }

    private void kill()
    {
        Player.WindupIsOver = true;

        this.gameObject.SetActive(false);
    }
}
