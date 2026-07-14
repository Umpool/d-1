using UnityEngine;
using TMPro;

public class SecondEventScreenManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public TextMeshProUGUI storyText;
    public GameObject nextButton;

    [Header("화면 전환 설정")]
    public GameObject villagePanel;

    [Header("타이핑 효과 설정")]
    public float typingSpeed = 0.05f;

    private int currentStep = 0;

    // 현재 타이핑 연출이 진행 중인지 체크하는 안전장치
    private bool isTyping = false;
    private string fullText = "";
    private float lastClickTime = 0f;

    private void Start()
    {
        currentStep = 0;
        isTyping = false;
        fullText = "";

        if (storyText != null)
        {
            storyText.text = "";
            storyText.color = Color.white;
        }

        StartEventWithDelay();
    }



    public void StartEventWithDelay()
    {
        // 0.5초 동안 유니티 엔진과 유저가 숨을 완전히 고른 뒤에 첫 시나리오를 가동하라고 명합니다.
        Invoke("ExecuteCurrentStep", 0.2f);
    }


    /// <summary>
    /// 유니티 에디터에 배치한 [다음] 버튼을 누를 때마다 실행될 함수입니다.
    /// </summary>
    public void OnClickNextButton()
    {
        // [강력한 연타 방어막] 이전 클릭 후 0.4초가 지나지 않았다면 클릭을 무시합니다.
        if (Time.time - lastClickTime < 0.3f)
        {
            return;
        }
        lastClickTime = Time.time;

        if (isTyping)
        {
            StopAllCoroutines();
            storyText.text = fullText;
            isTyping = false;
            Debug.Log($"[Event] 타이핑 연출 스킵 완료 - 현재 단계: {currentStep}");
            return;
        }

        currentStep++;
        ExecuteCurrentStep();
    }




    private void ExecuteCurrentStep()
    {
        Debug.Log($"[Event] 시나리오 {currentStep} 단계 실행 시작");

        switch (currentStep)
        {
            case 0:
                storyText.color = Color.white;
                StartTypingEffect("네번째 텍스트입니다.");
                break;

            case 1:
                storyText.color = Color.blue;
                Debug.Log("[Event] 텍스트 색상 파란색(Blue) 변경 완료");
                break;

            case 2:
                storyText.color = Color.white;
                StartTypingEffect("다섯번째 텍스트입니다.");
                break;

            case 3:
                storyText.color = new Color(0.5f, 0f, 0.5f);
                Debug.Log("[Event] 텍스트 색상 보라색(Purple) 변경 완료");
                break;

            case 4:
                storyText.color = Color.white;
                StartTypingEffect("여섯번째 텍스트입니다.");
                break;

            case 5:
                storyText.color = Color.red;
                Debug.Log("[Event] 텍스트 색상 빨간색(Red) 변경 완료");
                break;
            case 6:
                Debug.Log("[Event] 다음 화면으로 넘어가기 전 대사와 글자창을 완전히 포맷합니다.");
                currentStep = 0;
                if (storyText != null) { storyText.text = ""; storyText.color = Color.white; }
                if (villagePanel != null) villagePanel.SetActive(true);
                this.gameObject.SetActive(false);
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
