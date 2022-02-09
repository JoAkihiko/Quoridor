using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // �e�Ֆʂɐ������蓖��
    enum COLOR
    {
        // �� = 0
        EMPTY,

        // �� = 1
        GREEN,

        // �� = 2
        ORANGE,

        // ���n = 3
        CANDIDATE
    }

    // �����ɐ������蓖��
    enum DIRECTION
    {
        // �� = 0
        UP,

        // �� = 1
        DOWN,

        // �� = 2
        LEFT,

        // �E = 3
        RIGHT
    }

    // �Ֆʂ̍����ƕ� (Start()�Œl�ύX)
    private int HEIGHT = 9, WIDTH=9;

    // ��̔ՖʁC�΂̋�C��̋�
    [SerializeField]
    GameObject emptyObject = null, greenObject=null, orangeObject = null, candidateObject = null;

    // ��
    [SerializeField]
    GameObject wallObject = null;

    // �ǐ����{�^��
    [SerializeField]
    GameObject verticalWallButtonObject = null, horizontalWallButtonObject = null;

    // �c��Ǖ\���e�L�X�g
    [SerializeField]
    Text textFrameGreen, textFrameOrange;

    // �Ֆʂ̓y��
    [SerializeField]
    GameObject boardDisplay = null;

    // �ǂ̓y��
    [SerializeField]
    GameObject wallDisplay = null;

    // �ǂ̐ݒu�ꏊ
    [SerializeField]
    GameObject wallField = null;

    // �w�i
    [SerializeField]
    GameObject backGround = null;

    // �Q�[���I���_�C�A���O
    [SerializeField]
    ResetDialog dialogGameEnd = null;

    // �Q�[���I���e�L�X�g
    [SerializeField]
    Text textUpper = null;

    // SE
    [SerializeField]
    AudioClip soundPieceMoving = null, soundGameEnd = null;

    // �񎟌��z�� �Ֆ� (�c�~��)
    COLOR[,] board;

    // �s���v���C���[
    COLOR player = COLOR.GREEN;

    // �ǂ̃X�N���v�g
    DragVerticalWallsScript dvws;
    DragHorizontalWallsScript dhws;

    // �Ǘ��đI�𒆂��ǂ���
    private bool isBuildingWall = false;

    // ��̈ʒu
    private int[] greenPosition, orangePosition;

    // �ǐݒu�\���̃`�F�b�N���ɕK�v (���Ƀ`�F�b�N�ς݂̃|�W�V�������ǂ���)
    private bool[,] passedPosition;

    // �����̕ǂ̐�
    private int numWallGreen = 10, numWallOrange = 10;

    // �c���Ă���g�p�\�ȕǂ̐�
    private int remainWallGreen, remainWallOrange;

    // �����I�����ǂ���
    public bool isGameEnd;

    // �w�i�̐F�w��
    private Color32 greenColor = new Color32(170, 255, 230, 255), orangeColor = new Color32(255, 230, 170, 255);

    // ����
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        // �^�C�g���V�[�����瓾��l
        int[] titlePram = TitleScene.GetParam();
        HEIGHT = titlePram[0];
        WIDTH = titlePram[1];
        numWallGreen = titlePram[2];
        numWallOrange = titlePram[3];

        // RectTransform�̑傫�����}�X�ڂ̐��ɉ����ĕύX
        RectTransform rt = boardDisplay.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50*WIDTH, 50*HEIGHT);

        board = new COLOR[HEIGHT, WIDTH];
        greenPosition = new int[] { 0, WIDTH / 2 };
        orangePosition = new int[] { HEIGHT - 1, WIDTH / 2 };

        // �c�ǂƉ��ǂ̃X�N���v�g
        dvws = verticalWallButtonObject.GetComponent<DragVerticalWallsScript>();
        dhws = horizontalWallButtonObject.GetComponent<DragHorizontalWallsScript>();

        // ����
        audioSource = GetComponent<AudioSource>();

        // ������
        Initialized(); 
    }

    private void Update()
    {
        textFrameGreen.text = "�c��ǖ��� : " + remainWallGreen;
        textFrameOrange.text = "�c��ǖ��� : " + remainWallOrange;
    }

    public void Initialized()
    {
        // �e�p�����[�^������
        greenPosition =  new int[] { 0, WIDTH / 2 };
        orangePosition = new int[] { HEIGHT - 1, WIDTH / 2 };
        player = COLOR.GREEN;
        backGround.GetComponent<Image>().color = greenColor;
        isBuildingWall = false;
        remainWallGreen = numWallGreen;
        remainWallOrange = numWallOrange;
        isGameEnd = false;

        textUpper.text = "�~�h���̃^�[��";

        dvws.isWallsAtPI = new bool[HEIGHT-1, WIDTH-1];
        dhws.isWallsAtPI = new bool[HEIGHT-1, WIDTH-1];

        UpdateAll();

        // wallField�̎q�I�u�W�F�N�g��S�č폜
        foreach (Transform child in wallField.transform)
        {
            Destroy(child.gameObject);
        }
    }


    /// <summary>
    /// �v���C���[�̈ʒu�ƕǂ̈ʒu�����ݒu�\�ʒu�i���n�j��T���ăv���C���[�̈ʒu�ƌ��n���Q�[����ʂɔ��f
    /// </summary>
    void UpdateAll()
    {
        // �ՖʃA�b�v�f�[�g
        UpdateBoard();
        if (!isGameEnd)
        {
            // ���u����ꏊ��T��
            FindCandidate();
        }
        // �Ֆʕ\��
        ShowBoard();
    }

    /// <summary>
    /// �v���C���[�̐V���Ȉʒu�������f�����ՖʂɃA�b�v�f�[�g
    /// </summary>
    void UpdateBoard()
    {
        for (int h = 0; h < HEIGHT; h++)
        {
            for (int w = 0; w < WIDTH; w++)
            {
                // �S�Ֆʂ���
                board[h, w] = COLOR.EMPTY;
            }
        }
        board[greenPosition[0], greenPosition[1]] = COLOR.GREEN;
        board[orangePosition[0], orangePosition[1]] = COLOR.ORANGE;
    }

    /// <summary>
    /// ���݂̔Ֆʂ��Q�[����ʂɔ��f
    /// </summary>
    void ShowBoard()
    {
        // boardDisplay�̎q�I�u�W�F�N�g��S�č폜
        foreach (Transform child in boardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        for (int h = 0; h < HEIGHT; h++)
        {
            for (int w = 0; w < WIDTH; w++)
            {
                // Prefab�擾
                GameObject piece = GetPrefab(board[h, w]);

                if (board[h, w] == COLOR.CANDIDATE && !isBuildingWall)
                {
                    // piece�ɃC�x���g�ݒ�
                    int y = h;
                    int x = w;
                    piece.GetComponent<Button>().onClick.AddListener(() => { MovePiece(y + "," + x); });
                }

                // �擾����Prefab��boardDisplay�̎q�I�u�W�F�N�g�ɂ���
                piece.transform.SetParent(boardDisplay.transform);
            }
        }
    }

    /// <summary>
    /// ��̐ݒu�\�ʒu��T��
    /// </summary>
    void FindCandidate()
    {
        // �s���v���C���[�̋�̈ʒu
        int[] playerPosition = player == COLOR.GREEN ? greenPosition : orangePosition;

        Debug.Log(playerPosition[0]);
        Debug.Log(playerPosition[1]);

        // ��̈ʒu (�c)
        int ppH = playerPosition[0];
        // ��̈ʒu (��)
        int ppW = playerPosition[1]; 

        if (IsUp(ppH, ppW))
        {
            CandidateDisplay(new int[] { ppH - 1, ppW }, DIRECTION.UP);
        }
        if (IsDown(ppH, ppW))
        { 
            CandidateDisplay(new int[] { ppH + 1, ppW }, DIRECTION.DOWN);
        }
        if (IsLeft(ppH, ppW))
        {
            CandidateDisplay(new int[] { ppH, ppW - 1 }, DIRECTION.LEFT);
        }
        if (IsRight(ppH, ppW))
        {
            CandidateDisplay(new int[] { ppH, ppW + 1 }, DIRECTION.RIGHT);
        }
    }

    /// <summary>
    /// ���n��T��
    /// </summary>
    /// <param name="candidatePosition">���n�̈ʒu</param>
    void CandidateDisplay(int[] candidatePosition, DIRECTION direction)
    {
        // �ݒu�\�ӏ����`�F�b�N
        if (candidatePosition[0] >=0 && candidatePosition[0] < HEIGHT && candidatePosition[1]>=0 && candidatePosition[1] < WIDTH)
        {
            COLOR candidatePiece = board[candidatePosition[0], candidatePosition[1]];
            // ���n�ɗ΃v���C���[�������Ƃ�
            if (candidatePiece == COLOR.GREEN)
            {
                JumpOver(orangePosition, candidatePosition, direction);
                /*
                int[] position = new int[] { candidatePosition[0] * 2 - orangePosition[0], candidatePosition[1] * 2 - orangePosition[1] };
                if(position[0] >=0 && position[0] < HEIGHT && position[1] >= 0 && position[1] < WIDTH)
                {
                    if ((direction != DIRECTION.UP || IsDown(position[0], position[1])) && (direction != DIRECTION.DOWN || IsUp(position[0], position[1])) && (direction != DIRECTION.LEFT || IsRight(position[0], position[1])) && (direction != DIRECTION.RIGHT || IsLeft(position[0], position[1])))
                    {
                        // �΂𒴂����ʒu�����n
                        board[position[0], position[1]] = COLOR.CANDIDATE;
                    }
                    if(direction == DIRECTION.UP)
                    {
                        if(IsDown(position[0], position[1]))
                        {
                            // �΂𒴂����ʒu�����n
                            board[position[0], position[1]] = COLOR.CANDIDATE;
                        }
                        else
                        {
                            if(IsLeft(candidatePosition[0], candidatePosition[1]))
                            {
                                // ����v���C���[�ׂ̗����n
                                board[candidatePosition[0], candidatePosition[1] - 1] = COLOR.CANDIDATE;
                            }
                            if (IsRight(candidatePosition[0], candidatePosition[1]))
                            {
                                // ����v���C���[�ׂ̗����n
                                board[candidatePosition[0], candidatePosition[1] + 1] = COLOR.CANDIDATE;
                            }
                        }
                    }
                }
            */
            }
            // ���n�ɞ�v���C���[�������Ƃ�
            else if (candidatePiece == COLOR.ORANGE)
            {
                JumpOver(greenPosition, candidatePosition, direction);
                /*
                int[] position = new int[] { candidatePosition[0] * 2 - greenPosition[0], candidatePosition[1] * 2 - greenPosition[1] };
                if (position[0] >= 0 && position[0] < HEIGHT && position[1] >= 0 && position[1] < WIDTH)
                {
                    if ((direction != DIRECTION.UP || IsDown(position[0], position[1])) && (direction != DIRECTION.DOWN || IsUp(position[0], position[1])) && (direction != DIRECTION.LEFT || IsRight(position[0], position[1])) && (direction != DIRECTION.RIGHT || IsLeft(position[0], position[1])))
                    {
                        // ��𒴂����ʒu�����n
                        board[position[0], position[1]] = COLOR.CANDIDATE;
                    }
                }
                */
            } 
            else
            {
                board[candidatePosition[0], candidatePosition[1]] = COLOR.CANDIDATE;
            }
        }
    }

    void JumpOver(int[] selfPosition, int[] opponentPosition, DIRECTION direction)
    {
        int[] position = new int[] { opponentPosition[0] * 2 - selfPosition[0], opponentPosition[1] * 2 - selfPosition[1] };
        if (position[0] >= 0 && position[0] < HEIGHT && position[1] >= 0 && position[1] < WIDTH)
        {
            if (direction == DIRECTION.UP)
            {
                if (IsDown(position[0], position[1]))
                {
                    // ����v���C���[�𒴂����ʒu�����n
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsLeft(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0], opponentPosition[1] - 1] = COLOR.CANDIDATE;
                    }
                    if (IsRight(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0], opponentPosition[1] + 1] = COLOR.CANDIDATE;
                    }
                }
            }
            else if (direction == DIRECTION.DOWN)
            {
                if (IsUp(position[0], position[1]))
                {
                    // ����v���C���[�𒴂����ʒu�����n
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsLeft(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0], opponentPosition[1] - 1] = COLOR.CANDIDATE;
                    }
                    if (IsRight(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0], opponentPosition[1] + 1] = COLOR.CANDIDATE;
                    }
                }
            }
            else if (direction == DIRECTION.LEFT)
            {
                if (IsRight(position[0], position[1]))
                {
                    // ����v���C���[�𒴂����ʒu�����n
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsUp(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0] - 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                    if (IsDown(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0] + 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                }
            }
            else if (direction == DIRECTION.RIGHT)
            {
                if (IsLeft(position[0], position[1]))
                {
                    // ����v���C���[�𒴂����ʒu�����n
                    board[position[0], position[1]] = COLOR.CANDIDATE;
                }
                else
                {
                    if (IsUp(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0] - 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                    if (IsDown(opponentPosition[0], opponentPosition[1]))
                    {
                        // ����v���C���[�ׂ̗����n
                        board[opponentPosition[0] + 1, opponentPosition[1]] = COLOR.CANDIDATE;
                    }
                }
            }
        }
    }

    /// <summary>
    /// �}�X�ɍ������v���n�u���擾
    /// </summary>
    /// <param name="color">�}�X�̎��</param>
    /// <returns>�}�X�ɍ������v���n�u</returns>
    GameObject GetPrefab(COLOR color)
    {
        GameObject prefab;
        switch(color)
        {
            // ��̂Ƃ�
            case COLOR.EMPTY:   
                prefab = Instantiate(emptyObject);
                break;
            // �΂̂Ƃ�
            case COLOR.GREEN:   
                prefab = Instantiate(greenObject);
                break;
            // ��̂Ƃ�
            case COLOR.ORANGE:  
                prefab = Instantiate(orangeObject);
                break;
            // ���n�̂Ƃ�
            case COLOR.CANDIDATE:   
                prefab = Instantiate(candidateObject);
                break;
            default:
                prefab = null;
                break;
        }
        return prefab;
    }

    /// <summary>
    /// ��𓮂���
    /// </summary>
    /// <param name="position">������̃}�X�istring�^�ł��邱�Ƃɒ��Ӂj</param>
    public void MovePiece(string position)
    {
        Debug.Log(position);
        int h = int.Parse(position.Split(',')[0]);
        int w = int.Parse(position.Split(',')[1]);
        // �s���v���C���[�̋�̈ʒu
        board[h, w] = player;

        if(player == COLOR.GREEN)
        {
            greenPosition = new int[] { h, w };
            player = COLOR.ORANGE;
            backGround.GetComponent<Image>().color = orangeColor;
            textUpper.text = "�I�����W�̃^�[��";
        }
        else
        {
            orangePosition = new int[] { h, w };
            player = COLOR.GREEN;
            backGround.GetComponent<Image>().color = greenColor;
            textUpper.text = "�~�h���̃^�[��";
        }

        // �����I������
        if (greenPosition[0] == HEIGHT-1 || orangePosition[0] == 0)
        {
            isGameEnd = true;

            // SE
            audioSource.clip = soundGameEnd;
            audioSource.time = 0.0f;
            audioSource.Play();
        }
        else
        {
            // SE
            audioSource.clip = soundPieceMoving;
            audioSource.time = 0.5f;
            audioSource.Play();
        }

        UpdateAll();

        // �����I�����̏���
        if (isGameEnd)
        {
            // �w�i�𔒂ɂ���
            backGround.GetComponent<Image>().color = Color.white;
            textUpper.text = "���̃Q�[�����n�߂�ɂ͍�����Reset�{�^���������Ă�������";

            // �����I���_�C�A���O�𐶐�����wallDisplay�̎q�I�u�W�F�N�g�ɂ���
            var _dialogGameEnd = Instantiate(dialogGameEnd);
            _dialogGameEnd.transform.SetParent(wallDisplay.transform, false);
            // �{�^���������ꂽ�Ƃ��̃C�x���g����
            _dialogGameEnd.FixDialog = result => Debug.Log(result);
        }
    }

    /// <summary>
    /// ���̃v���C���[�̃^�[���ɂȂ�
    /// </summary>
    public void NextTurn()
    {
        if(player == COLOR.GREEN)
        {
            player = COLOR.ORANGE;
            backGround.GetComponent<Image>().color = orangeColor;
            textUpper.text = "�I�����W�̃^�[��";
        }
        else
        {
            player = COLOR.GREEN;
            backGround.GetComponent<Image>().color = greenColor;
            textUpper.text = "�~�h���̃^�[��";
        }

        UpdateAll();
    }

    /**
    <summary>
    ���N���X����Ֆʂ��A�b�v�f�[�g�������ꍇ�ɌĂяo�����
    </summary>
    */
    public void KeepTurn()
    {
        UpdateAll();
    }

    /// <summary>
    /// �s���v���C���[�̎g�p�\�ȕǂ̐�������炷
    /// </summary>
    public void ReduceWall()
    {
        if (player == COLOR.GREEN)
        {
            if(remainWallGreen > 0)
            {
                remainWallGreen--;
            }
        }
        else
        {
            if (remainWallOrange > 0)
            {
                remainWallOrange--;
            }
        }
    }

    /// <summary>
    /// �ǂ��c���Ă��邩�ǂ���
    /// </summary>
    /// <returns>True�F�s���v���C���[�̎g�p�\�ȕǂ��c���Ă���ꍇ False�F�c���Ă��Ȃ��ꍇ</returns>
    public bool IsRemainWall()
    {
        int remainWall = player == COLOR.GREEN ? remainWallGreen : remainWallOrange;
        return remainWall > 0;
    }

    public (int, int) GetParam()
    {
        return (WIDTH, HEIGHT);
    }

    /// <summary>
    /// �S�[���܂ł̌o�H�����邩�ǂ���
    /// </summary>
    /// <returns>True�F���v���C���[���S�[���ɓ��B�\�ȏꍇ False�F�S�[���ɓ��B�s�\�ȃv���C���[������ꍇ</returns>
    public bool IsWay2Goal()
    {
        passedPosition = new bool[HEIGHT, WIDTH];
        if (!IsWay2Next(greenPosition[0], greenPosition[1], COLOR.GREEN))
        {
            return false;
        }

        passedPosition = new bool[HEIGHT, WIDTH];
        if (!IsWay2Next(orangePosition[0], orangePosition[1], COLOR.ORANGE))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ��}�X�ׂ̃}�X�ɒ��ڍs���邩�ǂ����i�ċA�j
    /// </summary>
    /// <param name="pH">�v���C���[�̈ʒu�i�c�j</param>
    /// <param name="pW">�v���C���[�̈ʒu�i���j</param>
    /// <param name="player">�s���v���C���[</param>
    /// <returns>True�F�l���ׂ̗̂ǂꂩ�̃}�X����S�[���ւ̌o�H������ꍇ False�F�S�[���ւ̌o�H���Ȃ��ꍇ</returns>
    bool IsWay2Next(int pH, int pW, COLOR player)
    {
        passedPosition[pH, pW] = true;
        if (pH == 0)
        {
            // ��̃S�[���܂œ��B�\
            if (player == COLOR.ORANGE) 
            {
                return true;
            }
        }
        else if(pH == HEIGHT-1)
        {
            // �΂̃S�[���܂œ��B�\
            if (player == COLOR.GREEN) 
            {
                return true;
            }
        }

        // �オ�}�X�ォ�A�����B�_���A�ǂ��Ȃ���
        if (pH != 0 && !passedPosition[pH - 1, pW] && IsUp(pH, pW)) 
        {
            if (IsWay2Next(pH - 1, pW, player))
            {
                return true;
            }
        }

        // �����}�X�ォ�A�����B�_���A�ǂ��Ȃ���
        if (pH != HEIGHT-1 && !passedPosition[pH + 1, pW] && IsDown(pH, pW)) 
        {
            if (IsWay2Next(pH + 1, pW, player))
            {
                return true;
            }
        }

        // �����}�X�ォ�A�����B�_���A�ǂ��Ȃ���
        if (pW != 0 && !passedPosition[pH, pW - 1] && IsLeft(pH, pW)) 
        {
            if (IsWay2Next(pH, pW - 1, player))
            {
                return true;
            }
        }

        // �E���}�X�ォ�A�����B�_���A�ǂ��Ȃ���
        if (pW != WIDTH-1 && !passedPosition[pH, pW + 1] && IsRight(pH, pW)) 
        {
            if (IsWay2Next(pH, pW + 1, player))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// ��̃}�X�ɒ��ڍs���邩�ǂ���
    /// </summary>
    /// <param name="pH">�v���C���[�̈ʒu�i�c�j</param>
    /// <param name="pW">�v���C���[�̈ʒu�i���j</param>
    /// <returns>True�F��̃}�X�ɒ��ڍs����ꍇ False�F��̃}�X�ɒ��ڍs���Ȃ��ꍇ</returns>
    private bool IsUp(int pH, int pW)
    {
        if(pH == 0 || ((pW == 0 || !dhws.isWallsAtPI[pH - 1, pW - 1]) && (pW == WIDTH-1 || !dhws.isWallsAtPI[pH - 1, pW])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ���̃}�X�ɒ��ڍs���邩�ǂ���
    /// </summary>
    /// <param name="pH">�v���C���[�̈ʒu�i�c�j</param>
    /// <param name="pW">�v���C���[�̈ʒu�i���j</param>
    /// <returns>True�F���̃}�X�ɒ��ڍs����ꍇ False�F���̃}�X�ɒ��ڍs���Ȃ��ꍇ</returns>
    private bool IsDown(int pH, int pW)
    {
        if (pH == HEIGHT-1 || ((pW == 0 || !dhws.isWallsAtPI[pH, pW - 1]) && (pW == WIDTH-1 || !dhws.isWallsAtPI[pH, pW])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ���̃}�X�ɒ��ڍs���邩�ǂ���
    /// </summary>
    /// <param name="pH">�v���C���[�̈ʒu�i�c�j</param>
    /// <param name="pW">�v���C���[�̈ʒu�i���j</param>
    /// <returns>True�F���̃}�X�ɒ��ڍs����ꍇ False�F���̃}�X�ɒ��ڍs���Ȃ��ꍇ</returns>
    private bool IsLeft(int pH, int pW)
    {
        if (pW == 0 || ((pH == 0 || !dvws.isWallsAtPI[pH - 1, pW - 1]) && (pH == HEIGHT-1 || !dvws.isWallsAtPI[pH, pW - 1])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �E�̃}�X�ɒ��ڍs���邩�ǂ���
    /// </summary>
    /// <param name="pH">�v���C���[�̈ʒu�i�c�j</param>
    /// <param name="pW">�v���C���[�̈ʒu�i���j</param>
    /// <returns>True�F�E�̃}�X�ɒ��ڍs����ꍇ False�F�E�̃}�X�ɒ��ڍs���Ȃ��ꍇ</returns>
    private bool IsRight(int pH, int pW)
    {
        if (pW == WIDTH-1 || ((pH == 0 || !dvws.isWallsAtPI[pH - 1, pW]) && (pH == HEIGHT-1 || !dvws.isWallsAtPI[pH, pW])))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �^�C�g���V�[���ɐ؂�ւ�
    /// </summary>
    public void ChangeScene()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // �ȉ��S�č��͎g���Ă��Ȃ�

    // not used
    void SetWall()  
    {
        for (int i = 0; i < 8; i++)
        {
            GetWall(i, 300);
            GetWall(i, -300);
        }
    }

    // not used
    void GetWall(int i, int h)
    {
        GameObject wallobj = null;
        wallobj = Instantiate(wallObject, new Vector3(175 - i * 50, h, 10), Quaternion.identity);
        // �擾����Prefab��wallDisplay�̎q�I�u�W�F�N�g�ɂ���
        wallobj.transform.SetParent(wallDisplay.transform, false);
    }

    // not used
    void BuildWall()
    {
        Debug.Log("wall");
        isBuildingWall = true;

        UpdateAll();
    }

    // not used
    public (int[], int[]) GetPlayerPosition()
    {
        return (greenPosition, orangePosition);
    }

    // not used
    public bool IsGreenTurn()
    {
        return player == COLOR.GREEN;
    }

}
