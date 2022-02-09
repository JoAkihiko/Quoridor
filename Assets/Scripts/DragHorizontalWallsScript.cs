using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHorizontalWallsScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 盤面の高さと幅
    int HEIGHT, WIDTH;

    // ゲームコントローラー
    [SerializeField]
    GameObject gameManager = null;

    // 親キャンバス
    [SerializeField]
    Canvas parentCanvas = null;

    // 壁
    [SerializeField]
    GameObject wallObject = null;

    // 壁の設置場所
    [SerializeField]
    GameObject wallField = null;

    // ゲームコントローラーオブジェクト
    GameController gc;

    // 壁生成ボタン
    [SerializeField]
    GameObject verticalWallButtonObject = null;

    // すぐに消える一時的なダイアログ
    [SerializeField]
    GameObject dialogTemporary = null;

    // 一時的なダイアログのテキスト
    [SerializeField]
    Text textDialog = null;

    // SE
    [SerializeField]
    AudioClip soundSetWall = null;

    DragVerticalWallsScript dvws;

    private Vector2 prevPosition;

    // point of intersection(PI)に壁が設置されているかどうか（[行, 列]）
    public bool[,] isWallsAtPI = new bool[8,8];

    // [行，列]
    // public bool[,] isWalls = new bool[9, 9];

    // 音源
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        gc = gameManager.GetComponent<GameController>();
        dvws = verticalWallButtonObject.GetComponent<DragVerticalWallsScript>();
        audioSource = GetComponent<AudioSource>();

        // タイトルシーンから得る値
        int[] titlePram = TitleScene.GetParam();
        HEIGHT = titlePram[0];
        WIDTH = titlePram[1];

        isWallsAtPI = new bool[HEIGHT-1, WIDTH-1];
    }

    /// <summary>
    /// ドラッグ開始時に呼び出される
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gc.IsRemainWall())
        {
            SetDialogTemporary("壁は使い切りました");
        }

        prevPosition = transform.position;
    }

    /// <summary>
    /// ドラッグ中に呼び出される
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if(gc.IsRemainWall() && !gc.isGameEnd)
        {
            transform.position = eventData.position;
        }
    }

    /// <summary>
    /// ドラッグ終わりに呼び出される
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (gc.isGameEnd)
        {
            return;
        }

        GameObject wallobj = null;

        Debug.Log(transform.position);

        (int h, int w) = GetNearestPoint(transform.position);

        // 壁が設置可能な位置かどうか（順番反対の場合バグ）
        if (gc.IsRemainWall() && IsSetWall(h, w))
        {
            wallobj = Instantiate(wallObject, new Vector2(455 - (25 * (WIDTH - 9)) + w * 50, 610 + (25 * (HEIGHT - 9)) - h * 50), Quaternion.identity);
            // 取得したPrefabをwallFieldの子オブジェクトにする
            wallobj.transform.SetParent(wallField.transform);

            // 壁設置SE
            audioSource.clip = soundSetWall;
            audioSource.time = 0.15f;
            audioSource.Play();

            gc.ReduceWall();
            gc.NextTurn();
            transform.position = prevPosition;
            return;
        }

        gc.KeepTurn();
        transform.position = prevPosition;
    }

    /// <summary>
    /// 引数の位置から最も近いマスの交差点の位置を求める
    /// </summary>
    /// <param name="dropPosition">ドラッグ終了位置</param>
    /// <returns>マスの交差点の位置（縦、横）</returns>
    private (int, int) GetNearestPoint(Vector2 dropPosition)
    {
        int h, w;
        for (h = 0; dropPosition.y < 585 + (25 * (HEIGHT-9)) - h * 50; h++) ;
        for (w = 0; dropPosition.x > 480 - (25 * (WIDTH - 9)) + w * 50; w++) ;

        Debug.Log("nearest point...");
        Debug.Log(w.ToString());
        Debug.Log(h.ToString());

        return (h, w);
    }

    /// <summary>
    /// 他のクラスがマスの交差位置に壁が設置されているかを確認する場合に呼び出される
    /// </summary>
    /// <param name="h">マスの交差位置+1（縦）</param>
    /// <param name="w">マスの交差位置+1（横）</param>
    /// <returns></returns>
    public bool GetIsWallsAtPI(int h, int w)
    {
        return isWallsAtPI[h-1, w-1];
    }

    /// <summary>
    /// 壁の設置可能位置かどうか
    /// </summary>
    /// <param name="h">マス位置（縦）</param>
    /// <param name="w">マス位置（横）</param>
    /// <returns></returns>
    private bool IsSetWall(int h, int w)
    {
        // 盤面外の場合
        if (!(w > 0 && w < WIDTH && h > 0 && h < HEIGHT))
        {
            SetDialogTemporary("盤面外に壁は置けません");
            return false;
        }
        // 既に横壁が置いてある場合
        if ((w != 1 && isWallsAtPI[h - 1, w - 2]) || isWallsAtPI[h - 1, w - 1] || (w != WIDTH-1 && isWallsAtPI[h - 1, w]))
        {
            SetDialogTemporary("壁が重なるところに壁は置けません");
            return false;
        }
        // 既に縦壁が置いてある場合
        if (dvws.GetIsWallsAtPI(h, w))
        {
            SetDialogTemporary("壁が重なるところに壁は置けません");
            return false;
        }

        isWallsAtPI[h - 1, w - 1] = true;
        // ゴールへの経路が塞がれていないか
        if (gc.IsWay2Goal()) 
        {
            Debug.Log("Can Reach GOAL!");
            return true;
        }
        else
        {
            SetDialogTemporary("ゴールへの経路がなくなるところに壁は置けません");
            Debug.Log("Can not Reach GOAL!");
            isWallsAtPI[h - 1, w - 1] = false;
            return false;
        }
    }

    /// <summary>
    /// すぐに消えるダイアログを表示する
    /// </summary>
    /// <param name="text">ダイアログに表示するメッセージ</param>
    private void SetDialogTemporary(string text)
    {
        textDialog.text = text;
        //ダイアログを生成してparentCanvasの子オブジェクトにする
        var _dialogTemporary = Instantiate(dialogTemporary);
        _dialogTemporary.transform.SetParent(parentCanvas.transform, false);
    }

}