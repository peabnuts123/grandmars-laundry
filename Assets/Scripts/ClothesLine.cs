using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesLine : MonoBehaviour
{
    public static ClothesLine _instance;

    public AudioClip hangSound;
    public AudioClip selectSlotSound;

    private bool _isSuspended;
    private LaundrySlot[] laundrySlots;

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

    public ClothesLine()
    {
        // Singleton
        if (ClothesLine._instance == null)
        {
            ClothesLine._instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        laundrySlots = GetComponentsInChildren<LaundrySlot>();

        if (laundrySlots.Length == 0)
        {
            throw new InvalidProgramException("Cannot start ClothesLine, no laundry slots specified");
        }
    }

    public void Init()
    {
        foreach (LaundrySlot slot in this.laundrySlots)
        {
            DestroyImmediate(slot.TakeItem());
        }

        Reset();
    }

    void Reset()
    {
        int firstAvailableSlot = -1;
        int index = 0;
        foreach (LaundrySlot slot in this.laundrySlots)
        {
            slot.Reset();
            if (firstAvailableSlot == -1 && !slot.HasItem())
            {
                firstAvailableSlot = index;
            }

            index++;
        }

        if (firstAvailableSlot != -1)
        {
            SelectSlot(firstAvailableSlot);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isSuspended)
        {
            return;
        }

        // Keyboard control selected slot
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OffsetSelectedSlotWithinBounds(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OffsetSelectedSlotWithinBounds(1);
        }

        // Mouse control selected slot
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int index = 0;
        foreach (LaundrySlot slot in this.laundrySlots)
        {
            slot.isHighlighted = slot.ContainsWorldPoint(mouseWorldPoint);

            if (Input.GetMouseButtonUp(0) && slot.isHighlighted)
            {
                SelectSlot(index);
            }

            index++;
        }
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= this.laundrySlots.Length)
        {
            throw new ArgumentOutOfRangeException("Laundry Slot index " + index + " is out of range");
        }

        LaundrySlot slot;
        for (int i = 0; i < this.laundrySlots.Length; i++)
        {
            slot = this.laundrySlots[i];
            slot.isSelected = i == index;
        }

        GameManager._instance.PlayAudioClipWithRandomPitch(selectSlotSound);
    }

    void OffsetSelectedSlotWithinBounds(int offset)
    {
        int selectedSlotIndex = GetCurrentlySelectedSlotIndex();
        int newSelectedSlotIndex = selectedSlotIndex + offset;

        if (newSelectedSlotIndex < 0 || newSelectedSlotIndex >= this.laundrySlots.Length)
        {
            return;
        }

        SelectSlot(newSelectedSlotIndex);
    }

    int GetCurrentlySelectedSlotIndex()
    {
        for (int i = 0; i < this.laundrySlots.Length; i++)
        {
            if (this.laundrySlots[i].isSelected)
            {
                return i;
            }
        }

        throw new InvalidOperationException("Impossible error – no Slot is selected");
    }

    LaundrySlot GetCurrentlySelectedLaundrySlot()
    {
        return this.laundrySlots[GetCurrentlySelectedSlotIndex()];
    }

    public void HangUpLaundryItem(GameObject laundryItem)
    {
        LaundrySlot selectedLaundrySlot = GetCurrentlySelectedLaundrySlot();

        // Negged?
        if (selectedLaundrySlot.HasItem())
        {
            return;
        }

        // Hang it up bro
        selectedLaundrySlot.GiveItem(laundryItem);
        GameManager._instance.PlayAudioClipWithRandomPitch(this.hangSound);
        GameManager._instance.UpdateDisplays();

        // Move cursor along AF
        OffsetSelectedSlotWithinBounds(1);

        // Evaluate Uno statuses
        EvaluateUno();

        // Evaluate if the line is full
        EvaluateLineIsFull();
    }

    // This needs documenting
    void EvaluateUno()
    {
        if (this.laundrySlots.Length < 2)
        {
            return;
        }

        foreach (LaundrySlot slot in this.laundrySlots)
        {
            slot.canUno = true;
        }

        Laundry leftState, rightState;
        LaundrySlot leftSlot, rightSlot;
        for (int i = 0; i < this.laundrySlots.Length - 1; i++)
        {
            leftSlot = this.laundrySlots[i];
            rightSlot = this.laundrySlots[i + 1];


            if (!leftSlot.HasItem() && rightSlot.HasItem())
            {
                rightSlot.canUno = false;
            }

            if (!leftSlot.HasItem() || !rightSlot.HasItem())
            {
                continue;
            }

            leftState = leftSlot.GetItem().GetComponent<Laundry>();
            rightState = rightSlot.GetItem().GetComponent<Laundry>();

            if ((leftState.color == rightState.color ||
                leftState.type == rightState.type) &&
                rightSlot.canUno &&
                leftSlot.canUno)
            {
                // Match, set both to Uno
                leftSlot.isUno = rightSlot.isUno = true;
            }
            else
            {
                this.laundrySlots[i + 1].canUno = false;
            }
        }
    }

    void EvaluateLineIsFull()
    {
        bool isNotFull = false;
        foreach (LaundrySlot slot in this.laundrySlots)
        {
            if (!slot.HasItem())
            {
                // Slot is empty, line is not full
                isNotFull = true;
                break;
            }
        }

        if (isNotFull)
        {
            // Do nothing
            return;
        }
        else
        {
            // Get Uno'd objects
            IList<Laundry> unoObjects = new List<Laundry>();
            IList<LaundrySlot> bankingSlots = new List<LaundrySlot>();
            foreach (LaundrySlot slot in this.laundrySlots)
            {
                if (slot.isUno)
                {
                    unoObjects.Add(slot.GetItem().GetComponent<Laundry>());
                    bankingSlots.Add(slot);
                }
            }

            // Check for loss
            if (unoObjects.Count == 0 && GameManager._instance.GetTotalNumberOfCardsInPlay() > 0)
            {
                GameManager._instance.LevelFail();
                return;
            }

            // Bank items with Game Manager
            GameManager._instance.BankItems(unoObjects);

            foreach (LaundrySlot slot in bankingSlots)
            {
                DestroyImmediate(slot.TakeItem());
            }

            // Reset Clothesline
            Reset();
        }
    }
}
