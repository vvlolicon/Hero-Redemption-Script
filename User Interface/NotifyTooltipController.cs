using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifyTooltipController : MonoBehaviour
{
    Animator animator;
    [SerializeField] GameObject _notifyTooltipText;
    [SerializeField] RectTransform _container;
    [SerializeField] float _showMessageTime = 5f;
    [SerializeField] float _closingTime = 0.5f;
    [SerializeField] int _maxNotifies = 3;
    float _timer = 5f;
    Queue<GameObject> tooltips = new();
    bool _isClosing = false;
    Image _bg;

    private void OnEnable()
    {
        if (_bg == null)
        {
            _bg = GetComponent<Image>();
        }
        _bg.color = new Color(_bg.color.r, _bg.color.g, _bg.color.b, 1f);
        _isClosing = false;
    }

    public void PopMessage(string message)
    {
        if (_isClosing) return;
        var tooltip = Instantiate(_notifyTooltipText, _container);
        var text = tooltip.GetComponent<TMP_Text>();
        text.text = message;
        tooltips.Enqueue(tooltip);
        if(tooltips.Count > _maxNotifies)
        {
            Destroy(tooltips.Dequeue());
        }
        _timer = _showMessageTime;
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_isClosing)
        {
            _timer -= Time.deltaTime;
            if(_timer <= 0)
            {
                _isClosing = false;
                gameObject.SetActive(false);
            }
            else
            {
                _bg.color = new Color(_bg.color.r, _bg.color.g, _bg.color.b, 
                    Mathf.Lerp(0f, 1f, _timer / 0.5f));
            }
            return;
        }
        if(_timer > 0)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _isClosing = true;
                for (int i = tooltips.Count - 1; i >= 0; i--)
                {
                    Destroy(tooltips.Dequeue());
                }
                _timer = _closingTime;
            }
        }

    }
}
