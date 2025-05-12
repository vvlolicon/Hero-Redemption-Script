using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractableObject
{
    public string GetInterableTitle();

    public void Interact();
}
public class InteractObject : MonoBehaviour
{
    [SerializeField] GameObject _showObjectNamePrefab;
    [SerializeField] GameObject _tooltipContainer;
    [SerializeField] Transform _highlighter;
    [SerializeField] float _highlighterYOffset;

    [SerializeField] float _tooltipHeight = 60f;
    [SerializeField] float _tooltipSpacing = 10f;

    List<GameObject> _interactableObjects = new ();
    List<GameObject> _tooltips = new List<GameObject>();
    int _curSelectedIndex = -1;
    PlayerStateExecutor _player { get { return PlayerCompManager.TryGetPlayerComp<PlayerStateExecutor>(); } }
    UI_Controller UI_Controller { get { return UI_Controller.Instance; } }

    private void Start()
    {
        //for (int i = 0; i<1; i++)
        //{
        //    var tooltip = Instantiate(_showObjectNamePrefab, _tooltipContainer.transform);
        //    _tooltips.Add(tooltip);
        //    var tooltipScript = tooltip.GetComponent<InteractTooltipControl>();
        //    tooltipScript.SetTooltipTitle("test title" + i);
        //}
        //_tooltipContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(300, _tooltips.Count * _tooltipHeight - _tooltipSpacing);
    }

    void Update()
    {
        if (gameObject.activeSelf)
            HandleScrollInput();
    }

    public void HandleScrollInput()
    {
        if (_tooltips.Count == 0) return; // return if no interactable objects
        //Debug.Log("input: " + input);
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0)
        {// Scroll Up
            _curSelectedIndex = (_curSelectedIndex - 1 + _tooltips.Count) % _tooltips.Count;
        }
        else if (scroll < 0)
        {// Scroll Down
            _curSelectedIndex = (_curSelectedIndex + 1) % _tooltips.Count;
        }

        if (_curSelectedIndex != -1)
        {
            UpdateHighlighterPosition();
        }
    }

    void UpdateHighlighterPosition()
    {
        //Debug.Log("_curSelectedIndex: " + _curSelectedIndex);
        if (_tooltips.Count < 2) return;
        if (_curSelectedIndex < _tooltips.Count - 1)
        {
            _tooltipContainer.transform.localPosition = new Vector3(0, _curSelectedIndex * _tooltipHeight, 0);
            _highlighter.localPosition = new Vector3(5, -1, 0);
        }
        else if(_curSelectedIndex == _tooltips.Count - 1)
        {
            _tooltipContainer.transform.localPosition = new Vector3(0, (_curSelectedIndex-1) * _tooltipHeight, 0);
            _highlighter.localPosition = new Vector3(5, -(_tooltipHeight + _highlighterYOffset), 0);
        }
    }

    public void AddInteractObject(GameObject interactObj)
    {
        var interactable = interactObj.GetComponent<IInteractableObject>();
        if (!_interactableObjects.Contains(interactObj))
        {
            _interactableObjects.Add(interactObj);
            var tooltip = Instantiate(_showObjectNamePrefab, _tooltipContainer.transform);
            _tooltips.Add(tooltip);
            var tooltipScript = tooltip.GetComponent<InteractTooltipControl>();
            tooltipScript.SetTooltipTitle(interactable.GetInterableTitle());
            _tooltipContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(300, _tooltips.Count * _tooltipHeight - _tooltipSpacing);
            if(_tooltips.Count == 1)
            {
                _curSelectedIndex = 0;
            }
            if(_tooltips.Count > 0)
            {
                _highlighter.gameObject.SetActive(true);
                gameObject.SetActive(true);
            }
        }
    }

    public void RemoveInteractObject(GameObject interactObj)
    {
        var interactable = interactObj.GetComponent<IInteractableObject>();
        int index = _interactableObjects.IndexOf(interactObj);
        if (_interactableObjects.Contains(interactObj))
        {
            _interactableObjects.Remove(interactObj);
            GameObject removeObj = _tooltips[index];
            _tooltips.Remove(removeObj);
            Destroy(removeObj);
            _tooltipContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(300, Mathf.Max(100f, _tooltips.Count * _tooltipHeight - _tooltipSpacing));
            if(_interactableObjects.Count == 0)
            {
                _curSelectedIndex = -1;
                _highlighter.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
            if (_curSelectedIndex == index)
            {
                _curSelectedIndex = (_curSelectedIndex + 1)%_interactableObjects.Count;
            }
            else if (_curSelectedIndex > index)
            {
                _curSelectedIndex--;
            }
        }
    }

    public bool InteractWithObject()
    {
        if (_curSelectedIndex == -1 || _interactableObjects.Count == 0) return false;
        var interactObj = _interactableObjects[_curSelectedIndex];
        if (interactObj.TryGetComponent(out IInteractableObject interactable) &&
            !interactObj.IsGameObjectNullOrDestroyed())
        {
            interactable.Interact();
            return true;
        }
        return false;
    }

    public bool HasInteractObject()
    {
        return _interactableObjects.Count > 0;
    }

    public void ResetDetector()
    {
        for(int i = _interactableObjects.Count-1; i>=0; i--)
        {
            RemoveInteractObject(_interactableObjects[i]);
        }
    }
}
