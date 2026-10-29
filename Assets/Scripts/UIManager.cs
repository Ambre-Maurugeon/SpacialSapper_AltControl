using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _defeatScreen;

    public static UIManager Instance { get; private set; }


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

    private void Start()
    {
        HideMouseCursor();
    }

    #endregion

    void HideMouseCursor()
    {
        Cursor.visible = false;
    }

    public void ActivateStartCanva()
    {
        _startScreen.SetActive(true);
        _defeatScreen.SetActive(false);
    }
    public void ActiveDefeatScreen()
    {
        _startScreen.SetActive(false);
        _defeatScreen.SetActive(true);
    }
    public void RemoveStartScreen()
    {
        _startScreen.SetActive(false);
    }

    public void DeactivateAllCanvas()
    {
        _startScreen.SetActive(false);
        _defeatScreen.SetActive(false);
    }

}
