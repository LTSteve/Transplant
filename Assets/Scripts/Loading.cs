using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{

    private float updateNo = 0;

    private void Update()
    {
        updateNo++;
        if (updateNo > 2) {
            Load();
        }
    }

    private void Load()
    {
        SceneManager.LoadScene(1);
    }
}
