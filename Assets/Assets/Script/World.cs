using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class World : MonoBehaviour {
    const int OneEnergyPoints = 10;
    const int FullEnergyPoints = 50;
    const int OneEnergyEnergy = 7;
    const int MaximumEnergy = 66;
    const int PenetrableWallEnergy = -15;

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

    public static int CurrentPoints { get; private set; }

    public static GameState GameState { get; private set; }

    internal static float currentEnergy = 25;
    internal static float targetEnergy = 25;
    internal static int currentLevelIndex = 0;
    private static World Instance;

    void Awake () {
        // TODO Initial movements
        // GameState = GameState.Prologue;
        GameState = GameState.Playing;
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

        //Debug.Break();
    }

    void Update() {
        //print("Current energy: " + currentEnergy);
        if (currentEnergy != targetEnergy) {
            currentEnergy = Mathf.MoveTowards(currentEnergy, targetEnergy, Time.deltaTime * 10);
        }

        if (CurrentPoints > CurrentLevelRequiredPoints) {
            StartCoroutine(NextLevel());
        }

        if (currentEnergy <= 0) {
            StartCoroutine(GameOver());
        }
    }

    public static LevelSection InstantiateLevelSection() {
        return Instantiate(Instance.levelSectionTile, Instance.transform).GetComponent<LevelSection>();
    }

    public static LevelSection SnakeHeadEnters (int x, int z) {
        MoveView(x, z);
        return SnakeBodyEnters(x, z);
    }

    public static LevelSection SnakeBodyEnters (int x, int z) {
        int i, j;
        WorldToLevelCoords(x, z, out i, out j);
        return currentLevel[i, j]; // Rightfully assuming the head has already been wherever a body part is entering, we need not explicitly ensure the level section exists at that location
    }

    public static void CollectOneEnergy () {
        CurrentPoints += OneEnergyPoints;
        currentEnergy = targetEnergy =
            Mathf.Min(targetEnergy + OneEnergyEnergy, MaximumEnergy);
    }

    public static void CollectAllEnergy() {
        CurrentPoints += FullEnergyPoints;
        currentEnergy = targetEnergy = MaximumEnergy;
    }

    public static void HitPenetrableWall() {
        targetEnergy += PenetrableWallEnergy;
    }

    public static void Die() {
        targetEnergy = 0;
    }

    IEnumerator NextLevel () {
        GameState = GameState.LevelComplete;
        yield return new WaitForSeconds(9f);
        currentLevelIndex++;
        SceneManager.LoadSceneAsync("One");
    }

    IEnumerator GameOver () {
        GameState = GameState.GameOver;
        yield return new WaitForSeconds(9f);
        SceneManager.LoadSceneAsync("Menu");
    }

    private static void WorldToLevelCoords (int x, int z, out int i, out int j) {
        i = z % currentLevel.GetLength(0);
        j = x % currentLevel.GetLength(1);
        if (i < 0) i += currentLevel.GetLength(0);
        if (j < 0) j += currentLevel.GetLength(1);
    }

    private static void MoveView (int viewCentreX, int viewCentreZ) {
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
