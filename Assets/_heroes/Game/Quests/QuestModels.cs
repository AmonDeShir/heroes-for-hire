using System.Collections.Generic;
using UnityEngine;

namespace Heroes.Game.Quests
{
    public enum QuestType
    {
        Combat = 0,
        Explorer = 1,
    }

    public enum QuestTargetKind
    {
        Building = 0,
        Monster = 1,
    }

    public enum QuestState
    {
        Active = 0,
        Completed = 1,
    }

    public sealed class QuestInstance
    {
        public string QuestId;
        public QuestType Type;
        public QuestTargetKind TargetKind;
        public string TargetInstanceId;
        public int PoolGold;
        public readonly HashSet<string> Participants = new();
        public float CreatedAt;
        public QuestState State;

        public QuestInstance(string questId, QuestType type, QuestTargetKind targetKind, string targetInstanceId, int poolGold, float createdAt)
        {
            QuestId = questId;
            Type = type;
            TargetKind = targetKind;
            TargetInstanceId = targetInstanceId;
            PoolGold = poolGold;
            CreatedAt = createdAt;
            State = QuestState.Active;
        }
    }

    public readonly struct BestQuestSnapshot
    {
        public readonly bool Exists;
        public readonly string QuestId;
        public readonly QuestTargetKind TargetKind;
        public readonly string TargetInstanceId;
        public readonly int PoolGold;
        public readonly int Participants;
        public readonly float TargetHp;
        public readonly Vector3 TargetPosition;
        public readonly bool HeroIsParticipant;

        public BestQuestSnapshot(bool exists, string questId, QuestTargetKind targetKind, string targetInstanceId, int poolGold, int participants, float targetHp, Vector3 targetPosition, bool heroIsParticipant)
        {
            Exists = exists;
            QuestId = questId;
            TargetKind = targetKind;
            TargetInstanceId = targetInstanceId;
            PoolGold = poolGold;
            Participants = participants;
            TargetHp = targetHp;
            TargetPosition = targetPosition;
            HeroIsParticipant = heroIsParticipant;
        }
    }
}
