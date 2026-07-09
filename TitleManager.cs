using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    // [유니티 매뉴얼] 이 코드는 주석 포함 총 95줄입니다.

    [Header("버튼 오브젝트 설정")]
    [Tooltip("새로운 모험 버튼")] public GameObject newGameButton;
    [Tooltip("이어하기 버튼")] public GameObject continueButton;

    [Header("팝업 및 텍스트 설정")]
    [Tooltip("설정 창 패널 오브젝트 (팝업창)")] public GameObject settingsPopupPanel;
    [Tooltip("iOS 종료 안내 텍스트")] public TextMeshProUGUI iosNoticeText;

    // [유니티 매뉴얼] 이 코드 조각은 주석 포함 총 10줄입니다. 변수 선언부에 추가하세요.
    [Header("새로운 모험 흐름 설정 (오브젝트 방식)")]
    [Tooltip("타이틀 다음으로 켜줄 첫 이벤트 화면 오브젝트입니다.")]
    public GameObject firstEventPanel;

    [Tooltip("이벤트 다음에 켜줄 메인 캐릭터 선택창 오브젝트입니다.")]
    public GameObject characterSelectPanel;


    // [중요] 유니티 에디터 창이나 코드 상에서 테스트용으로 데이터를 껐다 켰다 할 수 있게 만듭니다.
    private bool hasUserData = false;

    private void Start()
    {
        // 1. 시작 시 설정 팝업창과 iOS 안내 문구는 기본적으로 숨겨둡니다.
        if (settingsPopupPanel != null) settingsPopupPanel.SetActive(false);
        if (iosNoticeText != null) iosNoticeText.gameObject.SetActive(false);
        // [선배의 팁] 시작할 때 첫 이벤트 화면과 캐릭터 선택창을 자동으로 꺼둡니다.
        if (firstEventPanel != null) firstEventPanel.SetActive(false);
        if (characterSelectPanel != null) characterSelectPanel.SetActive(false);

        // 2. [핵심 기능] 인트로에서 검사했던 것과 동일하게 유저 데이터 존재 여부를 파악합니다.
        CheckUserDataAndSetupButtons();
    }

    // [유니티 매뉴얼] 이 수정본 조각은 주석 포함 총 35줄입니다.
    /// <summary>
    /// 플레이어 데이터 여부를 판단하여 [새로운 모험] 또는 [이어하기] 버튼만 정교하게 켜고 니다.
    /// 설정 버튼과 휴식하기 버튼은 이 함수에서 건드리지 않으므로 항상 화면에 유지됩니다.
    /// </summary>
    private void CheckUserDataAndSetupButtons()
    {
        // 인트로의 데이터 저장 열쇠인 'HasSaveData'가 존재하는지 확인합니다.
        if (PlayerPrefs.HasKey("HasSaveData"))
        {
            hasUserData = true;
            Debug.Log("[TitleManager] 데이터 확인 완료: [이어하기] 버튼을 켭니다.");

            // 데이터가 있으므로 이어하기만 켜고, 새로운 모험은 끕니다.
            if (continueButton != null) continueButton.SetActive(true);
            if (newGameButton != null) newGameButton.SetActive(false);
        }
        else
        {
            hasUserData = false;
            Debug.Log("[TitleManager] 데이터 없음 확인: [새로운 모험] 버튼을 켭니다.");

            // 데이터가 없으므로 새로운 모험만 켜고, 이어하기는 끕니다.
            if (continueButton != null) continueButton.SetActive(false);
            if (newGameButton != null) newGameButton.SetActive(true);
            if (hasUserData) { /* 나중에 저장된 유저 정보를 불러올 때 활용할 예정입니다 */ }
        }
    }


    /// <summary>
    /// 1. [새로운 모험] 버튼을 눌렀을 때
    /// </summary>
    public void OnClickNewGame()
    {
        Debug.Log("[TitleManager] 새로운 모험 시작 - 타이틀을 끄고 첫 이벤트 화면을 켭니다.");

        // 1. 준비해둔 첫 이벤트 화면 오브젝트를 화면에 켭니다!
        if (firstEventPanel != null)
        {
            firstEventPanel.SetActive(true);
        }

        // 2. 현재 켜져 있는 타이틀 화면 전체는 깔끔하게 꺼줍니다.
        gameObject.SetActive(false);
    }


    /// <summary>
    /// 2. [이어하기] 버튼을 눌렀을 때
    /// </summary>
    public void OnClickContinue()
    {
        Debug.Log("[Title] 기존 모험 이어하기! 마을 화면으로 화면 전환요청.");
        SceneGroupManager.Instance.ChangeScene("TownScene");
    }

    /// <summary>
    /// 3. [설정] 버튼을 눌렀을 때 - 화면 전환 없이 팝업창을 켭니다.
    /// </summary>
    public void OnClickOpenSettings()
    {
        Debug.Log("[Title] 설정 창 버튼 클릭 - 설정 팝업창을 띄웁니다.");
        if (settingsPopupPanel != null)
        {
            settingsPopupPanel.SetActive(true); // 숨겨뒀던 설정창 ON!
        }
    }

    /// <summary>
    /// 3-1. 설정 창 내부에 만들 [닫기(X)] 버튼에 연결할 함수입니다.
    /// </summary>
    public void OnClickCloseSettings()
    {
        Debug.Log("[Title] 설정 창 닫기 버튼 클릭 - 설정 팝업창을 끕니다.");
        if (settingsPopupPanel != null)
        {
            settingsPopupPanel.SetActive(false); // 설정창 OFF!
        }
    }

    /// <summary>
    /// 4. [휴식하기] 버튼을 눌렀을 때 (iOS 및 타 OS 분기)
    /// </summary>
    // [유니티 매뉴얼] 이 수정본 조각은 코루틴을 포함하여 주석 포함 총 55줄입니다.

    /// <summary>
    /// 4. [휴식하기] 버튼을 눌렀을 때 실행되는 함수 (OS별 처리 + 아이폰 페이드아웃 연출)
    /// </summary>
    public void OnClickQuitGame()
    {
        Debug.Log("[Title] '휴식하기' 선택 - 운영체제(OS) 환경을 검사합니다.");

        // 현재 게임이 실행 중인 플랫폼이 아이폰/아이패드(iOS)인지 판별합니다.
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("[Title] iOS 환경 감지: 안내 문구 출력 후 페이드아웃 효과를 시작합니다.");

            // 이미 글자가 사라지는 중일 수도 있으므로, 혹시 돌고 있을지 모르는 연출을 초기화합니다.
            StopAllCoroutines();

            // 글자가 서서히 사라지는 마법(코루틴)을 실행합니다.
            StartCoroutine(FadeOutIosNotice());
        }
        // [유니티 매뉴얼] 기존 주석과 로그를 완벽히 살리면서 수정하는 부분입니다.
        else
        {
            Debug.Log("[Title] PC / 안드로이드 환경 감지: 게임을 완전히 종료합니다.");
            // 실제로 빌드된 게임(PC, 모바일 앱)을 종료하는 유니티 명령어입니다.
            Application.Quit();

            // ---------------- [여기서부터 새로 추가하는 한 줄!] ----------------
            // 유니티 에디터로 테스트 중일 때는 상단의 재생(▶) 버튼을 자동으로 꺼줍니다.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            // ---------------- [여기까지 새로 추가하는 한 줄!] ----------------
        }
    }


    /// <summary>
    /// 아이폰용 안내 텍스트를 띄우고 서서히 투명하게 만들어 사라지게 하는 컴퓨터 전용 타이머 기능입니다.
    /// </summary>
    private System.Collections.IEnumerator FadeOutIosNotice()
    {
        if (iosNoticeText == null) yield break;

        // 1. 먼저 글자를 화면에 확실하게 켜고 원래 선명한 흰색(투명도 1)으로 채워줍니다.
        iosNoticeText.gameObject.SetActive(true);
        Color txtColor = iosNoticeText.color;
        txtColor.a = 1f;
        iosNoticeText.color = txtColor;
        iosNoticeText.text = "홈 버튼을 눌러 주십시오.";

        // 2. 글자가 선명하게 뜬 상태로 잠시 유저가 읽을 시간(예: 1.5초) 동안 가만히 기다립니다.
        yield return new WaitForSeconds(1.5f);

        // 3. 이제 1초 동안 실시간으로 투명도를 낮춰서 흐릿하게 만듭니다.
        float duration = 1.0f; // 사라지는 데 걸릴 시간
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            // 시간에 따라 투명도(Alpha)를 1에서 0으로 수학적으로 부드럽게 깎아내립니다.
            txtColor.a = Mathf.Lerp(1f, 0f, currentTime / duration);
            iosNoticeText.color = txtColor;

            // 다음 프레임(화면 갱신)까지 잠시 대기했다가 다시 연산합니다.
            yield return null;
        }

        // 4. 완전히 투명해졌으므로 오브젝트를 깔끔하게 꺼서 마무리합니다.
        iosNoticeText.gameObject.SetActive(false);
        Debug.Log("[Title] iOS 안내 문구 페이드아웃 연출이 안전하게 끝났습니다.");
    }
    // [유니티 매뉴얼] 이 코드 조각은 주석 포함 총 25줄입니다. 스크립트 맨 아래에 붙여넣으세요.

    /// <summary>
    /// 3-1. [타이틀 화면으로] (취소) 버튼을 눌렀을 때 실행될 함수
    /// 어떤 변동사항이 있어도 무시하고 저장하지 않은 채 팝업창을 끕니다.
    /// </summary>
    public void OnClickCancelSettings()
    {
        Debug.Log("[TitleManager] 취소 버튼 클릭 - 변경 사항을 저장하지 않고 팝업을 닫습니다.");

        if (settingsPopupPanel != null)
        {
            settingsPopupPanel.SetActive(false); // 팝업창 OFF
        }
    }

    /// <summary>
    /// 3-2. [저장] 버튼을 눌렀을 때 실행될 함수
    /// 변경값이 있다면 기기에 안전하게 기록(저장)하고 팝업창을 닫습니다.
    /// </summary>
    public void OnClickSaveSettings()
    {
        Debug.Log("[TitleManager] 저장 버튼 클릭 - 변경된 설정값을 기기에 영구 저장합니다.");

        // [선배의 팁] 나중에 슬라이더나 볼륨 조절을 넣으면 여기에 저장 명령을 적을 예정입니다.
        PlayerPrefs.Save(); // 디스크에 즉시 쓰기 완료 명령

        if (settingsPopupPanel != null)
        {
            settingsPopupPanel.SetActive(false); // 팝업창 OFF
        }
    }

}


