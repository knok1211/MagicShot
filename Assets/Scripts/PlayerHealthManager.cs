using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 플레이어 체력 관리 및 사망 시 씬 재로드
/// </summary>
public class PlayerHealthManager : MonoBehaviour
{
    public Health playerHealth;
    public AudioSource audioSource;
    public AudioClip deathSound;

    void Start()
    {

        
        if (playerHealth != null)
        {
            playerHealth.onDie.AddListener(OnPlayerDeath);
            Debug.Log("플레이어 체력 관리자 초기화 완료");
        }
        else
        {
            Debug.LogError("PlayerHealthManager: playerHealth가 설정되지 않았습니다!");
        }
    }

    void OnPlayerDeath()
    {
        Debug.Log("플레이어 사망! 효과음 재생 후 3초 대기...");
        
        // 효과음 재생



        GameObject oldBGM = GameObject.Find("Stage1BGMPlayer");
        if (oldBGM != null)
        {
            Destroy(oldBGM);
        }
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);

        }

        transform.position = new Vector3(2, 1, -2);

        
        // 3초 후 씬 재로드
        StartCoroutine(DelayedReload());
    }

    IEnumerator DelayedReload()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.onDie.RemoveListener(OnPlayerDeath);
        }
    }
}