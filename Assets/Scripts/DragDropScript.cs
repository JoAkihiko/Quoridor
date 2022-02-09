using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// not used
/// </summary>
public class DragDropScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    // �Q�[���R���g���[���[
    [SerializeField]
    GameObject gameManager = null;

    // ��
    [SerializeField]
    GameObject wallObject = null;

    // �ǂ̐ݒu�ꏊ
    [SerializeField]
    GameObject wallField = null;

    // �Q�[���R���g���[���[�I�u�W�F�N�g
    GameController gc;

    private Vector2 prevPosition;

    public bool[,] isVerticalWalls = new bool[9, 9];

    void Start()
    {
        gc = gameManager.GetComponent<GameController>();
    }

    /// <summary>
    /// �h���b�O�J�n���ɌĂяo�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        prevPosition = transform.position;
    }

    /// <summary>
    /// �h���b�O���ɌĂяo�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    /// <summary>
    /// �h���b�O�I���ɌĂяo�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {

        GameObject wallobj = null;

        Debug.Log(transform.position);

        (int w, int h) = GetNearestPoint(transform.position);

        // �ǂ��ݒu�\�Ȉʒu���ǂ���
        if (w > 0 && w < 9 && h > 0 && h < 9 && !isVerticalWalls[h-1,w-1] && (h==9 || !isVerticalWalls[h , w - 1]))
        {
            wallobj = Instantiate(wallObject, new Vector2(455+w*50, 610-h*50), Quaternion.identity);
            // �擾����Prefab��wallDisplay�̎q�I�u�W�F�N�g�ɂ���
            wallobj.transform.SetParent(wallField.transform);
            isVerticalWalls[h - 1, w - 1] = true;
            if (h < 9)
            {
                isVerticalWalls[h, w - 1] = true;
            }
        }

        gc.NextTurn();

        transform.position = prevPosition;
    }

    (int, int) GetNearestPoint(Vector2 dropPosition)
    {
        int w, h;
        for (w = 0; dropPosition.x > 480 + w * 50; w++);
        for (h = 0; dropPosition.y< 585 - h * 50; h++);

        Debug.Log(w.ToString());
        Debug.Log(h.ToString());

        return (w, h);
    }

    public bool[,] GetIsWalls()
    {
        return isVerticalWalls;
    }

}