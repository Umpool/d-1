using UnityEngine;
using UnityEngine.EventSystems; // UI 클릭 시스템을 사용합니다.

// 이 스크립트는 유저가 마우스로 클릭하거나 손가락으로 터치했을 때 스스로 반응합니다.
public class CoinClickObject : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // 유저가 나를 터치했다면!
        Debug.Log("짤랑! 마을에서 재화를 발견해 획득했습니다.");

        // 내 자신을 화면에서 파괴하여 삭제합니다.
        Destroy(gameObject);
    }
}
