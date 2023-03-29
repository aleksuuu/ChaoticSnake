using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;
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

    public KeyCode UpButton = KeyCode.W;
    public KeyCode DownButton = KeyCode.S;
    public KeyCode LeftButton = KeyCode.A;
    public KeyCode RightButton = KeyCode.D;

    private State _currentState;

    public State CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            switch (value)
            {
                case State.Start:
                    Reset();
                    GuiBehavior.Instance.UpdateMessageGUI(message: "Press W, A, S, or D to start!");
                    break;
                case State.Play:
                    Time.timeScale = 1.0f;
                    GuiBehavior.Instance.UpdateMessageGUI(visibility: false);
                    break;
                case State.Pause:
                    Time.timeScale = 0.0f; // On pause set timeScale to 0 so that powerups and portals do not disappear
                    GuiBehavior.Instance.UpdateMessageGUI(message: "Press ESC again to quit and any other keys to resume");
                    break;
                case State.GameOver:
                    StopAllCoroutinesInScene();
                    GuiBehavior.Instance.UpdateMessageGUI(message: "GAME OVER! (Press return to restart)");
                    break;
                // Do nothing if State.PopUp (meaning all the coroutines WOULD keep going, including the power-ups and portals)
            }
        }
    }

 
    private void Start()
    {
        CurrentState = State.Start;
    }

    private void Update()
    {

        if (CurrentState == State.GameOver && Input.GetKeyDown(KeyCode.Return))
        {
            CurrentState = State.Start;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == State.Play)
            {
                CurrentState = State.Pause;
            }
            else 
            {
                ExitGame.Instance.Quit();
            }
        }
        else if (CurrentState == State.Pause
            && Input.anyKeyDown &&
            !(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
        {
            CurrentState = State.Play;
        }
        else if ((CurrentState == State.Start) && (
            Input.GetKeyDown(UpButton) || Input.GetKeyDown(DownButton) ||
            Input.GetKeyDown(LeftButton) || Input.GetKeyDown(RightButton)
            ))
        {
            CurrentState = State.Play;
        }
    }

    private void StopAllCoroutinesInScene()
    {
        SnakeBehavior.Instance.StopAllCoroutines();
        PowerUpBehavior.Instance.StopAllCoroutines();
        PortalBehavior.Instance.StopAllCoroutines();
    }

    private void Reset()
    {
        SnakeBehavior.Instance.ResetSnake();
        PowerUpBehavior.Instance.ResetPowerUp();
        FoodBehavior.Instance.ResetFood();
        PortalBehavior.Instance.ResetPortals();
    }
}
