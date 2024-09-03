using Imba.Audio;
using Imba.UI;
using System;
using UnityEngine;

public class SettingsPopup : UIPopup
{
    #region Fields
    [SerializeField] private SettingButton _musicBtn;
    [SerializeField] private SettingButton _sfxBtn;
    [SerializeField] private SettingButton _vibrationBtn;

    private SettingControl _settings;
    #endregion

    #region Properties
    internal SettingButton MusicBtn => _musicBtn;
    internal SettingButton SFXBtn => _sfxBtn;
    internal SettingButton VibrationBtn => _vibrationBtn;
    #endregion

    #region UI popup override method
    protected override void OnInit()
    {
        _settings = new SettingControl(this);
    }
    #endregion

    #region ON CLICK
    public void OnClickSFX()
    {
        _settings.SfxControl();
        Debug.Log($"<color=yellow>SFX Button Clicked</color>");
    }

    public void OnClickMusic()
    {
        _settings.MusicControl();
        Debug.Log($"<color=yellow>Music Button Clicked</color>");
    }

    public void OnClickVibration()
    {
        _settings.VibrationControl();
        Debug.Log($"<color=yellow>Vibration Button Clicked</color>");
    }

    public void OnClickSkin()
    {
        Debug.Log($"<color=purple>Skin Button Clicked</color>");
    }

    public void OnClickSupport()
    {
        Debug.Log($"<color=purple>Support Button Clicked</color>");
    }

    public virtual void OnClickHideSetting()
    {
        UIManager.Instance.PopupManager.HidePopup(UIPopupName.SettingsPopup);
    }
    #endregion
}

public class SettingControl
{
    private bool _muteMusic = false;
    private bool _muteSfx = false;
    private bool _muteVibration = false;

    private SettingsPopup _settingsPopup;

    // ref to AudioManager playerpref key
    private const string MUSIC = "MuteMusic";
    private const string SFX = "MuteSFX";
    private const string VIBRATION = "VibrationOff";

    public SettingControl(SettingsPopup settingsPopup)
    {
        _settingsPopup = settingsPopup;

        GetSavedSetting();
    }

    private void GetSavedSetting()
    {
        if (PlayerPrefs.HasKey("MuteMusic"))
        {
            _muteMusic = PlayerPrefs.GetInt("MuteMusic") == 1 ? true : false;
            _settingsPopup.MusicBtn.EnableSetting(_muteMusic);
        }

        if (PlayerPrefs.HasKey("MuteSFX"))
        {
            _muteSfx = PlayerPrefs.GetInt("MuteSFX") == 1 ? true : false;
            _settingsPopup.SFXBtn.EnableSetting(_muteSfx);
        }

        _muteVibration = AudioManager.Instance.IsVibrationOff;
    }

    internal void MusicControl()
    {
        _muteMusic = _muteMusic == true ? false : true;
        _settingsPopup.MusicBtn.EnableSetting(_muteMusic);

        if (_muteMusic)
            AudioManager.Instance.MuteMusic();
        //else
        //    AudioManager.Instance.PlayMusic();

        SaveSetting(MUSIC, _muteMusic);
    }

    internal void SfxControl()
    {
        _muteSfx = _muteSfx == true ? false : true;
        _settingsPopup.SFXBtn.EnableSetting(_muteSfx);

        if (_muteSfx)
            AudioManager.Instance.MuteSfx();

        SaveSetting(SFX, _muteSfx);
    }

    internal void VibrationControl()
    {
        _muteVibration = _muteVibration == true ? false : true;
        _settingsPopup.VibrationBtn.EnableSetting(_muteVibration);

        if (_muteVibration)
            AudioManager.Instance.IsVibrationOff = true;
        else
            AudioManager.Instance.IsVibrationOff = false;

        SaveSetting(VIBRATION, _muteVibration);
    }

    private void SaveSetting(string key, bool isMute)
    {
        var saveValue = isMute ? 1 : 0;
        PlayerPrefs.SetInt(key, saveValue);
    }
}