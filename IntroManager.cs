using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroLoadingManager : MonoBehaviour
{
    // [유니티 매뉴얼] 이 코드는 주석 포함 총 95줄입니다.
    public Slider loadingSlider;
    public TextMeshProUGUI statusText;
    public GameObject clickPanel;

    [Header("인트로 터치 제어 설정 (직접 연결 방식)")]
    [Tooltip("인스펙터 체크박스를 제어할 인트로 클릭 감지 버튼 컴포넌트를 직접 넣어주세요.")]
    public Button introClickButton;


    [Header("로딩 속도 설정")]
    public float loadingSpeed = 0.5f;

    private float currentProgress = 0f;
    private bool isLoadingComplete = false;
    private bool hasUserData = false;

    // 어떤 문구 세트를 사용할지 결정하는 변수 (0, 1, 2 중 하나)
    private int textGroupIndex = 0;

    void Start()
    {
        // [서열 정리] 인트로 캔버스의 그리기 순서를 '100층'으로 높여 무조건 화면 맨 앞에 오게 만듭니다! 구조층
        if (TryGetComponent<Canvas>(out Canvas introCanvas)) { introCanvas.overrideSorting = true; introCanvas.sortingOrder = 100; }

        // [강제 활성화 기능 추가] 게임이 시작되면 이 스크립트가 붙은 오브젝트를 무조건 활성화합니다!
        gameObject.SetActive(true);
        loadingSlider.value = 0f;

        // [당신의 설계] 연결된 버튼의 Interactable 네모 박스 체크를 시작하자마자 완전히 해제(OFF)합니다!
        if (introClickButton != null) introClickButton.interactable = false;

        // [새로운 기능] 게임이 켜질 때 0, 1, 2 중 하나의 숫자를 무작위로 뽑습니다.
        textGroupIndex = Random.Range(0, 3);
        Debug.Log($"[IntroLoading] 이번 판에 선택된 로딩 문구 세트 번호: {textGroupIndex}번");

        CheckUserData();
        if (hasUserData) { /* 나중에 저장된 유저 정보를 불러올 때 활용할 예정입니다 */ }
    }

    // [유니티 매뉴얼] 이 수정본 조각은 총 31줄입니다.
    void Update()
    {
        // 1. 로딩이 아직 안 끝났다면 계속 로딩바를 채웁니다.
        if (!isLoadingComplete)
        {
            currentProgress += Time.deltaTime * loadingSpeed;
            loadingSlider.value = Mathf.Clamp01(currentProgress);

            // ---------------- [여기서부터 새로 끼워 넣는 코드] ----------------
            // 0%일 땐 흰색, 100%일 땐 녹색이 되도록 비율을 계산합니다.
            Color barColor = Color.Lerp(Color.white, Color.green, loadingSlider.value);

            // 로딩바 내부의 채워지는 이미지(fillRect)를 찾아서 색상을 입혀줍니다.
            if (loadingSlider.fillRect != null && loadingSlider.fillRect.TryGetComponent<Image>(out Image fillImage))
            {
                fillImage.color = barColor;
            }
            // ---------------- [여기까지 새로 끼워 넣는 코드] ----------------

            // 로딩 수치에 따라 무작위로 선택된 세트의 텍스트를 보여줍니다.
            UpdateStatusText(loadingSlider.value);

            if (currentProgress >= 1.0f)
            {
                CompleteLoading();
            }
        }
        // 2. [새로 추가된 기능] 로딩이 끝났다면 글자를 깜빡깜빡하게 만듭니다!
        else
        {
            if (statusText != null)
            {
                // 시간에 따라 0.3에서 1.0 사이를 부드럽게 오가는 수학 공식입니다.
                // 뒤의 '5f' 숫자를 높이면 더 빠르게 깜빡거립니다.
                float alpha = Mathf.PingPong(Time.time * 2f, 0.7f) + 0.3f;

                // 텍스트의 색상에서 투명도(a)만 실시간으로 조절합니다.
                Color textColor = statusText.color;
                textColor.a = alpha;
                statusText.color = textColor;
            }
        }
    }


    // [핵심 변경 구간] 3가지 루트 중 하나를 선택해 텍스트를 출력합니다.
    private void UpdateStatusText(float progress)
    {
        // 1번 루트: 커피 세트
        if (textGroupIndex == 0)
        {
            if (progress < 0.33f) statusText.text = "당신을 위해 물 끓이는 중...";
            else if (progress < 0.66f) statusText.text = "컵에 물을 붓는 중...";
            else if (progress < 1.0f) statusText.text = "커피를 타는 중...";
        }
        // 2번 루트: 데이터 및 시스템 세트 (요청하신 문구)
        else if (textGroupIndex == 1)
        {
            if (progress < 0.25f) statusText.text = "데이터를 불러오는 중...";
            else if (progress < 0.50f) statusText.text = "리소스 검사 중...";
            else if (progress < 0.75f) statusText.text = "세계관 생성 중...";
            else if (progress < 1.0f) statusText.text = "모험을 준비하는 중...";
        }
        // 3번 루트: 선배 추천 판타지 세트
        else if (textGroupIndex == 2)
        {
            if (progress < 0.33f) statusText.text = "동굴 속 괴물에게 밥 주는 중...";
            else if (progress < 0.66f) statusText.text = "보물 상자에 독니 덫 설치하는 중...";
            else if (progress < 1.0f) statusText.text = "물감을 섞어 새로운 색을 만드는 중...";
        }
    }

    private void CompleteLoading()
    {
        isLoadingComplete = true;
        statusText.text = "화면을 클릭해주세요.";

        // [최종 마감] '로딩 완료!' 로그가 찍히기 직전에, 꺼두었던 버튼 컴포넌트의 체크박스를 다시 켭니다(ON)!
        if (clickPanel != null && clickPanel.TryGetComponent<Button>(out Button introBtn))
        {
            introBtn.interactable = true;
        }

        Debug.Log("[IntroLoading] 로딩 완료!");
        // 2. [당신의 설계] 메시지가 나오자마자 독립된 방에 연결된 버튼의 체크박스를 다시 탁! 체크(ON)합니다.
        if (introClickButton != null)
        {
            introClickButton.interactable = true;
        }
    }



    private void CheckUserData()
    {
        Debug.Log("[IntroLoading] 데이터 검사 중...");
        if (PlayerPrefs.HasKey("HasSaveData"))
        {
            hasUserData = true;
            Debug.Log("[IntroLoading] 💾 기존 유저 데이터 발견!");
        }
        else
        {
            hasUserData = false;
            Debug.Log("[IntroLoading] 🆕 새로운 유저로 판단!");
        }
    }
    // [유니티 매뉴얼] 이 코드 조각은 주석 포함 총 20줄입니다.

    [Header("타이틀 전환 설정")]
    [Tooltip("인트로가 끝나고 켜질 타이틀 화면 오브젝트를 넣어주세요.")]
    public GameObject titleScreenObject;

    /// <summary>
    /// 로딩이 끝난 후 화면 전체(ClickPanel 등)를 눌렀을 때 실행할 함수입니다.
    /// </summary>
    // [유니티 매뉴얼] IntroManager.cs의 맨 아래 클릭 함수 수정 조각입니다.
    // [유니티 매뉴얼] IntroManager.cs의 맨 아래 클릭 함수 수정 조각입니다. (총 18줄)
    public void OnClickToTitle()
    {
        Debug.Log("[IntroManager] 화면 클릭 감지! 타이틀 화면을 활성화하고 인트로 화면 전체를 오프(OFF)합니다.");

        // [핵심 수정] 씬 이동 대신, 연결해둔 타이틀 화면 오브젝트를 직접 켭니다.
        if (titleScreenObject != null)
        {
            titleScreenObject.SetActive(true); // 타이틀 화면 ON!
        }

        // 이 스크립트가 붙어있는 인트로 화면 오브젝트 자신을 통째로 끕니다.
        gameObject.SetActive(false); // 인트로 화면 OFF!
    }



}
