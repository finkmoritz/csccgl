﻿using Newtonsoft.Json;

namespace csbcgf
{
    public abstract class CastSpellAction : Action
    {
        [JsonProperty]
        protected IPlayer player = null!;

        [JsonProperty]
        protected ISpellCard spellCard = null!;

        protected CastSpellAction() { }

        public CastSpellAction(IPlayer player, ISpellCard spellCard, bool isAborted = false)
            : base(isAborted)
        {
            this.player = player;
            this.spellCard = spellCard;
        }

        [JsonIgnore]
        public IPlayer Player
        {
            get => player;
        }

        [JsonIgnore]
        public ISpellCard SpellCard
        {
            get => spellCard;
        }

        public override abstract void Execute(IGame game);

        public override abstract bool IsExecutable(IGameState gameState);
    }
}
