// Source: Assets/Scripts/CarCtrl.cs
// Selected excerpt: gear-button registration.
// A gear can be selected only while the clutch is engaged.

using UnityEngine;
using UnityEngine.UI;

public class GearSelectionInput : MonoBehaviour
{
    [SerializeField]
    private Button[] gearButtons;

    [SerializeField]
    private CarState carState;

    private const int Neutral = 0;
    private const int First = 1;
    private const int Second = 2;
    private const int Third = 3;
    private const int Fourth = 4;
    private const int Fifth = 5;
    private const int Reverse = 6;

    private void Start()
    {
        RegisterGearButton(Neutral);
        RegisterGearButton(First);
        RegisterGearButton(Second);
        RegisterGearButton(Third);
        RegisterGearButton(Fourth);
        RegisterGearButton(Fifth);
        RegisterGearButton(Reverse);
    }

    private void RegisterGearButton(int gear)
    {
        if (!HasButton(gear))
            return;

        gearButtons[gear].onClick.AddListener(
            () => TrySelectGear(gear));
    }

    private bool HasButton(int gear)
    {
        return gearButtons != null &&
               0 <= gear &&
               gear < gearButtons.Length &&
               gearButtons[gear] != null;
    }

    private void TrySelectGear(int gear)
    {
        if (carState == null)
            return;

        if (!carState.isClutch)
            return;

        if (carState.Gear == -1)
            return;

        // Selection and application are intentionally separated.
        // CarState applies ShiftGear when the clutch is released.
        carState.ShiftGear = gear;
    }
}
