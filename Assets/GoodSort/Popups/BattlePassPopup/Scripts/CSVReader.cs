using System.Collections.Generic;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    [SerializeField] TextAsset csvFile;

    private List<RewardBattlePass> rewards = new List<RewardBattlePass>();

    public List<RewardBattlePass>  GetBattlePassReward()
    {
        rewards.Clear();

        if (csvFile != null)
        {
            rewards = ParseRewardsData(csvFile.text);

            // Display parsed rewards
            //foreach (RewardGSPass reward in rewards)
            //{
            //    Debug.Log($"Level: {reward.Level}, Free: {reward.FreeAmount} {reward.FreeRewardType}, " +
            //              $"PRO: {reward.ProAmount} {reward.ProRewardType}, " +
            //}
        }
        else
        {
            Debug.LogError("CSV file is not assigned!");
        }

        return rewards;
    }

    //public List<AbilitiesReward> GetAbilitiesRewards()
    //{
    //    List<AbilitiesReward> abilitiesRewards = new List<AbilitiesReward>();
    //    if (csvFile != null)
    //    {
    //        abilitiesRewards = ParseAbilitiesRewardsData(csvFile.text);
    //    }
    //    else
    //    {
    //        Debug.LogError("CSV file is not assigned!");
    //    }

    //    return abilitiesRewards;
    //}

    void Start()
    {
       
    }

    private List<RewardBattlePass> ParseRewardsData(string csvText)
    {
        List<RewardBattlePass> rewards = new List<RewardBattlePass>();

        // Split the CSV text into lines
        string[] lines = csvText.Split('\n');

        for(int i=1;i<lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');

            RewardBattlePass reward = new RewardBattlePass
            {
                Level = int.Parse(fields[0]),
                FreeAmount = int.Parse(fields[1]),
                FreeRewardType = fields[2],

                ProAmount = int.Parse(fields[3]),
                ProRewardType = fields[4],
            };

            // Add the reward to the list
            rewards.Add(reward);
        }

        return rewards;
    }

    //private List<AbilitiesReward> ParseAbilitiesRewardsData(string csvText)
    //{
    //    List<AbilitiesReward> rewards = new List<AbilitiesReward>();

    //    // Split the CSV text into lines
    //    string[] lines = csvText.Split('\n');

    //    for (int i = 1; i < lines.Length; i++)
    //    {
    //        string[] fields = lines[i].Trim().Split(',');

    //        AbilitiesReward reward = new AbilitiesReward
    //        {
    //            lvl = int.Parse(fields[0]),
    //            tokenReq = int.Parse(fields[1]),
    //            rewardValue = int.Parse(fields[2]),
    //            desc = $"+{fields[2]}% {fields[3]}",
    //            abilityType = fields[4],
    //            isBigMilestone = fields[5].Equals("true"),

    //        };
    //        if (!reward.isBigMilestone)
    //            reward.icon = Resources.Load<Sprite>($"AbiltyIcons/{fields[4]}");
    //        else
    //            reward.icon = Resources.Load<Sprite>($"AbiltyIcons/{fields[4]}_Big");

    //        // Add the reward to the list
    //        rewards.Add(reward);
    //    }

    //    return rewards;
    //}
}

public class RewardBattlePass
{
    public int Level { get; set; }
    public int FreeAmount { get; set; }
    public bool isClaimedFree { get; set; }
    public string FreeRewardType { get; set; }
    public int ProAmount { get; set; }
    public string ProRewardType { get; set; }
    public bool isClaimedPro { get; set; }
}
