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
    private float lastMasterVolume = 0f;
    private float lastBGMVolume = 0f;
    private float lastAmbientVolume = 0f;
    private float lastSFXVolume = 0f;

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
        lastBGMVolume = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("BGMVolume", lastBGMVolume);
    }

    public void SetAmbientVolume()
    {
        float value = ambientSlider.value;
        lastAmbientVolume = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("AmbientVolume", lastAmbientVolume);
    }

    public void SetSFXVolume()
    {
        float value = sfxSlider.value;
        lastSFXVolume = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("SFXVolume", lastSFXVolume);
    }

    //Mute methods
    public void ToggleMuteMaster()
    {
        if (IsMuted("MasterVolume"))
        {
            masterMixer.SetFloat("MasterVolume", lastMasterVolume);
            masterSlider.value = Mathf.Pow(10f, lastMasterVolume / 20f);
        }
        else
        {
            masterMixer.GetFloat("MasterVolume", out lastMasterVolume);
            masterMixer.SetFloat("MasterVolume", -80f);
            masterSlider.value = 0f;
        }
    }

    public void ToggleMuteBGM()
    {
        if (IsMuted("BGMVolume"))
        {
            masterMixer.SetFloat("BGMVolume", lastBGMVolume);
            bgmSlider.value = Mathf.Pow(10f, lastBGMVolume / 20f);
        }
        else
        {
            masterMixer.GetFloat("BGMVolume", out lastBGMVolume);
            masterMixer.SetFloat("BGMVolume", -80f);
            bgmSlider.value = 0f;
        }
    }

    public void ToggleMuteAmbient()
    {
        if (IsMuted("AmbientVolume"))
        {
            masterMixer.SetFloat("AmbientVolume", lastAmbientVolume);
            ambientSlider.value = Mathf.Pow(10f, lastAmbientVolume / 20f);
        }
        else
        {
            masterMixer.GetFloat("AmbientVolume", out lastAmbientVolume);
            masterMixer.SetFloat("AmbientVolume", -80f);
            ambientSlider.value = 0f;
        }
    }

    public void ToggleMuteSFX()
    {
        if (IsMuted("SFXVolume"))
        {
            masterMixer.SetFloat("SFXVolume", lastSFXVolume);
            sfxSlider.value = Mathf.Pow(10f, lastSFXVolume / 20f);
        }
        else
        {
            masterMixer.GetFloat("SFXVolume", out lastSFXVolume);
            masterMixer.SetFloat("SFXVolume", -80f);
            sfxSlider.value = 0f;
        }
    }

    //Check mute state
    private bool IsMuted(string parameter)
    {
        if (masterMixer.GetFloat(parameter, out float value))
        {
            return value <= -79f;
        }
        return false;
    }
}