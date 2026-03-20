using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileID;

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI saveDateText;

    public void SetData(GameData data)
    {
        // There's no data for this profileID
        if(data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        // There's data for this profileID
        else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

        }
    }

    public string GetProfileID()
    {
        return this.profileID;
    }
}
