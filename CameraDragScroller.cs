using UnityEngine;
using UnityEngine.InputSystem;

public class CameraDragScroller : MonoBehaviour
{
    // [CameraDragScroller.cs 파일의 상단 변수와 Start 함수만 이 정답 코드로 교체하세요]

    [Header("[카메라 제어 세팅]")]
    public float dragSpeed = 0.5f;

    // [수리 완료]: SpriteRenderer 대신 UI 전용 가구 규칙인 'RectTransform'으로 성격을 변경합니다!
    public RectTransform townBackgroundUI;

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        // [수리 완료]: UI 이미지의 가로/세로 실제 픽셀 영역 크기를 계산해서 카메라 가두기 경계선을 설정합니다.
        if (townBackgroundUI != null)
        {
            float camHeight = Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            // UI 가로/세로 절반 크기를 구합니다.
            float halfWidth = (townBackgroundUI.rect.width * townBackgroundUI.lossyScale.x) / 2f;
            float halfHeight = (townBackgroundUI.rect.height * townBackgroundUI.lossyScale.y) / 2f;
            Vector3 centerPos = townBackgroundUI.position;

            // 검정색 UI 이미지 밖으로 카메라 눈이 나가지 못하게 경계선을 꽉 잡아둡니다.
            minBounds = new Vector2(centerPos.x - halfWidth + camWidth, centerPos.y - halfHeight + camHeight);
            maxBounds = new Vector2(centerPos.x + halfWidth - camWidth, centerPos.y + halfHeight - camHeight);
        }
    }

}
