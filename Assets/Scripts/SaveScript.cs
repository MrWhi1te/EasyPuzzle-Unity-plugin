using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveScript : MonoBehaviour
{
    public PuzzleScript PS;
    public static SaveScript Init;

    [HideInInspector] public Save SV = new();
    [HideInInspector] public bool saveActive; //

    private void Awake()
    {
        Init = this;
        if (PlayerPrefs.HasKey("sv") && saveActive)
        {
            SV = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("sv"));
            for (int i = 0; i < PS.puzzles.Count; i++)
            {
                PS.puzzles[i].puzzleActive = SV.puzzleActive[i];
            }
        }
    }

    public void SaveGame()
    {
        for(int i = 0; i < PS.puzzles.Count;i++)
        {
            SV.puzzleActive.Add(PS.puzzles[i].puzzleActive);
        }     
        PlayerPrefs.SetString("sv", JsonUtility.ToJson(SV));
    }
    public void SavePosPuzzle()
    {
        for (int i = 0; i < PS._puzzle.Count; i++)
        {
            SV.positionPuzzle.Add(PS._puzzle[i].transform.position);
        }
        SV.savePuzzle = true;
        PlayerPrefs.SetString("sv", JsonUtility.ToJson(SV));
    }
}

[Serializable]
public class Save
{
    public List<bool> puzzleActive;
    public List<Vector3> positionPuzzle;
    public int puzzleNumber;
    public int puzzleCount;
    public bool savePuzzle;
}
