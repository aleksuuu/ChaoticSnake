using UnityEngine;

public class TitleBehavior : MonoBehaviour
{
    [SerializeField] AudioClip startUpSound;
    [SerializeField] AudioClip loopedSoundtrack;
    [SerializeField] float startUpLen = 8.0f;
    private Animator _anim;
    private SpriteRenderer _renderer;
    private Color _color;
    private Vector3 _initScale = new(0f, 0f, 1f);
    private Vector3 _targetScale = new(2f, 2f, 1f);

    private float _currTime = 0.0f;

    private bool _didStartUp = false;

    private void Start()
    {
        startUpLen -= AudioBehavior.Instance.FadeTime;
        AudioBehavior.Instance.PlayLoopSound(startUpSound, 0.5f);
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _anim.enabled = false;

        transform.localScale = _initScale;
    }

    private void Update()
    {
        if (!_didStartUp)
        {
            if (_currTime > startUpLen)
            {
                AudioBehavior.Instance.PlayLoopSound(loopedSoundtrack, 0.5f);
                _anim.enabled = true;
                _color.a = 1.0f;
                _renderer.color = _color;
                _didStartUp = true;
            }
            else
            {
                float t = _currTime / startUpLen;
                _color.a = Mathf.Lerp(0.0f, 1.0f, t);
                _renderer.color = _color;
                transform.localScale = Vector3.Lerp(_initScale, _targetScale, t);
                _currTime += Time.deltaTime;
            }
        }
    }
    
}
