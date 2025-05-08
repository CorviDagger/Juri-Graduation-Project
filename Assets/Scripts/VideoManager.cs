using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player starting");
    }

    private void OnEnable()
    {
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    private void OnDisable()
    {
        videoPlayer.Stop();
    }
}
