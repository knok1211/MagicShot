using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Block Prefabs")]
    public GameObject normalBlockPrefab;  // NormalBlock 프리팹
    public GameObject iceBlockPrefab;     // IceBlock 프리팹
    public GameObject fireBlockPrefab;    // FireBlock 프리팹
    public GameObject poisonBlockPrefab;  // PoisonBlock 프리팹

    [Header("UI Prefab")]
    public GameObject canvasPrefab;  // Canvas UI 프리팹 (모든 스테이지에 공통 적용)
    // 참고: Resources 폴더에 Canvas 프리팹이 있으면 자동으로 로드됩니다.

    [Header("Block Map")]
    // 0: 블록 없음, 1: 일반 블록, 2: 얼음 블록, 3: 화염 블록, 4: 독 블록
    public int[,] blockMap = new int[50, 50];

    [Header("Danger Map")]
    public float[,] dangerMap = new float[50, 50];
    public float dangerDecayRate = 0.5f; // 초당 감소량

    public int selectedX = -1;
    public int selectedZ = -1;
    private int Stage = 0;
    private int currentCost = 0;
    private const int MAX_COST = 3;
    
    // 블록 타입별 코스트
    private static readonly int[] blockCosts = new int[] { 0, 1, 2, 3, 3 }; // 0: 없음, 1: 일반, 2: 얼음, 3: 화염, 4: 독

    void Awake()
    {
        Instance = this;
        SetupLayerCollisions();
    }

    void Start()
    {
        // UI가 없으면 자동으로 생성 (모든 스테이지에 공통 적용)
        EnsureUIExists();
        GenerateBlocks();
        ResetBlockCreationUI(); // 스테이지 시작 시 UI 다시 표시
    }

    void EnsureUIExists()
    {
        // BlockCreation 스크립트가 있는지 확인
        BlockCreation existingUI = FindObjectOfType<BlockCreation>();
        
        // UI가 없으면 생성
        if (existingUI == null)
        {
            GameObject prefabToInstantiate = null;
            
            // 1. 먼저 Inspector에서 할당된 프리팹 확인
            if (canvasPrefab != null)
            {
                prefabToInstantiate = canvasPrefab;
            }
            // 2. Resources 폴더에서 자동 로드 시도
            else
            {
                prefabToInstantiate = Resources.Load<GameObject>("Canvas");
            }
            
            // 3. 프리팹을 찾았으면 인스턴스화
            if (prefabToInstantiate != null)
            {
                GameObject uiInstance = Instantiate(prefabToInstantiate);
                uiInstance.name = "Canvas"; // 이름을 Canvas로 설정
                Debug.Log("UI가 자동으로 생성되었습니다.");
            }
            else
            {
                Debug.LogWarning("Canvas 프리팹을 찾을 수 없습니다. 다음 중 하나를 확인해주세요:\n" +
                    "1. GameController의 Canvas Prefab 필드에 Canvas 프리팹 할당\n" +
                    "2. Assets/Resources/ 폴더에 Canvas 프리팹 배치");
            }
        }
    }

    public void ResetBlockCreationUI()
    {
        // BlockCreation 스크립트를 찾아서 UI를 다시 표시
        BlockCreation blockCreation = FindObjectOfType<BlockCreation>();
        if (blockCreation != null)
        {
            blockCreation.ShowBlockCreationUI();
        }
        else
        {
            // UI가 없으면 다시 생성 시도
            EnsureUIExists();
            blockCreation = FindObjectOfType<BlockCreation>();
            if (blockCreation != null)
            {
                blockCreation.ShowBlockCreationUI();
            }
        }
    }

    void SetupLayerCollisions()
    {
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        int playerLayer = LayerMask.NameToLayer("Player");

        if (projectileLayer != -1 && playerLayer != -1)
        {
            Physics.IgnoreLayerCollision(projectileLayer, playerLayer, true);
        }

        if (projectileLayer != -1)
        {
            // Projectile끼리도 충돌 무시
            Physics.IgnoreLayerCollision(projectileLayer, projectileLayer, true);
        }
    }

    public int GetCurrentCost()
    {
        return currentCost;
    }

    public int GetBlockCost(int blockType)
    {
        if (blockType >= 0 && blockType < blockCosts.Length)
            return blockCosts[blockType];
        return 0;
    }

    public void SetSelectedPosition(int x, int z)
    {
        selectedX = x;
        selectedZ = z;
    }

    public void AddBlock(int blockType)
    {
        if (selectedX < 0 || selectedZ < 0)
        {
            Debug.Log("좌표를 먼저 선택해주세요!");
            return;
        }

        int rows = blockMap.GetLength(0);
        int cols = blockMap.GetLength(1);

        if (selectedZ >= 0 && selectedZ < rows && selectedX >= 0 && selectedX < cols)
        {
            int newBlockCost = GetBlockCost(blockType);
            int oldBlockCost = GetBlockCost(blockMap[selectedZ, selectedX]);
            int costDifference = newBlockCost - oldBlockCost;

            // 코스트 확인
            if (currentCost + costDifference > MAX_COST)
            {
                Debug.Log($"코스트 부족! 필요: {costDifference}, 남은 코스트: {MAX_COST - currentCost}");
                return;
            }

            blockMap[selectedZ, selectedX] = blockType;
            currentCost += costDifference;
            RegenerateBlocks();
            Debug.Log($"좌표 ({selectedX}, {selectedZ})에 블록 {blockType} 생성/대체! (코스트: {currentCost}/{MAX_COST})");
        }
        else
        {
            Debug.Log($"좌표 ({selectedX}, {selectedZ})가 맵 범위(0-{cols-1}, 0-{rows-1})를 벗어났습니다!");
        }
    }

    public void DeleteBlock()
    {
        if (selectedX < 0 || selectedZ < 0)
        {
            Debug.Log("좌표를 먼저 선택해주세요!");
            return;
        }

        int rows = blockMap.GetLength(0);
        int cols = blockMap.GetLength(1);

        if (selectedZ >= 0 && selectedZ < rows && selectedX >= 0 && selectedX < cols)
        {
            int blockType = blockMap[selectedZ, selectedX];
            if (blockType != 0)
            {
                int blockCost = GetBlockCost(blockType);
                currentCost -= blockCost;
                blockMap[selectedZ, selectedX] = 0;
                RegenerateBlocks();
                Debug.Log($"좌표 ({selectedX}, {selectedZ})의 블록 삭제! (코스트 반환: {blockCost}, 현재: {currentCost}/{MAX_COST})");
            }
            else
            {
                Debug.Log($"좌표 ({selectedX}, {selectedZ})에 블록이 없습니다!");
            }
        }
        else
        {
            Debug.Log($"좌표 ({selectedX}, {selectedZ})가 맵 범위(0-{cols-1}, 0-{rows-1})를 벗어났습니다!");
        }
    }

    public void RegenerateBlocks()
    {
        // 기존 블록 모두 삭제
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 블록 재생성
        GenerateBlocks();
    }

    void GenerateBlocks()
    {
        int rows = blockMap.GetLength(0);    // z축 크기
        int cols = blockMap.GetLength(1);    // x축 크기

        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < cols; x++)
            {
                int blockType = blockMap[z, x];
                
                if (blockType == 0) continue;  // 블록 없음

                Vector3 position = new Vector3(x, 1, z);
                GameObject prefab = GetBlockPrefab(blockType);

                if (prefab != null)
                {
                    GameObject block = Instantiate(prefab, position, Quaternion.identity);
                    block.transform.parent = transform;
                    block.name = $"{GetBlockName(blockType)}Block_{x}_{z}";
                    EnsureReflectiveComponents(block);
                }
            }
        }
    }

    GameObject GetBlockPrefab(int blockType)
    {
        switch (blockType)
        {
            case 1: return normalBlockPrefab;
            case 2: return iceBlockPrefab;
            case 3: return fireBlockPrefab;
            case 4: return poisonBlockPrefab;
            default: return null;
        }
    }

    string GetBlockName(int blockType)
    {
        switch (blockType)
        {
            case 1: return "Normal";
            case 2: return "Ice";
            case 3: return "Fire";
            case 4: return "Poison";
            default: return "Unknown";
        }
    }

    void EnsureReflectiveComponents(GameObject block)
    {
        if (block == null)
            return;

        Collider collider = block.GetComponent<Collider>();
        if (collider == null)
        {
            BoxCollider boxCollider = block.AddComponent<BoxCollider>();
            Renderer renderer = block.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                boxCollider.size = renderer.bounds.size;
            }
        }

        if (!block.TryGetComponent(out ProjectileReflector _))
        {
            block.AddComponent<ProjectileReflector>();
        }
    }

    void Update()
    {
        UpdateDangerMap();
    }

    void UpdateDangerMap()
    {
        int rows = dangerMap.GetLength(0);
        int cols = dangerMap.GetLength(1);

        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (dangerMap[z, x] > 0f)
                {
                    dangerMap[z, x] -= dangerDecayRate * Time.deltaTime;
                    if (dangerMap[z, x] < 0f)
                        dangerMap[z, x] = 0f;
                }
            }
        }
    }

    public void MarkDangerZone(int x, int z, float dangerValue)
    {
        int rows = dangerMap.GetLength(0);
        int cols = dangerMap.GetLength(1);

        if (x >= 0 && x < cols && z >= 0 && z < rows)
        {
            dangerMap[z, x] += dangerValue;
            if (dangerMap[z, x] > 3f)
                dangerMap[z, x] = 3f;
        }
    }

    public void StartGame()
    {
        Stage += 1;
        EnableAllEnemies();
        Debug.Log("게임 시작!");
    }

    void EnableAllEnemies()
    {
        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in allEnemies)
        {
            enemy.SetActive(true);
            
        }
    }
}
