// Source: Assets/Scripts/CarState.cs
// Excerpt: clutch input and temporary gear state
// Review sample only; project dependencies are not included.

if (Input.GetKeyDown(KeyCode.LeftShift) && Gear != -1)
{
    ShiftGear = Gear;
    CacGear = Gear;
    CalcLowAcc = m_LowSpeed;
    Gear = 0;
    isClutch = true;
}
