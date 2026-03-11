using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void OptionsButton()
    {
        GameManager.instance.ToggleOptionsMenu();
    }

    public void BackButton()
    {
        GameManager.instance.BackButton();
    }

    public void Exit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
