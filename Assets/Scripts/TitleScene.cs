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

    // ����
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        textHeight.text = "" + height;
        textWidth.text = "" + width;
        textGreenWall.text = "" + greenWall;
        textOrangeWall.text = "" + orangeWall;

        // ����
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
    /// ���C���V�[���i�Q�[���V�[���j�ɐ؂�ւ�
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
    /// �Ֆʂ̗񐔂�����₷
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
    /// �Ֆʂ̗񐔂�����炷
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
    /// �Ֆʂ̗񐔂��f�t�H���g�l�ɂ���
    /// </summary>
    public void DefaultHeight()
    {
        height = 9;
        PlayPushButtonSound();
    }

    /// <summary>
    /// �Ֆʂ̍s��������₷
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
    /// �Ֆʂ̍s��������炷
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
    /// �Ֆʂ̍s�����f�t�H���g�l�ɂ���
    /// </summary>
    public void DefaultWidth()
    {
        width = 9;
        PlayPushButtonSound();
    }

    /// <summary>
    /// �΃v���C���[�̕ǂ̐�������₷
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
    /// �΃v���C���[�̕ǂ̐�������炷
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
    /// �΃v���C���[�̕ǂ̐����f�t�H���g�l�ɂ���
    /// </summary>
    public void DefaultGreenWall()
    {
        greenWall = 10;
        PlayPushButtonSound();
    }

    /// <summary>
    /// ��v���C���[�̕ǂ̐�������₷
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
    /// ��v���C���[�̕ǂ̐�������炷
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
    /// ��v���C���[�̕ǂ̐����f�t�H���g�l�ɂ���
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
    /// �^�C�g���V�[���Őݒ肵���l��Ԃ�
    /// </summary>
    /// <returns>�Ֆʂ̍s���Ɨ񐔁A���v���C���[�̕ǂ̖���</returns>
    public static int[] GetParam()
    {
        return new int[] { height, width, greenWall, orangeWall };
    }
}
