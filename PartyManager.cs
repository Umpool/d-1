// [유니티 매뉴얼] 이 코드는 주석 포함 총 50줄입니다.
using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    [Header("파티 데이터")]
    [Tooltip("현재 파티에 참가 중인 캐릭터들의 ID 리스트")]
    public List<string> currentPartyList = new List<string>();

    [Tooltip("파티에 최대로 넣을 수 있는 인원수")]
    public int maxPartySize = 3;

    private void Awake()
    {
        // 어디서나 소환할 수 있는 중앙 통제실(싱글톤) 방식을 채택합니다.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 화면이 바뀌어도 절대 파괴되지 않음
        }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// [핵심 기능]: 파티에 캐릭터 추가 (자유롭게 넣기)
    /// </summary>
    public bool AddToParty(string characterID)
    {
        if (currentPartyList.Contains(characterID))
        {
            Debug.LogWarning($"[Party] {characterID}는 이미 파티에 참가 중입니다.");
            return false;
        }

        if (currentPartyList.Count >= maxPartySize)
        {
            Debug.LogWarning("[Party] 파티원이 가득 찼습니다!");
            return false;
        }

        currentPartyList.Add(characterID);
        Debug.Log($"[Party] 파티원 영입 성공! 현재 인원: {currentPartyList.Count}/{maxPartySize}");
        return true;
    }

    /// <summary>
    /// [핵심 기능]: 파티에서 캐릭터 제외 (자유롭게 빼기)
    /// </summary>
    public void RemoveFromParty(string characterID)
    {
        if (currentPartyList.Contains(characterID))
        {
            currentPartyList.Remove(characterID);
            Debug.Log($"[Party] {characterID} 탈퇴 완료. 현재 인원: {currentPartyList.Count}/{maxPartySize}");
        }
    }
}
