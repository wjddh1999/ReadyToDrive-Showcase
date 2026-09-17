// Source: Assets/Scripts/CarState.cs
// Excerpt: multi-gear upshift validation
// Earlier branches handle same-gear, low-gear, and single-step shifts.

int requiredRpm = 2000 + (500 * (ShiftGear - CacGear));

if (requiredRpm <= RPM)
{
    RPMChange = true;
    Gear = ShiftGear;
    GearSound.PlayOneShot(GearSound.clip, 0.7f);
}
else
{
    Gear = -1;
    m_LowSpeed = 0;
    ShiftGear = 0;
    SoundCtrl(audioSource, "Vehicle_Car_Stop_Engine_Exterior", false);
}
