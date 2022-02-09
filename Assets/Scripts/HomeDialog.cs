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

    // �Q�[���R���g���[���[
    [SerializeField]
    GameController gameManager = null;

    // �Q�[���R���g���[���[�I�u�W�F�N�g
    GameController gc;

    public Action<DIALOGRESULT> FixDialog { get; set; }

    void Start()
    {
        gc = gameManager.GetComponent<GameController>();
    }

    /// <summary>
    /// OK�{�^���������ꂽ�ꍇ�ɌĂяo�����
    /// </summary>
    public void OnOK()
    {
        this.FixDialog?.Invoke(DIALOGRESULT.OK);
        gc.ChangeScene();
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
