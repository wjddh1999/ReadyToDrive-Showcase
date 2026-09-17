// Source: Assets/Scripts/CarState.cs
// Selected excerpt: clutch input, shift validation, and RPM response.
// Surrounding vehicle, UI, fuel, and audio setup is omitted.

using UnityEngine;

public class CarState : MonoBehaviour
{
    public int Gear = -1;
    public int ShiftGear;
    public int CacGear;

    public bool isClutch;
    public bool RPMChange;

    public float RPM;
    public float m_LowSpeed;
    public float CalcLowAcc;

    public AudioSource audioSource;
    public AudioSource GearSound;

    private void Update()
    {
        GearShift();
        RpmDiffuse();
    }

    private void GearShift()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Gear != -1)
        {
            ShiftGear = Gear;
            CacGear = Gear;
            CalcLowAcc = m_LowSpeed;
            Gear = 0;
            isClutch = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && Gear != -1)
        {
            isClutch = false;

            if (ShiftGear == 6)
            {
                Gear = ShiftGear;
                GearSound.PlayOneShot(GearSound.clip, 0.7f);
                return;
            }

            if (ShiftGear - CacGear == 0)
            {
                Gear = ShiftGear;
                return;
            }

            if ((ShiftGear == 1 || ShiftGear == 2) &&
                (CacGear == 0 || CacGear == 1))
            {
                RPMChange = true;
                Gear = ShiftGear;
                GearSound.PlayOneShot(GearSound.clip, 0.7f);
                return;
            }

            if (ShiftGear - CacGear == 1)
            {
                if (2000 <= RPM)
                {
                    RPMChange = true;
                    Gear = ShiftGear;
                    GearSound.PlayOneShot(GearSound.clip, 0.7f);
                }
                else
                {
                    Gear = CacGear;
                }

                return;
            }

            if (2 <= ShiftGear - CacGear)
            {
                if (2000 + (500 * (ShiftGear - CacGear)) <= RPM)
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
                    SoundCtrl(
                        audioSource,
                        "Vehicle_Car_Stop_Engine_Exterior",
                        false);
                }

                return;
            }

            if (ShiftGear - CacGear < 0)
            {
                if (CacGear == 6)
                {
                    if (ShiftGear == 1 || ShiftGear == 0)
                    {
                        GearSound.PlayOneShot(GearSound.clip, 0.7f);
                        Gear = ShiftGear;
                    }

                    return;
                }

                GearSound.PlayOneShot(GearSound.clip, 0.7f);
                RPMChange = true;
                Gear = ShiftGear;
            }
        }
    }

    private void RpmDiffuse()
    {
        if (!RPMChange)
            return;

        if (CacGear < ShiftGear)
        {
            if (RPM <= 1000.0f)
                RPMChange = false;

            RPM = Mathf.Lerp(RPM, 0.0f, Time.deltaTime * 2.0f);
            return;
        }

        if (CacGear > ShiftGear)
        {
            if (RPM >= 2700.0f + (CacGear - ShiftGear) * 300.0f)
                RPMChange = false;

            if (ShiftGear == 0)
                RPMChange = false;

            if (ShiftGear == 1 && CacGear == 2)
                RPMChange = false;

            RPM = Mathf.Lerp(
                RPM,
                2700.0f + (CacGear - ShiftGear) * 500.0f,
                Time.deltaTime * 2.0f);
        }
    }

    public void SoundCtrl(
        AudioSource source,
        string resourceName,
        bool isLoop)
    {
        source.clip = Resources.Load(resourceName) as AudioClip;
        source.loop = isLoop;
        source.Play();
    }
}
