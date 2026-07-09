// [유니티 매뉴얼] 이 코드는 주석 포함 총 65줄입니다.
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public TextMeshProUGUI nameText;       // 선택된 영웅 이름 표시창
    public TextMeshProUGUI infoText;       // 선택된 영웅 정보 표시창
    public Button confirmButton;           // [출발하기] 최종 결정 버튼

    [Header("화면 전환용 오브젝트")]
    [Tooltip("다음에 활성화할 두번째 이벤트 화면 오브젝트")]
    public GameObject secondEventPanel;

    // 현재 유저가 마우스로 콕 집어둔 임시 선택 캐릭터 데이터
    private CharacterData selectedCharacter = null;

    private void Start()
    {
        // 시작 시 최종 확정 버튼을 잠가두고, 혹시 켜져있을지 모를 두 번째 이벤트창은 꺼둡니다.
        if (confirmButton != null) confirmButton.interactable = false;
        if (secondEventPanel != null) secondEventPanel.SetActive(false);

        nameText.text = "영웅을 선택해 주십시오.";
        infoText.text = "기존에 존재하지 않던 새로운 아군을 영입합니다.";
    }

    /// <summary>
    /// 캐릭터 카드를 딸깍 클릭했을 때 실시간으로 실행되는 함수
    /// </summary>
    public void OnSelectCharacter(CharacterData data)
    {
        if (data == null) return;
        selectedCharacter = data;

        // 1. 우측 자막창에 이름과 정보를 실시간으로 출력합니다.
        nameText.text = selectedCharacter.characterName;
        infoText.text = selectedCharacter.characterInfo;

        // 2. [기획 사양 완료] 클릭된 캐릭터를 실시간으로 중앙 파티창 장부에 '잠깐 담아둡니다'!
        if (PartyManager.Instance != null)
        {
            // 임시 세팅이므로, 클릭할 때마다 기존 파티를 한 번 비우고 새 영웅을 담아두는 예시입니다.
            PartyManager.Instance.currentPartyList.Clear();
            PartyManager.Instance.AddToParty(selectedCharacter.characterID);
        }

        // 선택이 완료되었으니 잠겨있던 출발하기 버튼 자물쇠를 탁 풀어줍니다!
        if (confirmButton != null) confirmButton.interactable = true;
    }

    /// <summary>
    /// [출발하기] 버튼을 누르는 순간 실행되는 함수
    /// </summary>
    public void OnClickConfirmSelection()
    {
        if (selectedCharacter == null) return;

        Debug.Log($"[CharSelect] '{selectedCharacter.characterName}'을 품고 출발합니다. 유저 데이터 영구 박제.");

        // 최종 확정된 캐릭터 ID를 기기 장부에 영구 저장합니다.
        PlayerPrefs.SetString("MySelectedCharacterID", selectedCharacter.characterID);
        PlayerPrefs.Save();

        // [기획 사양 완료] 현재 캐릭터 선택창은 깔끔하게 끄고, 준비해둔 두번째 이벤트 화면을 ON 합니다!
        Debug.Log("[CharSelect] 두번째 이벤트 화면으로 차원 이동!");
        if (secondEventPanel != null) secondEventPanel.SetActive(true);

        gameObject.SetActive(false); // 메인 캐릭터 선택창 OFF
    }
}
