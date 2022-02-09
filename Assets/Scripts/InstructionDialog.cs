using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionDialog : MonoBehaviour
{
    public enum DIALOGRESULT
    {
        CLOSE,
        BACK,
        FORWARD,
    }

    // 親キャンバス
    [SerializeField]
    Canvas parentCanvas = null;

    // 画像
    [SerializeField]
    GameObject imageBack = null, imageForward = null;

    public Action<DIALOGRESULT> FixDialog { get; set; }

    /// <summary>
    /// Closeボタンが押された場合に呼び出される
    /// </summary>
    public void OnClose()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.CLOSE);
        Destroy(this.gameObject);
    }


    /// <summary>
    /// Backボタンが押された場合に呼び出される
    /// </summary>
    public void OnBack()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.BACK);
        //画像を生成してparentCanvasの子オブジェクトにする
        var _imageBack = Instantiate(imageBack);
        _imageBack.transform.SetParent(parentCanvas.transform, false);
        Destroy(this.gameObject);
    }


    /// <summary>
    /// Forwardボタンが押された場合に呼び出される
    /// </summary>
    public void OnForward()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.FORWARD);
        //画像を生成してparentCanvasの子オブジェクトにする
        var _imageForward = Instantiate(imageForward);
        _imageForward.transform.SetParent(parentCanvas.transform, false);
        Destroy(this.gameObject);
    }
}
