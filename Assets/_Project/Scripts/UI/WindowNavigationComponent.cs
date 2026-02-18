using UnityEngine;

public class WindowNavigationComponent : MonoBehaviour
{
    [SerializeField] private UIInputHandler _canvas;
    [SerializeField] private MenuItem[] _menuItems;
    private int _currentFocused = 0;


    private void OnEnable()
    {
        _canvas.OnNavigation += CheckNavigation;
        _canvas.OnSubmit += Submit;
    }

    private void Submit()
    {
        _menuItems[_currentFocused].OnClick();
    }

    private void CheckNavigation(Vector2 _vector)
    {
        if (_vector.y > 0) Up();
        else if (_vector.y < 0) Down();
        if (_menuItems[_currentFocused].GetType() == typeof(MenuSlider)) _menuItems[_currentFocused].ChangeValue(_vector.x);
    }

    public void Up()
    {
        _menuItems[_currentFocused].OnUnfocused();
        _currentFocused--;
        _currentFocused = (int)Mathf.Repeat(_currentFocused, _menuItems.Length);
        _menuItems[_currentFocused].OnFocused();
    }
    public void Down()
    {
        _menuItems[_currentFocused].OnUnfocused();
        _currentFocused++;
        _currentFocused = (int)Mathf.Repeat(_currentFocused, _menuItems.Length);
        _menuItems[_currentFocused].OnFocused();
    }
    private void OnDisable()
    {
        _canvas.OnNavigation -= CheckNavigation;
        _canvas.OnSubmit -= Submit;
    }

    public void ResetHighlight()
    {
        _menuItems[_currentFocused].OnUnfocused();
        _currentFocused = 0;
        _menuItems[_currentFocused].OnFocused();
    }
}
