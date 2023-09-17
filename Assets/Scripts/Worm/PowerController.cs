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
        return _currentPowerLevel >= _powerIncreaseRate;
    }

    public void Modify(float amount)
    {
        _currentPowerLevel += amount * _powerIncreaseRate;
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
