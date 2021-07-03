using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredActionCameraLook : StoredAction
{
    public StoredActionCameraLook(EntityPlayer player, CameraLook cameraLook)
    {
        action = () =>
        {
            float moveH = Input.GetAxisRaw("Horizontal" + " #" + player.playerId);
            float moveV = Input.GetAxisRaw("Vertical" + " #" + player.playerId);
            float camH = Input.GetAxis("Camera X" + " #" + player.playerId);
            float camV = Input.GetAxis("Camera Y" + " #" + player.playerId);
            bool moveMod = Input.GetButton("Move Modifier" + " #" + player.playerId);
            bool camMod = Input.GetButton("Camera Modifier 1" + " #" + player.playerId) || Input.GetButton("Camera Modifier 2" + " #" + player.playerId);

            Vector2 camRot = (camMod) ? new Vector2(moveH, moveV) : new Vector2(camH, camV);
            cameraLook.HandleCameraWaitInput(camRot.x, camRot.y);

            actionHasDone = GameManager.Instance.phaseManager.processInput.currentTimeBeforeNextPhase >= GameManager.Instance.phaseManager.processInput.minimumTimeBeforeNextPhase;
        };
    }
}
