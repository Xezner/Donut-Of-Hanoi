using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Pick up and place sounds")]
    [SerializeField] private AudioClip[] _pickUp;
    [SerializeField] private AudioClip[] _placeDown;

    [Header("Foot steps")]
    [SerializeField] private AudioClip[] _footsteps;
    private float _footstepTimer;
    private float _footstepDuration = 0.15f;

    [Header("Success Prompt sound")]
    [SerializeField] private AudioClip[] _successPrompt;

    [Header("Error Prompt sound")]
    [SerializeField] private AudioClip[] _errorPrompt;

    [Header("Count down sound")]
    [SerializeField] private AudioClip _beepSound;
    private float beepDuration = 1.15f;
    private float beepTimer = 1.15f;

    private PlayerController _playerController;
    public static AudioManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Debug.Log("Audio manager start.");
        GameManager.Instance.OnSuccessPrompt += Instance_OnSuccessPrompt;
        GameManager.Instance.OnErrorPrompt += Instance_OnErrorPrompt;
    }

    private void Update()
    {
        if(_playerController != null && _playerController.IsMoving() && GameManager.Instance.IsPlayerControllable)
        {
            PlayFootSteps();
        }

        if (GameManager.Instance.IsCountdownStarted && _playerController != null)
        {
            beepTimer += Time.deltaTime;
            if (beepTimer >= beepDuration)
            {
                PlaySound(_beepSound, _playerController.transform.position, 0.5f);
                beepTimer = 0f;
            }
        }
        else if(beepTimer != beepDuration)
        {
            beepTimer = beepDuration;
        }
    }

    public void SubscribeToInteractionSound()
    {
        InteractionManager.Instance.OnDiskPickUp += Instance_OnDiskPickUp;
        InteractionManager.Instance.OnDiskPlaceDown += Instance_OnDiskPlaceDown;
    }

    public void UnsubscribeToInteractionSound()
    {
        InteractionManager.Instance.OnDiskPickUp -= Instance_OnDiskPickUp;
        InteractionManager.Instance.OnDiskPlaceDown -= Instance_OnDiskPlaceDown;
    }

    public void SetPlayerController()
    {
        _playerController = PlayerController.Instance;
    }

    public void ResetPlayerController()
    {
        _playerController = null;
    }

    private void PlayFootSteps()
    {
        _footstepTimer -= Time.deltaTime;
        if (_footstepTimer < 0f)
        {
            _footstepTimer = _footstepDuration;
            PlaySound(_footsteps, _playerController.transform.position, 0.5f);
        }
    }

    private void Instance_OnSuccessPrompt(object sender, System.EventArgs e)
    {
        PlaySound(_successPrompt, _playerController.transform.position, 0.5f);
    }

    private void Instance_OnErrorPrompt(object sender, System.EventArgs e)
    {
        PlaySound(_errorPrompt, _playerController.transform.position, 0.2f);
    }

    private void Instance_OnDiskPickUp(object sender, System.EventArgs e)
    {
        PlaySound(_pickUp, _playerController.transform.position, 0.25f);
    }

    private void Instance_OnDiskPlaceDown(object sender, System.EventArgs e)
    {
        PlaySound(_placeDown, _playerController.transform.position, 0.25f);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }
}
