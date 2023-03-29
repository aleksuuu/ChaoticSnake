using System.Collections;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
    public static PowerUpBehavior Instance;
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

    [SerializeField] Transform[] powerUpPrefabs;

    [SerializeField] float disappearTime = 10f;

    [SerializeField] float spawnDelay = 20f;

    private Transform _currentPowerUp;

    private bool _inCoroutine = false;

    private void Update()
    {
        if (!_inCoroutine && GameBehavior.Instance.CurrentState == State.Play)
        {
            StartCoroutine(SpawnPowerUp());
            _inCoroutine = true;
        }
    }

    public void ResetPowerUp()
    {
        if (_currentPowerUp)
        {
            DestroyCurrentPowerUp();
        }
        if (_inCoroutine)
        {
            StopCoroutine(SpawnPowerUp());
            _inCoroutine = false;
        }
    }

    private IEnumerator SpawnPowerUp()
    {
        if (!_currentPowerUp && !SnakeBehavior.Instance.PowerUpInEffect) // only spawn a power-up if there is currently no power-up in the scene
        {
            yield return new WaitForSeconds(Random.Range(0, 10));
            int randomIndex = Random.Range(0, powerUpPrefabs.Length); // choose a random power-up from the array
            _currentPowerUp = Instantiate(powerUpPrefabs[randomIndex]);
            _currentPowerUp.position = SafeAreaBehavior.Instance.RandomizePosition();
        }
        yield return new WaitForSeconds(disappearTime);
        DestroyCurrentPowerUp();
        yield return new WaitForSeconds(spawnDelay); // wait for spawnDelay seconds before checking again
        _inCoroutine = false;
    }

    public void DestroyCurrentPowerUp()
    {
        if (_currentPowerUp)
        {
            Destroy(_currentPowerUp.gameObject);
        }
    }
}
