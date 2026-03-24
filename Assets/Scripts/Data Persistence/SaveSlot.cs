using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile: ")]
    [SerializeField] private string profileID;

    [Header("Content: ")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI saveDateText;
    [SerializeField] private Button deleteDataButton;

    private Button saveSlotButton;

    public bool hasData { get; private set; } = false;

    private void Awake()
    {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        // There's no data for this profileID
        if(data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            deleteDataButton.gameObject.SetActive(false);
        }
        // There's data for this profileID
        else
        {
            hasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            saveDateText.text = "Saved: " + data.timeStamp;
            deleteDataButton.gameObject.SetActive(true);
        }
    }

    public string GetProfileID()
    {
        return this.profileID;
    }

    public void SetInteractable(bool interactable) 
    {
        saveSlotButton.interactable = interactable;
        deleteDataButton.interactable= interactable;
    }
}
