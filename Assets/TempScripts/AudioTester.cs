using UnityEngine;

public class AudioTester : MonoBehaviour
{
    [SerializeField] string groupName;
    [SerializeField] string soundName;

    private void Start()
    {
        print(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            SoundManager.Instance.PlaySound(soundName, groupName, 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SoundManager.Instance.PlayOneShot(soundName, groupName);
        }
    }
}
