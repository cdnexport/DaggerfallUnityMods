using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using System.Linq;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using System;

namespace mod1Mod
{
    public class QuestLogger : MonoBehaviour
    {
        static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<QuestLogger>();
        }

        void Start()
        {
            QuestMachine.OnQuestStarted += QuestMachine_OnQuestStarted;
            QuestMachine.OnQuestEnded += QuestMachine_OnQuestEnded;

            mod.IsReady = true;
        }

        static void QuestMachine_OnQuestEnded(Quest quest)
        {
            List<TextFile.Token[]> notes = GameManager.Instance.PlayerEntity.Notebook.GetNotes();

            int index = -1;
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                //0 is the date and town
                //1 is null
                //2 is the message
                var message = notes[i][2].text;

                if (message != null && message.Trim().Contains($"Accepted quest {quest.DisplayName}"))
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                Debug.LogWarning($"QuestLogger: Could not find log entry for {quest.DisplayName}.");
            }
            else
            {
                GameManager.Instance.PlayerEntity.Notebook.RemoveNote(index);
            }
        }

        static void QuestMachine_OnQuestStarted(Quest quest)
        {
            try
            {
                //Resources are stored in the Quest Machine as Key: Resource but the GetAllResources method does not include the key.
                //eg. "qgiver_home" : Place
                //Since we don't have the key we've hard coded the indexes that it appears correspond to the Quest Giver building and their name.
                var qResources = quest.GetAllResources();

                var buildingName = (qResources[1] as Place).SiteDetails.buildingName;
                var qGiver = (qResources[2] as Person).DisplayName;

                GameManager.Instance.PlayerEntity.Notebook.AddNote($"Accepted quest {quest.DisplayName} from {qGiver} at {buildingName}.");
            }
            catch (Exception ex)
            {
                Debug.Log($"QuestLogger: Could not log for {quest.DisplayName}. Exception: {ex.Message}");
            }
        }
    }
}
