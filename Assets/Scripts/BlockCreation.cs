using UnityEngine;
using UnityEngine.UI;

public class BlockCreation : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject normalBlockPrefab;
    public GameObject iceBlockPrefab;

    [Header("UI - 블록 이미지 (버튼으로 사용)")]
    public Image normalBlockImage;
    public Image iceBlockImage;

    void Start()
    {
        gameObject.SetActive(true);
        SetupBlockImages();
    }

    void SetupBlockImages()
    {
        if (normalBlockImage != null)
        {
            Sprite sprite = GetSpriteFromPrefab(normalBlockPrefab);
            if (sprite != null) normalBlockImage.sprite = sprite;
            
            Button button = normalBlockImage.GetComponent<Button>();
            if (button == null) button = normalBlockImage.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => CreateBlock(1));
        }
        
        if (iceBlockImage != null)
        {
            Sprite sprite = GetSpriteFromPrefab(iceBlockPrefab);
            if (sprite != null) iceBlockImage.sprite = sprite;
            
            Button button = iceBlockImage.GetComponent<Button>();
            if (button == null) button = iceBlockImage.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => CreateBlock(2));
        }
    }

    void CreateBlock(int blockType)
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.AddBlock(blockType);
            Debug.Log($"{(blockType == 1 ? "일반" : "얼음")} 블록 생성!");
        }
        else
        {
            Debug.LogError("GameController를 찾을 수 없습니다!");
        }
    }

    Sprite GetSpriteFromPrefab(GameObject prefab)
    {
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            return spriteRenderer.sprite;
        
        return null;
    }

    void Update()
    {
        
    }
}
