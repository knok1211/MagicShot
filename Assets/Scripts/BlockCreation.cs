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

    [Header("UI Panel")]
    public GameObject panelObject; // Panel GameObject 참조
    private CanvasGroup panelCanvasGroup; // Panel의 CanvasGroup 컴포넌트

    void Start()
    {
        gameObject.SetActive(true);
        SetupBlockImages();
        SetupCompleteButton();
        UpdateCostUI();
        SetupPanel();
        ShowPanel(); // 스테이지 시작 시 UI 표시
    }

    void SetupPanel()
    {
        // Panel GameObject가 없으면 이 스크립트가 붙은 GameObject를 Panel로 사용
        if (panelObject == null)
        {
            panelObject = gameObject;
        }

        // CanvasGroup 컴포넌트가 없으면 추가
        panelCanvasGroup = panelObject.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = panelObject.AddComponent<CanvasGroup>();
        }
    }

    void ShowPanel()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 1f; // 완전히 보이게
            panelCanvasGroup.interactable = true; // 상호작용 가능
            panelCanvasGroup.blocksRaycasts = true; // 레이캐스트 차단
        }
    }

    // 외부에서 호출 가능한 public 메서드
    public void ShowBlockCreationUI()
    {
        gameObject.SetActive(true);
        ShowPanel();
        // 코스트 초기화
        if (GameController.Instance != null)
        {
            UpdateCostUI();
        }
    }

    void HidePanel()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f; // 투명하게
            panelCanvasGroup.interactable = false; // 상호작용 불가
            panelCanvasGroup.blocksRaycasts = false; // 레이캐스트 통과
        }
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
        HidePanel(); // Panel을 투명하게 만들기
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
