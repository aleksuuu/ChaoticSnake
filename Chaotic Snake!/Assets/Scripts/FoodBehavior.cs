using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    //private void Start()
    //{
    //    ResetFood();
    //}

    public static FoodBehavior Instance;
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

    private void Update()
    {
        if (GameBehavior.Instance.CurrentState == State.Play)
        {
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.position = SafeAreaBehavior.Instance.RandomizePosition();
        }
    }

    public void ResetFood()
    {
        GetComponent<Renderer>().enabled = false;
        transform.position = SafeAreaBehavior.Instance.RandomizePosition();
    }
}
