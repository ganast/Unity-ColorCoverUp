using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager inst;

    private float XMax = 3.344f;

    private int Turn;

    private GameObject[,] BoardGameObjects = new GameObject[7, 4];

    private int[,] Board = new int[7, 4];

    private int[] Secret = new int[4];

    private Dictionary<string, GameObject> PlayerColorPrefabs;
    private Dictionary<string, int> PlayerColorIndices;
    private string[] PlayerColorNames;

    private GameObject BlackPin;
    private GameObject WhitePin;

    [SerializeField]
    private Transform PlacedPinsParent;

    [SerializeField]
    private Transform SecretPinsParent;

    [SerializeField]
    private Transform ResultPinsParent;

    [SerializeField]
    private GameObject GoButton;

    [SerializeField]
    private GameObject AgainButton;

    [SerializeField]
    private GameObject QuestionMarksText;

    [SerializeField]
    private bool UniqueSecretElements = true;

    [SerializeField]
    private bool UniqueChoiceElements = true;

    [SerializeField]
    private bool FullChoiceOnly = true;

    [SerializeField]
    private bool DebugMode;

    private GameManager() {
        PlayerColorPrefabs = new Dictionary<string, GameObject>();
        PlayerColorIndices = new Dictionary<string, int>();
        PlayerColorNames = new string[6];
        DebugMode = false;
    }

    protected void Awake() {
        PlayerColorPrefabs["Red"] = Resources.Load<GameObject>("Red");
        PlayerColorIndices["Red"] = 0;
        PlayerColorNames[0] = "Red";

        PlayerColorPrefabs["Blue"] = Resources.Load<GameObject>("Blue");
        PlayerColorIndices["Blue"] = 1;
        PlayerColorNames[1] = "Blue";

        PlayerColorPrefabs["Green"] = Resources.Load<GameObject>("Green");
        PlayerColorIndices["Green"] = 2;
        PlayerColorNames[2] = "Green";

        PlayerColorPrefabs["Orange"] = Resources.Load<GameObject>("Orange");
        PlayerColorIndices["Orange"] = 3;
        PlayerColorNames[3] = "Orange";

        PlayerColorPrefabs["Pink"] = Resources.Load<GameObject>("Pink");
        PlayerColorIndices["Pink"] = 4;
        PlayerColorNames[4] = "Pink";

        PlayerColorPrefabs["Yellow"] = Resources.Load<GameObject>("Yellow");
        PlayerColorIndices["Yellow"] = 5;
        PlayerColorNames[5] = "Yellow";

        BlackPin = Resources.Load<GameObject>("Black");
        WhitePin = Resources.Load<GameObject>("White");

        inst = this;
    }

    protected void Start() {
        NewGame();
    }

    protected void Update() {

    }

    public void DropOn(Vector3 worldPos, GameObject choice) {

        if (Turn == 7) {
            return;
        }

        string name = choice.name;
        int i = PlayerColorIndices[name];

        if (UniqueChoiceElements) {
            for (int j = 0; j != 4; j++) {
                if (Board[Turn, j] == i) {
                    return;
                }
            }
        }

        float boardX;
        float boardY;
        WorldToBoard(worldPos, out boardX, out boardY);

        if (IsValidChoice(boardX, boardY)) {

            int r = Turn;
            int c = GetBoardIndex(boardX);

            Board[r, c] = i;

            GameObject g = Instantiate<GameObject>(PlayerColorPrefabs[name], PlacedPinsParent);
            g.transform.position = AdjustPosition(boardX, boardY);
            g.name = string.Format("{0}{1}{2}", name, r, c);
            if (BoardGameObjects[r, c] != null) {
                GameObject.Destroy(BoardGameObjects[r, c]);
            }
            BoardGameObjects[r, c] = g;

            AudioManager.GetAudioManager().PlayPinDropSound();
        }
    }

    protected void NewGame() {
        Turn = 0;
        ClearBoard();
        for (int i = 0; i != 4; i++) {
            bool b = false;
            while (!b) {
                Secret[i] = Random.Range(0, 6);
                b = true;
                if (UniqueSecretElements) {
                    for (int j = 0; j != i; j++) {
                        if (Secret[j] == Secret[i]) {
                            b = false;
                        }
                    }
                }
            }
            // Debug.Log(Guess[i]);
        }
        if (DebugMode) {
            ShowSecret();
        }
        QuestionMarksText.SetActive(true);
        AgainButton.SetActive(false);
        GoButton.SetActive(true);
    }

    protected void Win() {
        ShowSecret();
        AudioManager.GetAudioManager().PlayWinSound();
        QuestionMarksText.SetActive(false);
        GoButton.SetActive(false);
        AgainButton.SetActive(true);
    }

    protected void Lose() {
        ShowSecret();
        AudioManager.GetAudioManager().PlayLoseSound();
        QuestionMarksText.SetActive(false);
        GoButton.SetActive(false);
        AgainButton.SetActive(true);
    }

    public void Go() {

        if (FullChoiceOnly) {
            for (int j = 0; j != 4; j++) {
                if (Board[Turn, j] == -1) {
                    return;
                }
            }
        }

        ProcessPlayerChoices();
        if (SecretFound()) {
            Win();
        }
        else {
            Turn++;
            if (Turn == 7) {
                Lose();
            }
            else {
                AudioManager.GetAudioManager().PlayNextTurnSound();
            }
        }
    }

    protected bool SecretFound() {
        return Board[Turn, 0] == Secret[0] && Board[Turn, 1] == Secret[1] &&
            Board[Turn, 2] == Secret[2] && Board[Turn, 3] == Secret[3];
    }

    protected void ProcessPlayerChoices() {

        int blacks = 0;

        int whites = 0;

        bool[] boardChecked = new bool[4];
        bool[] secretChecked = new bool[4];
        for (int i = 0; i != 4; i++) {
            boardChecked[i] = false;
            secretChecked[i] = false;
        }

        for (int i = 0; i != 4; i++) {
            if (Board[Turn, i] == Secret[i]) {
                boardChecked[i] = true;
                secretChecked[i] = true;
                blacks++;
            }
        }
        for (int i = 0; i != 4; i++) {
            if (boardChecked[i] == false) {
                for (int j = 0; j != 4; j++) {
                    if (secretChecked[j] == false && Board[Turn, i] == Secret[j]) {
                        boardChecked[i] = true;
                        secretChecked[j] = true;
                        whites++;
                        break;
                    }
                }
            }
        }
        // Debug.LogFormat("{0} blacks, {1} whites", blacks, whites);

        int x = 0;
        int y = 0;
        for (int i = 0; i != blacks; i++) {
            GameObject g = Instantiate<GameObject>(BlackPin, ResultPinsParent);
            g.transform.position = GetResultPinPosition(x, y, Turn);
            x++;
            if (x == 2) {
                y++;
                x = 0;
            }
        }
        for (int i = 0; i != whites; i++) {
            GameObject g = Instantiate<GameObject>(WhitePin, ResultPinsParent);
            g.transform.position = GetResultPinPosition(x, y, Turn);
            x++;
            if (x == 2) {
                y++;
                x = 0;
            }
        }
    }

    public void Again() {
        NewGame();
    }

    protected void ClearBoard() {
        foreach (Transform placedPin in SecretPinsParent.transform) {
            GameObject.Destroy(placedPin.gameObject);
        }
        foreach (Transform placedPin in ResultPinsParent.transform) {
            GameObject.Destroy(placedPin.gameObject);
        }
        for (int r = 0; r != 7; r++) {
            for (int c = 0; c != 4; c++) {
                GameObject g = BoardGameObjects[r, c];
                if (g != null) {
                    GameObject.Destroy(g);
                }
                BoardGameObjects[r, c] = null;
                Board[r, c] = -1;
            }
        }
    }

    protected void ShowSecret() {
        for (int i = 0; i != 4; i++) {
            GameObject g = Instantiate<GameObject>(
                PlayerColorPrefabs[PlayerColorNames[Secret[i]]], SecretPinsParent);
            g.transform.position = new Vector3(-1.916f + i * 0.836f, -3.28f, 0.0f);
        }
    }

    public static GameManager GetGameManager() {
        return inst;
    }

    protected static void WorldToBoard(Vector3 worldPos, out float boardX, out float boardY) {
        boardX = worldPos.x + 1.916f + 0.375f;
        boardY = -worldPos.y + 3.05f + 0.375f;
    }

    protected bool IsValidChoice(float boardX, float boardY) {
        return boardX >= 0.0f
            && boardX <= XMax
            && boardY >= Turn * 0.836f
            && boardY <= (Turn + 1) * 0.836f;
    }

    protected static Vector3 AdjustPosition(float boardX, float boardY) {
        float adjustedBoardX = GetBoardIndex(boardX) * 0.836f - 1.916f;
        float adjustedBoardY = GetBoardIndex(-boardY) * 0.836f + 3.05f;
        Vector3 adjustedBoardPos = new Vector3(adjustedBoardX, adjustedBoardY, 0.0f);
        return adjustedBoardPos;
    }

    protected static int GetBoardIndex(float boardCoord) {
        return ((int) (boardCoord / 0.836f));
    }

    protected static Vector3 GetResultPinPosition(int x, int y, int turn) {
        float dp = 0.402f;
        float dy = 0.837f;
        return new Vector3(1.628f + x * dp, 3.25f - y * dp - turn * dy, 0);
    }
}
