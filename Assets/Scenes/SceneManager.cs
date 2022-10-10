using System.Linq;
using TMPro;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    Timer sceneTimer;
    string newScene;

    [SerializeField] GameObject fadeScreenPrefab;
    GameObject screenFade;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Transition("MainMenu", 0);

        sceneTimer?.Update();

        if (screenFade)
            screenFade.GetComponentsInChildren<CanvasRenderer>().ToList().ForEach(x => x.SetAlpha(sceneTimer.GetElapsed));
    }

    public void Transition(string sceneName, float tranisionTime)
    {
        newScene = sceneName;
        sceneTimer = new Timer(tranisionTime, Transition);

        if(tranisionTime > 0)
        {
            screenFade = Instantiate(fadeScreenPrefab);
        }
    }

    public void Transition()
    {
        if (string.IsNullOrEmpty(newScene)) return;

        bool sceneValid = false;


        for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            print(UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(i).name);

            if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(i).name.Equals(newScene))
            {
                sceneValid = true;
                break;
            }
        }

        if (!sceneValid)
        {
            print("Invalid Scene: " + newScene);
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(newScene);
        newScene = "";
    }
}
