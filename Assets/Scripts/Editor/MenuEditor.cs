using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuScripts))]
public class MenuEditor : Editor
{
    private MenuScripts MS;

    private bool activeDev;

    private void OnEnable()
    {
        MS = (MenuScripts)target;
    }

    public override void OnInspectorGUI()
    {
        activeDev = GUILayout.Toggle(activeDev, "Активировать DEV-режим");
        if (activeDev) { base.OnInspectorGUI(); }

        EditorGUILayout.Space();

        MS.nameGameText.text = EditorGUILayout.TextField("Название вашей игры", MS.nameGameText.text);
        MS.nameGameText.color = EditorGUILayout.ColorField("Цвет названия", MS.nameGameText.color);
        MS.backgroundGame.sprite = (Sprite)EditorGUILayout.ObjectField("Задний фон игры", MS.backgroundGame.sprite, typeof(Sprite), false);

        EditorGUILayout.Space();

        MS.openAllPuzzle = GUILayout.Toggle(MS.openAllPuzzle, "Открыть все пазлы");

        EditorGUILayout.Space();

        MS.SV.saveActive = GUILayout.Toggle(MS.SV.saveActive, "Использовать сохранения");

        EditorGUILayout.Space();

        int maxItemsPerRow = 3;
        EditorGUILayout.BeginVertical();
        if (MS.PS.puzzles.Count > 0)
        {
            for (int i = 0; i < MS.PS.puzzles.Count; i++)
            {
                if (i % maxItemsPerRow == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                }
                MS.PS.puzzles[i].puzzleSprite = (Sprite)EditorGUILayout.ObjectField(MS.PS.puzzles[i].puzzleSprite, typeof(Sprite), false, GUILayout.Width(90), GUILayout.Height(90));
                MS.PS.puzzles[i].puzzleBttn.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = MS.PS.puzzles[i].puzzleSprite;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Х", GUILayout.Width(30))) 
                {
                    DestroyImmediate(MS.PS.puzzles[i].puzzleBttn); MS.PS.puzzles.Remove(MS.PS.puzzles[i]); break;
                } 
                GUI.backgroundColor = Color.white;
                if (i % maxItemsPerRow == maxItemsPerRow - 1 || i == MS.PS.puzzles.Count - 1)
                {
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        else { EditorGUILayout.LabelField("Здесь пока пусто, но вы можете добавить картинок для пазлов!"); }
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button("Добавить")) 
        {
            MS.PS.puzzles.Add(new Puzzles());
            GameObject obj = Instantiate(MS.prefabBttn, MS.content.transform);
            for (int i = 0; i < MS.PS.puzzles.Count; i++)
            {
                if (MS.PS.puzzles[i].puzzleBttn == null)
                {
                    MS.PS.puzzles[i].puzzleBttn = obj;
                    MS.PS.puzzles[i].puzzleSprite = obj.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite;
                }
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();
    }
}
