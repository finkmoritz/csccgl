﻿using System;
using System.Collections.Generic;
using csbcgf;

namespace csccgl
{
    [Serializable]
    public class MonsterCardComponent : CardComponent, IMonsterCardComponent
    {
        public MonsterCardComponent(int mana, int attack, int life) : base(mana)
        {
            attackStat = new AttackStat(attack);
            lifeStat = new LifeStat(life);
        }

        public int AttackValue
        {
            get => attackStat.Value;
            set => attackStat.Value = value;
        }

        public int AttackBaseValue
        {
            get => attackStat.BaseValue;
            set => attackStat.BaseValue = value;
        }

        public int LifeValue
        {
            get => lifeStat.Value;
            set => lifeStat.Value = value;
        }

        public int LifeBaseValue
        {
            get => lifeStat.BaseValue;
            set => lifeStat.BaseValue = value;
        }

        protected AttackStat attackStat;

        protected LifeStat lifeStat;

        public HashSet<ICharacter> GetPotentialTargets(IGame game)
        {
            HashSet<ICharacter> potTargets = new HashSet<ICharacter>(
                (IEnumerable<ICharacter>)game.NonActivePlayer.Board.AllCards
            );
            potTargets.Add(game.NonActivePlayer);

            return potTargets;
        }
    }
}