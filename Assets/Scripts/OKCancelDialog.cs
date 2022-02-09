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
    /// OK�{�^���������ꂽ�ꍇ�ɌĂяo�����
    /// </summary>
    public void OnOK()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.OK);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Cancel�{�^���������ꂽ�ꍇ�ɌĂяo�����
    /// </summary>
    public void OnCancel()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.CANCEL);
        Destroy(this.gameObject);
    }

}
