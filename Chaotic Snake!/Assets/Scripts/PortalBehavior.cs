using System.Collections;
using UnityEngine;

public class PortalBehavior : MonoBehaviour
{
    public static PortalBehavior Instance;
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

    [SerializeField] Transform portalPrefab;
    private readonly Transform[] _portals = new Transform[2];

    private bool _portalsExist = false;

    [SerializeField] float disappearTime = 15f;

    [SerializeField] float spawnDelay = 15f;

    private bool _inCoroutine = false;


    private void Update()
    {
        if (!_inCoroutine && GameBehavior.Instance.CurrentState == State.Play)
        {
            StartCoroutine(SpawnPortals());
            _inCoroutine = true;
        }
    }

    public void ResetPortals()
    {
        if (_portalsExist)
        {
            DestroyCurrentPortals();
        }
        if (_inCoroutine)
        {
            StopCoroutine(SpawnPortals());
            _inCoroutine = false;
        }
    }

    private IEnumerator SpawnPortals()
    {
        if (!_portalsExist) // Only spawn a set of new portals if portals do not exist
        {
            _portalsExist = true;
            yield return new WaitForSeconds(Random.Range(0, 10));
            _portals[0] = Instantiate(portalPrefab);
            _portals[0].position = SafeAreaBehavior.Instance.RandomizePositionForPortal();
            _portals[0].tag = "Portal1";
            _portals[1] = Instantiate(portalPrefab);
            Vector3 tmp;
            tmp = SafeAreaBehavior.Instance.RandomizePosition();
            // Making sure the two portals are not placed at the same location
            while (tmp == _portals[0].position)
            {
                tmp = SafeAreaBehavior.Instance.RandomizePositionForPortal();
            }
            _portals[1].position = tmp;
            _portals[1].tag = "Portal2";
            yield return new WaitForSeconds(disappearTime);
            DestroyCurrentPortals();
            yield return new WaitForSeconds(spawnDelay); // Wait for spawnDelay seconds before checking again
            _inCoroutine = false;
        }
    }

    private void DestroyCurrentPortals()
    {
        if (_portalsExist)
        {
            _portalsExist = false;
            foreach (Transform portal in _portals)
            {
                if (portal)
                {
                    Destroy(portal.gameObject);
                }
            }
        }
    }

    public Vector3 GetPosition(int portalIdx)
    {
        // Just in case this function is called when there're no instantiated portals in the scene
        return _portalsExist ? _portals[portalIdx].position : new Vector3(0,0,0);
    }
}
