using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 전체 시나리오 관리 클래스
public class SecondEventScreenManager : MonoBehaviour
{
    [Header("[UI 컴포넌트 및 버튼 연결]")]
    [SerializeField] private TextMeshProUGUI storyText; // 대사 출력 TMPro
    [SerializeField] private Button nextButton;        // 클릭 버튼

    [Header("[최종 화면 전환]")]
    [SerializeField] private GameObject villagePanel;  // 마을 패널

    [Header("[타이핑 연출 설정]")]
    [SerializeField] private float typingSpeed = 0.05f; // 글자 출력 속도

    // 상태 제어용 프라이빗 변수들
    private int currentStep = 0;      // 현재 대사 인덱스
    private bool isTyping = false;     // 타이핑 중 여부 (스킵 체크용)
    private string fullText = "";      // 현재 화면에 출력할 전체 원본 대사
    private Coroutine colorCoroutine;  // 텍스트 색상 변경 코루틴 제어용
}
private void OnEnable()
{
    currentStep = 0; // 단계 초기화

    // 버튼 클릭 이벤트 연결
    if (nextButton != null)
    {
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnClickNextButton);
    }
    ExecuteCurrentStep(); // 첫 문장 출력
}

private void OnDisable()
{
    if (nextButton != null) nextButton.onClick.RemoveListener(OnClickNextButton);
}

/// <summary>
/// 클릭 시 타이핑 중이면 전체 출력(스킵), 완료 상태면 다음 대사로 진행
/// </summary>
public void OnClickNextButton()
{
    if (isTyping) // 타이핑 중일 때 -> 스킵
    {
        StopAllCoroutines();
        storyText.text = fullText;
        isTyping = false;
        return;
    }
    // 타이핑 완료 상태일 때 -> 다음 단계
    currentStep++;
    ExecuteCurrentStep();
}
/// <summary>
/// 기획한 시나리오 조건에 맞춰 대사와 색상을 준비하는 함수입니다.
/// </summary>
private void ExecuteCurrentStep()
{
    // 연출이 시작되기 전, 텍스트 색상을 기본 흰색으로 맑게 초기화합니다.
    if (storyText != null)
    {
        storyText.color = Color.white;
    }

    Color targetColor = Color.white;

    switch (currentStep)
    {
        case 0:
            fullText = "네번째 텍스트입니다.";
            targetColor = Color.blue; // 파란색으로 물들 예정
            break;

        case 1:
            fullText = "다섯번째 텍스트입니다.";
            targetColor = new Color(0.5f, 0f, 0.5f); // 보라색으로 물들 예정
            break;
        case 2:
            fullText = "여섯번째 텍스트입니다.";
            targetColor = Color.red; // 빨간색으로 물들 예정
            break;

        default:
            // 모든 대사가 끝나면 숫자를 0으로 돌려놓습니다.
            currentStep = 0;

            // 다음번에 찰나라도 잔상이 보이지 않게 텍스트를 깨끗하게 비웁니다.
            if (storyText != null)
            {
                storyText.text = "";
            }

            // 최종 마을 화면 패널은 켜고, 현재 이벤트창은 비활성화합니다.
            if (villagePanel != null)
            {
                villagePanel.SetActive(true);
            }
            this.gameObject.SetActive(false);
            return;
    }

    // 대사와 색상 지정이 끝나면 타이핑과 색상 변경 연출을 시작합니다.
    StartEffects(targetColor);
}
/// <summary>
/// 기존 연출들을 모두 안전하게 중지시키고 새로운 연출을 시작합니다.
/// </summary>
private void StartEffects(Color targetColor)
{
    StopAllCoroutines(); // 진행 중이던 모든 타이밍 기능을 리셋합니다.

    // 1. 한 글자씩 출력되는 타이핑 효과 가동
    StartCoroutine(TypeTextRoutine());

    // 2. 색상이 스르륵 물드는 페이드 효과 가동 (지속시간 0.8초)
    colorCoroutine = StartCoroutine(FadeTextColorRoutine(targetColor, 0.8f));
}

/// <summary>
/// 글자 주머니에서 한 글자씩 꺼내어 조립 후 화면에 출력하는 기능입니다.
/// </summary>
private System.Collections.IEnumerator TypeTextRoutine()
{
    isTyping = true; // "지금 글자 찍는 중이야" 라고 컴퓨터에게 알립니다.

    if (storyText != null)
    {
        storyText.text = ""; // 글자창을 먼저 뽀얗게 비웁니다.
    }

    // 전체 문장을 한 글자씩 순서대로 화면에 더해나갑니다.
    foreach (char letter in fullText)
    {
        if (storyText != null)
        {
            storyText.text += letter;
        }
        yield return new WaitForSeconds(typingSpeed); // 설정한 시간만큼 대기합니다.
    }

    isTyping = false; // "글자 다 찍었어" 라고 상태를 변경합니다.
}
/// <summary>
/// 글자 색상을 부드럽게 목표 색상(endColor)으로 변하게 만드는 기능입니다.
/// </summary>
private System.Collections.IEnumerator FadeTextColorRoutine(Color endColor, float duration)
{
    float elapsedTime = 0f;
    Color startColor = Color.white; // 항상 깨끗한 흰색에서 시작합니다.

    if (storyText != null)
    {
        startColor = storyText.color;
    }

    // 설정한 지속 시간(duration) 동안 매 프레임마다 색상을 스르륵 변경합니다.
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;

        if (storyText != null)
        {
            // 두 색상 사이를 부드럽게 이어주는 Lerp 기능입니다.
            storyText.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
        }
        yield return null; // 다음 프레임까지 대기합니다.
    }

    // 마지막 오차를 방지하기 위해 목표 색상을 정확하게 최종 대입합니다.
    if (storyText != null)
    {
        storyText.color = endColor;
    }
}
}
