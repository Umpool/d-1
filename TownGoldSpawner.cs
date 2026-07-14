using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TownGoldSpawner : MonoBehaviour
{
    [Header("프리팹 설정")]
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private RectTransform townRectTransform;

    [Header("스폰 규칙 설정")]
    [SerializeField] private float initialDelay = 10f;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxGoldCount = 5;

    private List<GameObject> activeGolds = new List<GameObject>();

    // 코인 프리팹의 가로/세로 크기를 미리 저장할 변수
    private float coinHalfWidth = 0f;
    private float coinHalfHeight = 0f;

    void Start()
    {
        if (townRectTransform == null)
        {
            townRectTransform = GetComponent<RectTransform>();
        }

        // 게임 시작 시 코인 프리팹의 크기를 가져와서 절반 값을 미리 계산합니다.
        if (goldPrefab != null)
        {
            RectTransform prefabRect = goldPrefab.GetComponent<RectTransform>();
            if (prefabRect != null)
            {
                coinHalfWidth = prefabRect.rect.width / 2f;
                coinHalfHeight = prefabRect.rect.height / 2f;
            }
        }

        StartCoroutine(SpawnGoldRoutine());
    }

    private IEnumerator SpawnGoldRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            if (activeGolds.Count < maxGoldCount)
            {
                SpawnGold();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnGold()
    {
        if (goldPrefab == null || townRectTransform == null) return;

        // 1. 마을 해상도(4000x2500)의 중심(0,0) 기준 최대 가동 영역 계산
        float maxHalfWidth = townRectTransform.rect.width / 2f;
        float maxHalfHeight = townRectTransform.rect.height / 2f;

        // 2. [핵심] 마을 최대 크기에서 코인의 반지름(크기의 절반)만큼 안쪽으로 영역을 좁힙니다.
        // 추가로 유저가 커스텀 여백을 더 주고 싶다면 + 20f 처럼 여백을 더 더해줄 수도 있습니다.
        float edgeSafetyMargin = 30f; // 마을 테두리 벽에서 조금 더 떨어지게 만드는 추가 여백

        float safeMinX = -maxHalfWidth + coinHalfWidth + edgeSafetyMargin;
        float safeMaxX = maxHalfWidth - coinHalfWidth - edgeSafetyMargin;

        float safeMinY = -maxHalfHeight + coinHalfHeight + edgeSafetyMargin;
        float safeMaxY = maxHalfHeight - coinHalfHeight - edgeSafetyMargin;

        // 3. 완벽하게 안전한 내부 영역 안에서만 랜덤 좌표 추출
        float randomX = Random.Range(safeMinX, safeMaxX);
        float randomY = Random.Range(safeMinY, safeMaxY);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        // 자식 오브젝트로 코인 생성
        GameObject newGold = Instantiate(goldPrefab, townRectTransform);

        RectTransform goldRect = newGold.GetComponent<RectTransform>();
        if (goldRect != null)
        {
            goldRect.anchoredPosition = spawnPosition;
        }

        TownGoldItem goldItem = newGold.GetComponent<TownGoldItem>();
        if (goldItem != null)
        {
            goldItem.Initialize(this);
        }

        activeGolds.Add(newGold);
    }

    public void OnGoldCollected(GameObject goldObject)
    {
        if (activeGolds.Contains(goldObject))
        {
            activeGolds.Remove(goldObject);
        }
    }
}
