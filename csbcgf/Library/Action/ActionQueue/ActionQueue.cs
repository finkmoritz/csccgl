﻿using Newtonsoft.Json;

namespace csbcgf
{
    public class ActionQueue : IActionQueue
    {
        [JsonProperty]
        protected bool isGameOver = false;

        public bool ExecuteReactions { get; set; }

        protected ActionQueue() { }

        public ActionQueue(bool executeReactions = true)
            : this(executeReactions, false)
        {
        }

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
                game.ReactTo(game, actionEvent);
            }
        }
    }
}
