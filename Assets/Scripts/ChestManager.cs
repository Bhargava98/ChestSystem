using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class ChestManager : MonoBehaviour
{
    public static ChestManager instance;

    public Transform chestSlotContainer;
    public GameObject chestSlotPrefab;
    public List<ChestType> availableChests;
    public List<Transform> slots;

    public Button AddChestBtn;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadChests();
    }

    public void AddRandomChest()
    {
        if (chestSlotContainer.childCount < 4)
        {
            ChestType randomChest = availableChests[UnityEngine.Random.Range(0, availableChests.Count)];
            GameObject newSlot = Instantiate(chestSlotPrefab, chestSlotContainer);
            newSlot.transform.position = slots[chestSlotContainer.childCount - 1].position;

            ChestSlot chestSlot = newSlot.GetComponent<ChestSlot>();
            chestSlot.Initialize(randomChest, "Locked", DateTime.MinValue, 0);
            AnimateSlotAppearance(newSlot.transform);
            SaveChests(chestSlot);
        }
        else
        {
            AddChestBtn.interactable = false;
        }
    }

    private void AnimateSlotAppearance(Transform slot)
    {
        slot.localScale = Vector3.zero;
        slot.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    public void SaveChests(ChestSlot chestSlot)
    {
        PlayerPrefs.SetInt("ChestCount", chestSlotContainer.childCount);
        for (int i = 0; i < chestSlotContainer.childCount; i++)
        {
            chestSlot = chestSlotContainer.GetChild(i).GetComponent<ChestSlot>();
            PlayerPrefs.SetString($"Chest_{i}_Type", chestSlot.chestType.chestName);
            PlayerPrefs.SetString($"Chest_{i}_Status", chestSlot.GetStatus());
            PlayerPrefs.SetString($"Chest_{i}_EndTime", chestSlot.unlockEndTime.ToString());
        }
        PlayerPrefs.Save();
    }

    private void LoadChests()
    {
        int chestCount = PlayerPrefs.GetInt("ChestCount", 0);
        for (int i = 0; i < chestCount; i++)
        {
            string chestName = PlayerPrefs.GetString($"Chest_{i}_Type", "");
            string status = PlayerPrefs.GetString($"Chest_{i}_Status", "Locked");
            string endTimeString = PlayerPrefs.GetString($"Chest_{i}_EndTime", "");

            if (!string.IsNullOrEmpty(chestName))
            {
                ChestType chestType = availableChests.Find(chest => chest.chestName == chestName);
                GameObject newSlot = Instantiate(chestSlotPrefab, chestSlotContainer);
                newSlot.transform.position = slots[i].position;

                ChestSlot chestSlot = newSlot.GetComponent<ChestSlot>();
                DateTime unlockEndTime = string.IsNullOrEmpty(endTimeString) ? DateTime.MinValue : DateTime.Parse(endTimeString);
                chestSlot.Initialize(chestType, status, unlockEndTime, i);

                if (status == "Unlocking" && DateTime.Now >= unlockEndTime)
                {
                    chestSlot.UnlockComplete();
                }
                else if (status == "Unlocking" && DateTime.Now < unlockEndTime)
                {
                    chestSlot.StartCoroutine(chestSlot.TrackTime());
                }
            }
        }
    }
}
