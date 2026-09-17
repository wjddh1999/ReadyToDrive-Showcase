// Source: Assets/Scripts/CarCtrl.cs
// Excerpt: neutral gear button registration
// The UI records the selected gear; the clutch-release branch applies it.

if (GearBtn[0] != null)
{
    GearBtn[0].onClick.AddListener(() =>
    {
        if (CarState.inst.isClutch && CarState.inst.Gear != -1)
            CarState.inst.ShiftGear = 0;
    });
}
