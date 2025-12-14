using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    
    [Header("Target")]
    //public Transform player;
    GameObject playerObj;
    
    Rigidbody _rigidbody;
    Vector3 _targetPosition;
    float _pathUpdateInterval = 0.1f;
    float _lastPathUpdateTime;
    float _originalMoveSpeed;



    private void Start()
    {

        enabled = false;

    }



    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }


            playerObj = GameObject.FindGameObjectWithTag("Player");

    }

    void Update()
    {


        // y축 위치를 1로 고정
        Vector3 pos = transform.position;
        pos.y = 1f;
        transform.position = pos;

        // 경로 업데이트
        if (Time.time - _lastPathUpdateTime >= _pathUpdateInterval)
        {
            _lastPathUpdateTime = Time.time;
            UpdatePath();
        }


        MarkCurrentPositionAsDanger();
    }


        void MarkCurrentPositionAsDanger()
    {
        if (GameController.Instance == null)
            return;

        int x = Mathf.RoundToInt(transform.position.x);
        int z = Mathf.RoundToInt(transform.position.z);

        GameController.Instance.MarkDangerZone(x, z, 2f);
    }

    void FixedUpdate()
    {


        MoveTowardsTarget();
    }

    void UpdatePath()
    {
        if (GameController.Instance == null)
            return;

        Vector3 startPos = transform.position;
        Vector3 endPos = playerObj.transform.position;

        int startX = Mathf.RoundToInt(startPos.x);
        int startZ = Mathf.RoundToInt(startPos.z);
        int endX = Mathf.RoundToInt(endPos.x);
        int endZ = Mathf.RoundToInt(endPos.z);

        // 현재 위치가 벽인지 확인
        int[,] blockMap = GameController.Instance.blockMap;
        int rows = blockMap.GetLength(0);
        int cols = blockMap.GetLength(1);

        // 현재 위치에 벽이 생겼으면 즉시 경로 재계산
        if (startX >= 0 && startX < cols && startZ >= 0 && startZ < rows)
        {
            if (blockMap[startZ, startX] != 0)
            {
                // 가장 가까운 빈 공간 찾기
                FindNearestEmptySpace(ref startX, ref startZ);
            }
        }

        List<Vector2Int> path = FindPathDijkstra(startX, startZ, endX, endZ);
        
        if (path != null && path.Count > 1)
        {
            Vector2Int nextStep = path[1];
            _targetPosition = new Vector3(nextStep.x, 1f, nextStep.y);
        }
        else if (path != null && path.Count == 1)
        {
            _targetPosition = new Vector3(endX, 1f, endZ);
        }
        else
        {
            // 경로를 찾을 수 없으면 현재 위치 유지
            _targetPosition = transform.position;
        }
    }

    void FindNearestEmptySpace(ref int x, ref int z)
    {
        if (GameController.Instance == null)
            return;

        int[,] blockMap = GameController.Instance.blockMap;
        int rows = blockMap.GetLength(0);
        int cols = blockMap.GetLength(1);

        // 주변 8방향 탐색
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            int newX = x + dir.x;
            int newZ = z + dir.y;

            if (newX >= 0 && newX < cols && newZ >= 0 && newZ < rows)
            {
                if (blockMap[newZ, newX] == 0)
                {
                    x = newX;
                    z = newZ;
                    return;
                }
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (GameController.Instance == null)
            return;

        // 목표 위치가 벽인지 확인
        int targetX = Mathf.RoundToInt(_targetPosition.x);
        int targetZ = Mathf.RoundToInt(_targetPosition.z);
        int[,] blockMap = GameController.Instance.blockMap;
        int rows = blockMap.GetLength(0);
        int cols = blockMap.GetLength(1);

        if (targetX >= 0 && targetX < cols && targetZ >= 0 && targetZ < rows)
        {
            if (blockMap[targetZ, targetX] != 0)
            {
                // 목표 위치가 벽이면 경로 재계산
                _lastPathUpdateTime = 0f;
                _rigidbody.linearVelocity = Vector3.zero;
                return;
            }
        }

        Vector3 direction = (_targetPosition - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.1f)
        {
            direction.Normalize();
            Vector3 targetVelocity = direction * moveSpeed;
            targetVelocity.y = 0f;
            _rigidbody.linearVelocity = targetVelocity;
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }

    public void ReduceSpeed(float speedMultiplier, float duration = 10f)
    {
        Debug.Log($"적 속도 감소: {speedMultiplier}배, 지속시간: {duration}초");
        _originalMoveSpeed = moveSpeed;
        moveSpeed *= speedMultiplier;
        Invoke(nameof(RestoreSpeed), duration);
    }
    
    void RestoreSpeed()
    {
        moveSpeed = _originalMoveSpeed;
    }

    List<Vector2Int> FindPathDijkstra(int startX, int startZ, int endX, int endZ)
    {
        if (GameController.Instance == null)
            return null;

        int[,] blockMap = GameController.Instance.blockMap;
        int rows = blockMap.GetLength(0);
        int cols = blockMap.GetLength(1);

        // 범위 체크
        if (startX < 0 || startX >= cols || startZ < 0 || startZ >= rows ||
            endX < 0 || endX >= cols || endZ < 0 || endZ >= rows)
            return null;

        // 다익스트라 초기화
        float[,] distances = new float[rows, cols];
        Vector2Int[,] previous = new Vector2Int[rows, cols];
        bool[,] visited = new bool[rows, cols];

        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < cols; x++)
            {
                distances[z, x] = float.MaxValue;
                previous[z, x] = new Vector2Int(-1, -1);
            }
        }

        distances[startZ, startX] = 0f;

        // 우선순위 큐 (간단한 리스트로 구현)
        List<Vector2Int> queue = new List<Vector2Int>();
        queue.Add(new Vector2Int(startX, startZ));

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // 위
            new Vector2Int(0, -1),  // 아래
            new Vector2Int(1, 0),   // 오른쪽
            new Vector2Int(-1, 0),  // 왼쪽
            new Vector2Int(1, 1),   // 오른쪽 위 대각선
            new Vector2Int(1, -1),  // 오른쪽 아래 대각선
            new Vector2Int(-1, 1),  // 왼쪽 위 대각선
            new Vector2Int(-1, -1)  // 왼쪽 아래 대각선
        };

        float[] directionCosts = new float[]
        {
            1f,                     // 위
            1f,                     // 아래
            1f,                     // 오른쪽
            1f,                     // 왼쪽
            1.414f,                 // 대각선 (루트 2)
            1.414f,                 // 대각선
            1.414f,                 // 대각선
            1.414f                  // 대각선
        };

        while (queue.Count > 0)
        {
            // 가장 거리가 짧은 노드 찾기
            Vector2Int current = queue[0];
            float minDist = distances[current.y, current.x];
            int minIndex = 0;

            for (int i = 1; i < queue.Count; i++)
            {
                Vector2Int node = queue[i];
                if (distances[node.y, node.x] < minDist)
                {
                    minDist = distances[node.y, node.x];
                    current = node;
                    minIndex = i;
                }
            }

            queue.RemoveAt(minIndex);

            if (visited[current.y, current.x])
                continue;

            visited[current.y, current.x] = true;

            // 목표 도달
            if (current.x == endX && current.y == endZ)
                break;

            // 인접 노드 탐색
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int dir = directions[i];
                int newX = current.x + dir.x;
                int newZ = current.y + dir.y;

                if (newX < 0 || newX >= cols || newZ < 0 || newZ >= rows)
                    continue;

                if (visited[newZ, newX])
                    continue;

                // 대각선 이동인 경우 (인덱스 4~7)
                if (i >= 4)
                {
                    // 대각선 이동 시 양 옆 타일에 벽이 있으면 이동 불가
                    int checkX1 = current.x + dir.x;
                    int checkZ1 = current.y;
                    int checkX2 = current.x;
                    int checkZ2 = current.y + dir.y;

                    if (blockMap[checkZ1, checkX1] != 0 || blockMap[checkZ2, checkX2] != 0)
                        continue;
                }

                // 벽이 있으면 비용 20 추가
                float moveCost = directionCosts[i];
                if (blockMap[newZ, newX] != 0)
                    moveCost += 20f;

                // 발사체가 지나간 위험 지역 비용 추가
                if (GameController.Instance.dangerMap[newZ, newX] > 0f)
                    moveCost += GameController.Instance.dangerMap[newZ, newX];

                float newDist = distances[current.y, current.x] + moveCost;

                if (newDist < distances[newZ, newX])
                {
                    distances[newZ, newX] = newDist;
                    previous[newZ, newX] = current;
                    
                    Vector2Int newNode = new Vector2Int(newX, newZ);
                    if (!queue.Contains(newNode))
                        queue.Add(newNode);
                }
            }
        }

        // 경로 재구성
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int step = new Vector2Int(endX, endZ);

        while (step.x != -1 && step.y != -1)
        {
            path.Add(step);
            if (step.x == startX && step.y == startZ)
                break;
            step = previous[step.y, step.x];
        }

        path.Reverse();
        return path.Count > 0 ? path : null;
    }


    public void SetActive(bool active)
{
    enabled = active;
}
}

