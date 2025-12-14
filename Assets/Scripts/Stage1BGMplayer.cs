using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameBGMManager : MonoBehaviour
{
    public AudioClip inGameBGM; // �ΰ��ӿ� BGM ����

    void Start()
    {
        //if (SceneManager.GetActiveScene().name != "Stage1")
           // return;

        // ���� BGMPlayer�� ������ ����
        GameObject oldBGM = GameObject.Find("StartBGM");
        if (oldBGM != null)
        {
            Destroy(oldBGM);
        }

        // ���ο� ������� �÷��̾� �����
        GameObject newBGM = new GameObject("Stage1BGMPlayer");
        AudioSource audioSource = newBGM.AddComponent<AudioSource>();
        audioSource.clip = inGameBGM;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }
}
