using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSlideSounds : MonoBehaviour
{
    public RuntimeSlide controller;
    public AudioSource ambientLoop;
    public AudioSource startActivity;
    public float activityTime; // in units of ambientLoop.time
    public AudioSource endActivity;

    public AudioSource[] randomOnStart;

    public AudioSource closedSound;

    
    public bool isRunning = false;
    public bool isTransitioning = false;
    public bool inActivityLoop = false;

    void Start()
    {
        controller = transform.parent.GetComponent<RuntimeSlide>();
        Debug.Log("Slide sound init");
    }

    void Update()
    {
        if (!controller.closed && isRunning)
        {
            if (!inActivityLoop && !isTransitioning)
            {
                Debug.Log("Not in activity loop, stating it");
                StartCoroutine(ActivityLoop());
            }
        }
        else if (!controller.closed && !isRunning)
        {
            Debug.Log("Opening ride");
            if (!isTransitioning)
            {
                StartCoroutine(Open());
                isRunning = true;
            }
        }
        else if (controller.closed && isRunning)
        {
            Debug.Log("Closing ride");
            if (!isTransitioning)
            {
                StartCoroutine(Close());
                isRunning = false;
            }
        }
    }

    private IEnumerator ActivityLoop()
    {
        Debug.Log("Activity loop start");
        inActivityLoop = true;
        startActivity.PlayDelayed(Random.Range(0f, 1f));
        int x = Random.Range(0, 10);
        if (x < randomOnStart.Length)
        {
            randomOnStart[x].Play();
        }
        yield return new WaitForSeconds(ambientLoop.clip.length * activityTime);
        if (isRunning)
        {
            endActivity.Play();
        }
        inActivityLoop = false;
        Debug.Log("Activity loop end");
    }

    private IEnumerator Open()
    {
        Debug.Log("open loop start");

        isTransitioning = true;
        startActivity.Play();
        ambientLoop.PlayDelayed(Random.Range(0f, 1f));
        for (int i = 0; i < 30; i++)
        {
            if (closedSound != null)
            {
                closedSound.volume = Mathf.MoveTowards(closedSound.volume, 0f, 0.1f);
            }
            ambientLoop.volume = Mathf.MoveTowards(ambientLoop.volume, 1f, 0.1f);
            yield return new WaitForSeconds(0.16f);
        }
        if (closedSound != null)
        {
            closedSound.Stop();
        }
        isTransitioning = false;
        Debug.Log("Activity loop end");
    }

    private IEnumerator Close()
    {
        Debug.Log("close start");

        if (closedSound != null)
        {
            closedSound.Play();
        }
        isTransitioning = true;
        for (int i = 0; i < 30; i++)
        {
            ambientLoop.volume = Mathf.MoveTowards(ambientLoop.volume, 0f, 0.1f);
            if (closedSound != null)
            {
                closedSound.volume = Mathf.MoveTowards(closedSound.volume, 1f, 0.1f);
            }
            yield return new WaitForSeconds(0.16f);
        }
        ambientLoop.Stop();
        endActivity.Play();
        isTransitioning = false;
        Debug.Log("close end");
    }
}
