using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System;

public class World : MonoBehaviour {
    internal const float initialSpeed = 5.2f;
    internal const float initialEnergy = 25f;
    internal const float speedIncrementBig = 1.25f;
    internal const float speedIncrementSmall = 0.5f;

    const int OneEnergyPoints = 10;
    const int FullEnergyPoints = 50;
    const int OneEnergyEnergy = 7;
    const int MaximumEnergy = 66;
    const int PenetrableWallEnergy = -15;
    const int viewCentreShift = 4;

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
    private static World Instance;

    static float _curBaseSpeed = initialSpeed;
    static float _curInputSpeed = 0f;
    static float minSpeed = 3.4f;
    static float maxSpeed = 7f;
    internal static event Action OnSpeedChange;

    Coroutine mutex;

    void Awake () {
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

        // Input: Accelerate/slow
        // TODO restrict how long this effect lasts - perhaps in another class...
        if (Input.GetButtonDown("Fast")) {
            CurrentInputSpeed = speedIncrementBig;
        } else if (Input.GetButtonDown("Slow")) {
            CurrentInputSpeed = -speedIncrementBig;
        } else if (Input.GetButtonUp("Slow") || Input.GetButtonUp("Fast")) {
            CurrentInputSpeed = 0f;
        }
        
        // Slowly reduce snake length
        // n.b. seems a regular framerate is too slow for this anyway...
        if (currentEnergy != targetEnergy) {
            currentEnergy = Mathf.MoveTowards(currentEnergy, targetEnergy, Time.deltaTime * 66);
        }

        // Check level completed
        if (CurrentPoints > CurrentLevelRequiredPoints) {
            StartCoroutine(NextLevel());
        }

        // Check death (only launch coroutine once)
        if (currentEnergy <= 0 || Time.time > CurrentLevelTimeLimit) {
            if (mutex == null) {
                mutex = StartCoroutine(GameOver());
            }
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
    }

    IEnumerator NextLevel () {
        GameState = GameState.LevelComplete;
        yield return new WaitForSeconds(1.8f);
        currentLevelIndex++;
        SceneManager.LoadScene("One");
    }

    IEnumerator GameOver () {
        GameState = GameState.GameOver;
        currentLives--;
        yield return new WaitForSeconds(1.8f);
        if (currentLives > 0) {
            targetEnergy = currentEnergy = initialEnergy;
            SceneManager.LoadSceneAsync("One");
        } else {
            SceneManager.LoadScene("Menu");
        }
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
