using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private AudioClip loseMusic;
    [SerializeField] private AudioClip takeDamageSFX;
    [SerializeField] private AudioClip fishCaughtSFX;
    [SerializeField] private AudioClip schoolCaughtSFX;
    [SerializeField] private AudioClip playerEnterWaterSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Subscribe to events
        PlayerController.OnEnterWater += OnPlayerEnterWater;
        FishController.OnFishCaught += OnFishCaught;
        SchoolController.OnSchoolCaught += OnSchoolCaught;
        OrcaController.OnPlayerCaught += OnPlayerTakeDamage;
        LevelManager.OnLevelComplete += OnWin;
        PlayerHealth.OnPlayerDeath += OnLose;
    }

    //Unsubscribe from events
    private void OnDisable()
    {
        PlayerController.OnEnterWater -= OnPlayerEnterWater;
        FishController.OnFishCaught -= OnFishCaught;
        SchoolController.OnSchoolCaught -= OnSchoolCaught;
        OrcaController.OnPlayerCaught -= OnPlayerTakeDamage;
        LevelManager.OnLevelComplete -= OnWin;
        PlayerHealth.OnPlayerDeath -= OnLose;
    }

    //Background music
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }
    public void StopBGM() => bgmSource.Stop();

    //Ambient
    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null) return;
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }
    public void StopAmbient() => ambientSource.Stop();

    //SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    //Event-driven methods
    private void OnFishCaught(int score) => PlaySFX(fishCaughtSFX); //score is unused here but required by the event signature
    private void OnSchoolCaught(int score) => PlaySFX(schoolCaughtSFX); //score is unused here but required by the event signature
    private void OnPlayerEnterWater() => PlaySFX(playerEnterWaterSFX);
    private void OnPlayerTakeDamage() => PlaySFX(takeDamageSFX);
    private void OnWin() 
    { 
        PlayBGM(winMusic, false); 
        StopAmbient(); 
    }
    private void OnLose()
    {
        PlayBGM(loseMusic, false);
        StopAmbient();
    }
}