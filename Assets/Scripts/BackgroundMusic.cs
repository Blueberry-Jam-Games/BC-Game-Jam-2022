using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource slowMusic;
    public AudioSource fastMusic;

    private float timeMap;

    private List<RuntimeSlide> allSlides = new List<RuntimeSlide>();

    private bool playFast = false;
    private bool transitionComplete = true;

    void Start()
    {
        timeMap =  fastMusic.clip.length / slowMusic.clip.length;
        fastMusic.pitch = timeMap;
        fastMusic.volume = 0f;

        GameObject[] slides = GameObject.FindGameObjectsWithTag("WaterSlide");
        foreach(GameObject go in slides)
        {
            allSlides.Add(go.GetComponent<RuntimeSlide>());
        }
    }

    void Update()
    {
        foreach(RuntimeSlide rs in allSlides)
        {
            if (rs.brokenDown)
            {
                StartCoroutine(ToggleMusic(true));
                return;
            }
        }

        // none broken down
        StartCoroutine(ToggleMusic(false));
    }

    private readonly int transitionTime = 60;

    private IEnumerator ToggleMusic(bool fast)
    {
        if (transitionComplete && fast != playFast)
        {
            Debug.Log($"Starting transition of music from {playFast} to {fast}");
            playFast = fast;
            transitionComplete = false;

            for (float volumeTransition = 0; volumeTransition < transitionTime; volumeTransition++)
            {
                Debug.Log($"Pitch adjusting loop {volumeTransition}");
                if (playFast)
                {
                    slowMusic.volume = Mathf.MoveTowards(slowMusic.volume, 0f, 0.16f);
                    fastMusic.volume = Mathf.MoveTowards(fastMusic.volume, 1f, 0.16f);
                    fastMusic.pitch = Mathf.MoveTowards(fastMusic.pitch, 1f, (timeMap / 60f));
                }
                else
                {
                    slowMusic.volume = Mathf.MoveTowards(slowMusic.volume, 1f, 0.16f);
                    fastMusic.volume = Mathf.MoveTowards(fastMusic.volume, 0f, 0.16f);
                    fastMusic.pitch = Mathf.MoveTowards(fastMusic.pitch, timeMap, (timeMap / 60f));
                }

                yield return new WaitForSeconds(0.15f);
            }
            Debug.Log("Transition done");
            transitionComplete = true;
        }
        yield break;
    }
}
