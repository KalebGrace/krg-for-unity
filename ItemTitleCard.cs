using TMPro;
using UnityEngine;

namespace _0G.Legacy
{
    public class ItemTitleCard : MonoBehaviour
    {
        private const int PAUSE_KEY = 100;
        private const int WAIT_TIME = 3;

        public TextMeshProUGUI itemDisplayNameText;
        public TextMeshProUGUI itemInstructionText;

        private CanvasGroup canvasGroup;

        private TimeThread ttApplication;
        private TimeThread ttGameplay;
        private TimeThread ttField;

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

            G.inv.ItemAcquired += OnItemAcquired;
        }

        private void OnItemAcquired(ItemData itemData, GameObjectBody owner, bool isNewlyAcquired)
        {
            if (itemData == null)
            {
                G.U.Warn("Missing item data.");
                return;
            }

            if (!isNewlyAcquired || !itemData.ShowTitleCardOnCollect) return;

            itemDisplayNameText.text = itemData.DisplayName;
            itemInstructionText.text = itemData.Instruction;

            Show();

            ttGameplay.QueuePause(PAUSE_KEY);
            ttField.QueuePause(PAUSE_KEY);

            ttApplication.AddTrigger(WAIT_TIME, OnWaitDone);
        }

        private void OnWaitDone(TimeTrigger tt)
        {
            ttField.QueueUnpause(PAUSE_KEY, Hide);
            ttGameplay.QueueUnpause(PAUSE_KEY);
        }

        private void OnDestroy()
        {
            G.inv.ItemAcquired -= OnItemAcquired;
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