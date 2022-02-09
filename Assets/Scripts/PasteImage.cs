using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasteImage : MonoBehaviour
{
    // 親キャンバス
    [SerializeField]
    Canvas parentCanvas = null;

    // 画像
    [SerializeField]
    GameObject image = null;

    // 音源
    private AudioSource audioSource;

    void Start()
    {
        // 音源
        audioSource = GetComponent<AudioSource>();
    }

    public void ButtonPaste()
    {
        audioSource.Play();
        //画像を生成してparentCanvasの子オブジェクトにする
        var _image = Instantiate(image);
        _image.transform.SetParent(parentCanvas.transform, false);
        // ボタンが押されたときのイベント処理
    }
}
