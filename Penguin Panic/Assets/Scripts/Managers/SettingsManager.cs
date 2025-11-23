using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer masterMixer;

    //Cached volumes before mute
    private float lastBGMVolume = 0f;
    private float lastAmbientVolume = 0f;
    private float lastSFXVolume = 0f;

    //Volume setters
    public void SetBGMVolume(float value)
    {
        lastBGMVolume = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("BGMVolume", lastBGMVolume);
    }

    public void SetAmbientVolume(float value)
    {
        lastAmbientVolume = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("AmbientVolume", lastAmbientVolume);
    }

    public void SetSFXVolume(float value)
    {
        lastSFXVolume = Mathf.Log10(value) * 20;
        masterMixer.SetFloat("SFXVolume", lastSFXVolume);
    }

    //Mute methods
    public void MuteBGM()
    {
        masterMixer.GetFloat("BGMVolume", out lastBGMVolume);
        masterMixer.SetFloat("BGMVolume", -80f);
    }

    public void MuteAmbient()
    {
        masterMixer.GetFloat("AmbientVolume", out lastAmbientVolume);
        masterMixer.SetFloat("AmbientVolume", -80f);
    }

    public void MuteSFX()
    {
        masterMixer.GetFloat("SFXVolume", out lastSFXVolume);
        masterMixer.SetFloat("SFXVolume", -80f);
    }

    //Unmute methods
    public void UnmuteBGM()
    {
        masterMixer.SetFloat("BGMVolume", lastBGMVolume);
    }

    public void UnmuteAmbient()
    {
        masterMixer.SetFloat("AmbientVolume", lastAmbientVolume);
    }

    public void UnmuteSFX()
    {
        masterMixer.SetFloat("SFXVolume", lastSFXVolume);
    }
}