using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Block Prefabs")]
    public GameObject normalBlockPrefab;  // NormalBlock 프리팹
    public GameObject iceBlockPrefab;     // IceBlock 프리팹

    [Header("Block Map")]
    // 0: 블록 없음, 1: 일반 블록, 2: 얼음 블록
    public int[,] blockMap = new int[50, 50];

    public int selectedX = -1;
    public int selectedZ = -1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateBlocks();
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
            blockMap[selectedZ, selectedX] = blockType;
            RegenerateBlocks();
            Debug.Log($"좌표 ({selectedX}, {selectedZ})에 블록 {blockType} 생성/대체!");
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
                GameObject prefab = blockType == 1 ? normalBlockPrefab : iceBlockPrefab;

                if (prefab != null)
                {
                    GameObject block = Instantiate(prefab, position, Quaternion.identity);
                    block.transform.parent = transform;
                    block.name = $"{(blockType == 1 ? "Normal" : "Ice")}Block_{x}_{z}";
                }
            }
        }
    }

    void Update()
    {
        
    }
}
