using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private string nextScene;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipCutscene();
        }
    }

    public void SkipCutscene()
    {
        SceneManager.LoadScene(nextScene);
    }
}