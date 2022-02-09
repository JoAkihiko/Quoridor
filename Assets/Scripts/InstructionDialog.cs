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

    // �e�L�����o�X
    [SerializeField]
    Canvas parentCanvas = null;

    // �摜
    [SerializeField]
    GameObject imageBack = null, imageForward = null;

    public Action<DIALOGRESULT> FixDialog { get; set; }

    /// <summary>
    /// Close�{�^���������ꂽ�ꍇ�ɌĂяo�����
    /// </summary>
    public void OnClose()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.CLOSE);
        Destroy(this.gameObject);
    }


    /// <summary>
    /// Back�{�^���������ꂽ�ꍇ�ɌĂяo�����
    /// </summary>
    public void OnBack()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.BACK);
        //�摜�𐶐�����parentCanvas�̎q�I�u�W�F�N�g�ɂ���
        var _imageBack = Instantiate(imageBack);
        _imageBack.transform.SetParent(parentCanvas.transform, false);
        Destroy(this.gameObject);
    }


    /// <summary>
    /// Forward�{�^���������ꂽ�ꍇ�ɌĂяo�����
    /// </summary>
    public void OnForward()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.FORWARD);
        //�摜�𐶐�����parentCanvas�̎q�I�u�W�F�N�g�ɂ���
        var _imageForward = Instantiate(imageForward);
        _imageForward.transform.SetParent(parentCanvas.transform, false);
        Destroy(this.gameObject);
    }
}
