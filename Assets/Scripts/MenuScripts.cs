using UnityEngine;
using UnityEngine.UI;

public class MenuScripts : MonoBehaviour
{
    public static MenuScripts Init;
    public PuzzleScript PS;
    public SaveScript SV;

    public GameObject puzzleObj; // 
    public GameObject puzzleDoneObj; //
    public GameObject puzzleAllDoneObj; //
    public GameObject mainMenuObj; //
    public GameObject puzzleCountObj; //
    public GameObject gameMenu;
    public GameObject resumePuzzle;

    public Transform content;
    public GameObject prefabBttn;

    public Text nameGameText; //
    public Image backgroundGame; //

    [HideInInspector] public bool openAllPuzzle;

    private void Awake()
    {
        Init = this;
    }

    private void Start()
    {
        PS.puzzles[0].puzzleActive = true;
        OpenPuzzlePan();
        if (SV.SV.savePuzzle)
        {
            resumePuzzle.SetActive(true);
        }
    }

    public void ChoicePuzzle(int index) // Выбор пазлов
    {
        if(PS.puzzles[index].puzzleActive)
        {
            PS.puzzleNumber = index;
            puzzleCountObj.SetActive(true);
        }
        SV.SV.puzzleNumber = index;
    }

    public void ChoicePuzzleCount(int index) // Выбор количества пазлов 
    {
        if(index == 0) { PS.columns = 2; PS.lines = 2; }
        else if (index == 1) { PS.columns = 3; PS.lines = 3; }
        else if (index == 2) { PS.columns = 4; PS.lines = 4; }
        else if (index == 3) { PS.columns = 5; PS.lines = 5; }
        else if (index == 4) { PS.columns = 6; PS.lines = 6; }
        puzzleCountObj.SetActive(false); mainMenuObj.SetActive(false); puzzleObj.SetActive(true);
        SV.SV.puzzleCount = index;
        PS.StartGeneratePuzzle();
    }

    public void CancelChoicePuzzleCount() // Отмена выбора
    {
        puzzleCountObj.SetActive(false);
    }

    public void ExitMenu() // Выход в меню
    {
        OpenPuzzlePan();
        puzzleDoneObj.SetActive(false);
        puzzleAllDoneObj.SetActive(false);
        puzzleObj.SetActive(false);
        resumePuzzle.SetActive(false);
        mainMenuObj.SetActive(true);
    }

    public void ResetPuzzle() // Сброс пазлов
    {
        PS.Clear(); PS.StartGeneratePuzzle();
    }

    public void NextPuzzle() // Следующий пазл
    {
        PS.puzzleNumber++;
        if(PS.puzzleNumber >= PS.puzzles.Count)
        {
            puzzleAllDoneObj.SetActive(true);
        }
        else { SV.SV.puzzleNumber = PS.puzzleNumber; SV.SV.savePuzzle = false; SV.SaveGame(); PS.StartGeneratePuzzle(); }
        puzzleDoneObj.SetActive(false);
    }

    public void ResumePuzzle() // Продолжить сборку
    {
        resumePuzzle.SetActive(false);
        PS.puzzleNumber = SV.SV.puzzleNumber;
        ChoicePuzzleCount(SV.SV.puzzleCount);
    }

    void OpenPuzzlePan() //
    {
        if (!openAllPuzzle)
        {
            if (PS.puzzles.Count > 0)
            {
                for (int i = 0; i < PS.puzzles.Count; i++)
                {
                    if (!PS.puzzles[i].puzzleActive) PS.puzzles[i].puzzleBttn.transform.Find("Open").GetComponentInChildren<Image>().color = new Color32(0, 0, 0, 210);
                    else PS.puzzles[i].puzzleBttn.transform.Find("Open").GetComponentInChildren<Image>().color = new Color32(0, 0, 0, 0);
                }
            }
        }
        else
        {
            for(int i = 0; i < PS.puzzles.Count; i++)
            {
                PS.puzzles[i].puzzleActive = true;
                PS.puzzles[i].puzzleBttn.transform.Find("Open").GetComponentInChildren<Image>().color = new Color32(0, 0, 0, 0);
            }
        }
    }
}
