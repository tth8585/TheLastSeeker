using UnityEngine;

namespace Imba.Audio
{
    [SerializeField]
    public enum AudioName
    {
        // Main Music
        Track_1 = -2,

        NoSound = -1,

        BGM_Menu = 0,
        BGM_Garage = 1,

        CollectItemBox = 21,
        UseDashPad = 22,
        UseRampPad = 23,

        LightCarRun = 25,
        MediumCarRun = 26,
        HeavyCarRun = 27,

        TapButton = 28,
        BackOrCancel = 29,

        ConfirmOrPick = 30,
        LevelUp = 31,

        OpenKubik = 32,
        SelectItem = 33,

        Upgrade = 34,
        ExpCount = 35,
        SelectTab = 64,

        Drift = 36,
        NitroActive = 37,
        CollectNitro = 38,

        #region UI Kubik Item
        SpawnCard = 39,
        ShowCardNormal = 40,
        ShowCardRare = 41,
        ShowCardEpic = 42,
        ShowCardLegendary = 43,
        #endregion

        Countdown = 53,
        Start = 54,
        Win = 55,
        Lose = 56,

        CollectCoin = 59,
        CollectGem = 60,

        OpenContainerBackside = 61,
        OpenContainerLRside = 62,
        OpenContainerFrontside = 63,
    }


    public enum AudioType
    {
        SFX = 0,
        BGM = 1
    }
}