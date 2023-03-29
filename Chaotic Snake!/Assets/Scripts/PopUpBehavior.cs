using UnityEngine;
using UnityEngine.UI;

public class PopUpBehavior : MonoBehaviour
{
    [SerializeField] Button okButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Button closeButton;

    private void Start()
    {
        okButton.onClick.AddListener(OK);
        cancelButton.onClick.AddListener(Cancel);
        closeButton.onClick.AddListener(Close);
    }

    private void OK()
    {
        DecideButtonFunc(6, 2, 1);
    }

    private void Cancel()
    {
        DecideButtonFunc(2, 0, 4);
        ErrMsgBehavior.Instance.PlayErrSound();
    }

    private void Close()
    {
        DecideButtonFunc(2, 0, 7);
        ErrMsgBehavior.Instance.PlayErrSound();
    }

    private void DecideButtonFunc(int closeProb, int closeAllProb, int popUpProb)
    {
        // The weights should add up to less than 10; closeProb is the likelihood of the current pop up being closed; closeAllProb is the likelihood of all pop ups being closed; and popUpProb is the likelihood of more pop ups; otherwise do nothing
        
        int num = Random.Range(0, 10);
        if (num < closeProb)
        {
            ErrMsgBehavior.Instance.CloseCurrentPopUp();
        }
        else if (num < closeProb + closeAllProb)
        {
            ErrMsgBehavior.Instance.CloseAllPopUps();
            
        }
        else if (num < closeProb + closeAllProb + popUpProb)
        {
            ErrMsgBehavior.Instance.MakeOnePopUpNextToCurrent();
        }
        else
        {
            ErrMsgBehavior.Instance.PlayErrSound();
        }
    }
}
