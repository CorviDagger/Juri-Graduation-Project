using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    private double pauseTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player starting");
        videoPlayer.Pause();
    }

    private void OnEnable()
    {
        pauseTime = videoPlayer.time;
        videoPlayer.Play();
    }

    private void OnDisable()
    {
        videoPlayer.time = pauseTime;
        videoPlayer.Pause();
    }
}
