
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Arduino Reload")]
    [SerializeField] float _arduinoReloadTimer; // values re
    private float _lastArduinoReload;

    public GAME_STATE State => _state;

    [Header("Debug")]
    [SerializeField]
    private GAME_STATE _state = GAME_STATE.HUB;


    #region Main
    private void Awake()
    {
        // -- INSTANCE --
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        //DontDestroyOnLoad(this.gameObject); // can be edit later
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitWithOthers();
        InitMyState();

        AudioManager.Instance.PlayMusic(SoundType.Music);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region Init
    private void InitWithOthers()
    {
        // link inputs
        PlayerController.OnKeyboardReload += OnReload;
        PlayerController.OnArduinoReload += OnArduinoReload;

    }

    private void InitMyState()
    {
        switch (_state)
        {
            case GAME_STATE.HUB:
                UIManager.Instance.ActivateStartCanva();
                break;
            case GAME_STATE.TUTO:
                LaunchTuto();
                break;
            case GAME_STATE.PLAYING:
                UIManager.Instance.DeactivateAllCanvas();
                LaunchGame();
                break;
            case GAME_STATE.DEFEAT:
                LaunchDefeat();
                break;
        }
    }

    #endregion

    private void OnReload()
    {
        WaterReloadBehaviour();
    }

    void OnArduinoReload()
    {
        if (Time.time < _lastArduinoReload + _arduinoReloadTimer) return;
        _lastArduinoReload = Time.time;

        WaterReloadBehaviour();
    }

    private void WaterReloadBehaviour()
    {
        switch (_state)
        {
            case GAME_STATE.HUB:
                LaunchTuto();
                break;
            case GAME_STATE.DEFEAT:
                Restart();
                break;
            default:
                break;
        }
    }

    public void ChangeState(GAME_STATE state)
    {
        _state = state;
    }

    private void Restart()
    {
        ChangeState(GAME_STATE.HUB);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Call after first reload
    private void LaunchTuto()
    {
        UIManager.Instance.DeactivateAllCanvas();
        ChangeState(GAME_STATE.TUTO);
    }

    // Call by the first destoyed fire controller
    public void LaunchGame()
    {
        ChangeState(GAME_STATE.PLAYING);
        WavesManager.Instance.StartCoroutine("StartWaves");
    }

    // Call at the end of all waves or when planet dies  
    public void LaunchDefeat()
    {
        ChangeState(GAME_STATE.DEFEAT);
        UIManager.Instance.ActiveDefeatScreen();
    }
}
