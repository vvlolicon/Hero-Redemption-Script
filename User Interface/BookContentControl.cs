
// BookContentControl.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BookContentControl : MonoBehaviour
{
    [SerializeField] TMP_Text _bookText;
    [SerializeField] TMP_Text _bookHeading;
    [SerializeField] Image _bookImage;

    List<string> _titles;
    List<string> _contents;
    List<Sprite> _images;
    int _currentPage = 0;

    public void SetBook(List<string> titles, List<string> contents, List<Sprite> images)
    {
        _titles = titles;
        _contents = contents;
        _images = images;
        _currentPage = 0;
        UpdatePage();
    }

    public void NextPage()
    {
        if (_contents != null && _contents.Count > 0)
        {
            _currentPage = (_currentPage + 1) % _contents.Count;
            UpdatePage();
        }
    }

    public void PreviousPage()
    {
        if (_contents != null && _contents.Count > 0)
        {
            _currentPage = (_currentPage - 1 + _contents.Count) % _contents.Count;
            UpdatePage();
        }
    }

    void UpdatePage()
    {
        _bookText.text = _contents[_currentPage];
        _bookHeading.text = _titles[_currentPage];
        
        if (_images != null && _currentPage < _images.Count && _images[_currentPage] != null)
        {
            _bookImage.sprite = _images[_currentPage];
            _bookImage.gameObject.SetActive(true);
        }
        else
        {
            _bookImage.gameObject.SetActive(false);
        }
    }
}