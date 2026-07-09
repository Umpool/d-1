using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterComponent : MonoBehaviour
{
    // [유니티 매뉴얼] 이 컴포넌트 스크립트는 주석 포함 총 40줄입니다.

    [Header("이 프리팹 고유 데이터 입력")]
    public CharacterData myData;

    [Header("내부 UI 컴포넌트 연결 (자동 출력용)")]
    [Tooltip("카드 버튼 자체에 들어있는 이름 텍스트를 넣어주세요.")]
    public TextMeshProUGUI cardNameText;

    [Tooltip("카드 버튼 자체의 배경 또는 일러스트 이미지를 넣어주세요.")]
    public Image cardBackgroundImage;

    private void Start()
    {
        // [기획 사양 추가] 게임이 시작되면 내가 가진 데이터를 기반으로 버튼 UI를 자동 갱신합니다.
        SetupCardUI();
    }

    /// <summary>
    /// 내 데이터 상자(My Data)에 입력된 값을 바탕으로 버튼의 이름과 이미지를 변경합니다.
    /// </summary>
    public void SetupCardUI()
    {
        if (myData == null) return;

        // 1. 카드 버튼 안의 글자를 데이터에 적힌 영웅 이름으로 자동 변경!
        if (cardNameText != null)
        {
            cardNameText.text = myData.characterName;
        }

        // 2. 카드 버튼 자체의 이미지를 데이터에 연결된 스프라이트로 자동 변경!
        if (cardBackgroundImage != null && myData.characterSprite != null)
        {
            cardBackgroundImage.sprite = myData.characterSprite;

            // [임시 컬러 믹싱] 아까 기획했던 고유 색상 데이터도 버튼 자체에 실시간 염색해 줍니다.
            cardBackgroundImage.color = myData.characterColor;
        }
    }

    /// <summary>
    /// 카드(버튼)를 클릭했을 때 총괄 매니저에게 데이터를 배달하는 기존 함수 (보존)
    /// </summary>
    public void OnClickThisCharacterCard()
    {
        CharacterSelectManager manager = FindAnyObjectByType<CharacterSelectManager>();
        if (manager != null)
        {
            manager.OnSelectCharacter(myData);
        }
    }
}
