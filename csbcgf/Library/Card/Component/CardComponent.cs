﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace csbcgf
{
    [Serializable]
    public class CardComponent : Reaction, ICardComponent
    {
        [JsonProperty]
        protected ManaCostStat manaCostStat;

        public List<IReaction> Reactions { get; }

        public ICard ParentCard { get; set; }

        public CardComponent(int mana)
            : this(new ManaCostStat(mana, mana), new List<IReaction>(), null)
        {
        }

        public CardComponent(int manaValue, int manaBaseValue)
            : this(new ManaCostStat(manaValue, manaBaseValue), new List<IReaction>(), null)
        {
        }

        [JsonConstructor]
        protected CardComponent(ManaCostStat manaCostStat, List<IReaction> reactions, ICard parentCard)
        {
            this.manaCostStat = manaCostStat;
            Reactions = reactions;
            ParentCard = parentCard;
        }

        [JsonIgnore]
        public int ManaValue {
            get => manaCostStat.Value;
            set => manaCostStat.Value = value;
        }

        [JsonIgnore]
        public int ManaBaseValue {
            get => manaCostStat.BaseValue;
            set => manaCostStat.BaseValue = value;
        }

        public void AddReaction(IReaction reaction)
        {
            Reactions.Add(reaction);
        }

        public override void ReactTo(IGame game, IActionEvent actionEvent)
        {
            Reactions.ForEach(r => r.ReactTo(game, actionEvent));
        }

        public void RemoveReaction(IReaction reaction)
        {
            Reactions.Remove(reaction);
        }

        public override object Clone()
        {
            List<IReaction> reactionsClone = new List<IReaction>();
            foreach (IReaction reaction in Reactions)
            {
                reactionsClone.Add((IReaction)reaction.Clone());
            }

            return new CardComponent(
                (ManaCostStat)manaCostStat.Clone(),
                reactionsClone,
                null // otherwise circular dependencies
            );
        }

        public ICard FindCard(IGameState gameState)
        {
            foreach (ICard card in gameState.AllCards)
            {
                foreach (ICardComponent cardComponent in card.Components)
                {
                    if (cardComponent == this)
                    {
                        return card;
                    }
                }
            }
            return null;
        }
    }
}
