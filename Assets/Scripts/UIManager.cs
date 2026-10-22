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
    #endregion


    public void RemoveStartScreen()
    {
        _startScreen.SetActive(false);
    }

    public void ActiveDefeatScreen()
    {
        _defeatScreen.SetActive(true);
    }
}
