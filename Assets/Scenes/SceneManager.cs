using System.Linq;
using UnityEngine;

public class SceneManager : Singleton<SceneManager>
{
    Timer sceneTimer;
    string newScene;

    [SerializeField] GameObject fadeScreenPrefab;
    GameObject screenFade;

    public float defaultDelay = 0;

    private void Update()
    {
        sceneTimer?.Update();

        if (screenFade)
            screenFade.GetComponentsInChildren<CanvasRenderer>().ToList().ForEach(x => x.SetAlpha(sceneTimer.GetElapsed));
    }

    public void SetDefaultDelay(float newDelay)
    {
        defaultDelay = newDelay;
    }

    public void Transition(string sceneName)
    {
        Transition(sceneName, defaultDelay);
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

        bool validScene = false;
        for(int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            int slash = path.LastIndexOf('/');
            string name = path.Substring(slash + 1);
            int dot = name.LastIndexOf('.');

            if(name.Substring(0, dot).Equals(newScene))
            {
                validScene = true;
                break;
            }
        }

        if (validScene)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(newScene);
            newScene = "";
        }
        else
        {
            print("Invalid Scene: " + newScene);
            return;
        }
    }
}
