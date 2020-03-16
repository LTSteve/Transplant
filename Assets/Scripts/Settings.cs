using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool Active = false;

    public static float Sensitivity = 0.5f;

    public static bool ScreenShake = true;
    public static bool ScreenFlash = true;

    public Transform PauseMenu;
    public AudioSource PauseMenuAudio;

    public AudioClip MenuOpen;
    public AudioClip MenuClose;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenClose();
        }
    }

    public void SensitivityChanged(float value)
    {
        Sensitivity = value;
    }

    public void ScreenShakeChanged(bool value)
    {
        ScreenShake = value;
    }

    public void ScreenFlashChanged(bool value)
    {
        ScreenFlash = value;
    }

    public void OpenClose()
    {
        Active = !Active;

        PauseMenuAudio.PlayOneShot(Active ? MenuOpen : MenuClose);

        PauseMenu.gameObject.SetActive(Active);

        Cursor.lockState = Active ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = Active;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
