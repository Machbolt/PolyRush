using UnityEngine;

public class CarCustomizer : MonoBehaviour
{
    [Header("References")]
    public Transform bodyParent;

    [Tooltip("Unique PlayerPrefs key for this car")]
    public string colorPrefKey = "SelectedCarColor_Player";

    private GameObject[] colorVariants;
    private string[] colorNames;

    void Awake()
    {
        int count = bodyParent.childCount;
        colorVariants = new GameObject[count];
        colorNames = new string[count];

        string initiallyActiveColor = null;

        for (int i = 0; i < count; i++)
        {
            Transform child = bodyParent.GetChild(i);
            colorVariants[i] = child.gameObject;
            colorNames[i] = child.name;

            if (child.gameObject.activeSelf)
            {
                initiallyActiveColor = child.name;
            }
        }

        // If there's a saved color AND we want to apply it, use it
        string savedColor = PlayerPrefs.GetString(colorPrefKey, null);
        if (!string.IsNullOrEmpty(savedColor))
        {
            SetCarColor(savedColor);
        }
        else if (!string.IsNullOrEmpty(initiallyActiveColor))
        {
            // Save whatever was active by default
            PlayerPrefs.SetString(colorPrefKey, initiallyActiveColor);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning($"No active body color found for car {name}.");
        }
    }

    public void SetCarColor(string colorName)
    {
        for (int i = 0; i < colorVariants.Length; i++)
        {
            bool isMatch = colorVariants[i].name == colorName;
            colorVariants[i].SetActive(isMatch);
        }

        PlayerPrefs.SetString(colorPrefKey, colorName);
        PlayerPrefs.Save();
    }
}
