using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeDialog : MonoBehaviour
{
    public enum DIALOGRESULT
    {
        OK,
        CANCEL,
    }

    // ゲームコントローラー
    [SerializeField]
    GameController gameManager = null;

    // ゲームコントローラーオブジェクト
    GameController gc;

    public Action<DIALOGRESULT> FixDialog { get; set; }

    void Start()
    {
        gc = gameManager.GetComponent<GameController>();
    }

    /// <summary>
    /// OKボタンが押された場合に呼び出される
    /// </summary>
    public void OnOK()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.OK);
        gc.ChangeScene();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Cancelボタンが押された場合に呼び出される
    /// </summary>
    public void OnCancel()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.CANCEL);
        Destroy(this.gameObject);
    }

}
