using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BookContentControl: MonoBehaviour
{
    List<string> _bookContent;
    [SerializeField] TMP_Text _bookText;
    [SerializeField] TMP_Text _bookTitle;

    int _currentPage = 0;

    public void SetBook(string title, List<string> bookContent)
    {
        _bookTitle.text = title;
        _bookContent = bookContent;
        SetContent(0);
    }

    public void NextPage()
    {
        _currentPage = (_currentPage + 1) % _bookContent.Count;
        SetContent(_currentPage);
    }

    public void PreviousPage()
    {
        _currentPage = (_currentPage - 1 + _bookContent.Count) % _bookContent.Count;
        SetContent(_currentPage);
    }

    public void SetContent(int page)
    {
        _bookText.text = _bookContent[page];
    }
}
