﻿using Newtonsoft.Json;

namespace csbcgf
{
    public class DieAction : Action
    {
        [JsonProperty]
        protected IMonsterCard monsterCard = null!;

        protected DieAction() {}

        public DieAction(IMonsterCard monsterCard, bool isAborted = false)
            : base(isAborted)
        {
            this.monsterCard = monsterCard;
        }

        [JsonIgnore]
        public IMonsterCard MonsterCard {
            get => monsterCard;
        }

        public override void Execute(IGame game)
        {
            IPlayer owner = MonsterCard.FindParentPlayer(game)!;
            game.Execute(new RemoveCardFromBoardAction(owner.Board, MonsterCard));
            game.Execute(new AddCardToGraveyardAction(owner.Graveyard, MonsterCard));
        }

        public override bool IsExecutable(IGameState gameState)
        {
            IPlayer? owner = MonsterCard.FindParentPlayer(gameState);
            return owner != null
                && owner.Board.Contains(MonsterCard);
        }
    }
}
