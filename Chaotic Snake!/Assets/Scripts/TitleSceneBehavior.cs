using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneBehavior : MonoBehaviour
{
    [SerializeField] Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(GoToPlay);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ExitGame.Instance.Quit();
        }
    }



    private void GoToPlay()
    {
        SceneManager.LoadScene("PlayScene");
    }
}