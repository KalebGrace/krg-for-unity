using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KRG
{
    public class UIMenu : MonoBehaviour
    {
        // SERIALIZED FIELDS

        public ScrollRect ScrollView;
        public GameObject MenuItem;
        public Vector3 ItemOffset;

        // PRIVATE FIELDS

        private List<Item> m_Items = new List<Item>();

        // STRUCTS

        public struct Item
        {
            public string Text;
            public UnityAction OnClick;
            public GameObject MenuItem;
        }

        // PUBLIC METHODS

        public void Clear()
        {
            foreach (Item item in m_Items)
            {
                GameObject menuItem = item.MenuItem;
                if (menuItem == MenuItem)
                {
                    menuItem.name = "Menu Item";
                    menuItem.GetComponentInChildren<TextMeshProUGUI>().text = "Menu Item";

                    Button button = menuItem.GetComponent<Button>();
                    Button.ButtonClickedEvent clickedEvent = button.onClick;
                    clickedEvent.RemoveAllListeners();
                }
                else
                {
                    Destroy(menuItem);
                }
            }

            m_Items.Clear();
        }

        public void AddItem(string text, UnityAction onClick)
        {
            GameObject menuItem;
            if (m_Items.Count == 0)
            {
                menuItem = MenuItem;
            }
            else
            {
                menuItem = Instantiate(MenuItem, MenuItem.transform.parent);
                RectTransform rt = menuItem.GetComponent<RectTransform>();
                rt.localPosition += ItemOffset * m_Items.Count;
            }

            menuItem.name = text;
            menuItem.GetComponentInChildren<TextMeshProUGUI>().text = text;

            Button button = menuItem.GetComponent<Button>();
            Button.ButtonClickedEvent clickedEvent = button.onClick;
            clickedEvent.RemoveAllListeners();
            clickedEvent.AddListener(onClick);

            m_Items.Add(new Item { Text = text, OnClick = onClick, MenuItem = menuItem });
        }

        public void RenameItem(int itemIndex, string text)
        {
            Item item = m_Items[itemIndex];
            item.Text = text;
            item.MenuItem.name = text;
            item.MenuItem.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

        public void SelectDefaultItem()
        {
            ISelectSilently iss = MenuItem.GetComponent<ISelectSilently>();
            if (iss != null)
            {
                iss.SelectSilently();
            }
            else
            {
                MenuItem.GetComponent<Button>().Select();
            }
        }
    }
}