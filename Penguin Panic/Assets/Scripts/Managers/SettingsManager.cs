using System.Data.Common;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider sfxSlider;

    //Cached volumes before mute
    private float lastMasterVolume;
    private float lastBGMVolume;
    private float lastAmbientVolume;
    private float lastSFXVolume;

    private float preMuteMasterVolume;
    private float preMuteBGMVolume;
    private float preMuteAmbientVolume;
    private float preMuteSFXVolume;

    private void Start()
    {
        // Initialise sliders to current mixer values
        lastMasterVolume = InitSlider(masterSlider, "MasterVolume");
        lastBGMVolume = InitSlider(bgmSlider, "BGMVolume");
        lastAmbientVolume = InitSlider(ambientSlider, "AmbientVolume");
        lastSFXVolume = InitSlider(sfxSlider, "SFXVolume");
    }

    private float InitSlider(Slider slider, string parameter)
    {
        if (masterMixer.GetFloat(parameter, out float dB))
        {
            slider.SetValueWithoutNotify(Mathf.Pow(10f, dB / 20f));
            return dB;
        }
        else
        {
            slider.SetValueWithoutNotify(1f);
            return 0f;
        }
    }

    //Volume setters
    public void SetMasterVolume()
    {
        float value = masterSlider.value;
        lastMasterVolume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        masterMixer.SetFloat("MasterVolume", lastMasterVolume);
    }

    public void SetBGMVolume()
    {
        float value = bgmSlider.value;
        lastBGMVolume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        masterMixer.SetFloat("BGMVolume", lastBGMVolume);
    }

    public void SetAmbientVolume()
    {
        float value = ambientSlider.value;
        lastAmbientVolume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        masterMixer.SetFloat("AmbientVolume", lastAmbientVolume);
    }

    public void SetSFXVolume()
    {
        float value = sfxSlider.value;
        lastSFXVolume = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
        masterMixer.SetFloat("SFXVolume", lastSFXVolume);
    }

    //Mute methods
    public void ToggleMuteMaster()
    {
        if (IsMuted("MasterVolume"))
        {
            masterMixer.SetFloat("MasterVolume", preMuteMasterVolume);
            masterSlider.SetValueWithoutNotify(Mathf.Pow(10f, preMuteMasterVolume / 20f));
        }
        else
        {
            if (masterMixer.GetFloat("MasterVolume", out float current))
            {
                preMuteMasterVolume = current;
            }
            masterMixer.SetFloat("MasterVolume", -80f);
            masterSlider.SetValueWithoutNotify(0f);
        }
    }

    public void ToggleMuteBGM()
    {
        if (IsMuted("BGMVolume"))
        {
            masterMixer.SetFloat("BGMVolume", preMuteBGMVolume);
            bgmSlider.SetValueWithoutNotify(Mathf.Pow(10f, preMuteBGMVolume / 20f));
        }
        else
        {
            if (masterMixer.GetFloat("BGMVolume", out float current))
                preMuteBGMVolume = current;

            masterMixer.SetFloat("BGMVolume", -80f);
            bgmSlider.SetValueWithoutNotify(0f);
        }
    }

    public void ToggleMuteAmbient()
    {
        if (IsMuted("AmbientVolume"))
        {
            masterMixer.SetFloat("AmbientVolume", preMuteAmbientVolume);
            ambientSlider.SetValueWithoutNotify(Mathf.Pow(10f, preMuteAmbientVolume / 20f));
        }
        else
        {
            if (masterMixer.GetFloat("AmbientVolume", out float current))
                preMuteAmbientVolume = current;

            masterMixer.SetFloat("AmbientVolume", -80f);
            ambientSlider.SetValueWithoutNotify(0f);
        }
    }

    public void ToggleMuteSFX()
    {
        if (IsMuted("SFXVolume"))
        {
            masterMixer.SetFloat("SFXVolume", preMuteSFXVolume);
            sfxSlider.SetValueWithoutNotify(Mathf.Pow(10f, preMuteSFXVolume / 20f));
        }
        else
        {
            if (masterMixer.GetFloat("SFXVolume", out float current))
                preMuteSFXVolume = current;

            masterMixer.SetFloat("SFXVolume", -80f);
            sfxSlider.SetValueWithoutNotify(0f);
        }
    }

    //Check mute state
    private bool IsMuted(string parameter)
    {
        print("Checking mute state for " + parameter);
        if (masterMixer.GetFloat(parameter, out float value))
        {
            print("Current volume: " + value);
            print("Is muted: " + (value <= -79f));
            return value <= -79f;
        }
        return false;
    }
}