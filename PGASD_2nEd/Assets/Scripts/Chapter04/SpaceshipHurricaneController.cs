
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class SpaceshipHurricaneController_InputSystem : MonoBehaviour
{
    [Header("Movement References")]
    public Rigidbody rb;

    [Header("Movement - Translation")]
    public float maxSpeed = 35f;
    public float acceleration = 25f;
    public float deceleration = 18f;
    public float speedDrag = 0.02f;

    [Header("Movement - Rotation")]
    public float yawSpeed = 90f;
    public float pitchSpeed = 70f;
    public float rollSpeed = 120f;
    public bool mouseLook = true;
    public float mouseSensitivity = 2.0f;
    public bool clampPitch = true;
    public float pitchClamp = 80f;

    [Header("Keys (Input System)")]
    public Key thrustKey = Key.W;
    public Key brakeKey = Key.S;
    public Key strafeLeftKey = Key.A;
    public Key strafeRightKey = Key.D;
    public Key rollLeftKey = Key.Q;
    public Key rollRightKey = Key.E;

    [Header("Audio - Engine Loop")]
    public AudioSource engineLoop;
    [Range(0f, 1f)] public float engineBaseVolume = 0.7f;
    [Tooltip("Hard ceiling on the engine loop volume, whatever the modulation does.")]
    [Range(0f, 1f)] public float engineMaxVolume = 0.8f;

    [Header("Audio - Thruster One-Shots")]
    [Tooltip("Leave empty and a separate, filter free source is created automatically. Do NOT reuse the engine loop source.")]
    public AudioSource oneShotSource;
    public AudioClip thrusterOnClip;
    public AudioClip thrusterOffClip;
    [Range(0f, 1f)] public float thrusterOneShotVolume = 0.9f;
    [Tooltip("Minimum time between thruster one-shots, so tapping W cannot stack them.")]
    public float thrusterRetriggerCooldown = 0.15f;

    [Header("Audio - Pitch (Input Driven)")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.6f;
    public float pitchSmoothing = 6f;
    [Tooltip("ON: pitch only moves while W or S is held, and holds its value otherwise.\nOFF: pitch follows the throttle, so it also winds down after you release W.")]
    public bool freezePitchWithoutInput = false;

    [Header("Audio - Low Pass Filter")]
    public AudioLowPassFilter lowPass;
    public float lowPassCutoffMin = 700f;
    public float lowPassCutoffMax = 22000f;
    public bool lowPassOnlyAtLowSpeed = true;
    [Range(0f, 1f)] public float lowPassDisableAbove = 0.25f;

    [Header("Audio - Tremolo (OBVIOUS)")]
    [Range(0f, 1f)] public float tremoloStart = 0.15f;
    [Range(0f, 1f)] public float tremoloFull = 0.6f;
    [Range(0f, 1f)] public float tremoloDepthMax = 0.7f;
    public float tremoloRateMin = 2.0f;
    public float tremoloRateMax = 14.0f;

    [Header("Audio - Distortion (Top Speed Cue)")]
    public AudioDistortionFilter distortion;
    [Range(0f, 1f)] public float distortionStart = 0.85f;
    [Range(0f, 1f)] public float distortionAmountMax = 0.6f;
    [Tooltip("How much to pull the volume down as distortion comes in (distortion adds a lot of perceived loudness).")]
    [Range(0f, 1f)] public float distortionVolumeCompensation = 0.5f;

    [Header("Debug")]
    public bool showDebug = false;

    // Internal state
    private float _targetSpeed;     // throttle: only changes because of W / S
    private float _audioDrive;      // 0..1 value that drives every engine sound parameter
    private float _smoothedPitch;
    private float _tremoloPhase;
    private float _yaw, _pitch, _roll;
    private bool _thrustHeldLastFrame;
    private float _lastOneShotTime = -999f;

    private Keyboard _kb;
    private Mouse _mouse;

    void Awake()
    {
        _kb = Keyboard.current;
        _mouse = Mouse.current;

        if (!rb) rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (engineLoop)
        {
            engineLoop.loop = true;
            engineLoop.playOnAwake = true;
            if (!engineLoop.isPlaying) engineLoop.Play();
            engineLoop.volume = engineBaseVolume;
            engineLoop.pitch = minPitch;
            _smoothedPitch = minPitch;
        }

        // One-shots must NOT share the engine loop source: they would inherit its
        // pitch, low pass, and distortion (filters affect every AudioSource on that GameObject).
        if (!oneShotSource || oneShotSource == engineLoop)
        {
            var go = new GameObject("ThrusterOneShots");
            go.transform.SetParent(transform, false);
            oneShotSource = go.AddComponent<AudioSource>();
            oneShotSource.playOnAwake = false;
            if (engineLoop)
            {
                oneShotSource.spatialBlend = engineLoop.spatialBlend;
                oneShotSource.outputAudioMixerGroup = engineLoop.outputAudioMixerGroup;
                oneShotSource.minDistance = engineLoop.minDistance;
                oneShotSource.maxDistance = engineLoop.maxDistance;
                oneShotSource.rolloffMode = engineLoop.rolloffMode;
            }
        }
    }

    void Update()
    {
        if (_kb == null) _kb = Keyboard.current;
        if (_mouse == null) _mouse = Mouse.current;
        if (_kb == null) return;

        HandleThrusterOneShots();
        HandleRotation();
        UpdateAudio();
    }

    void FixedUpdate()
    {
        if (!rb || _kb == null) return;

        float thrustInput = 0f;
        if (_kb[thrustKey].isPressed) thrustInput += 1f;
        if (_kb[brakeKey].isPressed) thrustInput -= 1f;

        float strafeInput = 0f;
        if (_kb[strafeRightKey].isPressed) strafeInput += 1f;
        if (_kb[strafeLeftKey].isPressed) strafeInput -= 1f;

        float desiredSpeed = Mathf.Clamp(thrustInput, -1f, 1f) * maxSpeed;

        float rate = Mathf.Abs(desiredSpeed) > Mathf.Abs(_targetSpeed)
            ? acceleration
            : deceleration;

        _targetSpeed = MoveToward(_targetSpeed, desiredSpeed, rate * Time.fixedDeltaTime);

        Vector3 forward = transform.forward;
        float currentForwardSpeed = Vector3.Dot(rb.linearVelocity, forward);

        float speedError = _targetSpeed - currentForwardSpeed;
        float accelCmd = Mathf.Clamp(speedError * 2f, -deceleration, acceleration);

        rb.AddForce(forward * accelCmd, ForceMode.Acceleration);

        if (Mathf.Abs(strafeInput) > 0.01f)
            rb.AddForce(transform.right * (strafeInput * acceleration * 0.6f), ForceMode.Acceleration);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        if (speedDrag > 0f)
            rb.linearVelocity *= (1f - speedDrag * Time.fixedDeltaTime * rb.linearVelocity.magnitude);
    }

    private void HandleThrusterOneShots()
    {
        bool thrustHeld = _kb[thrustKey].isPressed;
        bool cooledDown = Time.time - _lastOneShotTime >= thrusterRetriggerCooldown;

        if (cooledDown && thrustHeld != _thrustHeldLastFrame)
        {
            AudioClip clip = thrustHeld ? thrusterOnClip : thrusterOffClip;
            if (clip)
            {
                oneShotSource.Stop(); // kill any previous one-shot so they never pile up
                oneShotSource.PlayOneShot(clip, thrusterOneShotVolume);
                _lastOneShotTime = Time.time;
            }
        }

        _thrustHeldLastFrame = thrustHeld;
    }

    private void HandleRotation()
    {
        float dt = Time.deltaTime;
        float yawInput = 0f;
        float pitchInput = 0f;

        if (mouseLook && _mouse != null)
        {
            Vector2 delta = _mouse.delta.ReadValue();
            yawInput = delta.x * mouseSensitivity * 0.02f;
            pitchInput = -delta.y * mouseSensitivity * 0.02f;
        }

        float rollInput = 0f;
        if (_kb[rollLeftKey].isPressed) rollInput -= 1f;
        if (_kb[rollRightKey].isPressed) rollInput += 1f;

        _yaw += yawInput * yawSpeed * dt;
        _pitch += pitchInput * pitchSpeed * dt;
        _roll += rollInput * rollSpeed * dt;

        if (clampPitch)
            _pitch = Mathf.Clamp(_pitch, -pitchClamp, pitchClamp);

        transform.rotation = Quaternion.Euler(_pitch, _yaw, _roll);
    }

    private void UpdateAudio()
    {
        if (!engineLoop) return;

        // Audio is driven by the THROTTLE (player input), not by the Rigidbody's velocity.
        // Velocity changes from collisions, strafing, drag, and turning, which is what kept
        // the pitch moving on its own.
        bool throttleInput = _kb[thrustKey].isPressed || _kb[brakeKey].isPressed;
        if (!freezePitchWithoutInput || throttleInput)
            _audioDrive = Mathf.Clamp01(Mathf.Abs(_targetSpeed) / maxSpeed);

        float drive = _audioDrive;

        // Pitch
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, drive);
        float follow = 1f - Mathf.Exp(-pitchSmoothing * Time.deltaTime);
        _smoothedPitch = Mathf.Lerp(_smoothedPitch, targetPitch, follow);
        engineLoop.pitch = _smoothedPitch;

        // Low pass
        if (lowPass)
        {
            if (lowPassOnlyAtLowSpeed && drive > lowPassDisableAbove)
                lowPass.cutoffFrequency = lowPassCutoffMax;
            else
                lowPass.cutoffFrequency = Mathf.Lerp(lowPassCutoffMin, lowPassCutoffMax, drive);
        }

        // Tremolo. Phase is accumulated so changing the rate does not cause jumps.
        float tremoloAmt = Remap01Clamped(drive, tremoloStart, tremoloFull);
        tremoloAmt = Mathf.Sqrt(tremoloAmt); // exaggerate ramp

        float depth = tremoloAmt * tremoloDepthMax;
        float rate = Mathf.Lerp(tremoloRateMin, tremoloRateMax, tremoloAmt);
        _tremoloPhase = Mathf.Repeat(_tremoloPhase + rate * Time.deltaTime, 1f);

        float lfo = 0.5f + 0.5f * Mathf.Sin(_tremoloPhase * Mathf.PI * 2f);
        float tremolo = Mathf.Lerp(1f - depth, 1f, lfo);

        // Distortion (top speed) with volume compensation
        float distAmt = 0f;
        if (distortion)
        {
            distAmt = Remap01Clamped(drive, distortionStart, 1f);
            distortion.distortionLevel = distAmt * distortionAmountMax;
        }
        float distComp = 1f - distAmt * distortionVolumeCompensation;

        engineLoop.volume = Mathf.Min(engineBaseVolume * tremolo * distComp, engineMaxVolume);

        if (showDebug)
            Debug.Log($"Throttle={drive:F2} Pitch={_smoothedPitch:F2} Vol={engineLoop.volume:F2}");
    }

    private static float MoveToward(float current, float target, float maxDelta)
    {
        if (Mathf.Abs(target - current) <= maxDelta) return target;
        return current + Mathf.Sign(target - current) * maxDelta;
    }

    private static float Remap01Clamped(float x, float a, float b)
    {
        if (b <= a) return 0f;
        return Mathf.Clamp01((x - a) / (b - a));
    }
}
