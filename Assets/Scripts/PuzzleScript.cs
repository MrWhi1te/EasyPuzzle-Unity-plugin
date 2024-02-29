using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScript : MonoBehaviour
{
	public static PuzzleScript Init;
	private MenuScripts MN; // 
	private SaveScript SV;

	public SpriteRenderer picture; // Картинка (Объект), с которой будут генерироваться пазлы
	public SpriteRenderer backgroundPicture; // Фоновая картинка пазла

	[HideInInspector] public bool isGameActive; // Активна ли игра
	[HideInInspector] public int puzzleNumber; // Активный пазл
	[HideInInspector] public int columns = 2; // столбцы
	[HideInInspector] public int lines = 1; // линии

	public List<Puzzles> puzzles = new();

	private int puzzleCounter, sortingOrder;
	[HideInInspector] public List<SpriteRenderer> _puzzle = new();
	private List<Vector3> _puzzlePos = new();
	private Transform current;
	private Vector3 offset;
	private bool isWinner; // Собрал правильно

	private float targetDistance = 0.5f; // дистанция пазла от точки своего назначения, чем больше это значение, тем больше допускается неточность расположения
	private string puzzleTag = "GameController"; // тег для пазлов
	private float smooth = 5; // сглаживание, во время соединения всех пазлов

    private void Awake()
    {
		Init = this;
	}

    void Start()
	{
		MN = MenuScripts.Init;
		SV = SaveScript.Init;
		for (int i = 0; i < puzzles.Count; i++) // Цикл для назначения функций кнопок темам в меню
		{
			int temp = i;
			puzzles[i].puzzleBttn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => MN.ChoicePuzzle(temp)); // Назначение кнопкам функции слушателя
		}
	}

	void Update()
	{
        if (isGameActive) // Пока игра активна
        {
			if (isWinner) // Если пазл правильно собран
			{
				if (CheckPuzzle(0.1f) == _puzzle.Count)
				{
					Clear();
					picture.gameObject.SetActive(true);
					isGameActive = false;
					if((puzzleNumber+1) < puzzles.Count)
                    {
						puzzles[puzzleNumber+1].puzzleActive = true;
					}
					MN.puzzleDoneObj.SetActive(true);
				}
				else
				{
					for (int j = 0; j < _puzzle.Count; j++)
					{
						_puzzle[j].transform.position = Vector3.Lerp(_puzzle[j].transform.position, _puzzlePos[j], smooth * Time.deltaTime);
					}
				}
			}
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					GetPuzzle();
				}
				else if (Input.GetMouseButtonUp(0) && current)
				{
					current.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
					current = null;

					if (CheckPuzzle(targetDistance) == _puzzle.Count)
					{
						isWinner = true;
					}
				}
			}

			if (current)
			{
				Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				current.position = new Vector3(position.x, position.y, current.position.z) + new Vector3(offset.x, offset.y, 0);
			}
		}
	}

	int CheckPuzzle(float distance) // проверка всех пазлов, относительно точек назначения
	{
		int i = 0;
		for (int j = 0; j < _puzzle.Count; j++)
		{
			if (Vector3.Distance(_puzzle[j].transform.position, _puzzlePos[j]) < distance)
			{
				i++;
			}
		}
		return i;
	}

	public void Clear() // Очистка
	{
		isWinner = false;
		puzzleCounter = 0;
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
		_puzzle = new List<SpriteRenderer>();
		_puzzlePos = new List<Vector3>();
	}

	void RandomizePosition() // раскидываем пазлы по случайным точкам
	{
		float[] x = new float[_puzzle.Count];
		float[] y = new float[_puzzle.Count];

		for (int j = 0; j < _puzzle.Count; j++)
		{
            if (SV.SV.savePuzzle)
            {
				x[j] = SV.SV.positionPuzzle[j].x;
				y[j] = SV.SV.positionPuzzle[j].y;
			}
            else
            {
				x[j] = _puzzlePos[j].x;
				y[j] = _puzzlePos[j].y;
			}
		}

		float minX = Mathf.Min(x);
		float maxX = Mathf.Max(x);
		float minY = Mathf.Min(y);
		float maxY = Mathf.Max(y);

		int r = 0;
		foreach (SpriteRenderer e in _puzzle)
		{
			if (SV.SV.savePuzzle)
			{
				e.transform.position = SV.SV.positionPuzzle[r];
				r++;
			}
			else 
			{
				e.transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), e.transform.position.z);
			}
		}

		isGameActive = true;

		SV.SV.savePuzzle = false;
		SV.SV.positionPuzzle.Clear();
	}

	public void StartGeneratePuzzle()
    {
		picture.sprite = backgroundPicture.sprite = puzzles[puzzleNumber].puzzleSprite;
		picture.gameObject.SetActive(true);
		StartCoroutine(GeneratePuzzle());
	}

	IEnumerator GeneratePuzzle() // создание пазлов / нарезка текстуры
	{
		MN.gameMenu.SetActive(false);
		float SpriteWidth = picture.bounds.size.x;
		float SpriteHeight = picture.bounds.size.y;
		float screenRatio = (float)Screen.width / (float)Screen.height;
		float targetRatio = SpriteWidth / SpriteHeight;
		
		Camera.main.transform.position = new Vector3(picture.transform.position.x, picture.transform.position.y, -10f);
		
		if(screenRatio >= targetRatio)
        {
			Camera.main.orthographicSize = SpriteHeight / 2;
        }
        else
        {
			float diffSize = targetRatio / screenRatio;
			Camera.main.orthographicSize = SpriteHeight / 2 * diffSize;
        }

		// переносим размеры холста в пространство экрана
		Vector3 posStart = Camera.main.WorldToScreenPoint(new Vector3(picture.bounds.min.x, picture.bounds.min.y, picture.bounds.min.z));
        Vector3 posEnd = Camera.main.WorldToScreenPoint(new Vector3(picture.bounds.max.x, picture.bounds.max.y, picture.bounds.min.z));

        int width = (int)(posEnd.x - posStart.x);
		int height = (int)(posEnd.y - posStart.y);

		// определяем размеры пазла
		int w_cell = width / columns;
		int h_cell = height / lines;

		// учитываем рамку, т.е. неиспользуемое пространство вокруг холста
		int xAdd = (Screen.width - width) / 2;
		int yAdd = (Screen.height - height) / 2;

		yield return new WaitForEndOfFrame();

		for (int y = 0; y < lines; y++)
		{
			for (int x = 0; x < columns; x++)
			{
				// делаем снимок части экрана
				Rect rect = new Rect(0, 0, w_cell, h_cell);
				rect.center = new Vector2((w_cell * x + w_cell / 2) + xAdd, (h_cell * y + h_cell / 2) + yAdd);
				Vector3 position = Camera.main.ScreenToWorldPoint(rect.center);
				Texture2D tex = new Texture2D(w_cell, h_cell, TextureFormat.ARGB32, false);
				tex.ReadPixels(rect, 0, 0);
				tex.Apply();

				// создание нового объекта и настройка его позиции
				GameObject obj = new GameObject("Puzzle: " + puzzleCounter);
				obj.transform.parent = transform;
				position = new Vector3(((int)(position.x * 100f)) / 100f, ((int)(position.y * 100f)) / 100f, puzzleCounter / 100f);
				obj.transform.localPosition = position;

				// конвертируем текстуру в спрайт
				SpriteRenderer ren = obj.AddComponent<SpriteRenderer>();
				int unit = Mathf.RoundToInt((float)Screen.height / (Camera.main.orthographicSize * 2f)); // формула расчета размера спрайта (только для режима камеры Оrthographic)
				ren.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), unit);
				obj.AddComponent<BoxCollider2D>();
				obj.tag = puzzleTag;

				_puzzlePos.Add(position);
				_puzzle.Add(ren);
				puzzleCounter++;
			}
		}
		picture.gameObject.SetActive(false);
		Camera.main.orthographicSize = 5f;
		Camera.main.transform.position = new Vector3(0f, 0f,-10f);
		MN.gameMenu.SetActive(true);
		RandomizePosition();
	}

	void GetPuzzle()
	{
		// массив рейкаста, чтобы фильтровать спрайты по глубине Z (тот что ближе, будет первым элементом массива)
		RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		if (hit.Length > 0 && hit[0].transform.tag == puzzleTag)
		{
			offset = hit[0].transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
			current = hit[0].transform;
			sortingOrder = current.GetComponent<SpriteRenderer>().sortingOrder;
			current.GetComponent<SpriteRenderer>().sortingOrder = puzzleCounter + 1;
		}
	}
}

[System.Serializable]
public class Puzzles
{
	public Sprite puzzleSprite; //
	[HideInInspector] public GameObject puzzleBttn; //
	[HideInInspector] public bool puzzleActive; //
}
