// [유니티 매뉴얼] 이 데이터 구조 스크립트는 총 20줄입니다.
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [Header("영웅 고유 정보")]
    [Tooltip("캐릭터의 고유 ID (예: Warrior, Mage 등)")]
    public string characterID;

    [Tooltip("출력될 영웅 이름")]
    public string characterName;

    [TextArea(2, 5)]
    [Tooltip("영웅 설명 및 정보")]
    public string characterInfo;

    [Tooltip("영웅 일러스트 이미지")]
    public Sprite characterSprite;

    // [유니티 매뉴얼] 이미지 밑에 이 한 줄만 쏙 추가해 줍니다.
    [Tooltip("임시 테스트용 영웅 고유 색상 (나중에 진짜 이미지가 오면 흰색으로 두면 됩니다)")]
    public Color characterColor = Color.white;

}
