using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KRG
{
    public class UIMenu : MonoBehaviour
    {
        // EVENTS

        public event ItemSelectionHandler ItemSelectionChanged;

        // DELEGATES

        public delegate void ItemSelectionHandler(int newItemIndex);

        // SERIALIZED FIELDS

        public ScrollRect ScrollView;
        public GameObject MenuItem;
        public Vector3 ItemOffset;
        public bool AllowNoSelectedItem;

        // PRIVATE FIELDS

        private List<Item> m_Items = new List<Item>();
        private int m_PrevSelectedItemIndex = -1;

        // STRUCTS

        public struct Item
        {
            public string Key;
            public string Text;
            public UnityAction OnClick;
            public GameObject MenuItem;
        }

        // PROPERTIES

        public int ItemCount => m_Items.Count;

        public Item SelectedItem
        {
            get
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    Item item = m_Items[i];
                    if (item.MenuItem == EventSystem.current.currentSelectedGameObject)
                    {
                        return item;
                    }
                }
                return new Item();
            }
        }

        public int SelectedItemIndex
        {
            get
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    Item item = m_Items[i];
                    if (item.MenuItem == EventSystem.current.currentSelectedGameObject)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        // MONOBEHAVIOUR METHODS

        private void Update()
        {
            if (!AllowNoSelectedItem)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    SelectDefaultItem();
                }
            }
            int i = SelectedItemIndex;
            if (i != m_PrevSelectedItemIndex)
            {
                if (ScrollView != null && ScrollView.vertical)
                {
                    ScrollView.verticalNormalizedPosition = Mathf.InverseLerp(m_Items.Count - 1, 0, i);
                }
                ItemSelectionChanged?.Invoke(i);
                m_PrevSelectedItemIndex = i;
            }
        }

        // PUBLIC CUSTOM METHODS

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
            AddItem(null, text, onClick);
        }

        public void AddItem(string key, string text, UnityAction onClick)
        {
            GameObject menuItem;
            if (m_Items.Count == 0)
            {
                menuItem = MenuItem;
                ScrollView.content.sizeDelta = Vector2.zero;
            }
            else
            {
                menuItem = Instantiate(MenuItem, MenuItem.transform.parent);
                RectTransform rt = menuItem.GetComponent<RectTransform>();
                rt.localPosition += ItemOffset * m_Items.Count;

                // we need to set the new content size so the scrolling works properly
                if (ScrollView.vertical)
                {
                    const float referenceHeight = 1080; // TODO: get this dynamically from canvas
                    float y = Mathf.Abs(rt.localPosition.y) + rt.sizeDelta.y - referenceHeight;
                    ScrollView.content.sizeDelta = ScrollView.content.sizeDelta.SetY(y);
                }
                else
                {
                    ScrollView.content.sizeDelta = rt.localPosition.ToVector2().Abs() + rt.sizeDelta;
                }
            }

            menuItem.name = key ?? text;
            menuItem.GetComponentInChildren<TextMeshProUGUI>().text = text;

            Button button = menuItem.GetComponent<Button>();
            Button.ButtonClickedEvent clickedEvent = button.onClick;
            clickedEvent.RemoveAllListeners();
            clickedEvent.AddListener(onClick);

            m_Items.Add(new Item
            {
                Key = key,
                Text = text,
                OnClick = onClick,
                MenuItem = menuItem,
            });
        }

        public void RenameItem(int itemIndex, string text)
        {
            Item item = m_Items[itemIndex];
            item.Text = text;
            item.MenuItem.name = item.Key ?? text;
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