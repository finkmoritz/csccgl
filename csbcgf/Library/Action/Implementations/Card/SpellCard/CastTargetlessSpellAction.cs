﻿namespace csbcgf
{
    public class CastTargetlessSpellAction : CastSpellAction
    {
        protected CastTargetlessSpellAction() { }

        public CastTargetlessSpellAction(
            IPlayer player,
            ITargetlessSpellCard spellCard,
            bool isAborted = false
            ) : base(player, spellCard, isAborted)
        {
        }

        public override void Execute(IGame game)
        {
            game.Execute(new ModifyManaStatAction(Player, -SpellCard.ManaValue, 0));
            game.Execute(new RemoveCardFromCardCollectionAction(Player.Hand, SpellCard));
            ((ITargetlessSpellCard)SpellCard).Cast(game);
            game.Execute(new AddCardToCardCollectionAction(Player.Graveyard, SpellCard));
        }

        public override bool IsExecutable(IGameState gameState)
        {
            return SpellCard.IsCastable(gameState);
        }
    }
}
