using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private List<TimeActions> _actions;

    private void Start()
    {
        StartCoroutine(TimeManage());
    }

    private IEnumerator TimeManage()
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            yield return new WaitForSeconds(_actions[i]._startTime);
            _actions[i]._event?.Invoke();
        }
    }
}

[Serializable]
public class TimeActions
{
    public float _startTime;
    public UnityEvent _event;
}