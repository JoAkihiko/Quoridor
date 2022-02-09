using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // 各盤面に数字割り当て
    enum COLOR
    {
        // 空 = 0
        EMPTY,

        // 緑 = 1
        GREEN,

        // 橙 = 2
        ORANGE,

        // 候補地 = 3
        CANDIDATE
    }

    // 方向に数字割り当て
    enum DIRECTION
    {
        // 上 = 0
        UP,

        // 下 = 1
        DOWN,

        // 左 = 2
        LEFT,

        // 右 = 3
        RIGHT
    }

    // 盤面の高さと幅 (Start()で値変更)
    private int HEIGHT = 9, WIDTH=9;

    // 空の盤面，緑の駒，橙の駒
    [SerializeField]
    GameObject emptyObject = null, greenObject=null, orangeObject = null, candidateObject = null;

    // 壁
    [SerializeField]
    GameObject wallObject = null;

    // 壁生成ボタン
    [SerializeField]
    GameObject verticalWallButtonObject = null, horizontalWallButtonObject = null;

    // 残り壁表示テキスト
    [SerializeField]
    Text textFrameGreen, textFrameOrange;

    // 盤面の土台
    [SerializeField]
    GameObject boardDisplay = null;

    // 壁の土台
    [SerializeField]
    GameObject wallDisplay = null;

    // 壁の設置場所
    [SerializeField]
    GameObject wallField = null;

    // 背景
    [SerializeField]
    GameObject backGround = null;

    // ゲーム終了ダイアログ
    [SerializeField]
    ResetDialog dialogGameEnd = null;

    // ゲーム終了テキスト
    [SerializeField]
    Text textUpper = null;

    // SE
    [SerializeField]
    AudioClip soundPieceMoving = null, soundGameEnd = null;

    // 二次元配列 盤面 (縦×横)
    COLOR[,] board;

    // 行動プレイヤー
    COLOR player = COLOR.GREEN;

    // 壁のスクリプト
    DragVerticalWallsScript dvws;
    DragHorizontalWallsScript dhws;

    // 壁立て選択中かどうか
    private bool isBuildingWall = false;

    // 駒の位置
    private int[] greenPosition, orangePosition;

    // 壁設置可能かのチェック時に必要 (既にチェック済みのポジションかどうか)
    private bool[,] passedPosition;

    // 初期の壁の数
    private int numWallGreen = 10, numWallOrange = 10;

    // 残っている使用可能な壁の数
    private int remainWallGreen, remainWallOrange;

    // 試合終了かどうか
    public bool isGameEnd;

    // 背景の色指定
    private Color32 greenColor = new Color32(170, 255, 230, 255), orangeColor = new Color32(255, 230, 170, 255);

    // 音源
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        // タイトルシーンから得る値
        int[] titlePram = TitleScene.GetParam();
        HEIGHT = titlePram[0];
        WIDTH = titlePram[1];
        numWallGreen = titlePram[2];
        numWallOrange = titlePram[3];

        // RectTransformの大きさをマス目の数に応じて変更
        RectTransform rt = boardDisplay.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50*WIDTH, 50*HEIGHT);

        board = new COLOR[HEIGHT, WIDTH];
        greenPosition = new int[] { 0, WIDTH / 2 };
        orangePosition = new int[] { HEIGHT - 1, WIDTH / 2 };

        // 縦壁と横壁のスクリプト
        dvws = verticalWallButtonObject.GetComponent<DragVerticalWallsScript>();
        dhws = horizontalWallButtonObject.GetComponent<DragHorizontalWallsScript>();

        // 音源
        audioSource = GetComponent<AudioSource>();

        // 初期化
        Initialized(); 
    }

    private void Update()
    {
        textFrameGreen.text = "残り壁枚数 : " + remainWallGreen;
        textFrameOrange.text = "残り壁枚数 : " + remainWallOrange;
    }

    public void Initialized()
    {
        // 各パラメータ初期化
        greenPosition =  new int[] { 0, WIDTH / 2 };
        orangePosition = new int[] { HEIGHT - 1, WIDTH / 2 };
        player = COLOR.GREEN;
        backGround.GetComponent<Image>().color = greenColor;
        isBuildingWall = false;
        remainWallGreen = numWallGreen;
        remainWallOrange = numWallOrange;
        isGameEnd = false;

        textUpper.text = "ミドリのターン";

        dvws.isWallsAtPI = new bool[HEIGHT-1, WIDTH-1];
        dhws.isWallsAtPI = new bool[HEIGHT-1, WIDTH-1];

        UpdateAll();

        // wallFieldの子オブジェクトを全て削除
        foreach (Transform child in wallField.transform)
        {
            Destroy(child.gameObject);
        }
    }


    /// <summary>
    /// プレイヤーの位置と壁の位置から駒設置可能位置（候補地）を探してプレイヤーの位置と候補地をゲーム画面に反映
    /// </summary>
    void UpdateAll()
    {
        // 盤面アップデート
        UpdateBoard();
        if (!isGameEnd)
        {
            // 駒を置ける場所を探す
            FindCandidate();
        }
        // 盤面表示
        ShowBoard();
    }

    /// <summary>
    /// プレイヤーの新たな位置だけ反映した盤面にアップデート
    /// </summary>
    void UpdateBoard()
    {
        for (int h = 0; h < HEIGHT; h++)
        {
            for (int w = 0; w < WIDTH; w++)
            {
                // 全盤面を空
                board[h, w] = COLOR.EMPTY;
            }
        }
        board[greenPosition[0], greenPosition[1]] = COLOR.GREEN;
        board[orangePosition[0], orangePosition[1]] = COLOR.ORANGE;
    }

    /// <summary>
    /// 現在の盤面をゲーム画面に反映
    /// </summary>
    void ShowBoard()
    {
        // boardDisplayの子オブジェクトを全て削除
        foreach (Transform child in boardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        for (int h = 0; h < HEIGHT; h++)
        {
            for (int w = 0; w < WIDTH; w++)
            {
                // Prefab取得
                GameObject piece = GetPrefab(board[h, w]);

                if (board[h, w] == COLOR.CANDIDATE && !isBuildingWall)
                {
                    // pieceにイベント設定
                    int y = h;
                    int x = w;
                    piece.GetComponent<Button>().onClick.AddListener(() => { MovePiece(y + "," + x); });
                }

                // 取得したPrefabをboardDisplayの子オブジェクトにする
                piece.transform.SetParent(boardDisplay.transform);
            }
        }
    }

    /// <summary>
    /// 駒の設置可能位置を探す
    /// </summary>
    void FindCandidate()
    {
        // 行動プレイヤーの駒の位置
        int[] playerPosition = player == COLOR.GREEN ? greenPosition : orangePosition;

        Debug.Log(playerPosition[0]);
        Debug.Log(playerPosition[1]);

        // 駒の位置 (縦)
        int ppH = playerPosition[0];
        // 駒の位置 (横)
        int ppW = playerPosition[1]; 

        if (IsUp(ppH, ppW))
        {
            CandidateDisplay(new int[] { ppH - 1, ppW }, DIRECTION.UP);
        }
        if (IsDown(ppH, ppW))
        { 
            CandidateDisplay(new int[] { ppH + 1, ppW }, DIRECTION.DOWN);
        }
        if (IsLeft(ppH, ppW))
        {
            CandidateDisplay(new int[] { ppH, ppW - 1 }, DIRECTION.LEFT);
        }
        if (IsRight(ppH, ppW))
        {
            CandidateDisplay(new int[] { ppH, ppW + 1 }, DIRECTION.RIGHT);
        }
    }

    /// <summary>
    /// 候補地を探す
    /// </summary>
    /// <param name="candidatePosition">候補地の位置</param>
    void CandidateDisplay(int[] candidatePosition, DIRECTION direction)
    {
        // 設置可能箇所かチェック
        if (candidatePosition[0] >=0 && candidatePosition[0] < HEIGHT && candidatePosition[1]>=0 && candidatePosition[1] < WIDTH)
        {
            COLOR candidatePiece = board[candidatePosition[0], candidatePosition[1]];
            // 候補地に緑プレイヤーがいたとき
            if (candidatePiece == COLOR.GREEN)
            {
                JumpOver(orangePosition, candidatePosition, direction);
                /*
                int[] position = new int[] { candidatePosition[0] * 2 - orangePosition[0], candidatePosition[1] * 2 - orangePosition[1] };
                if(position[0] >=0 && position[0] < HEIGHT && position[1] >= 0 && position[1] < WIDTH)
                {
                    if ((direction != DIRECTION.UP || IsDown(position[0], position[1])) && (direction != DIRECTION.DOWN || IsUp(position[0], position[1])) && (direction != DIRECTION.LEFT || IsRight(position[0], position[1])) && (direction != DIRECTION.RIGHT || IsLeft(position[0], position[1])))
                    {
                        // 緑を超えた位置が候補地
                        board[position[0], position[1]] = COLOR.CANDIDATE;
                    }
                    if(direction == DIRECTION.UP)
                    {
                        if(IsDown(position[0], position[1]))
                        {
                            // 緑を超えた位置が候補地
                            board[position[0], position[1]] = COLOR.CANDIDATE;
                        }
                        else
                        {
                            if(IsLeft(candidatePosition[0], candidatePosition[1]))
                            {
                                // 相手プレイヤーの隣が候補地
                                board[candidatePosition[0], candidatePosition[1] - 1] = COLOR.CANDIDATE;
                            }
                            if (IsRight(candidatePosition[0], candidatePosition[1]))
                            {
                                // 相手プレイヤーの隣が候補地
                                board[candidatePosition[0], candidatePosition[1] + 1] = COLOR.CANDIDATE;
                            }
                        }
                    }
                }
            */
            }
            // 候補地に橙プレイヤーがいたとき
            else if (candidatePiece == COLOR.ORANGE)
            {
                JumpOver(greenPosition, candidatePosition, direction);
                /*
                int[] position = new int[] { candidatePosition[0] * 2 - greenPosition[0], candidatePosition[1] * 2 - greenPosition[1] };
                if (position[0] >= 0 && position[0] < HEIGHT && position[1] >= 0 && position[1] < WIDTH)
                {
                    if ((direction != DIRECTION.UP || IsDown(position[0], position[1])) && (direction != DIRECTION.DOWN || IsUp(position[0], position[1])) && (direction != DIRECTION.LEFT || IsRight(position[0], position[1])) && (direction != DIRECTION.RIGHT || IsLeft(position[0], position[1])))
                    {
                        // 橙を超えた位置が候補地
                        board[position[0], position[1]] = COLOR.CANDIDATE;
                    }
                }
                */
            } 
            else
            {
                board[candidatePosition[0], candidatePosition[1]] = COLOR.CANDIDATE;
            }
        }
    }

    void JumpOver(int[] selfPosition, int[] opponentPosition, DIRECTION direction)
    {
        int[] position = new int[] { opponentPosition[0] * 2 - selfPosition[0], opponentPosition[1] * 2 - selfPosition[1] };
        if (position[0] >= 0 && position[0] < HEIGHT && position[1] >= 0 && position[1] < WIDTH)
        {
            if (direction == DIRECTION.UP)
            {
                if (IsDown(position[0], position[1]))
                {
                    // 相手プレイヤーを超えた位置が候補地
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsLeft(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0], opponentPosition[1] - 1] = COLOR.CANDIDATE;
                    }
                    if (IsRight(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0], opponentPosition[1] + 1] = COLOR.CANDIDATE;
                    }
                }
            }
            else if (direction == DIRECTION.DOWN)
            {
                if (IsUp(position[0], position[1]))
                {
                    // 相手プレイヤーを超えた位置が候補地
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsLeft(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0], opponentPosition[1] - 1] = COLOR.CANDIDATE;
                    }
                    if (IsRight(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0], opponentPosition[1] + 1] = COLOR.CANDIDATE;
                    }
                }
            }
            else if (direction == DIRECTION.LEFT)
            {
                if (IsRight(position[0], position[1]))
                {
                    // 相手プレイヤーを超えた位置が候補地
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsUp(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0] - 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                    if (IsDown(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0] + 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                }
            }
            else if (direction == DIRECTION.RIGHT)
            {
                if (IsLeft(position[0], position[1]))
                {
                    // 相手プレイヤーを超えた位置が候補地
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsUp(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0] - 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                    if (IsDown(opponentPosition[0], opponentPosition[1]))
                    {
                        // 相手プレイヤーの隣が候補地
                        board[opponentPosition[0] + 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                }
            }
        }
    }

    /// <summary>
    /// マスに合ったプレハブを取得
    /// </summary>
    /// <param name="color">マスの種類</param>
    /// <returns>マスに合ったプレハブ</returns>
    GameObject GetPrefab(COLOR color)
    {
        GameObject prefab;
        switch(color)
        {
            // 空のとき
            case COLOR.EMPTY:   
                prefab = Instantiate(emptyObject);
                break;
            // 緑のとき
            case COLOR.GREEN:   
                prefab = Instantiate(greenObject);
                break;
            // 橙のとき
            case COLOR.ORANGE:  
                prefab = Instantiate(orangeObject);
                break;
            // 候補地のとき
            case COLOR.CANDIDATE:   
                prefab = Instantiate(candidateObject);
                break;
            default:
                prefab = null;
                break;
        }
        return prefab;
    }

    /// <summary>
    /// 駒を動かす
    /// </summary>
    /// <param name="position">動く先のマス（string型であることに注意）</param>
    public void MovePiece(string position)
    {
        Debug.Log(position);
        int h = int.Parse(position.Split(',')[0]);
        int w = int.Parse(position.Split(',')[1]);
        // 行動プレイヤーの駒の位置
        board[h, w] = player;

        if(player == COLOR.GREEN)
        {
            greenPosition = new int[] { h, w };
            player = COLOR.ORANGE;
            backGround.GetComponent<Image>().color = orangeColor;
            textUpper.text = "オレンジのターン";
        }
        else
        {
            orangePosition = new int[] { h, w };
            player = COLOR.GREEN;
            backGround.GetComponent<Image>().color = greenColor;
            textUpper.text = "ミドリのターン";
        }

        // 試合終了判定
        if (greenPosition[0] == HEIGHT-1 || orangePosition[0] == 0)
        {
            isGameEnd = true;

            // SE
            audioSource.clip = soundGameEnd;
            audioSource.time = 0.0f;
            audioSource.Play();
        }
        else
        {
            // SE
            audioSource.clip = soundPieceMoving;
            audioSource.time = 0.5f;
            audioSource.Play();
        }

        UpdateAll();

        // 試合終了時の処理
        if (isGameEnd)
        {
            // 背景を白にする
            backGround.GetComponent<Image>().color = Color.white;
            textUpper.text = "次のゲームを始めるには左下のResetボタンを押してください";

            // 試合終了ダイアログを生成してwallDisplayの子オブジェクトにする
            var _dialogGameEnd = Instantiate(dialogGameEnd);
            _dialogGameEnd.transform.SetParent(wallDisplay.transform, false);
            // ボタンが押されたときのイベント処理
            _dialogGameEnd.FixDialog = result => Debug.Log(result);
        }
    }

    /// <summary>
    /// 次のプレイヤーのターンになる
    /// </summary>
    public void NextTurn()
    {
        if(player == COLOR.GREEN)
        {
            player = COLOR.ORANGE;
            backGround.GetComponent<Image>().color = orangeColor;
            textUpper.text = "オレンジのターン";
        }
        else
        {
            player = COLOR.GREEN;
            backGround.GetComponent<Image>().color = greenColor;
            textUpper.text = "ミドリのターン";
        }

        UpdateAll();
    }

    /**
    <summary>
    他クラスから盤面をアップデートしたい場合に呼び出される
    </summary>
    */
    public void KeepTurn()
    {
        UpdateAll();
    }

    /// <summary>
    /// 行動プレイヤーの使用可能な壁の数を一つ減らす
    /// </summary>
    public void ReduceWall()
    {
        if (player == COLOR.GREEN)
        {
            if(remainWallGreen > 0)
            {
                remainWallGreen--;
            }
        }
        else
        {
            if (remainWallOrange > 0)
            {
                remainWallOrange--;
            }
        }
    }

    /// <summary>
    /// 壁が残っているかどうか
    /// </summary>
    /// <returns>True：行動プレイヤーの使用可能な壁が残っている場合 False：残っていない場合</returns>
    public bool IsRemainWall()
    {
        int remainWall = player == COLOR.GREEN ? remainWallGreen : remainWallOrange;
        return remainWall > 0;
    }

    public (int, int) GetParam()
    {
        return (WIDTH, HEIGHT);
    }

    /// <summary>
    /// ゴールまでの経路があるかどうか
    /// </summary>
    /// <returns>True：両プレイヤーがゴールに到達可能な場合 False：ゴールに到達不可能なプレイヤーがいる場合</returns>
    public bool IsWay2Goal()
    {
        passedPosition = new bool[HEIGHT, WIDTH];
        if (!IsWay2Next(greenPosition[0], greenPosition[1], COLOR.GREEN))
        {
            return false;
        }

        passedPosition = new bool[HEIGHT, WIDTH];
        if (!IsWay2Next(orangePosition[0], orangePosition[1], COLOR.ORANGE))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 一マス隣のマスに直接行けるかどうか（再帰）
    /// </summary>
    /// <param name="pH">プレイヤーの位置（縦）</param>
    /// <param name="pW">プレイヤーの位置（横）</param>
    /// <param name="player">行動プレイヤー</param>
    /// <returns>True：四方の隣のどれかのマスからゴールへの経路がある場合 False：ゴールへの経路がない場合</returns>
    bool IsWay2Next(int pH, int pW, COLOR player)
    {
        passedPosition[pH, pW] = true;
        if (pH == 0)
        {
            // 橙のゴールまで到達可能
            if (player == COLOR.ORANGE) 
            {
                return true;
            }
        }
        else if(pH == HEIGHT-1)
        {
            // 緑のゴールまで到達可能
            if (player == COLOR.GREEN) 
            {
                return true;
            }
        }

        // 上がマス上か、未到達点か、壁がないか
        if (pH != 0 && !passedPosition[pH - 1, pW] && IsUp(pH, pW)) 
        {
            if (IsWay2Next(pH - 1, pW, player))
            {
                return true;
            }
        }

        // 下がマス上か、未到達点か、壁がないか
        if (pH != HEIGHT-1 && !passedPosition[pH + 1, pW] && IsDown(pH, pW)) 
        {
            if (IsWay2Next(pH + 1, pW, player))
            {
                return true;
            }
        }

        // 左がマス上か、未到達点か、壁がないか
        if (pW != 0 && !passedPosition[pH, pW - 1] && IsLeft(pH, pW)) 
        {
            if (IsWay2Next(pH, pW - 1, player))
            {
                return true;
            }
        }

        // 右がマス上か、未到達点か、壁がないか
        if (pW != WIDTH-1 && !passedPosition[pH, pW + 1] && IsRight(pH, pW)) 
        {
            if (IsWay2Next(pH, pW + 1, player))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 上のマスに直接行けるかどうか
    /// </summary>
    /// <param name="pH">プレイヤーの位置（縦）</param>
    /// <param name="pW">プレイヤーの位置（横）</param>
    /// <returns>True：上のマスに直接行ける場合 False：上のマスに直接行けない場合</returns>
    private bool IsUp(int pH, int pW)
    {
        if(pH == 0 || ((pW == 0 || !dhws.isWallsAtPI[pH - 1, pW - 1]) && (pW == WIDTH-1 || !dhws.isWallsAtPI[pH - 1, pW])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 下のマスに直接行けるかどうか
    /// </summary>
    /// <param name="pH">プレイヤーの位置（縦）</param>
    /// <param name="pW">プレイヤーの位置（横）</param>
    /// <returns>True：下のマスに直接行ける場合 False：下のマスに直接行けない場合</returns>
    private bool IsDown(int pH, int pW)
    {
        if (pH == HEIGHT-1 || ((pW == 0 || !dhws.isWallsAtPI[pH, pW - 1]) && (pW == WIDTH-1 || !dhws.isWallsAtPI[pH, pW])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 左のマスに直接行けるかどうか
    /// </summary>
    /// <param name="pH">プレイヤーの位置（縦）</param>
    /// <param name="pW">プレイヤーの位置（横）</param>
    /// <returns>True：左のマスに直接行ける場合 False：左のマスに直接行けない場合</returns>
    private bool IsLeft(int pH, int pW)
    {
        if (pW == 0 || ((pH == 0 || !dvws.isWallsAtPI[pH - 1, pW - 1]) && (pH == HEIGHT-1 || !dvws.isWallsAtPI[pH, pW - 1])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 右のマスに直接行けるかどうか
    /// </summary>
    /// <param name="pH">プレイヤーの位置（縦）</param>
    /// <param name="pW">プレイヤーの位置（横）</param>
    /// <returns>True：右のマスに直接行ける場合 False：右のマスに直接行けない場合</returns>
    private bool IsRight(int pH, int pW)
    {
        if (pW == WIDTH-1 || ((pH == 0 || !dvws.isWallsAtPI[pH - 1, pW]) && (pH == HEIGHT-1 || !dvws.isWallsAtPI[pH, pW])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// タイトルシーンに切り替え
    /// </summary>
    public void ChangeScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 以下全て今は使っていない

    // not used
    void SetWall()  
    {
        for (int i = 0; i < 8; i++)
        {
            GetWall(i, 300);
            GetWall(i, -300);
        }
    }

    // not used
    void GetWall(int i, int h)
    {
        GameObject wallobj = null;
        wallobj = Instantiate(wallObject, new Vector3(175 - i * 50, h, 10), Quaternion.identity);
        // 取得したPrefabをwallDisplayの子オブジェクトにする
        wallobj.transform.SetParent(wallDisplay.transform, false);
    }

    // not used
    void BuildWall()
    {
        Debug.Log("wall");
        isBuildingWall = true;

        UpdateAll();
    }

    // not used
    public (int[], int[]) GetPlayerPosition()
    {
        return (greenPosition, orangePosition);
    }

    // not used
    public bool IsGreenTurn()
    {
        return player == COLOR.GREEN;
    }

}
