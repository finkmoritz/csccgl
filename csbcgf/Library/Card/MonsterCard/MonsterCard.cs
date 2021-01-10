﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace csbcgf
{
    [Serializable]
    public class MonsterCard : Card, IMonsterCard
    {
        [JsonProperty]
        protected AttackStat attackOffsetStat;

        [JsonProperty]
        protected LifeStat lifeOffsetStat;

        public bool IsReadyToAttack { get; set; }

        /// <summary>
        /// Represents a certain type of Card that is played
        /// onto the Player's Board.
        /// </summary>
        /// <param name="components"></param>
        public MonsterCard(List<IMonsterCardComponent> components)
            : this(components, new AttackStat(0), new LifeStat(0), false)
        {
        }

        /// <summary>
        /// Represents a certain type of Card that is played
        /// onto the Player's Board.
        /// </summary>
        /// <param name="mana"></param>
        /// <param name="attack"></param>
        /// <param name="life"></param>
        public MonsterCard(int mana, int attack, int life)
            : this(new List<IMonsterCardComponent> { new MonsterCardComponent(mana, attack, life) })
        {
        }

        public MonsterCard() : this(new List<IMonsterCardComponent>())
        {
        }

        [JsonConstructor]
        protected MonsterCard(List<IMonsterCardComponent> components, AttackStat attackOffsetStat, LifeStat lifeOffsetStat, bool isReadyToAttack)
            : base(components.ConvertAll(c => (ICardComponent)c))
        {
            this.attackOffsetStat = attackOffsetStat;
            this.lifeOffsetStat = lifeOffsetStat;
            IsReadyToAttack = isReadyToAttack;

            AddReaction(new SetReadyToAttackOnStartOfTurnEventReaction(this));
        }

        [JsonIgnore]
        public bool IsAlive => LifeValue > 0;

        [JsonIgnore]
        public int AttackValue
        {
            get => attackOffsetStat.Value + Components.Sum(c => ((IMonsterCardComponent)c).AttackValue);
            set => attackOffsetStat.Value = value - Components.Sum(c => ((IMonsterCardComponent)c).AttackValue);
        }

        [JsonIgnore]
        public int AttackBaseValue
        {
            get => attackOffsetStat.BaseValue + Components.Sum(c => ((IMonsterCardComponent)c).AttackBaseValue);
            set => attackOffsetStat.BaseValue = value - Components.Sum(c => ((IMonsterCardComponent)c).AttackBaseValue);
        }

        [JsonIgnore]
        public int LifeValue
        {
            get => lifeOffsetStat.Value + Components.Sum(c => ((IMonsterCardComponent)c).LifeValue);
            set => lifeOffsetStat.Value = value - Components.Sum(c => ((IMonsterCardComponent)c).LifeValue);
        }

        [JsonIgnore]
        public int LifeBaseValue
        {
            get => lifeOffsetStat.BaseValue + Components.Sum(c => ((IMonsterCardComponent)c).LifeBaseValue);
            set => lifeOffsetStat.BaseValue = value - Components.Sum(c => ((IMonsterCardComponent)c).LifeBaseValue);
        }

        public void Attack(IGame game, ICharacter target)
        {
            if(!IsReadyToAttack)
            {
                throw new CsbcgfException("Failed to attack with a MonsterCard " +
                    "that is not ready to attack!");
            }
            if(!GetPotentialTargets(game).Contains(target))
            {
                throw new CsbcgfException("Cannot attack a target character " +
                    "that is not specified in the list of potential targets!");
            }

            game.Execute(new ModifyLifeStatAction(target, -AttackValue));
            game.Execute(new ModifyLifeStatAction(this, -target.AttackValue));
            game.Execute(new ModifyReadyToAttackAction(this, false));
        }

        public virtual HashSet<ICharacter> GetPotentialTargets(IGame game)
        {
            if (Components.Count == 0)
            {
                return new HashSet<ICharacter>();
            }

            //Compute the intersection of all potential targets
            HashSet<ICharacter> potentialTargets = ((ITargetful)Components[0]).GetPotentialTargets(game);
            foreach (ICardComponent component in Components)
            {
                potentialTargets.IntersectWith(((ITargetful)component).GetPotentialTargets(game));
            }
            return potentialTargets;
        }

        public override bool IsPlayable(IGameState gameState)
        {
            IBoard board = gameState.ActivePlayer.Board;
            return base.IsPlayable(gameState)
                    && board.AllCards.Count < board.MaxSize;
        }
    }
}
