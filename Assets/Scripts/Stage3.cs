using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage3Clear : MonoBehaviour
{
    public string endingSceneName = "EndingScene";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(endingSceneName);
        }
    }
}
