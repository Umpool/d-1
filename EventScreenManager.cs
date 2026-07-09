using UnityEngine;
using TMPro;

public class EventScreenManager : MonoBehaviour
{
    // [유니티 매뉴얼] 이 코드는 주석 포함 총 100줄입니다.

    [Header("UI 컴포넌트 연결")]
    [Tooltip("스토리 글자가 출력될 텍스트 오브젝트")] public TextMeshProUGUI storyText;
    [Tooltip("다음 단계로 넘어가는 다음(Next) 버튼")] public GameObject nextButton;

    [Header("화면 전환 오브젝트 연결")]
    [Tooltip("현재 이벤트 화면 오브젝트 자신")] public GameObject firstEventPanel;
    [Tooltip("다음에 켜줄 메인 캐릭터 선택창 오브젝트")] public GameObject characterSelectPanel;

    [Header("타이핑 효과 설정")]
    [Tooltip("글자가 한 글자씩 찍히는 속도 (낮을수록 빠름)")] public float typingSpeed = 0.05f;

    // 현재 진행 중인 단계를 기록하는 이정표 변수 (0부터 시작)
    private int currentStep = 0;

    // 현재 타이핑 연출이 진행 중인지 체크하는 안전장치
    private bool isTyping = false;
    private string fullText = "";

    public void StartEventWithDelay()
    {
        // 0.5초 동안 유니티 엔진과 유저가 숨을 완전히 고른 뒤에 첫 시나리오를 가동하라고 명합니다.
        Invoke("ExecuteCurrentStep", 0.5f);
    }


    /// <summary>
    /// 유니티 에디터에 배치한 [다음] 버튼을 누를 때마다 실행될 함수입니다.
    /// </summary>
    public void OnClickNextButton()
    {
        // [안전장치] 만약 글자가 타닥타닥 찍히는 중이라면, 버튼을 눌러도 다음 단계로 안 넘어가고 
        // 문장을 한 번에 짜잔! 하고 전부 보여주는 스킵(Skip) 처리를 합니다.
        if (isTyping)
        {
            StopAllCoroutines();
            storyText.text = fullText;
            isTyping = false;
            Debug.Log($"[Event] 타이핑 연출 스킵 완료 - 현재 단계: {currentStep}");
            return;
        }

        // 타이핑이 완전히 끝난 상태에서 버튼을 누르면 다음 단계로 숫자를 올립니다.
        currentStep++;
        ExecuteCurrentStep();
    }

    /// <summary>
    /// 기획하신 시나리오 조건(글자 출력 및 색상 변경)을 순서대로 실행하는 핵심 함수입니다.
    /// </summary>
    private void ExecuteCurrentStep()
    {
        Debug.Log($"[Event] 시나리오 {currentStep}단계 실행 시작");

        switch (currentStep)
        {
            case 0: // 1. 첫 번째 텍스트 출력
                StartTypingEffect("첫번째 컬러입니다.");
                break;

            case 1: // 2. 다음 버튼 ➡️ 빨간색으로 변경
                storyText.color = Color.red;
                Debug.Log("[Event] 텍스트 색상 빨간색(Red) 변경 완료");
                break;

            case 2: // 3. 다음 버튼 ➡️ 두 번째 텍스트 출력
                storyText.color = Color.white; // 색상을 평소 상태(흰색)로 되돌림
                StartTypingEffect("두번째 컬러입니다.");
                break;

            case 3: // 4. 다음 버튼 ➡️ 노란색으로 변경
                storyText.color = new Color(1f, 0.92f, 0.016f); // 황색
                Debug.Log("[Event] 텍스트 색상 노란색(Yellow) 변경 완료");
                break;

            case 4: // 5. 다음 버튼 ➡️ 세 번째 텍스트 출력
                storyText.color = Color.white; // 색상 초기화
                StartTypingEffect("마지막 텍스트입니다.");
                break;

            case 5: // 6. 다음 버튼 ➡️ 초록색으로 변경
                storyText.color = Color.green;
                Debug.Log("[Event] 텍스트 색상 초록색(Green) 변경 완료");
                break;

            case 6: // 7. 마지막 다음 버튼 ➡️ 메인 캐릭터 선택창으로 대전환!
                Debug.Log("[Event] 시나리오 종료! 첫 이벤트 화면을 끄고 캐릭터 선택창을 켭니다.");
                if (characterSelectPanel != null) characterSelectPanel.SetActive(true);
                if (firstEventPanel != null) firstEventPanel.SetActive(false);
                break;
        }
    }

    // --- 글자를 한 글자씩 타닥타닥 출력해 주는 코루틴 기능 부속품 ---
    private void StartTypingEffect(string targetText)
    {
        StopAllCoroutines();
        fullText = targetText;
        StartCoroutine(TypeTextRoutine());
    }

    private System.Collections.IEnumerator TypeTextRoutine()
    {
        isTyping = true;
        storyText.text = ""; // 글자창을 깨끗하게 비웁니다.

        // 글자 배열을 돌며 한 글자씩 조립해서 화면에 뿌려줍니다.
        foreach (char letter in fullText)
        {
            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // 설정한 속도만큼 잠깐 대기
        }

        isTyping = false;
    }
}
