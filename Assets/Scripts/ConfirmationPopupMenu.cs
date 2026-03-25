using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopupMenu : MonoBehaviour
{
    [Header("Components: ")]
    [SerializeField] TextMeshProUGUI displayText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void ActivateMenu(string displayText, UnityAction confirmAction, UnityAction cancelAction)
    {
        this.gameObject.SetActive(true);

        // Set the display text
        this.displayText.text = displayText;

        // Remove any existing listeners just to make sure there aren't any previous ones hanging around
        // Note - this only removes listeners added through code
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        // Assign the onclick listeners
        confirmButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            confirmAction();
        });

        cancelButton.onClick.AddListener(() =>
        {
            DeactivateMenu();
            cancelAction();
        });

        void DeactivateMenu() 
        {
            this.gameObject.SetActive(false);
        }
    }
}
