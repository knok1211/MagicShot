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
    public Image deleteBlockImage;
    public Button completeButton;

    [Header("Cost UI")]
    public Text costText;
    private const int MAX_COST = 3;

    void Start()
    {
        gameObject.SetActive(true);
        SetupBlockImages();
        SetupCompleteButton();
        UpdateCostUI();
    }

    void SetupBlockImages()
    {
        SetupBlockButton(normalBlockImage, normalBlockPrefab, 1);
        SetupBlockButton(iceBlockImage, iceBlockPrefab, 2);
        SetupBlockButton(fireBlockImage, fireBlockPrefab, 3);
        SetupBlockButton(poisonBlockImage, poisonBlockPrefab, 4);
        SetupDeleteButton(deleteBlockImage);
    }

    void SetupCompleteButton()
    {
        if (completeButton != null)
        {
            completeButton.onClick.RemoveAllListeners();
            completeButton.onClick.AddListener(() => OnCompleteButtonClicked());
        }
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

    void SetupDeleteButton(Image deleteImage)
    {
        if (deleteImage != null)
        {
            Button button = deleteImage.GetComponent<Button>();
            if (button == null) button = deleteImage.gameObject.AddComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => DeleteBlock());
        }
    }

    void CreateBlock(int blockType)
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.AddBlock(blockType);
            string blockName = GetBlockName(blockType);
            Debug.Log($"{blockName} 블록 생성!");
            UpdateCostUI();
        }
        else
        {
            Debug.LogError("GameController를 찾을 수 없습니다!");
        }
    }

    void DeleteBlock()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.DeleteBlock();
            Debug.Log("블록 삭제!");
            UpdateCostUI();
        }
        else
        {
            Debug.LogError("GameController를 찾을 수 없습니다!");
        }
    }

    void OnCompleteButtonClicked()
    {
        Debug.Log("설치 완료 버튼 클릭!");
        if (GameController.Instance != null)
        {
            GameController.Instance.StartGame();
            Debug.Log("게임 시작 호출됨");
        }
        gameObject.SetActive(false);
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

    void UpdateCostUI()
    {
        if (costText != null && GameController.Instance != null)
        {
            int currentCost = GameController.Instance.GetCurrentCost();
            costText.text = $"{currentCost}/{MAX_COST}";
        }
    }

    void Update()
    {
        
    }
}
