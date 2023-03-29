using UnityEngine;
using TMPro;

public class GuiBehavior : MonoBehaviour
{
    public static GuiBehavior Instance;

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

    [SerializeField] private TextMeshProUGUI messageGui;
    [SerializeField] private TextMeshProUGUI scoreGui;
    [SerializeField] private TextMeshProUGUI highScoreGui;

    private void SetGUIVisibility(TextMeshProUGUI gui, bool visibility)
    {
        if (visibility && !gui.enabled)
        {
            gui.enabled = true;
        }
        else if (!visibility && gui.enabled)
        {
            gui.enabled = false;
        }
    }

    public void UpdateMessageGUI(bool visibility = true, string message = "")
    {
        messageGui.text = message;
        SetGUIVisibility(messageGui, visibility);
    }

    public void UpdateScoreGui(int score)
    {
        scoreGui.text = $"Score: {score}";
    }

    public void UpdateHighScoreGui(int score)
    {
        highScoreGui.text = $"Best: {score}";
    }
}