using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] float initialSphereHeight;
    [Range(-1,120)]
    [SerializeField] int targetFPS;
    [Range(0f,5f)]
    [SerializeField] float timeScale;
    [Range(0.01f,.25f)]
    [SerializeField] float fixedDeltaTime;
    [Range(0f,.5f)]
    [SerializeField] float captureDeltaTime;

    [Range(0f, .1f)]
    [SerializeField] float fixedProcessingTime;
    [Range(0f, .1f)]
    [SerializeField] float frameProcessingTime;

    [Range(0,5)]
    [SerializeField] int qualityLevel;
    [Range(0,2)]
    [SerializeField] int rigidbodyInter;
    [Range(0,4)]
    [SerializeField] int vSyncCount;
    
    [Range(1,100)]
    [SerializeField] int ballCount;
    [SerializeField] GameObject ballPrefab;

    int updateCount;
    float lastFPSUpdatesTime;
    //float lastFixedUpdatesTime;
    int fixedUpdateCount;
    float lastFPS;
    float lastFFPS;

    string settingsStr;

    [SerializeField] GUISkin skin;

    private float lastSpawnTime;
    private int curBalls;

    void Awake()
    {
        curBalls = 1;
        lastSpawnTime = Time.fixedTime;
        
        UpdateSettingsStr();

        ApplySettings();
    }

    void UpdateSettingsStr() {
        settingsStr = string.Format("Settings: targetF[P]S: {0}, [v]SyncCount: {1}, ScreenHZ: {2}, [f]ixedDeltaTime: {3}, frameProcessingTime [Y]: {10}, fixedProcessing[T]ime: {9}, Time[S]cale: {4}, [C]aputreDT: {5}, Rigidbody[I]nterplation: {6}, [Q]ualityLevel: {7}, [B]alls: {8}",
                                    Application.targetFrameRate,
                                    QualitySettings.vSyncCount,
                                    Screen.currentResolution.refreshRate,
                                    Time.fixedDeltaTime,
                                    Time.timeScale,
                                    Time.captureDeltaTime,
                                    rigidbodyInter,
                                    QualitySettings.GetQualityLevel(),
                                    ballCount,
                                    fixedProcessingTime,
                                    frameProcessingTime);

        Debug.Log(settingsStr);
    }

    void ApplySettings() {
        Application.targetFrameRate = targetFPS;
        QualitySettings.SetQualityLevel(qualityLevel, true);
        QualitySettings.vSyncCount = vSyncCount;
        Time.fixedDeltaTime = fixedDeltaTime;
        Time.timeScale = timeScale;
        Time.captureDeltaTime = captureDeltaTime;

        lastFPSUpdatesTime = Time.unscaledTime;
        fixedUpdateCount = updateCount = 0;

        UpdateSettingsStr();

        foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>()) {
            rb.interpolation = (RigidbodyInterpolation)rigidbodyInter;
        }
    }

    void ResetBalls() {
        foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>()) {
            GameObject.Destroy(rb.gameObject);
            curBalls--;
        }
    }

    void SpawnBall() {
        float height = Random.Range(0.5f * initialSphereHeight, 1.5f * initialSphereHeight);
        GameObject newball = GameObject.Instantiate(ballPrefab, Vector3.up * height, Quaternion.identity, transform);
        Rigidbody rb = newball.GetComponent<Rigidbody>();
        Vector3 v = Random.insideUnitSphere;
        v.z = v.y;
        v.y = 0f;
        v = v.normalized * 2;
        rb.velocity = v;
        rb.interpolation = (RigidbodyInterpolation)rigidbodyInter;
        curBalls++;
    }

    void FixedUpdate() {
        ++fixedUpdateCount;

        if (Time.fixedTime - lastSpawnTime > 0.05f && curBalls < ballCount) {
            SpawnBall();
            lastSpawnTime = Time.fixedTime;
        }

        foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>()) {
            if (rb.position.y < -1f) {
                GameObject.Destroy(rb.gameObject);
                curBalls--;
            }
        }

        if (fixedProcessingTime > 0f)
        {
            Thread.Sleep(Mathf.RoundToInt(fixedProcessingTime * 1000));
        }
    }

    float[] timeScaleSteps = new float[] { 0f, 0.1f, 0.2f, 0.5f, 1f, 2f, 5f };
    int[] targetFPSSteps = new int[] { -1, 1, 10, 30, 60, 90, 120 };
    float[] fixedDeltaTimeSteps = new float[] { 0.01f, 0.02f, 0.1f, 0.25f };
    float[] CaptureDeltaTimeSteps = new float[] { 0f, 0.01f, 0.02f, 0.1f, 0.25f, 0.5f };
    int[] vSyncCountSteps = new int[] { 0, 1, 2, 3, 4 };
    int[] rigidBodyInterSteps = new int[] { 0, 1, 2 };
    int[] qualityLevelSteps = new int[] { 0, 1, 2, 3, 4, 5 };
    int[] ballCountSteps = new int[] { 1, 11, 21, 41, 81, 101 };
    float[] frameProcessingTimeSteps = new float[] { 0f, 0.005f, 0.01f, 0.02f, 0.05f, 0.1f };
    float[] fixedProcessingTimeSteps = new float[] { 0f, 0.005f, 0.01f, 0.02f, 0.05f, 0.1f };

    private float NextStepValue(float curVal, float[] steps) {
        for (int i = 0; i < steps.Length; i++) {
            if (curVal < steps[i]) {
                return steps[i];
            }
        }

        return steps[0];
    }

    private int NextStepValue(int curVal, int[] steps) {
        for (int i = 0; i < steps.Length; i++) {
            if (curVal < steps[i]) {
                return steps[i];
            }
        }

        return steps[0];
    }
    

    void Update() {
        ++updateCount;
        if (Time.unscaledTime - lastFPSUpdatesTime > 1f) {
            lastFPSUpdatesTime = Time.unscaledTime;
            lastFPS = updateCount;
            updateCount = 0;
            lastFFPS = fixedUpdateCount;
            fixedUpdateCount = 0;
        }

        bool settingsChanged = false;

        if (Input.GetKeyDown(KeyCode.R)) {
            ResetBalls();
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            vSyncCount = NextStepValue(vSyncCount, vSyncCountSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            targetFPS = NextStepValue(targetFPS, targetFPSSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            fixedDeltaTime = NextStepValue(fixedDeltaTime, fixedDeltaTimeSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.S)) {
            timeScale = NextStepValue(timeScale, timeScaleSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            captureDeltaTime = NextStepValue(captureDeltaTime, CaptureDeltaTimeSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            rigidbodyInter = NextStepValue(rigidbodyInter, rigidBodyInterSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            qualityLevel = NextStepValue(qualityLevel, qualityLevelSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            ballCount = NextStepValue(ballCount, ballCountSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            fixedProcessingTime = NextStepValue(fixedProcessingTime, fixedProcessingTimeSteps);
            settingsChanged = true;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            frameProcessingTime = NextStepValue(frameProcessingTime, frameProcessingTimeSteps);
            settingsChanged = true;
        }

        if (settingsChanged) {
            ApplySettings();
        }

        if (frameProcessingTime > 0f) { 
            Thread.Sleep(Mathf.RoundToInt(frameProcessingTime * 1000));
        }
    }

    void OnValidate() {
        Debug.Log("onvalidate");
        ApplySettings();
    }

    void OnGUI()
    {
        GUI.skin = skin;
        GUILayout.BeginArea(new Rect(3 * Screen.width / 4, 0, Screen.width / 4, Screen.height));
        GUILayout.BeginVertical();
        GUILayout.Label("FPS:  " + lastFPS);
        GUILayout.Label("FFPS: " + lastFFPS);
        GUILayout.Label("realT: " + Time.realtimeSinceStartup);
        GUILayout.Label("unscaledT: " + Time.unscaledTime);
        GUILayout.Label(string.Format("fixedT: {0:0.00}", Time.fixedTime));
        GUILayout.Label(string.Format("gameT: {0:0.00}", Time.time));
        GUILayout.Label("Display Refresh Rate: " + Screen.currentResolution.refreshRate);
        GUILayout.EndVertical();
        GUILayout.EndArea();

        bool settingsChanged = false;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width / 7, Screen.height));
        GUILayout.BeginVertical();
        //GUILayout.Label(settingsStr);
        {
            GUILayout.BeginHorizontal();
            float newTargetFPS = GUILayout.HorizontalSlider(targetFPS, -1f, 120f);
            if (Mathf.RoundToInt(newTargetFPS) != targetFPS) {
                targetFPS = Mathf.RoundToInt(newTargetFPS);
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newFixedDeltaTime = GUILayout.HorizontalSlider(fixedDeltaTime, 0.01f, 0.25f);
            if (newFixedDeltaTime != fixedDeltaTime) {
                fixedDeltaTime = newFixedDeltaTime;
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newTimeScale = GUILayout.HorizontalSlider(timeScale, 0f, 5f);
            if (newTimeScale != timeScale) {
                timeScale = newTimeScale;
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newCaptureDeltaTime = GUILayout.HorizontalSlider(captureDeltaTime, 0f, 5f);
            if (newCaptureDeltaTime != captureDeltaTime) {
                captureDeltaTime = newCaptureDeltaTime;
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newVSyncCount = GUILayout.HorizontalSlider(vSyncCount, 0, 4);
            if (Mathf.RoundToInt(newVSyncCount) != vSyncCount) {
                vSyncCount = Mathf.RoundToInt(newVSyncCount);
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newRigidbodyInter = GUILayout.HorizontalSlider(rigidbodyInter, 0, 2);
            if (Mathf.RoundToInt(newRigidbodyInter) != rigidbodyInter) {
                rigidbodyInter = Mathf.RoundToInt(newRigidbodyInter);
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newQualityLevel = GUILayout.HorizontalSlider(qualityLevel, 0, 5);
            if (Mathf.RoundToInt(newQualityLevel) != qualityLevel) {
                qualityLevel = Mathf.RoundToInt(newQualityLevel);
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newBallCount = GUILayout.HorizontalSlider(ballCount, 1, 100);
            if (Mathf.RoundToInt(newBallCount) != ballCount) {
                ballCount = Mathf.RoundToInt(newBallCount);
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newFixedProcessingTime = GUILayout.HorizontalSlider(fixedProcessingTime, 0, 0.1f);
            if (newFixedProcessingTime != fixedProcessingTime)
            {
                fixedProcessingTime = newFixedProcessingTime;
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float newFrameProcessingTime = GUILayout.HorizontalSlider(frameProcessingTime, 0, 0.1f);
            if (newFrameProcessingTime != frameProcessingTime)
            {
                frameProcessingTime = newFrameProcessingTime;
                settingsChanged = true;
            }
            GUILayout.EndHorizontal();
        } GUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width / 7, 0, 2 * Screen.width / 7, Screen.height));
        GUILayout.BeginVertical(); {
            GUILayout.Label("TargetF[P]S: " + targetFPS);
            GUILayout.Label(string.Format("[F]ixed Delta Time: {0:0.00}", fixedDeltaTime));
            GUILayout.Label(string.Format("Time [S]cale: {0:0.00}", timeScale));
            GUILayout.Label(string.Format("[C]apture Delta Time: {0:0.00}", captureDeltaTime));
            GUILayout.Label("[V]SyncCount: " + vSyncCount);
            GUILayout.Label("Rigidbody [I]nterpolation: " + (rigidbodyInter == 0 ? "None" : (rigidbodyInter == 1 ? "Inter" : "exter")));
            GUILayout.Label("[Q]uality Level: " + qualityLevel);
            GUILayout.Label(string.Format("[B]alls: {0} (cur: {1})", ballCount, curBalls));
            GUILayout.Label(string.Format("FixedProcessing[T]ime: {0:0.00})", fixedProcessingTime));
            GUILayout.Label(string.Format("FrameProcessingTime[Y]: {0:0.00})", frameProcessingTime));
            GUILayout.Label("[R]eset");
        } GUILayout.EndVertical();
        GUILayout.EndArea();

        if (settingsChanged) {
            ApplySettings();
        }
    }
}
