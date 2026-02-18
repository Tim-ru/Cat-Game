using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuNavigationComponent : MonoBehaviour
{
    [SerializeField] private WindowNavigationComponent[] _windows;
    private PlayerInput _playerInput;
    private readonly List<int> _windowsIndexesList = new() { 0 };
    private void Start()
    {
        _playerInput = FindAnyObjectByType<PlayerInput>();
        _windows[_windowsIndexesList.Last()].gameObject.SetActive(true);
    }

    public void ChangeWindow(int windowIndex)
    {
        _windows[_windowsIndexesList.Last()].gameObject.SetActive(false);
        _windows[windowIndex].gameObject.SetActive(true);
        _windowsIndexesList.Add(windowIndex);
    }
    public void Back()
    {
        if (_windowsIndexesList.Count != 1)
        {
            _windows[_windowsIndexesList.Last()].gameObject.SetActive(false);
            _windowsIndexesList.RemoveAt(_windowsIndexesList.Count - 1);
            _windows[_windowsIndexesList.Last()].gameObject.SetActive(true);
        }
        else
        {
            _windows[_windowsIndexesList[0]].ResetHighlight();
            _playerInput.SwitchCurrentActionMap("Gameplay");
            gameObject.SetActive(false);
        }
    }
}
