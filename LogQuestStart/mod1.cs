using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using System.Linq;
using DaggerfallConnect.Arena2;
using System.Collections.Generic;

namespace mod1Mod
{
    public class mod1 : MonoBehaviour
    {
        static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<mod1>();
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
                var array = notes[i];
                foreach (var item in array)
                {
                    if (item.text != null && item.text.Trim().Equals($"I've accepted {quest.DisplayName}."))
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    break;
                }
            }

            if (index < 0)
            {
                Debug.LogWarning($"mod1: Could not find log entry for {quest.DisplayName}.");
            }
            else
            {
                GameManager.Instance.PlayerEntity.Notebook.RemoveNote(index);
            }
        }

        static void QuestMachine_OnQuestStarted(Quest quest)
        {
            if (quest.LastResourceReferenced.GetType() == typeof(Person))
            {
                GameManager.Instance.PlayerEntity.Notebook.AddNote($"I've accepted {quest.DisplayName}.");
            }
            else
            {
                Debug.Log($"mod1: Last resource {quest.LastResourceReferenced.GetType()} was not a person, no notebook entry created.");
            }
        }
    }
}
