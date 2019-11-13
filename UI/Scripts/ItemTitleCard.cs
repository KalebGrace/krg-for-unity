using UnityEngine;
using TMPro;

namespace KRG
{
    public class ItemTitleCard : MonoBehaviour
    {
        private const int PAUSE_KEY = 100;
        private const int WAIT_TIME = 3;

        public TextMeshProUGUI itemDisplayNameText;
        public TextMeshProUGUI itemInstructionText;

        [AudioEvent]
        public string sfxFmodEventOnShow;

        private CanvasGroup canvasGroup;

        private ITimeThread ttApplication;
        private ITimeThread ttGameplay;
        private ITimeThread ttField;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }

        private void Start()
        {
            ttApplication = G.time.GetTimeThread(TimeThreadInstance.Application);
            ttGameplay = G.time.GetTimeThread(TimeThreadInstance.Gameplay);
            ttField = G.time.GetTimeThread(TimeThreadInstance.Field);

            G.inv.KeyItemAcquired += OnItemAcquired;
        }

        private void OnItemAcquired(int acquiredItem)
        {
            ItemData id = G.inv.GetItemData(acquiredItem);

            if (!id.ShowCardOnAcquire) return;

            itemDisplayNameText.text = id.DisplayName;
            itemInstructionText.text = id.Instruction;

            Show();

            ttGameplay.QueuePause(PAUSE_KEY);
            ttField.QueuePause(PAUSE_KEY);

            ttApplication.AddTrigger(WAIT_TIME, OnWaitDone);

            if (!string.IsNullOrEmpty(sfxFmodEventOnShow))
            {
                G.audio.PlaySFX(sfxFmodEventOnShow);
            }
        }

        private void OnWaitDone(TimeTrigger tt)
        {
            ttField.QueueUnpause(PAUSE_KEY, Hide);
            ttGameplay.QueueUnpause(PAUSE_KEY);
        }

        private void OnDestroy()
        {
            G.inv.KeyItemAcquired -= OnItemAcquired;
        }

        private void Hide()
        {
            canvasGroup.alpha = 0;
        }

        private void Show()
        {
            canvasGroup.alpha = 1;
        }
    }
}
