using UnityEngine;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int currentMoney = 0;
    private Text moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadCurrency();
        TryFindUI();
        UpdateUI();
    }
    private void Update()
    {
        if (moneyText == null)
        {
            TryFindUI();
            if (moneyText != null)
                UpdateUI();
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        TryFindUI();
        UpdateUI();
    }

    public void SetMoneyText(Text newText)
    {
        moneyText = newText;
        UpdateUI();
    }

    private void TryFindUI()
    {
        moneyText = GameObject.FindWithTag("MoneyText")?.GetComponent<Text>();
    }
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        SaveCurrency();
        UpdateUI();
    }
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            SaveCurrency();
            UpdateUI();
            return true;
        }

        return false;
    }
    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"$ {currentMoney}";
        }
    }
    private void SaveCurrency()
    {
        PlayerPrefs.SetInt("PlayerMoney", currentMoney);
    }

    private void LoadCurrency()
    {
        currentMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
    }
}
