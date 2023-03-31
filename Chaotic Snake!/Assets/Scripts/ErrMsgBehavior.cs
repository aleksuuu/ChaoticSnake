using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrMsgBehavior : MonoBehaviour
{
    public static ErrMsgBehavior Instance;


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
    }

    [SerializeField] AudioClip popUpSound;
    [SerializeField] AudioClip errSound1;
    [SerializeField] AudioClip errSound2;
    [SerializeField] AudioClip allClearSound;
    [SerializeField] AudioClip clearSound;


    [SerializeField] float sfxVolume = 0.5f;

    [SerializeField] Transform canvas;
    [SerializeField] Transform errMsgPrefab;
    private List<Transform> _errMsgs = new();
    private readonly float _popUpInterval = 15f;
    private bool _inCoroutine = false;
    private readonly int _popUpChance = 2; // 0 (impossible) to 10 (new pop up every popUpInterval)


    void Update()
    {
        if (!_inCoroutine && GameBehavior.Instance.CurrentState == State.Play)
        {
            StartCoroutine(SchedulePopUps());
        }
    }

    private IEnumerator MakePopUps()
    {
        GameBehavior.Instance.CurrentState = State.PopUp;
        for (int i = 0; i < Random.Range(5, 15); i++)
        {
            yield return ScheduleOnePopUp(Random.Range(1, 4) * 0.1f, 0.5f * i, 0.5f * i);
        }
    }

    private IEnumerator SchedulePopUps()
    {
        _inCoroutine = true;
        yield return new WaitForSeconds(_popUpInterval);
        if (Random.Range(0, 10) < _popUpChance)
        {
            yield return MakePopUps();
        }
        _inCoroutine = false;
    }

    private IEnumerator ScheduleOnePopUp(float waitInterval, float offsetX, float offsetY)
    {
        MakeOnePopUp(errMsgPrefab.position + new Vector3(offsetX, offsetY, 0));
        yield return new WaitForSeconds(waitInterval);
    }

    private void MakeOnePopUp(Vector3 position)
    {
        AudioBehavior.Instance.PlayOneShotSound(popUpSound, sfxVolume);
        Transform errMsg = Instantiate(errMsgPrefab);
        errMsg.position = position;
        errMsg.SetParent(canvas);
        _errMsgs.Add(errMsg);
    }

    public void MakeOnePopUpNextToCurrent()
    {
        MakeOnePopUp(_errMsgs[^1].position + new Vector3(0.5f, 0.5f, 0));
    }

    public void CloseAllPopUps()
    {
        AudioBehavior.Instance.PlayOneShotSound(allClearSound, sfxVolume);
        int num = _errMsgs.Count;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                if (_errMsgs[i])
                {
                    Destroy(_errMsgs[i].gameObject);
                }
            }
            _errMsgs.Clear();
            GameBehavior.Instance.CurrentState = State.Play;
        }    
    }

    public void CloseCurrentPopUp()
    {
        AudioBehavior.Instance.PlayOneShotSound(clearSound, sfxVolume);
        int num = _errMsgs.Count;
        if (num > 0 && _errMsgs[num - 1])
        {
            Destroy(_errMsgs[num - 1].gameObject);
            _errMsgs.RemoveAt(num - 1);
            if (num == 1) // If this was the last pop up
            {
                GameBehavior.Instance.CurrentState = State.Play;
            }
        }
    }

    public void PlayErrSound()
    {
        if (Random.Range(0, 2) == 0)
        {
            AudioBehavior.Instance.PlayOneShotSound(errSound1, sfxVolume);
        }
        else
        {
            AudioBehavior.Instance.PlayOneShotSound(errSound2, sfxVolume);
        }
    }
}
