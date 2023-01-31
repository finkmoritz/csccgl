﻿using Newtonsoft.Json;

namespace csbcgf
{
    public class DrawCardAction : Action
    {
        [JsonProperty]
        protected IPlayer player = null!;

        [JsonProperty]
        protected ICard? drawnCard;

        protected DrawCardAction() {}

        public DrawCardAction(IPlayer player, bool isAborted = false)
            : base(isAborted)
        {
            this.player = player;
        }

        [JsonIgnore]
        public IPlayer Player {
            get => player;
        }

        [JsonIgnore]
        public ICard? DrawnCard {
            get => drawnCard;
        }

        public override void Execute(IGame game)
        {
            drawnCard = player.Deck.Last;
            game.Execute(new RemoveCardFromCardCollectionAction(player.Deck, drawnCard));
            game.Execute(new AddCardToCardCollectionAction(player.Hand, drawnCard));
        }

        public override bool IsExecutable(IGameState gameState)
        {
            return !player.Deck.IsEmpty;
        }
    }
}
