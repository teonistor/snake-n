using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDirector : MonoBehaviour {

    [SerializeField] private GameObject levelSectionTile;

    [SerializeField] private int viewRadius = 6;

    private TextAsset[] levelDefs;
    private string[] currentLevelDef;
    private LevelSection[,] currentLevel;

    public static LocalDirector Instance { get; private set; }

    void Awake () {
        Instance = this;
    }

    void Start () {
        levelDefs = Resources.LoadAll<TextAsset>("LevelDefs/");
        print("Found " + levelDefs.Length + " level definitons");

        currentLevelDef = levelDefs[0].text.Split('\n');
        currentLevel = new LevelSection[currentLevelDef.Length, currentLevelDef[0].Length];
        print("Initialising level " + 1 + " of size " + currentLevelDef.Length + " x " + currentLevelDef[0].Length);

        Debug.Break();
    }

    public Transform SnakeHeadEnters (int x, int z) {
        MoveView(x, z);
        return SnakeBodyEnters(x, z);
    }
    
    public Transform SnakeBodyEnters (int x, int z) {
        int i, j;
        WorldToLevelCoords(x, z, out i, out j);
        return currentLevel[i, j].transform; // Rightfully assuming the head has already been wherever a body part is entering, we need not explicitly ensure the level section exists at that location
    }

    private void WorldToLevelCoords (int x, int z, out int i, out int j) {
        i = z % currentLevel.GetLength(0);
        j = x % currentLevel.GetLength(1);
        if (i < 0) i += currentLevel.GetLength(0);
        if (j < 0) j += currentLevel.GetLength(1);
    }

    private void MoveView (int viewCentreX, int viewCentreZ) {
        int i, j;
        CheckVieweingRadiusPreconditions();
        for (int z = viewCentreZ - viewRadius; z <= viewCentreZ + viewRadius; z++)
            for (int x = viewCentreX - viewRadius; x <= viewCentreX + viewRadius; x++) {
                WorldToLevelCoords(x, z, out i, out j);
                GetLevelSection(i, j).transform.position = new Vector3(x, 0, z);
            }

    }

    private LevelSection GetLevelSection (int i, int j) {
        if (currentLevel[i, j] == null) {
            currentLevel[i, j] = Instantiate(levelSectionTile, transform).GetComponent<LevelSection>();
            currentLevel[i, j].Init(i, j, currentLevelDef[i][j]);
        }
        return currentLevel[i, j];
    }

    private void CheckVieweingRadiusPreconditions () {
        // Anti-stupid
        if (Mathf.Min(currentLevel.GetLength(0), currentLevel.GetLength(1)) < 2 * viewRadius + 1)
            Debug.LogWarning("Current level too small for given view radius");
        if (viewRadius < 0)
            Debug.LogWarning("Negative view radius makes no sense");
    }
}
