using System.Collections;
using UnityEngine;

public class AudioBehavior : MonoBehaviour
{
    // In order to avoid clicks when the player switches from title to play scene, AudioManager is only created once in the title scene, so that music can fade out when the player clicks "start"

    public static AudioBehavior Instance;

    private AudioSource _oneShotSource;
    private AudioSource _loopSource;

    public float FadeTime = 1.0f;
    private float _currTime = 0.0f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        _oneShotSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameObject child = new("OneShot");
        child.transform.parent = transform;
        _oneShotSource = child.AddComponent<AudioSource>();

        child = new("Loop");
        child.transform.parent = transform;
        _loopSource = child.AddComponent<AudioSource>();
        _loopSource.loop = true;
    }

    public void PlayOneShotSound(AudioClip clip, float volume = 1.0f)
    {
        _oneShotSource.PlayOneShot(clip, volume);
    }

    private IEnumerator FadeAndPlayLoopSound(AudioClip clip, float volume = 1.0f)
    {
        // A coroutine because a fade out might be necessary if a clip is currently looped
        yield return FadeLoop(0.0f);
        _loopSource.clip = clip;
        _loopSource.volume = volume;
        _loopSource.Play();
    }

    public void PlayLoopSound(AudioClip clip, float volume = 1.0f)
    {
        // Just a convenience function
        StartCoroutine(FadeAndPlayLoopSound(clip, volume));
    }

    private IEnumerator FadeLoop(float targetVol)
    {
        float initVol = _loopSource.volume;
        if (initVol == targetVol)
        {
            // If for some reason the volume of the loop is already at the target volume, exit
            yield break;
        }
        while (_loopSource.clip && _currTime < FadeTime)
        {
            // If there is currently another clip playing, fade it out first
            float t = _currTime / FadeTime;
            _loopSource.volume = Mathf.Lerp(initVol, targetVol, t);
            _currTime += Time.deltaTime;
            yield return null;
        }
        _currTime = 0.0f;
    }

    public void FadeOutLoop()
    {
        StartCoroutine(FadeLoop(0.0f));
    }

    public void FadeInLoop(float volume = 1.0f)
    {
        StartCoroutine(FadeLoop(volume));
    }

    public void DimLoop(float volume = 0.25f)
    {
        StartCoroutine(FadeLoop(volume));
    }
}
