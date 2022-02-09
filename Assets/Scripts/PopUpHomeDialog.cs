using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpHomeDialog : MonoBehaviour
{
    // ダイアログを追加する先の親キャンバス
    [SerializeField]
    Canvas parentCanvas = null;

    // ダイアログ
    [SerializeField]
    HomeDialog dialog = null;

    // 音源
    private AudioSource audioSource;

    void Start()
    {
        // 音源
        audioSource = GetComponent<AudioSource>();
    }

    public void ShowDialog()
    {
        audioSource.Play();
        // ダイアログを生成してboardDisplayの子オブジェクトにする
        var _dialogGameEnd = Instantiate(dialog);
        _dialogGameEnd.transform.SetParent(parentCanvas.transform, false);
        // ボタンが押されたときのイベント処理
        _dialogGameEnd.FixDialog = result => Debug.Log(result);
    }
}
