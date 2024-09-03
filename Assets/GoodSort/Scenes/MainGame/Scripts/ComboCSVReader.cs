using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCSVReader : MonoBehaviour
{
    [SerializeField] TextAsset csvFile;

    private Dictionary<int, ComboDataConfig> _dicCombo = new Dictionary<int, ComboDataConfig>();

    private void Start()
    {
        _dicCombo.Clear();
        _dicCombo = ParseRewardsData();
    }

    public Dictionary<int, ComboDataConfig> LoadConfig()
    {
        return ParseRewardsData();
    }

    private Dictionary<int, ComboDataConfig> ParseRewardsData()
    {
        Dictionary<int, ComboDataConfig> level = new Dictionary<int, ComboDataConfig>();

        // Split the CSV text into lines
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');

            ComboDataConfig dataConfig = new ComboDataConfig();
            dataConfig.Id = int.Parse(fields[0]);
            dataConfig.Name = fields[1];
            dataConfig.TimeValue = int.Parse(fields[2]);
            dataConfig.StarBonusValue = int.Parse(fields[3]);
            dataConfig.FloatingText= $"<sprite name=\"{fields[4]}\">";

            level.Add(dataConfig.Id, dataConfig);
        }

        return level;
    }

    public Dictionary<int, ComboDataConfig> GetReward()
    {
        return _dicCombo;
    }
}

public class ComboDataConfig
{
    public int Id;
    public string Name;
    public int TimeValue;
    public int StarBonusValue;
    public string FloatingText;
}
