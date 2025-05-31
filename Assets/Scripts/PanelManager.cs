using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        public List<float> timeThresholds = new List<float>();
        public List<int> viewCountThresholds = new List<int>();

        [HideInInspector] public float viewTime = 0f;
        [HideInInspector] public bool isActive = false;
        [HideInInspector] public int timesWatched = 0;

        private HashSet<float> timeRewardsGiven = new HashSet<float>();
        private HashSet<int> viewCountRewardsGiven = new HashSet<int>();
        public List<RewardEntry> rewardEntries = new List<RewardEntry>();

        public PanelManager panelManager;

        public void Initialize()
        {
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached += OnVideoFinished;
            }
        }

        private void OnVideoFinished(UnityEngine.Video.VideoPlayer vp)
        {
            timesWatched++;
            Debug.Log($"'{panelName}' video completed. Watched {timesWatched} times.");
            panelManager.IncrementArtistViewCount(artistName);
        }

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

    private Dictionary<string, float> artistViewTimes = new Dictionary<string, float>();
    private Dictionary<string, int> artistViewCounts = new Dictionary<string, int>();

    private HashSet<string> givenRewards = new HashSet<string>();
    public List<RewardEntry> rewardEntries;

    void Start()
    {
        panelDict = new Dictionary<string, PanelEntry>();
        foreach (var entry in panels)
        {
            entry.Initialize();
            panelDict[entry.panelName] = entry;
        }
    }

    // Update is called once per frame
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

    public void CheckArtistRewards(string artistName)
    {
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
            }
        }
    }

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

    public void ShowPanel(string name)
    {
        foreach( var entry in panels)
        {
            bool shouldBeActive = entry.panelName == name;
            entry.panelObject.SetActive(shouldBeActive);
            entry.isActive = shouldBeActive;
        }
    }

    public void CloseAllPanels()
    {
        foreach (var entry in panels)
        {
            entry.panelObject.SetActive(false);
            entry.isActive = false;
        }
    }

    public float GetViewTime(string panelName)
    {
        if(panelDict.TryGetValue(panelName, out var entry))
        {
            return entry.viewTime;
        }
        return 0f;
    }

    public float GetArtistViewTime(string artistName)
    {
        if(artistViewTimes.TryGetValue(artistName, out float time))
        {
            return time;
        }
        return 0f;
    }

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
    }

    public void CloseCharacterPanel()
    {
        characterPanel.SetActive(false);
    }

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



    public void OpenInventory()
    {
        if (inventoryPanel.activeSelf)
            inventoryPanel.SetActive(false);
        else inventoryPanel.SetActive(true);
    }

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
