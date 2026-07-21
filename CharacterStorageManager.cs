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

    [Header("💡 [기획 원안 추가] 격자 및 파티 레이아웃 설정")]
    [Tooltip("가로로 배치할 최대 칸 수 (기획서 기준: 5)")]
    private int gridColumnCount = 5;

    [Header("Character Prefab Setup")]
    public GameObject[] characterPrefabs;

    private List<string> storageTempPartyList = new List<string>();

    private void OnEnable()
    {
        // 1. 싱글톤 보유 가방으로부터 내가 가진 전체 캐릭터 ID를 내 창고 주머니로 이식합니다.
        if (UserCharacterInventory.Instance != null && UserCharacterInventory.Instance.ownedCharacterIDs != null)
        {
            // 💡 [수정] 선언된 변수 이름인 storageTempPartyList로 맞춰줍니다.
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

        // 3. 데이터 동기화가 완전히 끝났으므로 화면 UI들을 정석대로 다시 그립니다.
        RefreshUniqueStorageUI();
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

            if (originalData != null && storageCharacterSlotPrefab != null)
            {
                GameObject newCard = Instantiate(storageCharacterSlotPrefab, storageGridParentGroup);
                
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






    private void OnClickUniqueStorageCharacter(string characterID)
    {
        if (storageTempPartyList.Contains(characterID))
        {
            storageTempPartyList.Remove(characterID);
            Debug.Log($"[Storage] 파티에서 메인 캐릭터 제외 완료: {characterID}");
        }
        else
        {
            if (storageTempPartyList.Count >= uniquePartySlots.Count)
            {
                Debug.LogWarning("[Storage] 파티원이 이미 가득 찼습니다!");
                return;
            }

            storageTempPartyList.Add(characterID);
            Debug.Log($"[Storage] 파티 맨 왼쪽 리더 자리 자동 영입 성공: {characterID}");
        }

        RefreshUniquePartyUI();
    }

    public void RefreshUniquePartyUI()
    {
        // 1. 상단 슬롯 기화 및 기존 연출 잔여물 청소
        for (int i = 0; i < uniquePartySlots.Count; i++)
        {
            if (uniquePartySlots[i] != null)
            {
                uniquePartySlots[i].sprite = storageEmptySlotSprite;
                foreach (Transform child in uniquePartySlots[i].transform) 
                { 
                    Destroy(child.gameObject); 
                }
            }
        }

        // 2. 만능 '슬롯' 프리팹을 상단 칸에 소환하고, 하단에 생성된 진짜 데이터와 동기화합니다.
        CharacterComponent[] spawnedCards = storageGridParentGroup.GetComponentsInChildren<CharacterComponent>();
        
        for (int i = 0; i < storageTempPartyList.Count; i++)
        {
            if (i >= uniquePartySlots.Count) break;
            
            string partyCharID = storageTempPartyList[i];
            
            foreach (CharacterComponent card in spawnedCards)
            {
                if (card != null && card.myData != null && card.myData.characterID == partyCharID)
                {
                    if (uniquePartySlots[i] != null && storageCharacterSlotPrefab != null)
                    {
                        // 새 변수를 열어 부모 슬롯 자식 위치에 만능 프리팹을 생성합니다.
                        GameObject newSlotObj = Instantiate(storageCharacterSlotPrefab, uniquePartySlots[i].transform);
                        newSlotObj.transform.localPosition = Vector3.zero;
                        newSlotObj.transform.localScale = Vector3.one;

                        // 생성된 프리팹의 이미지 컴포넌트를 가져와 하단 원본 데이터의 그림과 색상을 똑같이 대입합니다.
                        Image slotImage = newSlotObj.GetComponent<Image>();
                        if (slotImage != null && card.myData.characterSprite != null)
                        {
                            slotImage.sprite = card.myData.characterSprite;
                            
                            // 💡 유저님이 지정하신 캐릭터의 고유 색상(빨강, 파랑 등)을 불투명하게 입혀줍니다.
                            slotImage.color = new Color(card.myData.characterColor.r, card.myData.characterColor.g, card.myData.characterColor.b, 1f);
                        }
                    }
                    break;
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
        string leaderID = storageTempPartyList.Count > 0 ? storageTempPartyList[0] : ""; resultText += "<color=yellow>[👑 현재 배치된 메인 리더 효과]</color>\n";

        if (leaderID == "01" || leaderID == "001") resultText += "- [촌장의 위엄] 맨 왼쪽 리더의 격려로 아군 전체 공격력 +10%\n";
        else if (leaderID == "02" || leaderID == "002") resultText += "- [하마의 뚝심] 맨 왼쪽 리더의 존재감으로 아군 전체 체력 +15%\n";
        else if (leaderID == "03" || leaderID == "003") resultText += "- [악어의 투지] 맨 왼쪽 리더의 포효로 치명타 확률 +12%\n";
        else if (leaderID == "04" || leaderID == "004") resultText += "- [너구리의 지혜] 맨 왼쪽 리더의 영리함으로 코인 골드 획득량 +20%\n";
        else resultText += $"- 일반 리더 보너스: 아군 전체 이동 속도 +5%\n";

        storageSynergyText.text = resultText;
    }

    public void OnClickUniquePartySlot(int slotIndex)
    {
        if (slotIndex < storageTempPartyList.Count)
        {
            string removedCharID = storageTempPartyList[slotIndex];
            storageTempPartyList.RemoveAt(slotIndex);
            Debug.Log($"[Storage] 상단 파티장 터치 -> 파티에서 제외 완료: {removedCharID}");
            RefreshUniquePartyUI();
        }
    }

    public void OnClickReturnToTown()
    {
        Debug.Log("[Storage] 변경 사항 세이브 및 마을 복귀 가동");

        if (PartyManager.Instance != null)
        {
            PartyManager.Instance.currentPartyList.Clear();
            foreach (string charID in storageTempPartyList)
            {
                PartyManager.Instance.currentPartyList.Add(charID);
            }

            string combinedParty = string.Join(",", PartyManager.Instance.currentPartyList);
            PlayerPrefs.SetString("SelectedCharacterID", storageTempPartyList.Count > 0 ? storageTempPartyList[0] : "");
            PlayerPrefs.SetString("CurrentPartyListString", combinedParty);
            PlayerPrefs.Save();
        }

        PlayerPrefs.SetInt("IsReturningFromStorage", 1);
        PlayerPrefs.Save();

        CancelInvoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene("게임초반에서마을까지");
    }
}
