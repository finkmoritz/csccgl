﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace csbcgf
{
    [Serializable]
    public class Player : IPlayer
    {
        [JsonProperty]
        protected ManaPoolStat manaPoolStat;

        [JsonProperty]
        protected AttackStat attackStat;

        [JsonProperty]
        protected LifeStat lifeStat;

        public IDeck Deck { get; protected set; }
        public IHand Hand { get; protected set; }
        public IBoard Board { get; protected set; }
        public IDeck Graveyard { get; protected set; }

        public List<IReaction> Reactions { get; }

        /// <summary>
        /// Represents a Player and all his/her associated Cards.
        /// </summary>
        /// <param name="deck"></param>
        public Player(IDeck deck)
            : this(deck, new Hand(), new Board(), new Deck(),
                  new ManaPoolStat(0, 0), new AttackStat(0), new LifeStat(0),
                  new List<IReaction>())
        {
        }

        [JsonConstructor]
        protected Player(IDeck deck, IHand hand, IBoard board, IDeck graveyard,
            ManaPoolStat manaPoolStat, AttackStat attackStat, LifeStat lifeStat,
            List<IReaction> reactions)
        {
            this.manaPoolStat = manaPoolStat;
            this.attackStat = attackStat;
            this.lifeStat = lifeStat;

            Deck = deck;
            Hand = hand;
            Board = board;
            Graveyard = graveyard;

            Deck.AllCards.ForEach(c => c.Owner = this);
            Hand.AllCards.ForEach(c => c.Owner = this);
            Board.AllCards.ForEach(c => c.Owner = this);
            Graveyard.AllCards.ForEach(c => c.Owner = this);

            Reactions = reactions;
        }

        [JsonIgnore]
        public bool IsAlive => lifeStat.Value > 0;

        [JsonIgnore]
        public List<ICard> AllCards
        {
            get
            {
                List<ICard> allCards = new List<ICard>();
                allCards.AddRange(Deck.AllCards);
                allCards.AddRange(Hand.AllCards);
                allCards.AddRange(Board.AllCards);
                allCards.AddRange(Graveyard.AllCards);
                return allCards;
            }
        }

        [JsonIgnore]
        public int AttackValue
        {
            get => attackStat.Value;
            set => attackStat.Value = value;
        }

        [JsonIgnore]
        public int AttackBaseValue
        {
            get => attackStat.BaseValue;
            set => attackStat.BaseValue = value;
        }

        [JsonIgnore]
        public int LifeValue
        {
            get => lifeStat.Value;
            set => lifeStat.Value = value;
        }

        [JsonIgnore]
        public int LifeBaseValue
        {
            get => lifeStat.BaseValue;
            set => lifeStat.BaseValue = value;
        }

        [JsonIgnore]
        public int ManaValue
        {
            get => manaPoolStat.Value;
            set => manaPoolStat.Value = value;
        }

        [JsonIgnore]
        public int ManaBaseValue
        {
            get => manaPoolStat.BaseValue;
            set => manaPoolStat.BaseValue = value;
        }

        [JsonIgnore]
        public List<ICharacter> Characters
        {
            get
            {
                List<ICharacter> characters = new List<ICharacter>
                {
                    this
                };
                Board.AllCards.ForEach(c => characters.Add((ICharacter)c));
                return characters;
            }
        }

        public void DrawCard(IGame game)
        {
            game.Execute(new DrawCardAction(this));
        }

        public void PlayMonster(IGame game, IMonsterCard monsterCard, int boardIndex)
        {
            if (!monsterCard.IsPlayable(game))
            {
                throw new CsbcgfException("Tried to play a card that is " +
                    "not playable!");
            }

            if (!Board.IsFreeSlot(boardIndex))
            {
                throw new CsbcgfException("Slot with index " + boardIndex +
                    " is already occupied!");
            }

            game.Execute(new ModifyManaStatAction(this, -monsterCard.ManaValue, 0));
            game.Execute(new RemoveCardFromHandAction(Hand, monsterCard));
            game.Execute(new AddCardToBoardAction(Board, monsterCard, boardIndex));
        }

        public void PlaySpell(IGame game, ITargetlessSpellCard spellCard)
        {
            if (!spellCard.IsPlayable(game))
            {
                throw new CsbcgfException("Tried to play a card that is " +
                    "not playable!");
            }

            game.Execute(new ModifyManaStatAction(this, -spellCard.ManaValue, 0));
            game.Execute(new RemoveCardFromHandAction(Hand, spellCard));
            spellCard.GetActions(game).ForEach(a => game.Execute(a));
            game.Execute(new AddCardToGraveyardAction(Graveyard, spellCard));
        }

        public void PlaySpell(IGame game, ITargetfulSpellCard spellCard, ICharacter target)
        {
            if (!spellCard.IsPlayable(game))
            {
                throw new CsbcgfException("Tried to play a card that is " +
                    "not playable!");
            }

            game.Execute(new ModifyManaStatAction(this, -spellCard.ManaValue, 0));
            game.Execute(new RemoveCardFromHandAction(Hand, spellCard));
            spellCard.GetActions(game, target).ForEach(a => game.Execute(a));
            game.Execute(new AddCardToGraveyardAction(Graveyard, spellCard));
        }

        public HashSet<ICharacter> GetPotentialTargets(IGame game)
        {
            return new HashSet<ICharacter>();
        }

        public void AddReaction(IReaction reaction)
        {
            Reactions.Add(reaction);
        }

        public void RemoveReaction(IReaction reaction)
        {
            Reactions.Remove(reaction);
        }

        public void ReactTo(IGame game, IActionEvent actionEvent)
        {
            Reactions.ForEach(r => r.ReactTo(game, actionEvent));
        }
    }
}
