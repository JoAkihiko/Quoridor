using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDialog : MonoBehaviour
{
    // ダイアログを追加する先の親キャンバス
    [SerializeField]
    Canvas parentCanvas = null;

    // ダイアログ
    [SerializeField]
    ResetDialog dialog = null;

    public void ShowDialog()
    {
        // ダイアログを生成してboardDisplayの子オブジェクトにする
        var _dialogGameEnd = Instantiate(dialog);
        _dialogGameEnd.transform.SetParent(parentCanvas.transform, false);
        // ボタンが押されたときのイベント処理
        _dialogGameEnd.FixDialog = result => Debug.Log(result);
    }
}
