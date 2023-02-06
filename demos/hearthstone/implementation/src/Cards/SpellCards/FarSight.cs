﻿using csbcgf;

namespace hearthstone
{
    public class FarSight : TargetlessSpellCard
    {
        protected FarSight() { }

        public FarSight(bool _ = true) : base(_)
        {
            AddComponent(new FarSightComponent());
        }

        public class FarSightComponent : TargetlessSpellCardComponent
        {
            protected FarSightComponent() { }

            public FarSightComponent(bool _ = true) : base(3)
            {
            }

            public override void Cast(IGame game)
            {
                game.ActionQueue.Execute(new FarSightAction());
            }

            public class FarSightAction : csbcgf.Action
            {
                public override void Execute(IGame game)
                {
                    DrawCardAction drawCardAction = new DrawCardAction(game.ActivePlayer);
                    game.ActionQueue.Execute(drawCardAction);
                    game.ActionQueue.Execute(new ModifyManaStatAction(drawCardAction.DrawnCard!, -3, 0));
                }

                public override bool IsExecutable(IGameState gameState)
                {
                    return true;
                }
            }
        }
    }
}
