﻿using System;
using System.Collections.Generic;
using csbcgf;

namespace csccgl
{
    [Serializable]
    public abstract class TargetlessSpellCardComponent : CardComponent, ITargetlessSpellCardComponent
    {
        public TargetlessSpellCardComponent(int mana) : base(mana)
        {
        }

        public abstract List<IAction> GetActions(IGame game);
    }
}
