using UnityEngine;
using UnityEngine.SceneManagement;

public class SideSelector : MonoBehaviour
{
    public static string SelectedSide { get; private set; } = "Crips";

    public void SelectBloods()
    {
        SelectedSide = "Bloods";
        LoadNextScene();
    }

    public void SelectCrips()
    {
        SelectedSide = "Crips";
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
