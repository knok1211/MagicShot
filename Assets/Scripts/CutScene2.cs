using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageCutscene1 : MonoBehaviour
{
    public Sprite[] cutsceneImages; // 컷씬 이미지 배열
    public Image displayImage;      // 보여줄 이미지 UI
    public float delay = 2f;        // 자동 전환 간격 (초)
    public string nextSceneName = "Stage2"; // 컷씬 후 이동할 씬 이름

    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        if (cutsceneImages.Length > 0)
        {
            displayImage.sprite = cutsceneImages[0];
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delay)
        {
            timer = 0f;
            ShowNextImage();
        }
    }

    void ShowNextImage()
    {
        currentIndex++;

        if (currentIndex < cutsceneImages.Length)
        {
            displayImage.sprite = cutsceneImages[currentIndex];
        }
        else
        {
            // 컷씬 끝 → 다음 씬으로 이동
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // (선택) Skip 버튼 연결용
    public void SkipCutscene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
