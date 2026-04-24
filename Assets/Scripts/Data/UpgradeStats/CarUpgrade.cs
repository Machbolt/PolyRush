using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Racing/Car Upgrade")]
public class CarUpgrade : ScriptableObject
{
    public CarUpgradeType upgradeType;
    public float upgradeAmount;
    public int cost;
    public string description;
}
