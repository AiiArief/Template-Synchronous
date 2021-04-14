using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StoredAction
{
    public Action action;
    public bool actionHasDone = false;
}

public class SkipTurn : StoredAction
{
    public SkipTurn()
    {
        action = () => { actionHasDone = true; };
    }
}

public class TurnEntity : StoredAction
{
    public TurnEntity(Transform entityTransform, Camera m_entityCamera, Vector3 m_currentCameraRot)
    {
        action = () =>
        {
            Vector3 faceCameraDir = new Vector3(0.0f, _ConvertTo90Degrees(m_currentCameraRot.y), 0.0f);
            entityTransform.rotation = Quaternion.Euler(faceCameraDir);
            m_entityCamera.transform.parent.rotation = Quaternion.Euler(m_currentCameraRot);
            actionHasDone = true;
        };
    }

    private float _ConvertTo90Degrees(float value, bool isRepeat = true)
    {
        if (isRepeat)
        {
            if (value >= 315.0f && value < 45.0f) return 0.0f;
            if (value >= 45.0f && value < 135.0f) return 90.0f;
            if (value >= 135.0f && value < 225.0f) return 180.0f;
            if (value >= 225.0f && value < 315.0f) return 270.0f;
        }
        else
        {
            if (value >= -45.0f && value < 45.0f) return 0.0f;
            if (value >= 45.0f && value < 135.0f) return 90.0f;
            if (value <= -45.0f && value > -135.0f) return -90.0f;
            if (value <= -135.0f || value > 135.0f) return 180.0f;
        }
        return 0.0f;
    }
}

public class MoveEntity : StoredAction
{
    private Vector3 m_posBeforeMove;
    private float m_moveSpeed = 1.0f;

    public MoveEntity(CharacterController m_characterController, Vector3 direction, float range = 1)
    {
        m_posBeforeMove = m_characterController.transform.position;
        m_moveSpeed = 1.0f / PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase;

        action = () =>
        {
            Transform transform = m_characterController.transform;
            Vector3 localTarget = transform.right * direction.x * range + transform.forward * direction.z * range;
            Vector3 target = new Vector3(m_posBeforeMove.x + localTarget.x, transform.position.y, m_posBeforeMove.z + localTarget.z);
            if (Vector3.Distance(transform.position, target) > 0.1f && !_CheckProcessInputHasOverMinimumTime())
            {
                m_characterController.Move(localTarget * m_moveSpeed * Time.deltaTime); 
            }
            else
            {
                transform.position = target;
                actionHasDone = true;
            }
        };
    }

    public MoveEntity(CharacterController m_characterController, float gravitySpeed)
    {
        m_posBeforeMove = m_characterController.transform.position;

        action = () =>
        {
            //m_characterController.Move(Vector3.down * gravitySpeed * Time.deltaTime);
            //actionHasDone = _CheckProcessInputHasOverMinimumTime();

            Transform transform = m_characterController.transform;
            Vector3 localTarget = Vector3.down * -(gravitySpeed / PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase);
            Vector3 target = new Vector3(transform.position.x, m_posBeforeMove.y + localTarget.y, transform.position.z);
            if (Vector3.Distance(transform.position, target) > 0.1f && !_CheckProcessInputHasOverMinimumTime())
            {
                m_characterController.Move(localTarget * Time.deltaTime);
            }
            else
            {
                transform.position = target;
                actionHasDone = true;
            }
        };
    }

    private bool _CheckProcessInputHasOverMinimumTime()
    {
        return PhaseManager.Instance.processInput.currentTimeBeforeNextPhase >= PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase;
    }
}

public class Player : MonoBehaviour
{
    public List<StoredAction> storedActions { get; private set; } = new List<StoredAction>();
    public bool afterActionHasDone { get; private set; } = false; // temp

    public bool isPlayable { get { return gameObject.activeSelf; } set { gameObject.SetActive(isPlayable); } }
    public int playerId { get { return transform.GetSiblingIndex(); } }

    private CharacterController m_characterController;

    [SerializeField] Camera m_playerCamera;
    private Vector3 m_currentCameraRot;

    [SerializeField] Transform m_crosshair_shoot;

    public void SetupWaitInput()
    {
        storedActions.Clear();
    }

    public void WaitInput()
    {
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        float camH = Input.GetAxis("Mouse X");
        float camV = Input.GetAxis("Mouse Y");
        bool moveMod = Input.GetButton("Move Modifier");
        bool camMod = Input.GetButton("Camera Modifier 1") || Input.GetButton("Camera Modifier 2");

        Vector2 camRot = (camMod) ? new Vector2(moveH, moveV) : new Vector2(camH, camV);
        _HandleCameraWaitInput(camRot.x, camRot.y, moveMod);

        bool isMoving = Mathf.Abs(moveH) > 0.0f ^ Mathf.Abs(moveV) > 0.0f;
        if(isMoving && !camMod)
        {
            float moveRange = 1;
            if(moveMod)
            {
                if(moveV < 0.0f)
                {
                    storedActions.Add(new SkipTurn());
                    return;
                }

                moveRange = 2;
            }

            storedActions.Add(new TurnEntity(transform, m_playerCamera, m_currentCameraRot));
            storedActions.Add(new MoveEntity(m_characterController, new Vector3(moveH, 0.0f, moveV), moveRange));

        }
    }

    public void SetupProcessInput()
    {
        foreach (StoredAction storedAction in storedActions)
            storedAction.actionHasDone = false;
         
        m_crosshair_shoot.gameObject.SetActive(false);

        //storedActions.Add(new MoveEntity(m_characterController, 12.0f));
    }

    public bool CheckAllActionHasDone()
    {
        foreach(StoredAction storedAction in storedActions)
        {
            if (!storedAction.actionHasDone) return false;
        }

        return true;
    }

    public void SetupAfterInput()
    {
        afterActionHasDone = false;
    }

    public void AfterInput()
    {
        Debug.Log("done move");
        afterActionHasDone = true;
    }

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void _HandleCameraWaitInput(float camH, float camV, bool moveMod)
    {
        if(moveMod)
        {
            // peek here
        } else
        {
            m_currentCameraRot.y = Mathf.Repeat(m_currentCameraRot.y + camH, 360);
            m_currentCameraRot.x = Mathf.Clamp(m_currentCameraRot.x - camV, -60.0f, 90.0f);

            m_playerCamera.transform.parent.rotation = Quaternion.Euler(m_currentCameraRot.x, m_currentCameraRot.y, 0.0f);
        }

        //RaycastHit hit;
        //m_crosshair_shoot.gameObject.SetActive(Physics.Raycast(m_playerCamera.transform.position, m_playerCamera.transform.forward, out hit));
        //m_crosshair_shoot.position = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
    }
}
