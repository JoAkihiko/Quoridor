using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHorizontalWallsScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // �Ֆʂ̍����ƕ�
    int HEIGHT, WIDTH;

    // �Q�[���R���g���[���[
    [SerializeField]
    GameObject gameManager = null;

    // �e�L�����o�X
    [SerializeField]
    Canvas parentCanvas = null;

    // ��
    [SerializeField]
    GameObject wallObject = null;

    // �ǂ̐ݒu�ꏊ
    [SerializeField]
    GameObject wallField = null;

    // �Q�[���R���g���[���[�I�u�W�F�N�g
    GameController gc;

    // �ǐ����{�^��
    [SerializeField]
    GameObject verticalWallButtonObject = null;

    // �����ɏ�����ꎞ�I�ȃ_�C�A���O
    [SerializeField]
    GameObject dialogTemporary = null;

    // �ꎞ�I�ȃ_�C�A���O�̃e�L�X�g
    [SerializeField]
    Text textDialog = null;

    // SE
    [SerializeField]
    AudioClip soundSetWall = null;

    DragVerticalWallsScript dvws;

    private Vector2 prevPosition;

    // point of intersection(PI)�ɕǂ��ݒu����Ă��邩�ǂ����i[�s, ��]�j
    public bool[,] isWallsAtPI = new bool[8,8];

    // [�s�C��]
    // public bool[,] isWalls = new bool[9, 9];

    // ����
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        gc = gameManager.GetComponent<GameController>();
        dvws = verticalWallButtonObject.GetComponent<DragVerticalWallsScript>();
        audioSource = GetComponent<AudioSource>();

        // �^�C�g���V�[�����瓾��l
        int[] titlePram = TitleScene.GetParam();
        HEIGHT = titlePram[0];
        WIDTH = titlePram[1];

        isWallsAtPI = new bool[HEIGHT-1, WIDTH-1];
    }

    /// <summary>
    /// �h���b�O�J�n���ɌĂяo�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gc.IsRemainWall())
        {
            SetDialogTemporary("�ǂ͎g���؂�܂���");
        }

        prevPosition = transform.position;
    }

    /// <summary>
    /// �h���b�O���ɌĂяo�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if(gc.IsRemainWall() && !gc.isGameEnd)
        {
            transform.position = eventData.position;
        }
    }

    /// <summary>
    /// �h���b�O�I���ɌĂяo�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (gc.isGameEnd)
        {
            return;
        }

        GameObject wallobj = null;

        Debug.Log(transform.position);

        (int h, int w) = GetNearestPoint(transform.position);

        // �ǂ��ݒu�\�Ȉʒu���ǂ����i���Ԕ��΂̏ꍇ�o�O�j
        if (gc.IsRemainWall() && IsSetWall(h, w))
        {
            wallobj = Instantiate(wallObject, new Vector2(455 - (25 * (WIDTH - 9)) + w * 50, 610 + (25 * (HEIGHT - 9)) - h * 50), Quaternion.identity);
            // �擾����Prefab��wallField�̎q�I�u�W�F�N�g�ɂ���
            wallobj.transform.SetParent(wallField.transform);

            // �ǐݒuSE
            audioSource.clip = soundSetWall;
            audioSource.time = 0.15f;
            audioSource.Play();

            gc.ReduceWall();
            gc.NextTurn();
            transform.position = prevPosition;
            return;
        }

        gc.KeepTurn();
        transform.position = prevPosition;
    }

    /// <summary>
    /// �����̈ʒu����ł��߂��}�X�̌����_�̈ʒu�����߂�
    /// </summary>
    /// <param name="dropPosition">�h���b�O�I���ʒu</param>
    /// <returns>�}�X�̌����_�̈ʒu�i�c�A���j</returns>
    private (int, int) GetNearestPoint(Vector2 dropPosition)
    {
        int h, w;
        for (h = 0; dropPosition.y < 585 + (25 * (HEIGHT-9)) - h * 50; h++) ;
        for (w = 0; dropPosition.x > 480 - (25 * (WIDTH - 9)) + w * 50; w++) ;

        Debug.Log("nearest point...");
        Debug.Log(w.ToString());
        Debug.Log(h.ToString());

        return (h, w);
    }

    /// <summary>
    /// ���̃N���X���}�X�̌����ʒu�ɕǂ��ݒu����Ă��邩���m�F����ꍇ�ɌĂяo�����
    /// </summary>
    /// <param name="h">�}�X�̌����ʒu+1�i�c�j</param>
    /// <param name="w">�}�X�̌����ʒu+1�i���j</param>
    /// <returns></returns>
    public bool GetIsWallsAtPI(int h, int w)
    {
        return isWallsAtPI[h-1, w-1];
    }

    /// <summary>
    /// �ǂ̐ݒu�\�ʒu���ǂ���
    /// </summary>
    /// <param name="h">�}�X�ʒu�i�c�j</param>
    /// <param name="w">�}�X�ʒu�i���j</param>
    /// <returns></returns>
    private bool IsSetWall(int h, int w)
    {
        // �ՖʊO�̏ꍇ
        if (!(w > 0 && w < WIDTH && h > 0 && h < HEIGHT))
        {
            SetDialogTemporary("�ՖʊO�ɕǂ͒u���܂���");
            return false;
        }
        // ���ɉ��ǂ��u���Ă���ꍇ
        if ((w != 1 && isWallsAtPI[h - 1, w - 2]) || isWallsAtPI[h - 1, w - 1] || (w != WIDTH-1 && isWallsAtPI[h - 1, w]))
        {
            SetDialogTemporary("�ǂ��d�Ȃ�Ƃ���ɕǂ͒u���܂���");
            return false;
        }
        // ���ɏc�ǂ��u���Ă���ꍇ
        if (dvws.GetIsWallsAtPI(h, w))
        {
            SetDialogTemporary("�ǂ��d�Ȃ�Ƃ���ɕǂ͒u���܂���");
            return false;
        }

        isWallsAtPI[h - 1, w - 1] = true;
        // �S�[���ւ̌o�H���ǂ���Ă��Ȃ���
        if (gc.IsWay2Goal()) 
        {
            Debug.Log("Can Reach GOAL!");
            return true;
        }
        else
        {
            SetDialogTemporary("�S�[���ւ̌o�H���Ȃ��Ȃ�Ƃ���ɕǂ͒u���܂���");
            Debug.Log("Can not Reach GOAL!");
            isWallsAtPI[h - 1, w - 1] = false;
            return false;
        }
    }

    /// <summary>
    /// �����ɏ�����_�C�A���O��\������
    /// </summary>
    /// <param name="text">�_�C�A���O�ɕ\�����郁�b�Z�[�W</param>
    private void SetDialogTemporary(string text)
    {
        textDialog.text = text;
        //�_�C�A���O�𐶐�����parentCanvas�̎q�I�u�W�F�N�g�ɂ���
        var _dialogTemporary = Instantiate(dialogTemporary);
        _dialogTemporary.transform.SetParent(parentCanvas.transform, false);
    }

}