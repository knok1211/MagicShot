using UnityEngine;
using UnityEngine.UI;

public class BlockCreation : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject normalBlockPrefab;
    public GameObject iceBlockPrefab;
    public GameObject fireBlockPrefab;
    public GameObject poisonBlockPrefab;

    [Header("UI - 블록 이미지 (버튼으로 사용)")]
    public Image normalBlockImage;
    public Image iceBlockImage;
    public Image fireBlockImage;
    public Image poisonBlockImage;

    void Start()
    {
        gameObject.SetActive(true);
        SetupBlockImages();
    }

    void SetupBlockImages()
    {
        SetupBlockButton(normalBlockImage, normalBlockPrefab, 1);
        SetupBlockButton(iceBlockImage, iceBlockPrefab, 2);
        SetupBlockButton(fireBlockImage, fireBlockPrefab, 3);
        SetupBlockButton(poisonBlockImage, poisonBlockPrefab, 4);
    }

    void SetupBlockButton(Image blockImage, GameObject prefab, int blockType)
    {
        if (blockImage != null)
        {
            Sprite sprite = GetSpriteFromPrefab(prefab);
            if (sprite != null) blockImage.sprite = sprite;
            
            Button button = blockImage.GetComponent<Button>();
            if (button == null) button = blockImage.gameObject.AddComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => CreateBlock(blockType));
        }
    }

    void CreateBlock(int blockType)
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.AddBlock(blockType);
            string blockName = GetBlockName(blockType);
            Debug.Log($"{blockName} 블록 생성!");
        }
        else
        {
            Debug.LogError("GameController를 찾을 수 없습니다!");
        }
    }

    string GetBlockName(int blockType)
    {
        switch (blockType)
        {
            case 1: return "일반";
            case 2: return "얼음";
            case 3: return "화염";
            case 4: return "독";
            default: return "알 수 없는";
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
