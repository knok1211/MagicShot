using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameBGMManager : MonoBehaviour
{
    public AudioClip inGameBGM; // 인게임용 BGM 파일

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Stage1")
            return;

        // 기존 BGMPlayer가 있으면 제거
        GameObject oldBGM = GameObject.Find("StartBGM");
        if (oldBGM != null)
        {
            Destroy(oldBGM);
        }

        // 새로운 배경음악 플레이어 만들기
        GameObject newBGM = new GameObject("Stage1BGMPlayer");
        AudioSource audioSource = newBGM.AddComponent<AudioSource>();
        audioSource.clip = inGameBGM;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }
}
