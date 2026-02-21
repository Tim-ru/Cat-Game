using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneComponent : MonoBehaviour
{
    [SerializeField] private List<CutsceneActions> _actions;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private ScreenShake _screenShake;
    [SerializeField] private PlayerController _player;
    private Coroutine _CutsceneCoroutine;
    private float currentCamSize;
    private Vector3 currentCamPosition;
    private float targetCamSize;
    private Vector3 targetCamPosition;
    private float _movementTime;
    private float _AllMovementTime;
    private float _resizeTime;
    private float _AllResizeTime;
    private float _playerMoveTime;
    private bool _isPlaying = false;

    [ContextMenu("Play")]
    public void Play()
    {
        if (_CutsceneCoroutine != null) StopCoroutine(_CutsceneCoroutine);
        _CutsceneCoroutine = StartCoroutine(StartCutscene());
        _playerInput.enabled = false;
    }

    private IEnumerator StartCutscene()
    {
        for (int i = 0; i < _actions.Count; ++i)
        {
            yield return StartCutsceneItem(_actions[i]._actions);
        }
        _playerInput.enabled = true;
    }
    public IEnumerator StartCutsceneItem(List<CutsceneActionsItem> _actions)
    {
        currentCamSize = _camera.Lens.OrthographicSize;
        currentCamPosition = _camera.transform.position;
        targetCamPosition = currentCamPosition;
        targetCamSize = currentCamSize;
        _isPlaying = true;
        for (int i = 0; i < _actions.Count; ++i)
        {
            switch (_actions[i]._actionType)
            {
                case Actions.MoveCamera:
                    yield return new WaitForSeconds(_actions[i]._pauseTime);
                    if (currentCamPosition != targetCamPosition)
                        yield return new WaitForSeconds(_movementTime);
                    targetCamPosition = _actions[i]._targetCameraPosition;
                    _movementTime = _actions[i]._duration;
                    _AllMovementTime = _movementTime;
                    break;
                case Actions.ResizeCamera:
                    yield return new WaitForSeconds(_actions[i]._pauseTime);
                    if (currentCamSize != targetCamSize)
                        yield return new WaitForSeconds(_resizeTime);
                    if (_actions[i]._unfollowTargetDuringScene) _camera.Follow = null;
                    targetCamSize = _actions[i]._targetCameraSize;
                    _resizeTime = _actions[i]._duration;
                    _AllResizeTime = _resizeTime;
                    break;
                case Actions.ShakeCamera:
                    _screenShake.ShakeCamera(_actions[i]._shakeStrength, _actions[i]._duration);
                    break;
                case Actions.MovePlayer:
                    yield return new WaitForSeconds(_actions[i]._pauseTime);
                    _playerMoveTime = _actions[i]._duration;
                    _player.OnMove(_actions[i]._playerMovementVector);
                    break;
                case Actions.AddVelocityToPlayer:
                    yield return new WaitForSeconds(_actions[i]._pauseTime);
                    _playerMoveTime = _actions[i]._duration;
                    _player.OnMove(_actions[i]._playerMovementVector);
                    break;
                case Actions.JumpPlayer:
                    yield return new WaitForSeconds(_actions[i]._pauseTime);
                    _player.OnJumpPressed();
                    break;
            }
        }
        var maxTime = Math.Max(_movementTime, _resizeTime);
        maxTime = Math.Max(maxTime, _playerMoveTime);
        yield return new WaitForSeconds(maxTime);
        _isPlaying = false;
    }
    private void Update()
    {
        if (!_isPlaying) return;
        if (_movementTime > 0)
        {
            _camera.transform.position = Vector3.Lerp(currentCamPosition, targetCamPosition, 1 - (_movementTime / _AllMovementTime));
            _movementTime -= Time.deltaTime;
            if (_movementTime <= 0)
            {
                currentCamPosition = targetCamPosition;
                _camera.transform.position = targetCamPosition;
            }
        }
        if (_resizeTime > 0)
        {
            _camera.Lens.OrthographicSize = Mathf.Lerp(currentCamSize, targetCamSize, 1 - (_resizeTime / _AllResizeTime));
            _resizeTime -= Time.deltaTime;
            if (_resizeTime <= 0)
            {
                currentCamSize = targetCamSize;
                _camera.Lens.OrthographicSize = targetCamSize;
                _camera.Follow = _player.transform;
            }
        }
        if (_playerMoveTime > 0)
        {
            _playerMoveTime -= Time.deltaTime;
            if (_playerMoveTime <= 0)
            {
                _player.OnMove(Vector2.zero);
            }
        }
    }
}
[Serializable]
public class CutsceneActions
{
    public List<CutsceneActionsItem> _actions;
}

[Serializable]
public class CutsceneActionsItem
{
    public Actions _actionType;
    public float _duration;
    [Tooltip("Pause before starting for Screen Shake")]
    public float _pauseTime;
    public Vector3 _targetCameraPosition;
    public Vector2 _playerMovementVector;
    //public AnimationCurve _speed;
    public float _targetCameraSize;
    public float _shakeStrength;
    public bool _unfollowTargetDuringScene;
}

[Serializable]
public enum Actions
{
    MoveCamera,
    ResizeCamera,
    ShakeCamera,
    MovePlayer,
    AddVelocityToPlayer,
    JumpPlayer
}