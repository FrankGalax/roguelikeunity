using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ActionQueue : MonoBehaviour
{
    public abstract void AddAction(GameAction gameAction);

    public abstract bool IsBusy();

    public void ChangeFloor()
    {
        GameManager.Instance.CurrentFloor = GameManager.Instance.CurrentFloor + 1;

        if (GameManager.Instance.CurrentFloor == Config.Instance.FloorDefinitions.Count)
        {
            GameManager.Instance.ResetGame();
            SceneManager.LoadScene("mainmenu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void CheatSetFloor(int floorIndex)
    {
        GameManager.Instance.CurrentFloor = floorIndex;

        if (GameManager.Instance.CurrentFloor == Config.Instance.FloorDefinitions.Count)
        {
            SceneManager.LoadScene("mainmenu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}