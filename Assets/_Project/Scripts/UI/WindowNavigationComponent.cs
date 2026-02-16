using System;
using Unity.VisualScripting;
using UnityEngine;

public class WindowNavigationComponent : MonoBehaviour
{
    [SerializeField] private UIInputHandler _canvas;
    [SerializeField] private MenuItem[] _menuItems;
    private int _currentFocused;


    private void OnEnable()
    {
        _currentFocused = 0;
        foreach (var item in _menuItems)
        {
            item.OnUnfocused();
        }
        _menuItems[_currentFocused].OnFocused();
        _canvas.OnNavigation += CheckNavigation;
        _canvas.OnSubmit += Submit;
    }

    private void Submit()
    {
        _menuItems[_currentFocused].OnClick();
    }

    private void CheckNavigation(float y)
    {
        if (y > 0) Up();
        else if (y < 0) Down();
    }

    public void Up()
    {
        _menuItems[_currentFocused].OnUnfocused();
        _currentFocused--;
        _currentFocused = (int)Mathf.Repeat(_currentFocused, _menuItems.Length - 1);
        _menuItems[_currentFocused].OnFocused();
    }
    public void Down()
    {
        _menuItems[_currentFocused].OnUnfocused();
        _currentFocused++;
        _currentFocused = (int)Mathf.Repeat(_currentFocused, _menuItems.Length - 1);
        _menuItems[_currentFocused].OnFocused();
    }
    private void OnDisable()
    {
        _canvas.OnNavigation -= CheckNavigation;
        _canvas.OnSubmit -= Submit;
    }
}
