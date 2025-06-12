using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SoundEffectVolume";

    public static AudioManager Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [Range(0f, 1f)] [SerializeField] private float defaultMusicVolume = 0.3f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip chopSound;
    [SerializeField] private AudioClip deliverySuccessSound;
    [SerializeField] private AudioClip deliveryFailSound;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip objectDropSound;
    [SerializeField] private AudioClip objectPickupSound;
    [SerializeField] private AudioClip stoveSizzleSound;
    [SerializeField] private AudioClip trashSound;
    [SerializeField] private AudioClip warningSound;

    private AudioSource musicSource;
    private float musicVolume;
    private float sfxVolume;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one AudioManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.clip = backgroundMusic;

        
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    #region Volume Control
    public void ChangeMusicVolume()
    {
        musicVolume += 0.1f;
        if (musicVolume > 1f)
        {
            musicVolume = 0f;
        }
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
    }

    public void ChangeSFXVolume()
    {
        sfxVolume += 0.1f;
        if (sfxVolume > 1f)
        {
            sfxVolume = 0f;
        }
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    #endregion

    #region Sound Effects
    public void PlayChopSound(Vector3 position) => PlaySound(chopSound, position);
    public void PlayDeliverySuccessSound(Vector3 position) => PlaySound(deliverySuccessSound, position);
    public void PlayDeliveryFailSound(Vector3 position) => PlaySound(deliveryFailSound, position);
    public void PlayFootstepSound(Vector3 position, float volumeMultiplier = 1f) => PlaySound(footstepSound, position, volumeMultiplier);
    public void PlayObjectDropSound(Vector3 position) => PlaySound(objectDropSound, position);
    public void PlayObjectPickupSound(Vector3 position) => PlaySound(objectPickupSound, position);
    public void PlayStoveSizzleSound(Vector3 position) => PlaySound(stoveSizzleSound, position);
    public void PlayTrashSound(Vector3 position) => PlaySound(trashSound, position);
    public void PlayWarningSound(Vector3 position) => PlaySound(warningSound, position);

    private void PlaySound(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        if (clip != null)
        {
            
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, sfxVolume * volumeMultiplier);
            Debug.Log(sfxVolume);
        }
    }
    #endregion
}