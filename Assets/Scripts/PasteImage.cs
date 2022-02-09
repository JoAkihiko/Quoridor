using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasteImage : MonoBehaviour
{
    // �e�L�����o�X
    [SerializeField]
    Canvas parentCanvas = null;

    // �摜
    [SerializeField]
    GameObject image = null;

    // ����
    private AudioSource audioSource;

    void Start()
    {
        // ����
        audioSource = GetComponent<AudioSource>();
    }

    public void ButtonPaste()
    {
        audioSource.Play();
        //�摜�𐶐�����parentCanvas�̎q�I�u�W�F�N�g�ɂ���
        var _image = Instantiate(image);
        _image.transform.SetParent(parentCanvas.transform, false);
        // �{�^���������ꂽ�Ƃ��̃C�x���g����
    }
}
