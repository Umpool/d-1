using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStorageManager : MonoBehaviour
{
    [Header("독립 파티창 슬롯 UI (0번이 맨 왼쪽 리더)")]
    [SerializeField] private List<Image> uniquePartySlots = new List<Image>();
    [SerializeField] private Sprite storageEmptySlotSprite;

    [Header("독립 격자 배치 UI (Grid Layout Group 부모)")]
    [SerializeField] private Transform storageGridParentGroup;
    [SerializeField] private GameObject storageCharacterSlotPrefab; // 만능 붕어빵 틀 프리팹

    [Header("독립 시너지 및 하단 정보 UI")]
    [SerializeField] private TextMeshProUGUI storageSynergyText;
    [SerializeField] private TextMeshProUGUI storageCountText;
    [SerializeField] private int storageMaxCount = 10;

    private List<string> storageTempPartyList = new List<string>();

    private void OnEnable()
    {
        // 1. 싱글톤 보유 가방으로부터 내가 가진 전체 캐릭터 ID를 내 창고 주머니로 이식합니다.
        if (UserCharacterInventory.Instance != null && UserCharacterInventory.Instance.ownedCharacterIDs != null)
        {
            storageTempPartyList.Clear();
            foreach (string id in UserCharacterInventory.Instance.ownedCharacterIDs)
            {
                storageTempPartyList.Add(id);
            }
        }
        
        // 2. 외부 파티 주머니에 저장된 진짜 리더 캐릭터 ID 데이터를 내 임시 주머니로 복사합니다.
        if (PartyManager.Instance != null && PartyManager.Instance.currentPartyList != null)
        {
            storageTempPartyList.Clear();
            foreach (string id in PartyManager.Instance.currentPartyList)
            {
                storageTempPartyList.Add(id);
            }
        }

        // 3. 데이터 동기화 바통터치가 끝났으므로 하단 창고 격자판 화면을 먼저 그립니다.
        RefreshUniqueStorageUI();

        // 💡 [핵심 엔진 추가]: 초행이든 복귀든 씬이 이동하자마자 상단 파티창 화면도 즉시 실시간으로 새로 그리게 만듭니다!
        RefreshUniquePartyUI();
    }




    /// <summary>
    /// [최종 혁신 엔진] 프로젝트 '캐릭터' 폴더에 들어있는 진짜 실물 데이터 시트(.asset)를 직접 찾아와 프리팹에 통째로 수동 도킹시킵니다!
    /// </summary>
    public void RefreshUniqueStorageUI()
    {
        // 1. 기존 창고 격자판 자식 오브젝트 청소
        foreach (Transform child in storageGridParentGroup)
        {
            Destroy(child.gameObject);
        }

        // 2. 선택창에서 유저가 콕 집어 고른 메인 캐릭터 ID 로드
        string currentMainHeroID = PlayerPrefs.GetString("SelectedCharacterID", "01");

        // 💡 파티창(storageTempPartyList)에 들어있지 않은 '보유 캐릭터'들만 창고 리스트에 담아 아래로 내립니다.
        List<string> warehouseIDs = new List<string>();
        
        // [규칙 확인] 만약 싱글톤 가방에 데이터가 있다면 복사, 없다면 테스트로 메인 영웅 적용
        if (UserCharacterInventory.Instance != null && UserCharacterInventory.Instance.ownedCharacterIDs.Count > 0)
        {
            foreach (string id in UserCharacterInventory.Instance.ownedCharacterIDs)
            {
                if (!storageTempPartyList.Contains(id)) warehouseIDs.Add(id);
            }
        }
        else
        {
            // 파티(맨 왼쪽 슬롯)에 등록되지 않은 상태일 때만 창고에 보여줍니다.
            if (!storageTempPartyList.Contains(currentMainHeroID)) warehouseIDs.Add(currentMainHeroID);
        }

        // 3. 우측 하단 보유 수량 텍스트 업데이트
        if (storageCountText != null)
        {
            storageCountText.text = (storageTempPartyList.Count + warehouseIDs.Count).ToString() + " / " + storageMaxCount.ToString();
        }

        // 4. Resources 폴더 최상단 데이터 시트 로드
CharacterData[] allDataSheets = Resources.LoadAll<CharacterData>("캐릭터");

warehouseIDs.Clear();
if (UserCharacterInventory.Instance != null)
{
    foreach (string id in UserCharacterInventory.Instance.ownedCharacterIDs)
    {
        warehouseIDs.Add(id);
    }
}

        // 5. 창고에 남은 캐릭터들을 좌측 상단부터 규칙대로 나열
        foreach (string charID in warehouseIDs)
        {
            CharacterData originalData = null;
            foreach (var sheet in allDataSheets)
            {
                if (sheet != null && sheet.name.Contains(charID))
                {
                    originalData = sheet;
                    break;
                }
            }
                    // 창고 격자판에 생성하지 않고 다음 캐릭터로 패스(continue)해 버립니다!
        if (storageTempPartyList.Contains(charID))
        {
            continue; 
        }

            if (originalData != null && storageCharacterSlotPrefab != null)
            {
                GameObject newCard = Instantiate(storageCharacterSlotPrefab, storageGridParentGroup);
                // 💡 [Z축 보정 코드 추가] 화면 멀리 파묻힌 카드를 마우스 광선이 닿는 평면(Z:0)으로 강제 정렬합니다!
                if (newCard != null)
                {
                    RectTransform rect = newCard.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.anchoredPosition3D = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, 0f);
                        rect.localScale = Vector3.one;
                        rect.localRotation = Quaternion.identity;
                    }
                }
                CharacterComponent cardComp = newCard.GetComponent<CharacterComponent>();
                if (cardComp != null) cardComp.myData = originalData;

                // 이미지 및 고유 색상 입히기
                Image[] allImages = newCard.GetComponentsInChildren<Image>(true);
                foreach (Image img in allImages)
                {
                    if (img != null)
                    {
                        if (originalData.characterSprite != null) img.sprite = originalData.characterSprite;
                        img.color = new Color(originalData.characterColor.r, originalData.characterColor.g, originalData.characterColor.b, 1f);
                    }
                }
            }
        }
    }






public void OnClickUniqueStorageCharacter(string characterID) // 💡 private을 public으로 변경!
    {
        // 1. 이미 파티에 들어가 있다면 중복 장착을 막습니다.
        if (storageTempPartyList.Contains(characterID))
        {
            Debug.LogWarning("[Storage] 이미 파티에 장착된 캐릭터입니다!");
            return;
        }

        // 2. 파티창 최대 칸수(5칸) 제한 검사
        if (storageTempPartyList.Count >= uniquePartySlots.Count)
        {
            Debug.LogWarning("[Storage] 파티원이 이미 가득 찼습니다!");
            return;
        }

        // 3. 파티 리스트에 추가하고 상하단 화면을 즉시 새로고침합니다.
        storageTempPartyList.Add(characterID);
        
        RefreshUniqueStorageUI();
        RefreshUniquePartyUI();
        Debug.Log($"[Storage] 파티에 캐릭터 추가 완료: {characterID}");
    }


    public void RefreshUniquePartyUI()
    {
        // 1. 상단 슬롯 기화 및 기존 연출 잔여물 청소
        for (int i = 0; i < uniquePartySlots.Count; i++)
        {
            if (uniquePartySlots[i] != null)
            {
                // 빈 칸 배경인 정사각형 회색 에셋으로 되돌립니다.
                uniquePartySlots[i].sprite = storageEmptySlotSprite;

                // 슬롯 내부(자식)를 이 잡듯 뒤져서 생성되어 있던 가짜 카드 오브젝트들을 싹 다 Destroy 시켜 버립니다!
                List<GameObject> childrenToDestroy = new List<GameObject>();
                foreach (Transform child in uniquePartySlots[i].transform)
                {
                    childrenToDestroy.Add(child.gameObject);
                }

                // 리스트에 담아 안전하고 깔끔하게 싹 지워버립니다.
                foreach (GameObject childObj in childrenToDestroy)
                {
                    Destroy(childObj);
                }
            }
        }

        // 2. 만능 '슬롯' 프리팹을 상단 칸에 소환하고, 하단에 생성된 진짜 데이터와 동기화합니다.
            // 💡 저장된 파티 목록(storageTempPartyList)으로 직접 UI 생성
            // 💡 [수정 구역] Resources/캐릭터 폴더 내부를 샅샅이 뒤져 데이터를 직통으로 꽂아 넣습니다.
            for (int i = 0; i < storageTempPartyList.Count; i++)
            {
                if (i >= uniquePartySlots.Count) break;

                // 유저님이 아까 세팅 완료하신 완벽한 폴더 경로 내부의 모든 시트를 긁어옵니다.
                string charID = storageTempPartyList[i];
                CharacterData[] allSheets = Resources.LoadAll<CharacterData>("");
                CharacterData targetSheet = null;

                foreach (var sheet in allSheets)
                {
                    if (sheet != null && sheet.name.Contains(charID))
                    {
                        targetSheet = sheet;
                        break;
                    }
                }

                // 진짜 데이터를 찾았다면, 상단 액자(uniquePartySlots) 자식 위치에 만능 프리팹을 즉시 소환합니다!
                if (targetSheet != null && uniquePartySlots[i] != null && storageCharacterSlotPrefab != null)
                {
                    GameObject newSlotObj = Instantiate(storageCharacterSlotPrefab, uniquePartySlots[i].transform);
        // 💡 [UI 전용 좌표 고정] 부모 정렬기가 카드를 화면 뒤로 숨기지 못하도록 UI 전용 3D 좌표계로 고정합니다!
        RectTransform slotRect = newSlotObj.GetComponent<RectTransform>();
        if (slotRect != null)
        {
            slotRect.anchoredPosition3D = Vector3.zero;
            slotRect.localScale = Vector3.one;
            slotRect.localRotation = Quaternion.identity;
        }


                    // 매니저의 파티 해제 함수(OnClickUniquePartySlot)가 내 방 번호(i)를 들고 실행되도록 직통 단추를 채웁니다!
                    Button slotBtn = newSlotObj.GetComponent<Button>();
                    if (slotBtn != null)
                    {
                        int slotIndex = i; // 반복문의 현재 방 번호(0~4)를 안전하게 가둡니다.
                        slotBtn.onClick.RemoveAllListeners(); // 기존 창고 장착용 리스너를 깔끔하게 밀어버립니다.
                        slotBtn.onClick.AddListener(() => OnClickUniquePartySlot(slotIndex));
                    }

                    // 생성된 카드 붕어빵에 진짜 기획서 문서를 주입합니다.
                    CharacterComponent comp = newSlotObj.GetComponent<CharacterComponent>();
                    if (comp != null)
                    {
                        comp.myData = targetSheet;
                    }

                    // 이미지와 텍스트를 포함한 자식 컴포넌트들을 샅샅이 뒤져 하얀 박스를 지우고 초록색 컬러를 수놓아 줍니다.
                    Image[] images = newSlotObj.GetComponentsInChildren<Image>(true);
                    foreach (Image img in images)
                    {
                        if (img != null)
                        {
                            if (targetSheet.characterSprite != null) img.sprite = targetSheet.characterSprite;
                            img.color = new Color(targetSheet.characterColor.r, targetSheet.characterColor.g, targetSheet.characterColor.b, 1f);
                        }
                    }
                }
            }


        UpdateUniqueSynergyText();
    }



    private void UpdateUniqueSynergyText()
    {
        if (storageSynergyText == null) return;

        if (storageTempPartyList.Count == 0)
        {
            storageSynergyText.text = "현재 배치된 아군이 없습니다.\n하단 보관함에서 내 메인 캐릭터를\n터치해 파티를 구성해 보세요.";
            return;
        }

        string resultText = "";
        string leaderID = storageTempPartyList.Count > 0 ? storageTempPartyList[0] : ""; resultText += "<color=yellow>[현재 배치된 메인 리더 효과]</color>\n";

        if (leaderID == "01" || leaderID == "001") resultText += "- [촌장의 위엄] 맨 왼쪽 리더의 격려로 아군 전체 공격력 +10%\n";
        else if (leaderID == "02" || leaderID == "002") resultText += "- [하마의 뚝심] 맨 왼쪽 리더의 존재감으로 아군 전체 체력 +15%\n";
        else if (leaderID == "03" || leaderID == "003") resultText += "- [악어의 투지] 맨 왼쪽 리더의 포효로 치명타 확률 +12%\n";
        else if (leaderID == "04" || leaderID == "004") resultText += "- [너구리의 지혜] 맨 왼쪽 리더의 영리함으로 코인 골드 획득량 +20%\n";
        else resultText += $"- 일반 리더 보너스: 아군 전체 이동 속도 +5%\n";

        storageSynergyText.text = resultText;
    }

    public void OnClickUniquePartySlot(int slotIndex)
    {
        // 클릭한 슬롯 번호가 현재 배치된 파티원 수 범위 안인지 안전 검사
        if (slotIndex >= 0 && slotIndex < storageTempPartyList.Count)
        {
            string removedCharID = storageTempPartyList[slotIndex];
            
            // 파티 리스트에서 삭제하고 상하단 화면을 즉시 새로고침합니다.
            storageTempPartyList.RemoveAt(slotIndex);
            
            RefreshUniqueStorageUI();
            RefreshUniquePartyUI();
            Debug.Log($"[Storage] 상단 파티 클릭 -> 창고로 복귀 완료: {removedCharID}");
        }
    }


    public void OnClickReturnToTown()
    {
        // 1. [영구 세이브] 파티 주머니에 담긴 ID들(예: "03")을 콤마(,)로 묶어 하드디스크에 저장합니다.
        string combinedPartyData = string.Join(",", storageTempPartyList);
        PlayerPrefs.SetString("SelectedCharacterID", combinedPartyData);
        PlayerPrefs.Save();
        Debug.Log($"[Storage] 파티 변동사항 영구 저장 완료: {combinedPartyData}");

        // 2. [바통 터치] 씬이 넘어가도 살아있는 메인 파티 매니저(PartyManager)의 명단도 실시간 동기화해 줍니다.
        if (PartyManager.Instance != null)
        {
            PartyManager.Instance.currentPartyList = new List<string>(storageTempPartyList);
        }

                // 💡 [낙인 코드 추가]: 하드디스크에 "나는 지금 복귀하는 중이다!"라는 증거를 1로 남깁니다.
        PlayerPrefs.SetInt("IsReturningFromStorage", 1);
        PlayerPrefs.Save();

        // 3. [마을 복귀] 유저님이 프로젝트 창에 만들어두신 진짜 마을 씬 이름으로 정확히 전환합니다!
        UnityEngine.SceneManagement.SceneManager.LoadScene("게임초반에서마을까지");
    }

}
