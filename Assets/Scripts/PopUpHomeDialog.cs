using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpHomeDialog : MonoBehaviour
{
    // �_�C�A���O��ǉ������̐e�L�����o�X
    [SerializeField]
    Canvas parentCanvas = null;

    // �_�C�A���O
    [SerializeField]
    HomeDialog dialog = null;

    // ����
    private AudioSource audioSource;

    void Start()
    {
        // ����
        audioSource = GetComponent<AudioSource>();
    }

    public void ShowDialog()
    {
        audioSource.Play();
        // �_�C�A���O�𐶐�����boardDisplay�̎q�I�u�W�F�N�g�ɂ���
        var _dialogGameEnd = Instantiate(dialog);
        _dialogGameEnd.transform.SetParent(parentCanvas.transform, false);
        // �{�^���������ꂽ�Ƃ��̃C�x���g����
        _dialogGameEnd.FixDialog = result => Debug.Log(result);
    }
}
