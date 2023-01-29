﻿using Newtonsoft.Json;

namespace csbcgf
{
    public class Player : IPlayer
    {
        [JsonProperty]
        protected ManaPoolStat manaPoolStat = null!;

        [JsonProperty]
        protected AttackStat attackStat = null!;

        [JsonProperty]
        protected LifeStat lifeStat = null!;

        [JsonProperty]
        protected List<IReaction> reactions = null!;

        [JsonProperty]
        protected IDeck deck = null!;

        [JsonProperty]
        protected IHand hand = null!;

        [JsonProperty]
        protected IBoard board = null!;

        [JsonProperty]
        protected IDeck graveyard = null!;

        protected Player() {}

        /// <summary>
        /// Represents a Player and all his/her associated Cards.
        /// </summary>
        public Player(bool initialize = true) : this(new Deck())
        {
        }

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

        protected Player(IDeck deck, IHand hand, IBoard board, IDeck graveyard,
            ManaPoolStat manaPoolStat, AttackStat attackStat, LifeStat lifeStat,
            List<IReaction> reactions)
        {
            this.deck = deck;
            this.deck.Owner = this;

            this.hand = hand;
            this.hand.Owner = this;

            this.board = board;
            this.board.Owner = this;

            this.graveyard = graveyard;
            this.graveyard.Owner = this;

            this.manaPoolStat = manaPoolStat;
            this.attackStat = attackStat;
            this.lifeStat = lifeStat;
            this.reactions = reactions;
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
            set => attackStat.Value = Math.Max(0, value);
        }

        [JsonIgnore]
        public int AttackBaseValue
        {
            get => attackStat.BaseValue;
            set => attackStat.BaseValue = Math.Max(0, value);
        }

        [JsonIgnore]
        public int LifeValue
        {
            get => lifeStat.Value;
            set => lifeStat.Value = Math.Max(0, value);
        }

        [JsonIgnore]
        public int LifeBaseValue
        {
            get => lifeStat.BaseValue;
            set => lifeStat.BaseValue = Math.Max(0, value);
        }

        [JsonIgnore]
        public int ManaValue
        {
            get => manaPoolStat.Value;
            set => manaPoolStat.Value = Math.Max(0, value);
        }

        [JsonIgnore]
        public int ManaBaseValue
        {
            get => manaPoolStat.BaseValue;
            set => manaPoolStat.BaseValue = Math.Max(0, value);
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

        [JsonIgnore]
        public List<IReaction> Reactions {
            get => reactions;
        }

        [JsonIgnore]
        public IDeck Deck {
            get => deck;
        }

        [JsonIgnore]
        public IHand Hand {
            get => hand;
        }

        [JsonIgnore]
        public IBoard Board {
            get => board;
        }

        [JsonIgnore]
        public IDeck Graveyard {
            get => graveyard;
        }

        public List<IReaction> AllReactions()
        {
            List<IReaction> allReactions = new List<IReaction>(Reactions);
            AllCards.ForEach(c => allReactions.AddRange(c.AllReactions()));
            return allReactions;
        }

        public void DrawCard(IGame game)
        {
            game.Execute(new DrawCardAction(this));
        }

        public void CastMonster(IGame game, IMonsterCard monsterCard, int boardIndex)
        {
            if (!monsterCard.IsSummonable(game))
            {
                throw new CsbcgfException("Tried to play a card that is " +
                    "not playable!");
            }

            if (!Board.IsFreeSlot(boardIndex))
            {
                throw new CsbcgfException("Slot with index " + boardIndex +
                    " is already occupied!");
            }

            game.Execute(new CastMonsterAction(this, monsterCard, boardIndex));
        }

        public void CastSpell(IGame game, ITargetlessSpellCard spellCard)
        {
            if (!spellCard.IsCastable(game))
            {
                throw new CsbcgfException("Tried to play a card that is " +
                    "not playable!");
            }

            game.Execute(new CastTargetlessSpellAction(this, spellCard));
        }

        public void CastSpell(IGame game, ITargetfulSpellCard spellCard, ICharacter target)
        {
            if (!spellCard.IsCastable(game))
            {
                throw new CsbcgfException("Tried to play a card that is " +
                    "not playable!");
            }

            game.Execute(new CastTargetfulSpellAction(this, spellCard, target));
        }

        public HashSet<ICharacter> GetPotentialTargets(IGameState gameState)
        {
            return new HashSet<ICharacter>();
        }

        public void ReactTo(IGame game, IActionEvent actionEvent)
        {
            AllReactions().ForEach(r => r.ReactTo(game, actionEvent));
        }
    }
}
