using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonthlyRewardCSVReader : MonoBehaviour
{
    [SerializeField] TextAsset csvFile;

    private Dictionary<int, MonthlyRewardDataConfig> _dicMonthlyReward = new Dictionary<int, MonthlyRewardDataConfig>();

    private void Start()
    {
        _dicMonthlyReward.Clear();
        _dicMonthlyReward = ParseRewardsData();
    }

    private Dictionary<int, MonthlyRewardDataConfig> ParseRewardsData()
    {
        Dictionary<int, MonthlyRewardDataConfig> level = new Dictionary<int, MonthlyRewardDataConfig>();

        // Split the CSV text into lines
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');
            List<MonthlyRewardDetailData> newList= new List<MonthlyRewardDetailData>();
            newList.Add(new MonthlyRewardDetailData(fields[1], int.Parse(fields[2])));
            newList.Add(new MonthlyRewardDetailData(fields[3], int.Parse(fields[4])));
            newList.Add(new MonthlyRewardDetailData(fields[5], int.Parse(fields[6])));
            level.Add(int.Parse(fields[0]), new MonthlyRewardDataConfig(int.Parse(fields[0]), newList.ToArray()));
        }

        return level;
    }

    public Dictionary<int, MonthlyRewardDataConfig> GetReward()
    {
        return _dicMonthlyReward;
    }
}

public class MonthlyRewardDataConfig
{
    public int Day;
    public MonthlyRewardDetailData[] Details;

    public MonthlyRewardDataConfig(int day, MonthlyRewardDetailData[] data)
    {
        Day = day;
        Details = data;
    }
}

public class MonthlyRewardDetailData
{
    public string TypeReward;
    public int Value;

    public MonthlyRewardDetailData(string type, int value)
    {
        TypeReward = type;
        Value = value;
    }
}
