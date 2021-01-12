﻿using System;
using Newtonsoft.Json;

namespace csbcgf
{
    [Serializable]
    public class ActionQueue : IActionQueue
    {
        [JsonProperty]
        protected bool isGameOver = false;

        public bool ExecuteReactions { get; set; }

        public ActionQueue(bool executeReactions = true)
            : this(executeReactions, false)
        {
        }

        [JsonConstructor]
        protected ActionQueue(bool executeReactions, bool isGameOver)
        {
            ExecuteReactions = executeReactions;
            this.isGameOver = isGameOver;
        }

        public void Execute(IGame game, IAction action)
        {
            if (!isGameOver && !action.IsAborted && action.IsExecutable(game))
            {
                ExecReactions(game, new BeforeActionEvent(action));
                if (!action.IsAborted)
                {
                    action.Execute(game);
                    ExecReactions(game, new AfterActionEvent(action));
                }
            }

            if (action is EndOfGameEvent)
            {
                isGameOver = true;
            }
        }

        private void ExecReactions(IGame game, IActionEvent actionEvent)
        {
            if (ExecuteReactions)
            {
                game.AllCards.ForEach(c => c.ReactTo(game, actionEvent));
                game.Players.ForEach(p => p.ReactTo(game, actionEvent));
                game.ReactTo(game, actionEvent);
            }
        }
    }
}
