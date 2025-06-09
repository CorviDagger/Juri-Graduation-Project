using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    private double pauseTime = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        //videoPlayer.Pause();
    }
    //Make sure the video is paused in the beginning
    //(there were some issues where it would play even though play on awake was checked off, this fixed it)
    void Start()
    {
        Debug.Log("Player starting");
        videoPlayer.Pause();
    }


    //These are now useless since the video players are not being enabled and disabled anymore
    //to be able to continue the video from where the panel that includes them is paused
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
