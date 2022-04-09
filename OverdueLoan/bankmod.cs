using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Banking;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.AudioSynthesis.Util;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

namespace bankmodMod
{
    public class bankmod : MonoBehaviour
    {
        private static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            var go = new GameObject(mod.Title);
            go.AddComponent<bankmod>();

        }

        void Start()
        {
            DaggerfallBankManager.OnBorrowLoan += DaggerfallBankManager_OnBorrowLoan;
            PlayerEnterExit.OnTransitionExterior += PlayerEnterExit_OnTransitionExterior;
            PlayerEnterExit.OnTransitionDungeonExterior += PlayerEnterExit_OnTransitionDungeonExterior;
            mod.IsReady = true;
        }

        void PlayerEnterExit_OnTransitionDungeonExterior(PlayerEnterExit.TransitionEventArgs args)
        {

        }

        void PlayerEnterExit_OnTransitionExterior(PlayerEnterExit.TransitionEventArgs args)
        {
            int region = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
            if (true || DaggerfallBankManager.HasDefaulted(region))
            {
                if (DoesAssassinsAppear())
                {
                    Debug.Log("Spawnin stuff yo");
                    var amt = DaggerfallBankManager.GetLoanedTotal(region);

                    if (GameManager.Instance.PlayerEntity.GoldPieces >= amt)
                    {
                        PayOptionPrompt(amt);
                    }
                    else
                    {
                        TheyGonnaDiePrompt();
                    }
                }
            }
            else
            {
                Debug.Log("Not defaulted. Yet...");
            }
        }

        private bool DoesAssassinsAppear()
        {
            //todo: Formula so that it is not 100% of the time.
            //Likelihood should increase the longer the loan is past due
            return true;
        }

        private void Prompt_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.No)
            {
                TheyGonnaDiePrompt();
            }
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Yes)
            {
                int region = GameManager.Instance.PlayerGPS.CurrentRegionIndex;
                var amt = DaggerfallBankManager.GetLoanedTotal(region);

                DaggerfallBankManager.MakeTransaction(TransactionType.Repaying_loan, (int)amt, region);

                DaggerfallConnect.Arena2.TextFile.Token[] tokens = new DaggerfallConnect.Arena2.TextFile.Token[1]
                {
                    new DaggerfallConnect.Arena2.TextFile.Token
                    {
                        text = "Aight we good then"
                    }
                };
                DaggerfallMessageBox messageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallMessageBox.CommonMessageBoxButtons.Nothing, tokens);
                messageBox.ClickAnywhereToClose = true;
                messageBox.AllowCancel = false;
                messageBox.ParentPanel.BackgroundColor = Color.clear;
                messageBox.Show();
            }
        }

        private void TheyGonnaDiePrompt()
        {
            DaggerfallMessageBox prompt;

            DaggerfallConnect.Arena2.TextFile.Token[] tokens = new DaggerfallConnect.Arena2.TextFile.Token[1]
            {
                    new DaggerfallConnect.Arena2.TextFile.Token
                    {
                        text = "BIG MISTAKE FOOOOOL"
                    }
            };
            prompt = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallMessageBox.CommonMessageBoxButtons.Nothing, tokens);
            prompt.ClickAnywhereToClose = true;
            prompt.AllowCancel = false;
            prompt.ParentPanel.BackgroundColor = Color.clear;
            prompt.Show();

            //UserInterfaceWindow.OnCloseHandler CreateFoes = () => GameObjectHelper.CreateFoeSpawner(true, DaggerfallWorkshop.MobileTypes.Assassin, 3);
            prompt.OnClose += CreateFoes;
        }

        void CreateFoes()
        {
            //todo: the longer the loan is past due the bigger the enemies.
            GameObjectHelper.CreateFoeSpawner(true, DaggerfallWorkshop.MobileTypes.Assassin, 3);
        }

        void DaggerfallBankManager_OnBorrowLoan(TransactionType type, TransactionResult result, int amount)
        {

        }

        public DaggerfallMessageBox PayOptionPrompt(long amt)
        {
            DaggerfallMessageBox prompt;

            DaggerfallConnect.Arena2.TextFile.Token[] tokens = new DaggerfallConnect.Arena2.TextFile.Token[3]
            {
                new DaggerfallConnect.Arena2.TextFile.Token
                {
                    text = "HONKEY~~~",
                    y = 5
                },
                new DaggerfallConnect.Arena2.TextFile.Token
                {
                    text = "YOU GONNA GIVE ME MY MONEY????",
                    y = 15
                },
                new DaggerfallConnect.Arena2.TextFile.Token
                {
                    text = $"{amt}!!!!",
                    y = 25
                }
            };
            prompt = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallMessageBox.CommonMessageBoxButtons.YesNo, tokens);
            prompt.ClickAnywhereToClose = false;
            prompt.AllowCancel = false;
            prompt.ParentPanel.BackgroundColor = Color.clear;

            prompt.OnButtonClick += Prompt_OnButtonClick;

            prompt.Show();

            return prompt;
        }
    }
}
