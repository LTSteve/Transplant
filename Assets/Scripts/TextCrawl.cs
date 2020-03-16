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
    public bool fin = false;

    private void Start()
    {
        Instance = this;

        letterDelay = LetterDelay;
    }

    bool ending = false;

    private void Update()
    {
        if (Settings.Active && ! ending)
        {
            return;
        }

        if (dead)
        {
            if (fin)
            {
                if (!ending)
                {
                    StartCoroutine(endGame());
                    ending = true;
                }
                return;
            }

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

        var currentText = TextElement.text;

        var split = currentText.Split('\n');

        if (split.Length > 7)
        {
            TextElement.text = split[split.Length - 7] + "\n" +
                split[split.Length - 6] + "\n" +
                split[split.Length - 5] + "\n" +
                split[split.Length - 4] + "\n" +
                split[split.Length - 3] + "\n" +
                split[split.Length - 2] + "\n" +
                split[split.Length - 1];
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

    private IEnumerator endGame()
    {
        yield return new WaitForSeconds(1f);

        Text = "\n \n \n\tSteve D";
        dead = false;

        yield return new WaitForSeconds(10f);

        Application.Quit();
    }
}
