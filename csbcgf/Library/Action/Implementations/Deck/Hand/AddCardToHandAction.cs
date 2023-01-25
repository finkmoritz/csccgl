﻿using Newtonsoft.Json;

namespace csbcgf
{
    public class AddCardToHandAction : Action
    {
        [JsonProperty]
        protected IHand hand = null!;

        [JsonProperty]
        protected ICard card = null!;

        protected AddCardToHandAction() {}

        public AddCardToHandAction(IHand hand, ICard card, bool isAborted = false)
            : base(isAborted)
        {
            this.hand = hand;
            this.card = card;
        }

        [JsonIgnore]
        public IHand Hand {
            get => hand;
        }

        [JsonIgnore]
        public ICard Card {
            get => card;
        }

        public override void Execute(IGame game)
        {
            Hand.Add(Card);
        }

        public override bool IsExecutable(IGameState gameState)
        {
            return Card != null && Hand.Size < Hand.MaxSize;
        }
    }
}
