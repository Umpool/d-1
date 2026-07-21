using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;

public class SecondEventScreenManager : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    public TextMeshProUGUI storyText;
    public GameObject nextButton;

    [Header("닉네임 입력 UI 설정")]
    public TMP_InputField nicknameInputField; // 유니티 에디터의 InputField 연결
    public GameObject nicknamePanel;          // 입력창, 확인버튼이 묶인 패널 오브젝트
    public TextMeshProUGUI alertText;         // "글자 수가 맞지 않습니다" 출력용 텍스트

    [SerializeField] private TextMeshProUGUI alertWarningText;

    private Coroutine alertFadeCoroutine;

    private Coroutine alertCoroutine;
    [Header("화면 전환 설정")]
    public GameObject villagePanel;

    [Header("타이핑 효과 설정")]
    public float typingSpeed = 0.05f;

    private int currentStep = 0;
    private bool isTyping = false;
    private string fullText = "";
    private Coroutine colorCoroutine;
    private float lastClickTime = 0f;

    public void StartEventWithDelay()
    {
        Invoke("ExecuteCurrentStep", 0.3f);
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
            if (storyText != null) storyText.color = Color.white;
            StartTypingEffect("네번째 텍스트입니다.");
            break;



            case 1:
                Debug.Log("[Event] 텍스트 색상 파란색(Blue) 스르륵 페이드 시작");
                StartColorFadeEffect(Color.blue);
                break;

            case 2:
                if (storyText != null) storyText.color = Color.white;
                StartTypingEffect("다섯번째 텍스트입니다.");
                break;

            case 3:
                Debug.Log("[Event] 텍스트 색상 보라색(Purple) 스르륵 페이드 시작");
                StartColorFadeEffect(new Color(0.5f, 0f, 0.5f));
                break;

            case 4:
                if (storyText != null) storyText.color = Color.white;
                StartTypingEffect("여섯번째 텍스트입니다.");
                break;

            case 5:
                Debug.Log("[Event] 텍스트 색상 빨간색(Red) 스르륵 페이드 시작");
                StartColorFadeEffect(Color.red);
                break;

            case 6:
                // 1. 대사 출력창 및 상태 초기화 (루프 방지)
                currentStep = 0;
                if (storyText != null)
                {
                    storyText.text = "";
                    storyText.color = Color.white;
                }

                // 2. [기존 기능 유지] 캐릭터 데이터 저장 및 인벤토리 영입 처리
                if (PartyManager.Instance != null && PartyManager.Instance.currentPartyList.Count > 0)
                {
                    string finalSelectedID = PartyManager.Instance.currentPartyList[0];

                    // 하드디스크에 메인 캐릭터 ID 기록
                    PlayerPrefs.SetString("SelectedCharacterID", finalSelectedID);

                    // 유저 전용 캐릭터 가방(인벤토리)에 최종 영입 처리
                    if (UserCharacterInventory.Instance != null)
                    {
                        UserCharacterInventory.Instance.AddCharacter(finalSelectedID);
                    }

                    // 통합 마스터 세이브 데이터 흔적 남기기
                    PlayerPrefs.SetInt("HasSaveData", 1);
                    PlayerPrefs.Save();

                    Debug.Log($"[Event] 캐릭터 창고 영입 및 통합 데이터 저장 완료: {finalSelectedID}");
                }

                // 3. [연출 개선] 대사 넘기기 버튼을 끄고, 닉네임 입력 패널 활성화
                if (nextButton != null)
                {
                    nextButton.SetActive(false);
                }

                if (nicknamePanel != null)
                {
                    nicknamePanel.SetActive(true);
                }

                if (alertText != null)
                {
                    alertText.text = "닉네임을 입력해 주세요.";
                }
                break;




        }
    }
    private void StartColorFadeEffect(Color targetColor)
    {
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }
        colorCoroutine = StartCoroutine(FadeTextColorRoutine(targetColor, 0.5f));
    }

    private System.Collections.IEnumerator FadeTextColorRoutine(Color endColor, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = Color.white;

        if (storyText != null)
        {
            startColor = storyText.color;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (storyText != null)
            {
                storyText.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            }
            yield return null;
        }

        if (storyText != null)
        {
            storyText.color = endColor;
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
    // ==========================================
    // [최종 마감] 닉네임 유효성 검증 및 마을 진입 함수
    // ==========================================
    public void OnClickConfirmNickname()
    {
        if (nicknameInputField == null) return;

        string inputName = nicknameInputField.text.Trim();
        inputName = Regex.Replace(inputName, @"\s", "");

        // // 3. 한글 포함 여부 확인 로직
        bool hasKorean = Regex.IsMatch(inputName, "[가-힣]");

        if (hasKorean)
        {
            // 한글이 섞여있다면 2자 이상 8자 이하 규칙을 적용합니다.
            if (inputName.Length < 2 || inputName.Length > 8)
            {
                ShowWarningMessage("한글 닉네임은 2 ~ 8자만 가능합니다.");
                return;
            }
        } // 👈 hasKorean if문 끝
        else
        {
            // 순수 영문과 숫자로만 이루어져 있다면 3자 이상 12자 이하 규칙을 적용합니다.
            if (inputName.Length < 3 || inputName.Length > 12)
            {
                ShowWarningMessage("영문/숫자 닉네임은 3 ~ 12자만 가능합니다.");
                return;
            }
        } // 👈 else문 끝

        // 💡 [여기가 중요!] 위 검사(return)들을 모두 통과해야만 아래 로직으로 내려옵니다.
        if (alertFadeCoroutine != null) StopCoroutine(alertFadeCoroutine);
        if (alertWarningText != null) alertWarningText.text = "";

        // // 4. [검증 통과 완료] 하드디스크에 유저 닉네임을 안전하게 영구 기록합니다.
        PlayerPrefs.SetString("UserNickname", inputName);
        PlayerPrefs.Save();
        Debug.Log($"[Nickname] 유저의 고유 닉네임 검증 승인 및 저장 완료: {inputName}");

        // // 5. 입력창 패널을 끄고, 대망의 마을 화면 패널을 시원하게 켭니다!
        if (nicknamePanel != null) nicknamePanel.SetActive(false);
        if (villagePanel != null) villagePanel.SetActive(true);

        // // 두 번째 이벤트 화면은 스스로 퇴장합니다.
        this.gameObject.SetActive(false);

    }
    // 💡 268번 줄부터 파일 맨 아래 끝날 때까지 이 코드를 그대로 붙여넣으세요!

    private void ShowWarningMessage(string message)
    {
        // 변수 이름을 기존에 사용 중이던 alertText로 맞춰줍니다.
        if (alertText == null) return;
        
        // 기존에 작동 중이던 페이드아웃 효과가 있다면 강제 종료 (연타 방지)
        if (alertFadeCoroutine != null) StopCoroutine(alertFadeCoroutine);
        
        // 텍스트 내용 지정 및 알파(불투명도) 값을 1(100%)로 복구
        alertText.text = message;
        alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, 1f);
        
        // 2초 대기 후 0.5초 동안 사라지기 시작
        alertFadeCoroutine = StartCoroutine(FadeOutAlertRoutine(2.0f, 0.5f));
    }

    // 중복되던 구버전 함수를 완전히 지우고 이 하나만 남겨둡니다!
    private IEnumerator FadeOutAlertRoutine(float delay, float fadeDuration)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            alertText.color = new Color(alertText.color.r, alertText.color.g, alertText.color.b, alpha);
            yield return null;
        }
        alertText.text = "";
    }
} // 👈 스크립트가 끝나는 마지막 중괄호입니다. 누락되지 않게 확인해 주세요!
