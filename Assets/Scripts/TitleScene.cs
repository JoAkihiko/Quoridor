using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    Text textWidth = null, textHeight = null;

    [SerializeField]
    Text textGreenWall = null, textOrangeWall = null;

    // SE
    [SerializeField]
    AudioClip soundSceneChange = null, soundPushButton = null, soundError = null;

    private static int width = 9, height = 9;
    private static int greenWall = 10, orangeWall = 10;

    // 音源
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        textHeight.text = "" + height;
        textWidth.text = "" + width;
        textGreenWall.text = "" + greenWall;
        textOrangeWall.text = "" + orangeWall;

        // 音源
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        textHeight.text = "" + height;
        textWidth.text = "" + width;
        textGreenWall.text = "" + greenWall;
        textOrangeWall.text = "" + orangeWall;
    }

    /// <summary>
    /// メインシーン（ゲームシーン）に切り替え
    /// </summary>
    public void ChangeScene()
    {
        // SE
        audioSource.clip = soundSceneChange;
        audioSource.volume = 0.5f;
        audioSource.Play();
        SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// 盤面の列数を一つ増やす
    /// </summary>
    public void IncrementHeight()
    {
        if (height < 11)
        {
            height++;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 盤面の列数を一つ減らす
    /// </summary>
    public void DecrementHeight()
    {
        if (height > 3)
        {
            height--;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 盤面の列数をデフォルト値にする
    /// </summary>
    public void DefaultHeight()
    {
        height = 9;
        PlayPushButtonSound();
    }

    /// <summary>
    /// 盤面の行数を一つ増やす
    /// </summary>
    public void IncrementWidth()
    {
        if(width < 11)
        {
            width++;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 盤面の行数を一つ減らす
    /// </summary>
    public void DecrementWidth()
    {
        if(width > 3)
        {
            width--;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 盤面の行数をデフォルト値にする
    /// </summary>
    public void DefaultWidth()
    {
        width = 9;
        PlayPushButtonSound();
    }

    /// <summary>
    /// 緑プレイヤーの壁の数を一つ増やす
    /// </summary>
    public void IncrementGreenWall()
    {
        if (greenWall < 99)
        {
            greenWall++;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 緑プレイヤーの壁の数を一つ減らす
    /// </summary>
    public void DecrementGreenWall()
    {
        if (greenWall > 0)
        {
            greenWall--;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 緑プレイヤーの壁の数をデフォルト値にする
    /// </summary>
    public void DefaultGreenWall()
    {
        greenWall = 10;
        PlayPushButtonSound();
    }

    /// <summary>
    /// 橙プレイヤーの壁の数を一つ増やす
    /// </summary>
    public void IncrementOrangeWall()
    {
        if (orangeWall < 99)
        {
            orangeWall++;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 橙プレイヤーの壁の数を一つ減らす
    /// </summary>
    public void DecrementOrangeWall()
    {
        if (orangeWall > 0)
        {
            orangeWall--;
            PlayPushButtonSound();
        }
        else
        {
            PlayErrorSound();
        }
    }

    /// <summary>
    /// 橙プレイヤーの壁の数をデフォルト値にする
    /// </summary>
    public void DefaultOrangeWall()
    {
        orangeWall = 10;
        PlayPushButtonSound();
    }

    private void PlayPushButtonSound()
    {
        // SE
        audioSource.clip = soundPushButton;
        audioSource.volume = 0.1f;
        audioSource.Play();
    }

    private void PlayErrorSound()
    {
        // SE
        audioSource.clip = soundError;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    /// <summary>
    /// タイトルシーンで設定した値を返す
    /// </summary>
    /// <returns>盤面の行数と列数、両プレイヤーの壁の枚数</returns>
    public static int[] GetParam()
    {
        return new int[] { height, width, greenWall, orangeWall };
    }
}
