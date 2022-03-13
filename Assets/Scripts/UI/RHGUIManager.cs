﻿using Game;
using MoreMountains.CorgiEngine;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Extends the default GUIManger.
    /// </summary>
    public class RHGUIManager : GUIManager
    {
        [Header("Quests")]
        [Tooltip("Label to put the display name of the quest")]
        public Text currentQuestLabel;

        protected override void Awake()
        {
            base.Awake();
            RefreshQuest();
        }

        public void RefreshQuest()
        {
            if (currentQuestLabel == null) return;
            var quest = ((RHGameManager)GameManager.Current).CurrentQuest;
            currentQuestLabel.text = quest != null ? quest.Data.GetDisplayName() : "";
        }
    }
}