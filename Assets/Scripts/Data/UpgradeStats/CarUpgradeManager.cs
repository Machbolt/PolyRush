using UnityEngine;
using System.Collections.Generic;

public class CarUpgradeManager : MonoBehaviour
{
    public CarStatsData carStats;
    public string carID = "Car1";

    private Dictionary<CarUpgradeType, int> upgradeCounts = new Dictionary<CarUpgradeType, int>();

    private void Awake()
    {
        // PlayerPrefs.DeleteAll();
        LoadUpgradeCounts();
    }

    public void ApplyUpgrade(CarUpgrade upgrade)
    {
        CarUpgradeType type = upgrade.upgradeType;

        if (!upgradeCounts.ContainsKey(type))
            upgradeCounts[type] = 0;

        if (upgradeCounts[type] >= 10)
        {
            Debug.Log($"Max upgrades reached for {type}");
            return;
        }

        // Apply upgrade
        switch (type)
        {
            case CarUpgradeType.TopSpeed:
                carStats.topSpeed += upgrade.upgradeAmount;
                break;
            case CarUpgradeType.Acceleration:
                carStats.accelerationPower += upgrade.upgradeAmount;
                break;
            case CarUpgradeType.SteerAngle:
                carStats.maxSteerAngle += upgrade.upgradeAmount;
                break;
            case CarUpgradeType.BrakeStrength:
                carStats.brakeStrength += upgrade.upgradeAmount;
                break;
        }
        
        upgradeCounts[type]++;
        SaveUpgradeCount(type);

        if (upgradeCounts[type] >= 10)
        {
            UpgradeButton[] allButtons = FindObjectsOfType<UpgradeButton>();
            foreach (UpgradeButton btn in allButtons)
            {
                if (btn.upgradeToApply.upgradeType == type && btn.upgradeManager == this)
                {
                    btn.gameObject.SetActive(false);
                }
            }
        }

    }

    private void SaveUpgradeCount(CarUpgradeType type)
    {
        string key = $"{carID}_{type}";
        PlayerPrefs.SetInt(key, upgradeCounts[type]);
        PlayerPrefs.Save();
    }

    private void LoadUpgradeCounts()
    {
        foreach (CarUpgradeType type in System.Enum.GetValues(typeof(CarUpgradeType)))
        {
            string key = $"{carID}_{type}";
            upgradeCounts[type] = PlayerPrefs.GetInt(key, 0); // Default to 0
        }
    }
}
