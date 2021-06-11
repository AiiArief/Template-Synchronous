using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IsInConeArea
{
    OutOfArea,
    SuspiciousArea,
    AlertArea
}

public class ConeOfVision : MonoBehaviour
{
    private Entity m_entity;

    private Dictionary<Entity, IsInConeArea> m_isInConeList = new Dictionary<Entity, IsInConeArea>();
    public Dictionary<Entity, IsInConeArea> isInConeList { get { return m_isInConeList; } }

    [SerializeField] bool m_haveAroundSense = false;
    [SerializeField] float m_coneAngle = 45.0f;
    [SerializeField] float m_alertDistance = 5.0f;
    [SerializeField] float m_warningDistance = 5.0f;
    
    [SerializeField] int m_susAlertLevelInc = 4;
    public int susAlertLevelInc { get { return m_susAlertLevelInc; } }

    public Entity alertTargetEntity { get; private set; }
    public Vector3 lastTargetPos { get; private set; }

    public void SetupConeOfVision(Entity entity)
    {
        m_entity = entity;
    }

    public void UpdateIsInCone()
    {
        foreach (EntityPlayer player in PlayerManager.Instance.players)
        {
            if (!player.isPlayable)
            {
                m_isInConeList.Remove(player);
                continue;
            }

            IsInConeArea isInConeArea = _GenerateIsInCone(player.transform.position, player.collisionEntityChecker.upCollider.transform.position);
            if (!m_isInConeList.ContainsKey(player))
                m_isInConeList.Add(player, isInConeArea);
            else
                m_isInConeList[player] = isInConeArea;
        }
    }

    public void UpdateAlertTargetEntity()
    {
        alertTargetEntity = null;
        foreach (KeyValuePair<Entity, IsInConeArea> entry in isInConeList)
        {
            if (entry.Value == IsInConeArea.SuspiciousArea || entry.Value == IsInConeArea.AlertArea)
            {
                if (alertTargetEntity == null)
                    alertTargetEntity = entry.Key;
                else
                    alertTargetEntity = (Vector3.Distance(entry.Key.transform.position, transform.position) < Vector3.Distance(alertTargetEntity.transform.position, transform.position)) ? entry.Key : alertTargetEntity;

                lastTargetPos = alertTargetEntity.transform.position;
            }
        }
    }

    public bool CheckAllIsInConeAreTheSame(IsInConeArea checkIsInConeArea)
    {
        if (m_isInConeList.Count == 0)
            return true;

        foreach (KeyValuePair<Entity, IsInConeArea> entry in m_isInConeList)
        {
            if (entry.Value != checkIsInConeArea)
                return false;
        }

        return true;
    }

    private IsInConeArea _GenerateIsInCone(Vector3 targetPosBottom, Vector3 targetPosTop)
    {
        // kasih nabrak & suara
        bool isInAround = m_haveAroundSense &&
            !(Vector3.Distance(m_entity.collisionEntityChecker.backCollider2.transform.position, targetPosBottom) <= 1.0f) &&
            (Vector3.Distance(m_entity.collisionEntityChecker.leftCollider2.transform.position, targetPosBottom) <= 1.25f ||
            Vector3.Distance(m_entity.collisionEntityChecker.rightCollider2.transform.position, targetPosBottom) <= 1.25f ||
            Vector3.Distance(m_entity.collisionEntityChecker.upCollider.transform.position, targetPosBottom) <= 0.5f ) ;
        if (isInAround)
            return IsInConeArea.SuspiciousArea;

        Vector3 targetDir = targetPosBottom - m_entity.transform.position;
        bool isInCone = Vector3.Angle(targetDir, transform.forward) < m_coneAngle && targetDir.magnitude < m_alertDistance + m_warningDistance;
        if (isInCone)
        {
            bool isFootVisible = !Physics.Linecast(transform.position, targetPosBottom, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            bool isHeadVisible = !Physics.Linecast(transform.position, targetPosTop, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            bool isClose = targetDir.magnitude < m_alertDistance;
            return (isFootVisible || isHeadVisible) ? ((isClose) ? IsInConeArea.AlertArea : IsInConeArea.SuspiciousArea) : IsInConeArea.OutOfArea;
        }

        return IsInConeArea.OutOfArea;
    }
}
