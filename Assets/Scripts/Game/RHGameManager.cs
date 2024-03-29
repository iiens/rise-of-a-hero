﻿using Game.Events;
using Game.Quests;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Game
{
    /// <summary>
    /// Extends the default GameManger.
    /// </summary>
    public class RHGameManager : GameManager, MMEventListener<EnemyDeathEvent>
    {
        public new static RHGameManager Instance => (RHGameManager)GameManager.Instance;
        public new static RHGameManager Current => (RHGameManager)GameManager.Current;

        // temporary until we are keeping track of the game progress
        private bool _hasDoneMainQuest;
        public bool HasDoneMainQuest() => _hasDoneMainQuest;

        // current quest
        public Quest CurrentQuest { get; private set; }
        public bool HasQuest() => CurrentQuest != null;
        
        [Header("Ads")]
        public string gameId;
        public bool testMode;
        
        protected override void Awake()
        {
            base.Awake();
            Advertisement.Initialize(gameId, testMode);
            Advertisement.Load("Rewarded_Android");
        }

        //  we can only have one quest at a time
        public void SetQuest(Quest quest)
        {
            CurrentQuest = quest;
            quest.StartQuest();

            // fire event
            QuestEvent.Trigger(CurrentQuest, QuestMethods.Started);
        }

        public void OnEnemyKilled()
        {
            // no quest
            if (CurrentQuest == null) return;
            // fire event
            CurrentQuest.OnEnemyKilled();
            
            QuestEvent.Trigger(CurrentQuest, QuestMethods.Updated);

            // the quest is done
            if (!CurrentQuest.QuestCompleted()) return;            
            // Yeah, we only have one quest in the Main Quest
            _hasDoneMainQuest = true;
            // Give reward if we got one
            CurrentQuest.Data.Reward()?.Give(RHLevelManager.GetPlayer);

            // clear
            CurrentQuest = null;

            // fire event with null, completed :D
            QuestEvent.Trigger(CurrentQuest, QuestMethods.Completed);
        }

        // Events

        public virtual void OnMMEvent(EnemyDeathEvent pointEvent)
        {
            OnEnemyKilled();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<EnemyDeathEvent> ();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.MMEventStopListening<EnemyDeathEvent> ();
        }

        public void ResetQuest()
        {
            // no
            _hasDoneMainQuest = false;
            CurrentQuest = null;
        }
    }
}