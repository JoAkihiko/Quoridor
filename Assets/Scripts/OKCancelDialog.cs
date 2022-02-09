using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OKCancelDialog : MonoBehaviour
{
    public enum DIALOGRESULT
    {
        OK,
        CANCEL,
    }

    public Action<DIALOGRESULT> FixDialog { get; set; }
    
    /// <summary>
    /// OKボタンが押された場合に呼び出される
    /// </summary>
    public void OnOK()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.OK);
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
