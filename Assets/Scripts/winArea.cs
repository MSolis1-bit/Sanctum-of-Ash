using UnityEngine;

public class winArea : MonoBehaviour
{
    public static winArea instance;
    [SerializeField] GameObject closedDoorPic;
    [SerializeField] GameObject openDoorPic;
    private bool doorOpened;
    private bool playerInTrigger;
    void Start()
    {
        instance = this;
        doorOpened = false;
        closedDoorPic.SetActive(true);
        openDoorPic.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            if (doorOpened)
            {
                Debug.Log("player tried to win");
                GameManager.instance.PlayerWins();
            }
            else
            {
                return;
            }
        }
    }

    public void OpenExitDoor()
    {
        if (!doorOpened)
        {
            doorOpened = true;
            closedDoorPic.SetActive(false);
            openDoorPic.SetActive(true);
        }
    }
}
