using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDirector : MonoBehaviour {

    [SerializeField] private GameObject levelSectionTile;

    [SerializeField] private int viewCentreX, viewCentreZ, viewRadius = 6;

    private TextAsset[] levelDefs;
    private string[] currentLevelDef;
    private LevelSection[,] currentLevel;
    
	void Start () {
        levelDefs = Resources.LoadAll<TextAsset>("LevelDefs/");
        print("Found " + levelDefs.Length + " level definitons");
        
        currentLevelDef = levelDefs[0].text.Split('\n');
        currentLevel = new LevelSection[currentLevelDef.Length, currentLevelDef[0].Length];
        print("Initialising level " + 1 + " of size " + currentLevelDef.Length + " x " + currentLevelDef[0].Length);

        Debug.Break();
	}
	
	void Update () {

        // Move view
        CheckVieweingRadiusPreconditions();
        for (int z = viewCentreZ - viewRadius; z <= viewCentreZ + viewRadius; z++)
            for (int x = viewCentreX - viewRadius; x <= viewCentreX + viewRadius; x++) {
                int i = z % currentLevel.GetLength(0);
                int j = x % currentLevel.GetLength(1);
                if (i < 0) i += currentLevel.GetLength(0);
                if (j < 0) j += currentLevel.GetLength(1);
                GetLevelSection(i, j).transform.position = new Vector3(x, 0, z);
            }

    }

    private LevelSection GetLevelSection(int i, int j) {
        if (currentLevel[i, j] == null) {
            currentLevel[i, j] = Instantiate(levelSectionTile, transform).GetComponent<LevelSection>();
            currentLevel[i, j].Init(i, j, currentLevelDef[i][j]);
        }
        return currentLevel[i, j];
    }

    private void CheckVieweingRadiusPreconditions() {
        // Anti-stupid
        if (Mathf.Min(currentLevel.GetLength(0), currentLevel.GetLength(1)) < 2 * viewRadius + 1)
            Debug.LogWarning("Current level too small for given view radius");
        if (viewRadius < 0)
            Debug.LogWarning("Negative view radius makes no sense");
    }
}
