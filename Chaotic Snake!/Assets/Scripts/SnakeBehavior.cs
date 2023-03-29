using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBehavior : MonoBehaviour
{
    public static SnakeBehavior Instance;
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

    public bool PowerUpInEffect = false;

    private bool _isInvincible = false;

    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite slowingSprite;
    [SerializeField] Sprite invincibleSprite;

    private Sprite _currentSprite;

    [SerializeField] float initTimestep = 0.1f; // 10 fps; this detemrines the speed of the snake; lower = faster

    //private float _currentTimestep = 0.1f;

    [SerializeField] float powerUpDuration = 10f;

    private Vector2 _direction = Vector2.zero;

    [SerializeField] Transform SegmentPrefab;
    private List<Transform> _segments;

    private int _score;

    private void Start()
    {
        GuiBehavior.Instance.UpdateHighScoreGui(PlayerPrefs.GetInt("highScore"));
        transform.position = SafeAreaBehavior.Instance.RandomizePosition();
        Time.fixedDeltaTime = initTimestep;
        _segments = new List<Transform>
        {
            transform
        };
        _currentSprite = normalSprite;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameBehavior.Instance.CurrentState == State.Play || GameBehavior.Instance.CurrentState == State.Start)
        {
            if (Input.GetKeyDown(GameBehavior.Instance.UpButton))
            {
                _direction = Vector2.up;
            }
            else if (Input.GetKeyDown(GameBehavior.Instance.DownButton))
            {
                _direction = Vector2.down;
            }
            else if (Input.GetKeyDown(GameBehavior.Instance.LeftButton))
            {
                _direction = Vector2.left;
            }
            else if (Input.GetKeyDown(GameBehavior.Instance.RightButton))
            {
                _direction = Vector2.right;
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameBehavior.Instance.CurrentState == State.Play)
        {
            for (int i = _segments.Count - 1; i > 0; i--)
            {
                _segments[i].position = _segments[i - 1].position;
                _segments[i].GetComponent<SpriteRenderer>().sprite = _currentSprite;
            }
            _segments[0].GetComponent<SpriteRenderer>().sprite = _currentSprite;
            transform.position = new Vector3(
                Mathf.Round(transform.position.x) + _direction.x,
                Mathf.Round(transform.position.y) + _direction.y,
                0
                );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Using if/else instead of switch because CompareTag is more efficient than string comparison
        if (collision.CompareTag("Food"))
        {
            Grow();
            SetScore(++_score);
        }
        else if (collision.CompareTag("Wall"))
        {
            // If it's invincible, come out from the other side of the wall
            if (_isInvincible)
            {
                MoveToOtherSide();
            }
            else
            {
                _direction = Vector2.zero;
                GameBehavior.Instance.CurrentState = State.GameOver;
            }
        }
        else if (collision.CompareTag("SnakeBody"))
        {
            if (!_isInvincible)
            {
                _direction = Vector2.zero;
                GameBehavior.Instance.CurrentState = State.GameOver;
            }
        }
        else if (collision.CompareTag("Portal1"))
        {
            Teleport(0);
        }
        else if (collision.CompareTag("Portal2"))
        {
            Teleport(1);
        }
        else if (collision.CompareTag("Slowing"))
        {
            StartCoroutine(SlowDown());
            PowerUpBehavior.Instance.DestroyCurrentPowerUp();
        }
        else if (collision.CompareTag("Invincible"))
        {
            StartCoroutine(MakeInvincible());
            PowerUpBehavior.Instance.DestroyCurrentPowerUp();
        }
    }

    private void Teleport(int fromPortalIdx)
    {
        if (_direction == Vector2.left || _direction == Vector2.right)
        {
            Vector3 fromPosition = PortalBehavior.Instance.GetPosition(fromPortalIdx);
            Vector3 toPosition = PortalBehavior.Instance.GetPosition(1 - fromPortalIdx);
            float offset = transform.position.y - fromPosition.y;
            toPosition.y += offset;
            toPosition.x += _direction.x; // Move it one grid more past the portal position towards the direction the snake is going
            transform.position = toPosition;
        }
        else
        {
            Vector3 fromPosition = PortalBehavior.Instance.GetPosition(fromPortalIdx);
            Vector3 toPosition = PortalBehavior.Instance.GetPosition(1 - fromPortalIdx);
            float offset = transform.position.x - fromPosition.x;
            toPosition.x += offset;
            toPosition.y += _direction.y;
            transform.position = toPosition;
        }
        // If neither conditions are met, do not teleport
    }


    private IEnumerator SlowDown()
    {
        PowerUpInEffect = true;
        Time.fixedDeltaTime = initTimestep * 2;
        yield return ChangeSprite(slowingSprite);
        Time.fixedDeltaTime = initTimestep;
        PowerUpInEffect = false;
    }

    private IEnumerator MakeInvincible()
    {
        PowerUpInEffect = true;
        _isInvincible = true;
        Time.fixedDeltaTime = initTimestep * 0.5f;
        yield return ChangeSprite(invincibleSprite);
        Time.fixedDeltaTime = initTimestep;
        _isInvincible = false;
        PowerUpInEffect = false;
    }

    public void ResetSnake()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(transform);
        transform.position = SafeAreaBehavior.Instance.RandomizePosition();
        _currentSprite = normalSprite;
        foreach (Transform seg in _segments)
        {
            seg.GetComponent<SpriteRenderer>().sprite = _currentSprite;
        }
        Time.fixedDeltaTime = initTimestep;
        _isInvincible = false;
        PowerUpInEffect = false;
        SetScore(0);
    }

    private void MoveToOtherSide()
    {
        if (_direction == Vector2.up)
        {
            transform.position = new Vector3(
                transform.position.x,
                SafeAreaBehavior.Instance.BottomBound,
                0);
        }
        else if (_direction == Vector2.down)
        {
            transform.position = new Vector3(
                transform.position.x,
                SafeAreaBehavior.Instance.TopBound,
                0);
        }
        else if (_direction == Vector2.left)
        {
            transform.position = new Vector3(
                SafeAreaBehavior.Instance.RightBound,
                transform.position.y,
                0);
        }
        else
        {
            transform.position = new Vector3(
                SafeAreaBehavior.Instance.LeftBound,
                transform.position.y,
                0);
        }
    }

    private void SetScore(int score)
    {
        _score = score;
        GuiBehavior.Instance.UpdateScoreGui(score);
        if (score > PlayerPrefs.GetInt("highScore"))
        {
            PlayerPrefs.SetInt("highScore", score);
            GuiBehavior.Instance.UpdateHighScoreGui(score);
        }
    }

    private void Grow()
    {
        Transform segment = Instantiate(SegmentPrefab);
        segment.position = _segments[^1].position;
        _segments.Add(segment);
    }

    //public void ResetTimestep()
    //{
    //    Time.fixedDeltaTime = _currentTimestep;
    //}

    private IEnumerator ChangeSprite(Sprite newSprite)
    {
        _currentSprite = newSprite;
        yield return new WaitForSeconds(powerUpDuration * 0.5f);

        int flicker = 0;
        while (flicker < 8)
        {
            _currentSprite = flicker % 2 == 0 ? normalSprite : newSprite;

            flicker++;

            yield return new WaitForSeconds(powerUpDuration * 0.25f / 8);
        }
        flicker = 0;
        while (flicker < 16)
        {
            _currentSprite = flicker % 2 == 0 ? normalSprite : newSprite;

            flicker++;

            yield return new WaitForSeconds(powerUpDuration * 0.25f / 16);
        }

        _currentSprite = normalSprite;
    }
}
