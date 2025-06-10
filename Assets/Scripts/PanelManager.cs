using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PanelManager : MonoBehaviour
{
    // Start is called before the first frame update

    [System.Serializable]
    public class PanelEntry
    {
        public string panelName;
        public GameObject panelObject;
        public UnityEngine.Video.VideoPlayer videoPlayer;
        public string artistName;
        public RawImage videoDisplay;

        public List<float> timeThresholds = new List<float>();
        public List<int> viewCountThresholds = new List<int>();

        [HideInInspector] public float viewTime = 0f;
        [HideInInspector] public bool isActive = false;
        [HideInInspector] public int timesWatched = 0;

        private HashSet<float> timeRewardsGiven = new HashSet<float>();
        private HashSet<int> viewCountRewardsGiven = new HashSet<int>();
        public List<RewardEntry> rewardEntries = new List<RewardEntry>();

        public PanelManager panelManager;

        //Makes OnVideoFinished usable
        public void Initialize()
        {
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached += OnVideoFinished;
            }
        }

        //Updates how often a video/a video of an artist has been viewed to the end
        private void OnVideoFinished(UnityEngine.Video.VideoPlayer vp)
        {
            timesWatched++;
            Debug.Log($"'{panelName}' video completed. Watched {timesWatched} times.");
            panelManager.IncrementArtistViewCount(artistName);
        }

        //Old function that is currently not used. Checks Rewards pased on panel, could be interesting for the future if rewards specific to videos need to be implemented
        public void CheckRewards(Dictionary<string, float> artistViewTimes)
        {
            foreach (var reward in rewardEntries)
            {
                if (reward.panelName != panelName) continue;

                if (reward.type == RewardEntry.RewardType.Time && artistName == reward.artistName && artistViewTimes.TryGetValue(artistName, out float artistTime) && artistTime >= reward.thresholdTime && !timeRewardsGiven.Contains(reward.thresholdTime))
                {
                    timeRewardsGiven.Add(reward.thresholdTime);
                    InventoryManager.Instance.UnlockItem(reward.itemToUnlock);
                    Debug.Log($"{panelName}: Unlocked {reward.itemToUnlock}");
                }
                if (reward.type == RewardEntry.RewardType.ViewCount && reward.panelName == panelName && timesWatched >= reward.thresholdCount && !viewCountRewardsGiven.Contains(reward.thresholdCount))
                {
                    viewCountRewardsGiven.Add(reward.thresholdCount);
                    InventoryManager.Instance.UnlockItem(reward.itemToUnlock);
                    Debug.Log($"{panelName}: Unlocked {reward.itemToUnlock}");
                }
            //    {
            //        timeRewardsGiven.Add(threshold);
            //        Debug.Log($"{panelName}: Earned reward for {threshold} seconds watched!");
            //        string rewardName = $"{panelName}_Time_{threshold}s";
            //        InventoryManager.Instance.UnlockItem(rewardName);
            //    }
            //}

            //foreach (var threshold in viewCountThresholds)
            //{
            //    if (timesWatched >= threshold && !viewCountRewardsGiven.Contains(threshold))
            //    {
            //        viewCountRewardsGiven.Add(threshold);
            //        Debug.Log($"{panelName}: Earned reward for {threshold} views!");
            //        string rewardName = $"{panelName}_Views_{threshold}";
            //        InventoryManager.Instance.UnlockItem(rewardName);
            //    }
            }
        }
    }

    public List<PanelEntry> panels;
    private Dictionary<string, PanelEntry> panelDict;

    public GameObject characterPanel;
    public GameObject statsPanel;
    public TextMeshProUGUI statsText;

    public GameObject inventoryPanel;
    public GameObject notificationSprite;

    private Dictionary<string, float> artistViewTimes = new Dictionary<string, float>();
    private Dictionary<string, int> artistViewCounts = new Dictionary<string, int>();

    private HashSet<string> givenRewards = new HashSet<string>();
    public List<RewardEntry> rewardEntries;
    public GameObject[] toHide;

    public Image firstPlaceTrophy;
    public Image secondPlaceTrophy;
    public Image thirdPlaceTrophy;

    public TMP_Text firstPlaceText;
    public TMP_Text secondPlaceText;
    public TMP_Text thirdPlaceText;

    void Start()
    {
        panelDict = new Dictionary<string, PanelEntry>();
        foreach (var entry in panels)
        {
            entry.Initialize();
            panelDict[entry.panelName] = entry;
        }
    }

    //Counts time and calls the function that checks for rewards
    private void Update()
    {
        foreach (var entry in panels)
        {
            if (entry.isActive)
            {
                entry.viewTime += Time.deltaTime;
                if (!artistViewTimes.ContainsKey(entry.artistName))
                {
                    artistViewTimes[entry.artistName] = 0f;
                }
                artistViewTimes[entry.artistName] += Time.deltaTime;
                CheckArtistRewards(entry.artistName);
            }
        }
    }

    //Checks whether rewards should be granted or not, then grants them
    public void CheckArtistRewards(string artistName)
    {
        //cycle through rewards and makes sure only rewards for correct artist and no duplicates are received
        foreach(var reward in rewardEntries)
        {
            if(reward.artistName != artistName)
            {
                continue;
            }
            string rewardKey = $"{artistName}_{reward.itemToUnlock}";
            if (givenRewards.Contains(rewardKey))
            {
                continue;
            }
            bool rewardGranted = false;
            //Checks if conditions for reward are met and then grants reward
            if(reward.type == RewardEntry.RewardType.Time && artistViewTimes.TryGetValue(artistName, out float time) && time >= reward.thresholdTime)
            {
                rewardGranted = true;
            }
            else if (reward.type == RewardEntry.RewardType.ViewCount && artistViewCounts.TryGetValue(artistName, out int views) && views >= reward.thresholdCount)
            {
                rewardGranted = true;
            }
            if (rewardGranted)
            {
                givenRewards.Add(rewardKey);
                InventoryManager.Instance.UnlockItem(reward.itemToUnlock);
                Debug.Log($"{ artistName}: Unlocked { reward.itemToUnlock}");
                EnableNotification();
            }
        }
    }

    //Counts artists views
    public void IncrementArtistViewCount(string artistName)
    {
        if (artistViewCounts.ContainsKey(artistName))
        {
            artistViewCounts[artistName]++;
        }
        else
        {
            artistViewCounts[artistName] = 1;
        }
    }

    //Is used to show video panels. Also unpauses video player and enables video visibility.
    public void ShowPanel(string name)
    {
        foreach( var entry in panels)
        {
            bool shouldBeActive = entry.panelName == name;
            entry.panelObject.SetActive(shouldBeActive);
            entry.isActive = shouldBeActive;
            if(entry.videoPlayer != null)
            {
                if (shouldBeActive)
                {
                    entry.videoDisplay.enabled = true;
                    entry.videoPlayer.Play();
                }
            }
        }
    }

    //Is used to close the video panels
    public void CloseAllPanels()
    {
        foreach (var entry in panels)
        {
            entry.panelObject.SetActive(false);
            entry.isActive = false;
            if(entry.videoPlayer != null)
            {
                entry.videoPlayer.Pause();
                entry.videoDisplay.enabled = false;
            }
        }
    }

    //Gets view time for a panel entry. Currently not in use but could be interesting for future use cases
    public float GetViewTime(string panelName)
    {
        if(panelDict.TryGetValue(panelName, out var entry))
        {
            return entry.viewTime;
        }
        return 0f;
    }

    //Gets the time of how long an artist ha been viewed
    public float GetArtistViewTime(string artistName)
    {
        if(artistViewTimes.TryGetValue(artistName, out float time))
        {
            return time;
        }
        return 0f;
    }

    //Functions for buttons to open and close panels.
    public void ShowStatsPanel()
    {
        statsPanel.SetActive(true);
        UpdateStatsText();
    }

    public void CloseStatsPanel()
    {
        statsPanel.SetActive(false);
    }

    public void ShowCharacterPanel()
    {
        characterPanel.SetActive(true);
        UpdateStatsText();
        HideButtons();
        UpdateTopArtist();
    }

    public void CloseCharacterPanel()
    {
        characterPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        UnhideButtons();
    }

    //Was used in an earlier version to display the view time and count, is now useless
    private void UpdateStatsText()
    {
        statsText.text = "";
        foreach (var entry in panels)
        {
            statsText.text += $"{entry.panelName} viewed for {entry.viewTime:F1} seconds\n";
            if (entry.videoPlayer != null)
            {
                statsText.text += $"and watched {entry.timesWatched} times";
            }
            statsText.text += "\n";
        }
        foreach (var kvp in artistViewTimes)
        {
            statsText.text += $"{kvp.Key} total view time: {kvp.Value:F1} seconds\n";
        }
    }

    //Enables notification sprite, gets called when a new item is unlocked
    public void EnableNotification()
    {
        notificationSprite.SetActive(true);
    }


    //Hides certain UI features
    public void HideButtons()
    {
        foreach (GameObject hide in toHide)
        {
            hide.SetActive(false);
        }
    }

    public void UnhideButtons()
    {
        foreach (GameObject unhide in toHide)
        {
            unhide.SetActive(true);
        }
    }


    
    //Button that opens inventory. Also deactivates the notification icon.
    public void OpenInventory()
    {
        if (inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }
        else 
        { 
            inventoryPanel.SetActive(true);
            notificationSprite.SetActive(false);
        }
    }

    //Quit Button
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    //Orders list of artist view times and then writes text for the Trophy rack
    private void UpdateTopArtist()
    {
        var topArtists = artistViewTimes.OrderByDescending(kvp => kvp.Value).Take(3).ToList();

        if (topArtists.Count > 0)
        {
            firstPlaceText.text = $"{topArtists[0].Key} Viewing Time: {(topArtists[0].Value / 60f):F0} minute(s) and {(topArtists[0].Value):F1} seconds";
        }
        else
        {
            firstPlaceText.text = "-";
        }

        if (topArtists.Count > 1)
        {
            secondPlaceText.text = $"{topArtists[1].Key} Viewing Time: {(topArtists[1].Value / 60f):F0} minute(s) and {(topArtists[1].Value):F1} seconds";
        }
        else
        {
            secondPlaceText.text = "-";
        }
        if (topArtists.Count > 2)
        {
            thirdPlaceText.text = $"{topArtists[2].Key} Viewing Time: {(topArtists[2].Value / 60f):F0} minute(s) and {(topArtists[2].Value):F1} seconds" ;
        }
        else
        {
            thirdPlaceText.text = "-";
        }


    }

    //Allows me to edit the rewards in Editor 
    [System.Serializable]
    public class RewardEntry
    {
        public string panelName;
        public enum RewardType {  Time, ViewCount }
        public RewardType type;
        public float thresholdTime;
        public int thresholdCount;
        public string itemToUnlock;
        public string artistName;
    }
    


}
