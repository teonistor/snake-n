using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System;

public class World : MonoBehaviour {
    internal const float initialSpeed = 5.2f;
    internal const float initialEnergy = 25f;
    internal const float speedIncrementBig = 1.25f;
    internal const float speedIncrementSmall = 0.5f;
    internal const float maxInputSpeedStamina = 1.5f;

    const int OneEnergyPoints = 10;
    const int FullEnergyPoints = 50;
    const int OneEnergyEnergy = 7;
    const int MaximumEnergy = 55;
    const int PenetrableWallEnergy = -15;
    const int viewCentreShift = 4;

    internal static readonly Color skyNormal = new Color(0.8f, 0.8f, 0.8f);
    internal static readonly Color skyNoControl = new Color(0.06574395f, 0.2440461f, 0.2794118f);
    internal static readonly Color skyTimeUp = new Color(0.9f, 0.2f, 0.3f);
    internal static readonly Color skyLevelComplete = new Color(0.25f, 0.5f, 0.25f);

    [SerializeField] private GameObject levelSectionTile;
    [SerializeField] private int viewRadius; // 9 would do for a start

    private TextAsset[] levelDefs;
    private static Level currentLevel;

    public static string CurrentLevelName { get {
        return currentLevel.Name;
    }}

    public static int CurrentLevelRequiredPoints { get {
        return currentLevel.RequiredPoints;
    }}

    public static int[] CurrentLevelOpeningMoves { get {
        return currentLevel.OpeningMoves;
    }}

    public static int CurrentPoints { get; private set; }

    public static GameState GameState { get; private set; }

    public static float CurrentLevelTimeLimit { get; private set; }

    internal static float CurrentBaseSpeed { get {
            return _curBaseSpeed;
        } private set {
            _curBaseSpeed = value;
            if (OnSpeedChange != null) OnSpeedChange();
        }
    }

    internal static float CurrentInputSpeed { get {
            return _curInputSpeed;
        } private set {
            _curInputSpeed = value;
            if (OnSpeedChange != null) OnSpeedChange();
        }
    }

    public static float CurrentTotalSpeed { get {
        return CurrentBaseSpeed + CurrentInputSpeed;
    }}

    internal static float currentEnergy = initialEnergy;
    internal static float targetEnergy = initialEnergy;
    internal static int currentLevelIndex = 0;
    internal static int currentLives = 3;
    internal static float currentInputSpeedStamina = maxInputSpeedStamina;
    private static World Instance;

    static float _curBaseSpeed = initialSpeed;
    static float _curInputSpeed = 0f;
    static float minSpeed = 3.4f;
    static float maxSpeed = 7f;
    internal static event Action OnSpeedChange;

    Coroutine changeSkyRepeatedly; // Don't make static

    void Awake () {
        string s = "150";
        for (int i = 0; i < 35; i++)
            for (int j = 0; j < 35; j++)
                if ((i * 2 % 5 + 1) % 5 == j % 5)
                    s += " " + i + "," + j;
        print(s);

        Time.timeScale = 1f;
        if (CurrentBaseSpeed == 0f)
            CurrentBaseSpeed = initialSpeed;
        GameState = GameState.Prologue;

        Instance = this;
        SceneManager.LoadSceneAsync("InGameUI", LoadSceneMode.Additive);
    }

    void Start () {
        CurrentPoints = 0;

        levelDefs = Resources.LoadAll<TextAsset>("LevelDefs/");
        print("Found " + levelDefs.Length + " level definitons");

        //currentLevelDef = levelDefs[0].text.Split('\n');
        if (currentLevelIndex >= levelDefs.Length) {
            Debug.LogWarning("Level index " + currentLevelIndex + " larger than highest available level. Restarting from 0.");
            currentLevelIndex = 0;
        }
        currentLevel = new Level(levelDefs[currentLevelIndex].text);
        
        CurrentLevelTimeLimit = Time.time + currentLevel.Timeout;
    }

    void Update() {

        // Input: Pause/cancel
        if (Input.GetButtonDown("Cancel")) {
            if (GameState == GameState.Playing) {
                GameState = GameState.Paused;
                Time.timeScale = 0f;
            } else if (GameState == GameState.Paused) {
                GameState = GameState.Playing;
                Time.timeScale = 1f;
            }
        }

        // Input: Accelerate/slow: controls
        if (Input.GetButtonDown("Fast")) {
            CurrentInputSpeed = speedIncrementBig;
        } else if (Input.GetButtonDown("Slow")) {
            CurrentInputSpeed = -speedIncrementBig;
        } else if (Input.GetButtonUp("Slow") || Input.GetButtonUp("Fast")) {
            CurrentInputSpeed = 0f;
        }

        // Input: Accelerate/slow: stamina limitation enforcement
        if (CurrentInputSpeed == 0f) {
            if (currentInputSpeedStamina < maxInputSpeedStamina)
                currentInputSpeedStamina += Time.deltaTime;
        } else if (currentInputSpeedStamina > 0f) {
            currentInputSpeedStamina -= Time.deltaTime;
        } else {
            CurrentInputSpeed = 0f;
        }

        // Slowly reduce snake length
        // FIXME seems a regular framerate is too slow for this anyway...
        if (currentEnergy != targetEnergy) {
            currentEnergy = Mathf.MoveTowards(currentEnergy, targetEnergy, Time.deltaTime * 66);
        }

        // Check level completed (only launch coroutine once)
        if (CurrentPoints > CurrentLevelRequiredPoints && GameState==GameState.Playing) {
            StartCoroutine(NextLevel());
        }

        // Check death (only launch coroutine once)
        if ((currentEnergy <= 0 || Time.time > CurrentLevelTimeLimit) && GameState == GameState.Playing) {
            StartCoroutine(GameOver());
        }

        // Sky change to red and back when time limit approaching
        if (Time.time+5f > CurrentLevelTimeLimit && changeSkyRepeatedly == null && GameState == GameState.Playing) {
            changeSkyRepeatedly = StartCoroutine(ChangeSkyRepeatedly(skyTimeUp));
        }
    }

    public static LevelTile InstantiateLevelSection() {
        return Instantiate(Instance.levelSectionTile, Instance.transform).GetComponent<LevelTile>();
    }

    public static LevelTile SnakeHeadEnters (int x, int z, int movementCode) {
        MoveView(x, z, movementCode);
        return SnakeBodyEnters(x, z);
    }

    public static LevelTile SnakeBodyEnters (int x, int z) {
        int i, j;
        WorldToLevelCoords(x, z, out i, out j);
        return currentLevel[i, j]; // Rightfully assuming the head has already been wherever a body part is entering, we need not explicitly ensure the level section exists at that location
    }

    public static void StartGame (int levelIndex = 0) {
        currentLevelIndex = levelIndex;
        currentLives = 3;
        currentEnergy = targetEnergy = initialEnergy;
        CurrentBaseSpeed = initialSpeed;
    }

    public static void CollectOneEnergy () {
        if (GameState == GameState.Playing) {
            CurrentPoints += OneEnergyPoints;
            currentEnergy = targetEnergy =
                Mathf.Min(targetEnergy + OneEnergyEnergy, MaximumEnergy);
        }
    }

    public static void CollectAllEnergy () {
        if (GameState == GameState.Playing) {
            CurrentPoints += FullEnergyPoints;
            currentEnergy = targetEnergy = MaximumEnergy;
        }
    }

    public static void CollectLife () {
        if (GameState == GameState.Playing) {
            currentLives++;
        }
    }

    public static void CollectSpeedUp () {
        if (GameState == GameState.Playing && CurrentBaseSpeed < maxSpeed) {
            CurrentBaseSpeed += speedIncrementSmall;
        }
    }

    public static void CollectSpeedDown () {
        if (GameState == GameState.Playing && CurrentBaseSpeed > minSpeed) {
            CurrentBaseSpeed -= speedIncrementSmall;
        }
    }

    public static void HitPenetrableWall() {
        if (GameState == GameState.Playing) {
            targetEnergy += PenetrableWallEnergy;
            if (targetEnergy < 0f) {
                targetEnergy = 0f;
                CurrentBaseSpeed = 0f;
            }
        }
    }

    public static void HitSelfOrImpenetrableWall () {
        if (GameState == GameState.Playing) {
            targetEnergy = 0f;
            CurrentBaseSpeed = 0f;
        }
    }

    public static void OpeningMovesFinished () {
        GameState = GameState.Playing;
        Instance.StartCoroutine(ChangeSkyOnce(skyNormal));
    }

    IEnumerator NextLevel () {
        GameState = GameState.LevelComplete;
        StartCoroutine(ChangeSkyOnce(skyLevelComplete));

        yield return new WaitForSeconds(2.1f);
        currentLevelIndex++;
        SceneManager.LoadScene("One");
    }

    IEnumerator GameOver () {
        GameState = GameState.GameOver;
        StartCoroutine(ChangeSkyOnce(skyNoControl));
        currentLives--;

        yield return new WaitForSeconds(2.2f);
        if (currentLives > 0) {
            targetEnergy = currentEnergy = initialEnergy;
            SceneManager.LoadSceneAsync("One");
        } else {
            SceneManager.LoadScene("Menu");
        }
    }

    static IEnumerator ChangeSkyOnce (Color target, float duration=0.4f) {
        WaitForSeconds wait = new WaitForSeconds(1 / 30f);
        Color start = RenderSettings.ambientSkyColor;
        for (float t=0f; t < 1f; t += 1/ 30f / duration) {
            RenderSettings.ambientSkyColor = Color.Lerp(start, target, t);
            yield return wait;
        }
        RenderSettings.ambientSkyColor = target;
    }

    static IEnumerator ChangeSkyRepeatedly (Color target, float duration=0.5f) {
        WaitForSeconds wait = new WaitForSeconds(1 / 30f);
        Color start = RenderSettings.ambientSkyColor;
        while (GameState == GameState.Playing) {
            for (float t = 0f; t < 1f && GameState == GameState.Playing; t += 1 / 30f / duration) {
                RenderSettings.ambientSkyColor = Color.Lerp(start, target, t);
                yield return wait;
            }
            for (float t = 1f; t > 0f && GameState == GameState.Playing; t -= 1 / 30f / duration) {
                RenderSettings.ambientSkyColor = Color.Lerp(start, target, t);
                yield return wait;
            }
        }
        // This whole hack of sky-changing routines is hacky and relies on correct but unclear assumptions
        // changeSkyRepeatedly = null;
        RenderSettings.ambientSkyColor = target;
    }

    private static void WorldToLevelCoords (int x, int z, out int i, out int j) {
        i = z % currentLevel.GetLength(0);
        j = x % currentLevel.GetLength(1);
        if (i < 0) i += currentLevel.GetLength(0);
        if (j < 0) j += currentLevel.GetLength(1);
    }

    private static void MoveView (int viewCentreX, int viewCentreZ, int movementCode) {

        // Adjust centre for orientation based on current movement
        switch (movementCode) {
            case 34:
            case 21: viewCentreZ += viewCentreShift; viewCentreX -= viewCentreShift; break;
            case 14:
            case 23: viewCentreZ -= viewCentreShift; viewCentreX -= viewCentreShift; break;
            case 24: viewCentreX -= viewCentreShift; break;
            case 43:
            case 12: viewCentreZ -= viewCentreShift; viewCentreX += viewCentreShift; break;
            case 13: viewCentreZ -= viewCentreShift; break;
            case 41:
            case 32: viewCentreZ += viewCentreShift; viewCentreX += viewCentreShift; break;
            case 31: viewCentreZ += viewCentreShift; break;
            case 42: viewCentreX += viewCentreShift; break;
            default: Debug.LogWarning("Unexpected movementCode in World.AdjustViewCentreForMovement: " + movementCode); break;
        }

        // Iterate over relevant region and put tiles where they belong
        int i, j;
        CheckVieweingRadiusPreconditions();
        for (int z = viewCentreZ - Instance.viewRadius; z <= viewCentreZ + Instance.viewRadius; z++)
            for (int x = viewCentreX - Instance.viewRadius; x <= viewCentreX + Instance.viewRadius; x++) {
                WorldToLevelCoords(x, z, out i, out j);
                currentLevel[i, j].transform.position = new Vector3(x, 0, z);
            }

    }

    private static void CheckVieweingRadiusPreconditions () {
        // Anti-stupid
        if (Mathf.Min(currentLevel.GetLength(0), currentLevel.GetLength(1)) < 2 * Instance.viewRadius + 1)
            Debug.LogWarning("Current level too small for given view radius" + Instance.viewRadius);
        if (Instance.viewRadius < 0)
            Debug.LogWarning("Negative view radius " + Instance.viewRadius + " makes no sense");
    }
}
