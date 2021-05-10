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

    [SerializeField] float m_maxDistanceView = 10.0f;

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

            IsInConeArea isInConeArea = _GenerateIsInCone(player.transform.position, player.collisionChecker.upCollider.transform.position);
            if (!m_isInConeList.ContainsKey(player))
                m_isInConeList.Add(player, isInConeArea);
            else
                m_isInConeList[player] = isInConeArea;
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
        bool isInAround =
            !(Vector3.Distance(m_entity.collisionChecker.backCollider2.transform.position, targetPosBottom) <= 1.0f) &&
            (Vector3.Distance(m_entity.collisionChecker.leftCollider2.transform.position, targetPosBottom) <= 1.25f ||
            Vector3.Distance(m_entity.collisionChecker.rightCollider2.transform.position, targetPosBottom) <= 1.25f ||
            Vector3.Distance(m_entity.collisionChecker.upCollider.transform.position, targetPosBottom) <= 0.5f ) ;
        if (isInAround)
            return IsInConeArea.SuspiciousArea;

        Vector3 targetDir = targetPosBottom - m_entity.transform.position;
        bool isInCone = Vector3.Angle(targetDir, transform.forward) < 45.0f && targetDir.magnitude < m_maxDistanceView;
        if (isInCone)
        {
            bool isFootVisible = !Physics.Linecast(transform.position, targetPosBottom, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            bool isHeadVisible = !Physics.Linecast(transform.position, targetPosTop, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            bool isClose = targetDir.magnitude < m_maxDistanceView / 2;
            return (isFootVisible || isHeadVisible) ? ((isClose) ? IsInConeArea.AlertArea : IsInConeArea.SuspiciousArea) : IsInConeArea.OutOfArea;
        }

        return IsInConeArea.OutOfArea;
    }
}
