using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public CarUpgrade upgradeToApply;
    public CarUpgradeManager upgradeManager;
    public Text costText;
    public int costIncrement = 750;
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip failClip;

    private string upgradeKey;
    private string costKey;
    private int currentCost;

    void Start()
    {
        string carID = upgradeManager.carID;
        upgradeKey = $"{carID}_{upgradeToApply.upgradeType}";
        costKey = $"{upgradeKey}_Cost";

        int upgradeCount = PlayerPrefs.GetInt(upgradeKey, 0);

        // Load local cost per car, defaulting to the base cost
        currentCost = PlayerPrefs.GetInt(costKey, upgradeToApply.cost);
        costText.text = $"${currentCost}";

        if (upgradeCount >= 10)
        {
            gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
        }
    }

    public void OnUpgradeButtonPressed()
    {
        if (!CurrencyManager.Instance.SpendMoney(currentCost))
        {
            Debug.Log($"Not enough money to upgrade {upgradeToApply.upgradeType} (cost: {currentCost})");
            if (audioSource && failClip)
                audioSource.PlayOneShot(failClip);
            return;
        }

        upgradeManager.ApplyUpgrade(upgradeToApply);

        int newCount = PlayerPrefs.GetInt(upgradeKey, 0);
        if (newCount >= 10)
        {
            gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
            return;
        }

        currentCost += costIncrement;
        PlayerPrefs.SetInt(costKey, currentCost);
        PlayerPrefs.Save();

        costText.text = $"${currentCost}";

        if (audioSource && successClip)
            audioSource.PlayOneShot(successClip);
    }
}
