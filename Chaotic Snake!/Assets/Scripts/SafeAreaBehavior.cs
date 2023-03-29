using UnityEngine;

public class SafeAreaBehavior : MonoBehaviour
{
    public static SafeAreaBehavior Instance;

    [SerializeField] BoxCollider2D SafeArea;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public float LeftBound
    {
        get => SafeArea.bounds.min.x;
    }

    public float RightBound
    {
        get => SafeArea.bounds.max.x;
    }

    public float BottomBound
    {
        get => SafeArea.bounds.min.y;
    }

    public float TopBound
    {
        get => SafeArea.bounds.max.y;
    }

    public Vector3 RandomizePosition()
    {
        Bounds bounds = SafeArea.bounds;

        float x = Mathf.Round(Random.Range(bounds.min.x, bounds.max.x));
        float y = Mathf.Round(Random.Range(bounds.min.y, bounds.max.y));

        return new Vector3(x, y, 0f);
    }

    // A separate method is defined for portals b/c they should not get too close to the borders
    public Vector3 RandomizePositionForPortal()
    {
        Bounds bounds = SafeArea.bounds;

        float x = Mathf.Round(Random.Range(bounds.min.x + 1, bounds.max.x - 1));
        float y = Mathf.Round(Random.Range(bounds.min.y + 1, bounds.max.y - 1));

        return new Vector3(x, y, 0f);
    }
}
