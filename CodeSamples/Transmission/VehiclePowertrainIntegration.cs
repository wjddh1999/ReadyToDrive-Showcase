// Source: Assets/Scripts/CarCtrl.cs
// Public-review adaptation of the vehicle input and drivetrain branch.
// Steering, braking, traction, lights, and wheel presentation are omitted.

using UnityEngine;

public class VehiclePowertrainIntegration : MonoBehaviour
{
    [SerializeField]
    private CarState carState;

    [SerializeField]
    private WheelCollider[] wheelColliders;

    [SerializeField]
    private float fullTorqueOverAllWheels = 2000.0f;

    private float accelerator;
    private float currentTorque;
    private int drivenWheelCount = 4;

    private void Awake()
    {
        currentTorque = fullTorqueOverAllWheels;
    }

    private void Update()
    {
        UpdateEngineState();
        UpdatePowertrain();
        UpdateRpmFromThrottle();
    }

    private void UpdateEngineState()
    {
        bool toggledEngine =
            Input.GetKeyUp(KeyCode.Q) &&
            Input.GetKey(KeyCode.Space) &&
            Input.GetKey(KeyCode.LeftShift);

        if (!toggledEngine)
            return;

        if (carState.Gear == -1)
        {
            carState.Gear = 0;
            carState.SoundCtrl(
                carState.audioSource,
                "Vehicle_Car_Start_Engine_Exterior",
                false);
            return;
        }

        carState.Gear = -1;
        carState.m_LowSpeed = 0;
        carState.ShiftGear = 0;
        carState.SoundCtrl(
            carState.audioSource,
            "Vehicle_Car_Stop_Engine_Exterior",
            false);
    }

    private void UpdatePowertrain()
    {
        accelerator = Mathf.Clamp01(Input.GetAxis("Vertical"));

        // The clutch disconnects throttle from the driven wheels.
        if (carState.isClutch)
            accelerator = 0;

        float torquePerWheel =
            currentTorque / Mathf.Max(1, drivenWheelCount);

        if (carState.Gear == 6)
        {
            ApplyMotorTorque(-(accelerator * torquePerWheel));
            return;
        }

        if (carState.Gear != 0 && carState.Gear != -1)
        {
            ApplyMotorTorque(accelerator * torquePerWheel);
            return;
        }

        // Neutral and engine-off states retain a minimal value
        // used by the original controller.
        ApplyMotorTorque(0.0001f);
    }

    private void ApplyMotorTorque(float torque)
    {
        if (wheelColliders == null)
            return;

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (wheelColliders[i] != null)
                wheelColliders[i].motorTorque = torque;
        }
    }

    private void UpdateRpmFromThrottle()
    {
        carState.RPM -= Time.deltaTime * 300.0f;

        if (carState.RPM <= 800 && carState.Gear != -1)
            carState.RPM += Time.deltaTime * 1500.0f;

        if (!Input.GetKey(KeyCode.W) || carState.Gear == -1)
            return;

        if (carState.RPM <= 2700)
            carState.RPM += 750.0f * Time.deltaTime;
        else if (carState.RPM <= 3600)
            carState.RPM += 350.0f * Time.deltaTime;
        else if (carState.RPM <= 4000)
            carState.RPM += 100.0f * Time.deltaTime;
        else
            carState.RPM += 15.0f * Time.deltaTime;
    }
}
