
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class XYEngine : MonoBehaviour
{
    [System.Serializable]
    public class Corner
    {
        public AudioSource source;
        public string name;

        [Header("Per-corner Controls")]
        [Range(0f, 1f)] public float volumeTrim = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;
        [Range(0.5f, 2f)] public float speed = 1f;
    }

    [Header("Corners (TL=Lo, TR=Mid, BR=Hi, BL=Boo)")]
    public Corner topLeft;     // Lo
    public Corner topRight;    // Mid
    public Corner bottomRight; // Hi
    public Corner bottomLeft;  // Boo

    [Header("XY Pad (0..1)")]
    [Range(0f, 1f)] public float x = 0.5f;
    [Range(0f, 1f)] public float y = 0.5f;
    public float mixSmoothing = 10f;

    [Header("Volume")]
    [Tooltip("Overall master for EVERYTHING (beds + sweeteners).")]
    [Range(0f, 1f)] public float masterVolume = 1f;

    [Tooltip("Master for the 4 looping XY pad beds only.")]
    [Range(0f, 1f)] public float bedsVolume = 1f;

    [Tooltip("Master for sweeteners only.")]
    [Range(0f, 1f)] public float sweetenersMasterVolume = 1f;

    [Header("Fade Law")]
    public bool equalPowerFades = true;

    [Header("Toggle Playback")]
    public bool spacebarTogglesPlayback = true;
    public Key toggleKey = Key.Space;

    [Header("Sweeteners (one-shots)")]
    public AudioSource sweetenerSource;
    public List<AudioClip> sweeteners = new List<AudioClip>();
    public bool sweetenersEnabled = true;
    public Vector2 sweetenerIntervalRange = new Vector2(6f, 18f);

    [Tooltip("Per-trigger level for sweeteners (pre-master).")]
    [Range(0f, 1f)] public float sweetenerVolume = 0.8f;

    [Range(0f, 1f)] public float sweetenerMinEnergy = 0.15f;

    [Header("Sweetener Debug / Test")]
    public bool logSweetenerEvents = true;
    public Key playSweetenerNowKey = Key.T;

    [Header("Auto-load Sweeteners (Resources)")]
    public bool autoLoadSweetenersFromResources = false;
    public string resourcesSweetenerPath = "Sweeteners";

    [Header("Debug Overlay")]
    public bool debugOverlay = true;
    public bool debugLogXYChanges = false;
    public float debugRefreshHz = 10f;
    public Vector2 debugScreenPos = new Vector2(12f, 12f);
    public Key infoToggleKey = Key.Digit1;

    // internal
    private bool _infoVisible = false;
    private bool _isPlaying = false;
    private float _wTL, _wTR, _wBR, _wBL;
    private float _tNextSweetener = 0f;

    private string _debugText = "";
    private float _nextDebugUpdate = 0f;

    private Keyboard _kb;

    void Awake()
    {
        _kb = Keyboard.current;

        if (autoLoadSweetenersFromResources)
        {
            var loaded = Resources.LoadAll<AudioClip>(resourcesSweetenerPath);
            sweeteners = new List<AudioClip>(loaded);
            if (logSweetenerEvents)
                Debug.Log($"[CrowdXYEngine] Loaded {sweeteners.Count} sweeteners from Resources/{resourcesSweetenerPath}");
                TogglePlayback();
        }

        ForceLoop(topLeft);
        ForceLoop(topRight);
        ForceLoop(bottomRight);
        ForceLoop(bottomLeft);

        if (sweetenerSource != null)
        {
            sweetenerSource.loop = false;
            // Set base volume to 1 so PlayOneShot scaling is predictable.
            sweetenerSource.volume = 1f;
        }
    }

    void Update()
    {
        if (_kb == null) _kb = Keyboard.current;

        // Toggle info screen
        if (_kb != null && _kb[infoToggleKey].wasPressedThisFrame)
            _infoVisible = !_infoVisible;

        // Manual sweetener test
        if (_kb != null && _kb[playSweetenerNowKey].wasPressedThisFrame)
            PlayRandomSweetener("manual");

        // Toggle playback
        if (spacebarTogglesPlayback && _kb != null && _kb[toggleKey].wasPressedThisFrame)
            TogglePlayback();

        if (!_isPlaying)
        {
            UpdateDebugText(Time.time);
            return;
        }

        UpdateWeightsAndMix(Time.deltaTime);
        UpdateSweeteners(Time.time);
        UpdateDebugText(Time.time);
    }

    public void SetXY(Vector2 xy01)
    {
        float newX = Mathf.Clamp01(xy01.x);
        float newY = Mathf.Clamp01(xy01.y);

        if (debugLogXYChanges && (Mathf.Abs(newX - x) > 0.001f || Mathf.Abs(newY - y) > 0.001f))
            Debug.Log($"[CrowdXYEngine] SetXY -> ({newX:F3}, {newY:F3}) on {name}");

        x = newX;
        y = newY;
    }

    public Vector2 GetXY() => new Vector2(x, y);

    public void StartPlayback()
    {
        _isPlaying = true;

        StartCornerIfNeeded(topLeft);
        StartCornerIfNeeded(topRight);
        StartCornerIfNeeded(bottomRight);
        StartCornerIfNeeded(bottomLeft);

        _tNextSweetener = Time.time + RandomInterval();

        if (logSweetenerEvents)
            Debug.Log($"[CrowdXYEngine] Playback START. Next sweetener in {(Mathf.Max(0f, _tNextSweetener - Time.time)):F1}s");
    }

    public void StopPlayback()
    {
        _isPlaying = false;

        StopCorner(topLeft);
        StopCorner(topRight);
        StopCorner(bottomRight);
        StopCorner(bottomLeft);

        if (logSweetenerEvents)
            Debug.Log("[CrowdXYEngine] Playback STOP.");
    }

    public void TogglePlayback()
    {
        if (_isPlaying) StopPlayback();
        else StartPlayback();
    }

    private void UpdateWeightsAndMix(float dt)
    {
        float wTL = (1f - x) * y;
        float wTR = x * y;
        float wBR = x * (1f - y);
        float wBL = (1f - x) * (1f - y);

        float a = 1f - Mathf.Exp(-mixSmoothing * dt);
        _wTL = Mathf.Lerp(_wTL, wTL, a);
        _wTR = Mathf.Lerp(_wTR, wTR, a);
        _wBR = Mathf.Lerp(_wBR, wBR, a);
        _wBL = Mathf.Lerp(_wBL, wBL, a);

        ApplyCorner(topLeft, _wTL);
        ApplyCorner(topRight, _wTR);
        ApplyCorner(bottomRight, _wBR);
        ApplyCorner(bottomLeft, _wBL);
    }

    private void ApplyCorner(Corner c, float weight)
    {
        if (c == null || c.source == null) return;

        c.source.pitch = Mathf.Clamp(c.pitch * c.speed, 0.1f, 3f);

        float w = Mathf.Clamp01(weight);
        float gain = equalPowerFades ? Mathf.Sqrt(w) : w;

        // Beds get their own master fader, and still pass through global master.
        float bedGain = masterVolume * bedsVolume;

        c.source.volume = Mathf.Clamp01(bedGain * gain * c.volumeTrim);
    }

    private void UpdateSweeteners(float now)
    {
        if (!sweetenersEnabled || sweetenerSource == null || sweeteners == null || sweeteners.Count == 0)
            return;

        float energy = _wTR * 0.5f + _wBR * 1.0f + _wBL * 0.8f;
        if (energy < sweetenerMinEnergy)
            return;

        if (now >= _tNextSweetener)
        {
            PlayRandomSweetener("timer");
            _tNextSweetener = now + RandomInterval();

            if (logSweetenerEvents)
                Debug.Log($"[CrowdXYEngine] Next sweetener in {(Mathf.Max(0f, _tNextSweetener - now)):F1}s");
        }
    }

    private void PlayRandomSweetener(string reason)
    {
        if (sweetenerSource == null)
        {
            if (logSweetenerEvents) Debug.LogWarning("[CrowdXYEngine] SweetenerSource is NULL.");
            return;
        }
        if (sweeteners == null || sweeteners.Count == 0)
        {
            if (logSweetenerEvents) Debug.LogWarning("[CrowdXYEngine] Sweeteners list is EMPTY.");
            return;
        }

        var clip = sweeteners[Random.Range(0, sweeteners.Count)];
        if (clip == null)
        {
            if (logSweetenerEvents) Debug.LogWarning("[CrowdXYEngine] Random sweetener clip was NULL.");
            return;
        }

        // Sweeteners get their own master fader, and still pass through global master.
        float oneShotGain = masterVolume * sweetenersMasterVolume * sweetenerVolume;

        sweetenerSource.PlayOneShot(clip, Mathf.Clamp01(oneShotGain));

        if (logSweetenerEvents)
            Debug.Log($"[CrowdXYEngine] Sweetener fired ({reason}): {clip.name} (gain={oneShotGain:F2})");
    }

    private float RandomInterval()
    {
        float a = Mathf.Max(0.1f, sweetenerIntervalRange.x);
        float b = Mathf.Max(a, sweetenerIntervalRange.y);
        return Random.Range(a, b);
    }

    private void StartCornerIfNeeded(Corner c)
    {
        if (c?.source == null) return;
        if (c.source.clip == null) return;

        c.source.loop = true;

        if (!c.source.isPlaying)
            c.source.Play();
    }

    private void StopCorner(Corner c)
    {
        if (c?.source == null) return;
        if (c.source.isPlaying)
            c.source.Stop();
    }

    private void ForceLoop(Corner c)
    {
        if (c?.source == null) return;
        c.source.loop = true;
        c.source.playOnAwake = false;
        c.source.spatialBlend = 0f; // 2D bed by default
    }

    private void UpdateDebugText(float now)
    {
        if (!debugOverlay) return;

        float hz = Mathf.Max(1f, debugRefreshHz);
        if (now < _nextDebugUpdate) return;
        _nextDebugUpdate = now + 1f / hz;

        var sb = new StringBuilder(900);

        sb.AppendLine($"CrowdXYEngine: {name}");
        sb.AppendLine($"Playing: {_isPlaying}   Smooth: {mixSmoothing:F1}   EqualPower: {equalPowerFades}");
        sb.AppendLine($"Volumes: Master={masterVolume:F2} Beds={bedsVolume:F2} Sweeteners={sweetenersMasterVolume:F2}");
        sb.AppendLine($"XY: ({x:F3}, {y:F3})");
        sb.AppendLine($"Weights TL/TR/BR/BL: {_wTL:F3} / {_wTR:F3} / {_wBR:F3} / {_wBL:F3}");
        sb.AppendLine("");

        AppendCorner(sb, "TL (Lo)", topLeft);
        AppendCorner(sb, "TR (Mid)", topRight);
        AppendCorner(sb, "BR (Hi)", bottomRight);
        AppendCorner(sb, "BL (Boo)", bottomLeft);

        sb.AppendLine("");
        sb.AppendLine($"Sweeteners: enabled={sweetenersEnabled} clips={(sweeteners == null ? 0 : sweeteners.Count)} " +
                      $"minEnergy={sweetenerMinEnergy:F2} nextIn={(Mathf.Max(0f, _tNextSweetener - now)):F1}s");

        if (sweetenerSource != null)
            sb.AppendLine($"SweetenerSource: playing={sweetenerSource.isPlaying} oneShotVol={sweetenerVolume:F2} src={sweetenerSource.name}");

        sb.AppendLine("");
        sb.AppendLine($"Keys: Toggle={toggleKey}  SweetenerTest={playSweetenerNowKey}");

        _debugText = sb.ToString();
    }

    private static void AppendCorner(StringBuilder sb, string label, Corner c)
    {
        if (c == null || c.source == null)
        {
            sb.AppendLine($"{label}: (missing source)");
            return;
        }

        sb.AppendLine($"{label}: playing={c.source.isPlaying} vol={c.source.volume:F3} pitch={c.source.pitch:F3} " +
                      $"trim={c.volumeTrim:F2} (pitch={c.pitch:F2} speed={c.speed:F2}) clip={(c.source.clip ? c.source.clip.name : "None")}");
    }

    void OnGUI()
    {
        if (!debugOverlay) return;

        // Always visible hint
        GUI.Label(new Rect(debugScreenPos.x, debugScreenPos.y, 400f, 24f),
                  "Press 1 to toggle the info screen");

        if (!_infoVisible) return;

        var rect = new Rect(debugScreenPos.x, debugScreenPos.y + 26f, 780f, 340f);
        GUI.TextArea(rect, _debugText);
    }
}
