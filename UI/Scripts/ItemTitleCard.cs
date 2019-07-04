using UnityEngine;
using TMPro;

namespace KRG
{
    public class ItemTitleCard : MonoBehaviour
    {
        private const int PAUSE_KEY = 100;
        private const int WAIT_TIME = 3;

        public GameplayHudElement gameplayHudElement;

        public TextMeshProUGUI itemDisplayNameText;
        public TextMeshProUGUI itemInstructionText;

        private ITimeThread ttApplication;
        private ITimeThread ttGameplay;
        private ITimeThread ttField;

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

            itemDisplayNameText.text = id.DisplayName;
            itemInstructionText.text = id.Instruction;

            gameplayHudElement.Show();

            ttGameplay.QueuePause(PAUSE_KEY);
            ttField.QueuePause(PAUSE_KEY);

            ttApplication.AddTrigger(WAIT_TIME, OnWaitDone);
        }

        private void OnWaitDone(TimeTrigger tt)
        {
            ttField.QueueUnpause(PAUSE_KEY, gameplayHudElement.Hide);
            ttGameplay.QueueUnpause(PAUSE_KEY);
        }

        private void OnDestroy()
        {
            G.inv.KeyItemAcquired -= OnItemAcquired;
        }
    }
}
