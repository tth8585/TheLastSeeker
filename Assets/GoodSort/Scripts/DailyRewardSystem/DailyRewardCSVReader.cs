using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardCSVReader : MonoBehaviour
{
    [SerializeField] TextAsset csvFile;

    private Dictionary<int, DailyRewardDataConfig> _dicDaylyReward = new Dictionary<int, DailyRewardDataConfig>();

    private void Start()
    {
        _dicDaylyReward.Clear();
        _dicDaylyReward = ParseRewardsData();
    }

    private Dictionary<int, DailyRewardDataConfig> ParseRewardsData()
    {
        Dictionary<int, DailyRewardDataConfig> level = new Dictionary<int, DailyRewardDataConfig>();

        // Split the CSV text into lines
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');
            level.Add(int.Parse(fields[0]), new DailyRewardDataConfig(int.Parse(fields[0]), fields[1], int.Parse(fields[2])));
        }

        return level;
    }

    public Dictionary<int, DailyRewardDataConfig> GetReward()
    {
        return _dicDaylyReward;
    }
}

public class DailyRewardDataConfig
{
    public int Day;
    public string Type;
    public int Value;

    public DailyRewardDataConfig(int day, string type, int value)
    {
        Day= day;
        Type= type;
        Value= value;
    }
}
