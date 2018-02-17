using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand _instance;

    private HandSlot[] handSlots;
    private bool _isSuspended;

    public bool isSuspended
    {
        get
        {
            return this._isSuspended;
        }
        set
        {
            this._isSuspended = value;
        }
    }

    public PlayerHand()
    {
        // Singleton
        if (PlayerHand._instance == null)
        {
            PlayerHand._instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void OnEnable()
    {
        this.handSlots = GetComponentsInChildren<HandSlot>();
        int index = 1;
        foreach (HandSlot slot in this.handSlots)
        {
            Text text = slot.GetComponentInChildren<Text>();
            text.text = "" + index++;
        }
    }

    public void Init()
    {
        DiscardHand();
        Deal();
    }

    public void Deal()
    {
        IEnumerable<HandSlot> emptyHandSlots = GetEmptyHandSlots();
        foreach (HandSlot slot in emptyHandSlots)
        {
            if (GameManager._instance.IsDeckEmpty())
            {
                // Stop dealing if the deck is now empty
                break;
            }

            LaundryItem dealtItem = GameManager._instance.RequestLaundryItemFromDeck();
            dealtItem.CreateInstance(slot.transform);
        }
    }

    void DiscardHand()
    {
        foreach (HandSlot slot in this.handSlots)
        {
            if (slot.HasObject())
            {
                DestroyImmediate(slot.GetObject());
            }
        }
    }

    public int GetNumberOfCardsInHand()
    {
        int count = 0;
        foreach (HandSlot slot in this.handSlots)
        {
            if (slot.HasObject())
            {
                count++;
            }
        }

        return count;
    }

    IEnumerable<HandSlot> GetEmptyHandSlots()
    {
        List<HandSlot> emptyHandSlots = new List<HandSlot>();
        foreach (HandSlot slot in this.handSlots)
        {
            if (!slot.HasObject())
            {
                emptyHandSlots.Add(slot);
            }
        }

        return emptyHandSlots;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isSuspended)
        {
            return;
        }

        // Keyboard placing of items on line
        for (int i = 1; i <= Mathf.Min(this.handSlots.Length, 9); i++)
        {
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), "Alpha" + (i));
            if (Input.GetKeyDown(keyCode))
            {
                PlaySlot(i - 1);
            }
        }

        // Mouse placing of items on line
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int index = 0;
        foreach (HandSlot slot in this.handSlots)
        {
            slot.isHighlighted = slot.ContainsWorldPoint(mouseWorldPoint);

            if (Input.GetMouseButtonUp(0) && slot.isHighlighted)
            {
                PlaySlot(index);
            }

            index++;
        }
    }

    void PlaySlot(int slotIndex)
    {
        HandSlot handSlotAtIndex = this.handSlots[slotIndex];
        if (!handSlotAtIndex.HasObject())
        {
            return;
        }

        ClothesLine._instance.HangUpLaundryItem(handSlotAtIndex.GetObject());

        if (!this.isSuspended && GameManager._instance.AreNoCardsLeft())
        {
            // Game hasn't ended, but no cards left in play
            // There are cards on the Clothesline
            GameManager._instance.LevelSuccess();
        }
    }
}
