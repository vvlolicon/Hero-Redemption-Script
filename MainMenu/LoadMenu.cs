using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour
{
    [SerializeField] GameObject SaveOptionPrefab;
    [SerializeField] RectTransform SaveOptionContainer;
    [SerializeField] GameObject SelectHightlighter;
    [SerializeField] Button LoadGameButton;
    List<SaveOption> SaveOptions = new();
    bool initialized = false;
    int selectedOptionIndex = -1;
    private void Start()
    {
        if (!initialized)
        {
            Initialize();
            initialized = true;
            SelectHightlighter.SetActive(false);
            LoadGameButton.interactable = false;
        }
    }
    public void Initialize()
    {
        string[] filePaths = SaveSystem.GetAllSavePaths(out List<string> fileNames);
        List<(string path, string name, DateTime lastModified)> saveFiles = new();

        for (int i = 0; i < filePaths.Length; i++)
        {
            string filePath = filePaths[i];
            string fileName = fileNames[i];
            DateTime lastModified = File.GetLastWriteTime(filePath);
            saveFiles.Add((filePath, fileName, lastModified));
        }

        // sort list with last modified time
        saveFiles.Sort((a, b) => b.lastModified.CompareTo(a.lastModified));
        SaveOptionContainer.sizeDelta = new Vector2(700f, 50f * saveFiles.Count + 10f);
        for (int i = 0; i < saveFiles.Count; i++)
        {
            string filePath = saveFiles[i].path;
            string fileName = saveFiles[i].name;
            string lastModified = saveFiles[i].lastModified.ToString("dd/MM/yyyy HH:mm:ss");
            GameObject saveOption = Instantiate(SaveOptionPrefab, SaveOptionContainer);
            SaveOption option = saveOption.GetComponent<SaveOption>();
            option.SetOption(filePath, fileName, lastModified);
            SaveOptions.Add(option);
            Button optionButton = saveOption.GetComponent<Button>();
            optionButton.onClick.AddListener(() => SelectOption(option));
        }
    }


    void SelectOption(SaveOption option)
    {
        LoadGameButton.interactable = true;
        if (selectedOptionIndex >= 0)
        {
            Button lastSelectOptionButton = SaveOptions[selectedOptionIndex].gameObject.GetComponent<Button>();
            lastSelectOptionButton.interactable = true;
        }
        selectedOptionIndex = SaveOptions.IndexOf(option);
        Button optionButton = option.gameObject.GetComponent<Button>();
        optionButton.interactable = false;
        SelectHightlighter.SetActive(true);
        SelectHightlighter.transform.position = option.transform.position;
    }

    public void OnClickLoad()
    {
        if(selectedOptionIndex >= 0)
        {
            string filePath = SaveOptions[selectedOptionIndex].filePath;
            MainMenuController.Instance.OnLoadGame(filePath);
        }
    }

    public void OnClickExit()
    {
        LoadGameButton.interactable = false;
        if (selectedOptionIndex >= 0)
        {
            Button lastSelectOptionButton = SaveOptions[selectedOptionIndex].gameObject.GetComponent<Button>();
            lastSelectOptionButton.interactable = true;
        }
        selectedOptionIndex = -1;
        SelectHightlighter.SetActive(false);
        gameObject.SetActive(false);
    }
}
