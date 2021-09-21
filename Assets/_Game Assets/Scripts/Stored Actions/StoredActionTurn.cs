using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredActionTurn : StoredAction
{
    public StoredActionTurn(Entity entity, float yDegree, bool isRepeatAngle = true)
    {
        action = () =>
        {
            Vector3 euler = new Vector3(0.0f, _ConvertTo90Degrees(yDegree, isRepeatAngle), 0.0f);
            entity.transform.rotation = Quaternion.Euler(euler);
            actionHasDone = true;
        };
    }

    public StoredActionTurn(EntityCharacterPlayer player, Vector3 m_currentCameraRot)
    {
        Vector3 faceCameraDir = new Vector3(0.0f, _ConvertTo90Degrees(m_currentCameraRot.y), 0.0f);
        player.transform.rotation = Quaternion.Euler(faceCameraDir);
        player.playerCameraLook.transform.rotation = Quaternion.Euler(m_currentCameraRot);

        action = () =>
        {
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
