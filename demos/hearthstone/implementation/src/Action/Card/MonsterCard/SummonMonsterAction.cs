﻿using csbcgf;
using Newtonsoft.Json;

namespace hearthstone
{
    public class SummonMonsterAction : csbcgf.Action
    {
        [JsonProperty]
        protected IPlayer player = null!;

        [JsonProperty]
        protected IMonsterCard monsterCard = null!;

        protected SummonMonsterAction() { }

        public SummonMonsterAction(IPlayer player, IMonsterCard monsterCard, bool isAborted = false
            ) : base(isAborted)
        {
            this.player = player;
            this.monsterCard = monsterCard;
        }

        [JsonIgnore]
        public IPlayer Player
        {
            get => player;
        }

        [JsonIgnore]
        public IMonsterCard MonsterCard
        {
            get => monsterCard;
        }

        public override void Execute(IGame game)
        {
            game.ExecuteSequentially(new List<IAction> {
                new ModifyManaStatAction(Player, -MonsterCard.ManaValue, 0),
                new RemoveCardFromCardCollectionAction(Player.GetCardCollection(CardCollectionKeys.Hand), MonsterCard),
                new AddCardToCardCollectionAction(Player.GetCardCollection(CardCollectionKeys.Board), MonsterCard)
            });
        }

        public override bool IsExecutable(IGameState gameState)
        {
            return MonsterCard.IsSummonable(gameState)
                && !Player.GetCardCollection(CardCollectionKeys.Board).IsFull;
        }
    }
}
