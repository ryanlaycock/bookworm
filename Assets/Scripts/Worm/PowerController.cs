using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerController : MonoBehaviour
{
    [SerializeField]
    private int _maxPower;
    [SerializeField]
    private int _doubleJumpPower;
    [SerializeField]
    private int _doubleJumpPowerUsage;
    [SerializeField]
    private float _powerIncreaseRate;
    [SerializeField]
    private float _currentPowerLevel;
    [SerializeField]
    private Slider _healthBar;

    void FixedUpdate()
    {
        _healthBar.value = _currentPowerLevel;
    }

    public bool CanDoubleJump()
    {
        return _currentPowerLevel >= _doubleJumpPower;
    }

    public bool IsOverPowered()
    {
        if (_currentPowerLevel == 0 || _maxPower == 0) return false; // TODO Weird hack for timing issues
        return _currentPowerLevel >= _maxPower;
    }

    public void Modify(float amount)
    {
        _currentPowerLevel += amount * _powerIncreaseRate;
        if (_currentPowerLevel > 100) _currentPowerLevel = 100;
    }

    public void ModifyDoubleJump()
    {
        _currentPowerLevel -= _doubleJumpPowerUsage;
    }

    public float GetCurrent()
    {
        return _currentPowerLevel;
    }
}
