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

        public List<float> timeThresholds = new List<float>();
        public List<int> viewCountThresholds = new List<int>();

        [HideInInspector] public float viewTime = 0f;
        [HideInInspector] public bool isActive = false;
        [HideInInspector] public int timesWatched = 0;

        private HashSet<float> timeRewardsGiven = new HashSet<float>();
        private HashSet<int> viewCountRewardsGiven = new HashSet<int>();


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
        }

        public void CheckRewards()
        {
            foreach (var threshold in timeThresholds)
            {
                if (viewTime >= threshold && !timeRewardsGiven.Contains(threshold))
                {
                    timeRewardsGiven.Add(threshold);
                    Debug.Log($"{panelName}: Earned reward for {threshold} seconds watched!");
                    string rewardName = $"{panelName}_Time_{threshold}s";
                    InventoryManager.Instance.UnlockItem(rewardName);
                }
            }

            foreach (var threshold in viewCountThresholds)
            {
                if (timesWatched >= threshold && !viewCountRewardsGiven.Contains(threshold))
                {
                    viewCountRewardsGiven.Add(threshold);
                    Debug.Log($"{panelName}: Earned reward for {threshold} views!");
                    string rewardName = $"{panelName}_Views_{threshold}";
                    InventoryManager.Instance.UnlockItem(rewardName);
                }
            }
        }
    }

    public List<PanelEntry> panels;
    private Dictionary<string, PanelEntry> panelDict;

    public GameObject characterPanel;
    public GameObject statsPanel;
    public TextMeshProUGUI statsText;

    public GameObject inventoryPanel;
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
                entry.CheckRewards();
            }
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
    }



    public void OpenInventory()
    {
        if (inventoryPanel.activeSelf)
            inventoryPanel.SetActive(false);
        else inventoryPanel.SetActive(true);
    }


}
