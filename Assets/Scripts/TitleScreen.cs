using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    public GameObject enterText;
    private bool loading = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !loading)
        {
            SceneManager.LoadScene("Base_Scene", LoadSceneMode.Single);
            loading = true;
            enterText.SetActive(false);
        }
    }
}
