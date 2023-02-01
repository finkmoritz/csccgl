﻿using csbcgf;

namespace csbcgfdemo
{
    public static class ConsoleUtil
    {
        private const int columnWidth = 20;

        private const ConsoleColor BackgroundColorDefault = ConsoleColor.Black;
        private const ConsoleColor ColorDefault = ConsoleColor.White;
        private const ConsoleColor ColorMana = ConsoleColor.Blue;
        private const ConsoleColor ColorAttack = ConsoleColor.Yellow;
        private const ConsoleColor ColorLife = ConsoleColor.Red;
        private const ConsoleColor ColorSelectable = ConsoleColor.Green;

        public static void PrintGame(IGameState gameState)
        {
            Console.BackgroundColor = BackgroundColorDefault;
            PrintPlayer(gameState, gameState.NonActivePlayers.First(), 0);
            Console.Write("\n\n");
            PrintHand(gameState, gameState.NonActivePlayers.First().Hand, 1);
            Console.Write("\n\n");
            PrintBoard(gameState.NonActivePlayers.First().Board, 2);
            Console.Write("\n\n");
            PrintBoard(gameState.ActivePlayer.Board, 3);
            Console.Write("\n\n");
            PrintHand(gameState, gameState.ActivePlayer.Hand, 4);
            Console.Write("\n\n");
            PrintPlayer(gameState, gameState.ActivePlayer, 5);
            Console.Write("\n\n");
        }

        public static void PrintPlayer(IGameState gameState, IPlayer player, int id)
        {
            Console.ForegroundColor = ColorDefault;
            Console.Write(string.Format("Player #{0}\n", gameState.Players.ToList().IndexOf(player)));
            Console.ForegroundColor = ColorMana;
            Console.Write(string.Format("Mana: {0:D2}/{1:D2}\n", player.ManaValue, player.ManaBaseValue));
            Console.ForegroundColor = ColorLife;
            Console.Write(string.Format("Life: {0:D2}/{1:D2}\n", player.LifeValue, player.LifeBaseValue));
            Console.ForegroundColor = ColorDefault;
            Console.Write(string.Format("Id: {0}", id));
        }

        public static void PrintHand(IGameState gameState, ICardCollection hand, int idPrefix)
        {
            Console.ForegroundColor = ColorMana;
            for (int i = 0; i < hand.Size; ++i)
            {
                ICard card = hand[i];
                Console.Write(string.Format(
                    "Mana: {0:D2}".PadRight(columnWidth),
                    card.ManaValue
                ));
            }
            Console.WriteLine();
            Console.ForegroundColor = ColorAttack;
            for (int i = 0; i < hand.Size; ++i)
            {
                ICard card = hand[i];
                if (card is IMonsterCard monsterCard)
                {
                    Console.Write(string.Format(
                        "Attack: {0:D2}".PadRight(columnWidth),
                        monsterCard.AttackValue
                    ));
                }
                else
                {
                    Console.Write("".PadRight(columnWidth - 4));
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ColorLife;
            for (int i = 0; i < hand.Size; ++i)
            {
                ICard card = hand[i];
                if (card is IMonsterCard monsterCard)
                {
                    Console.Write(string.Format(
                        "Life: {0:D2}".PadRight(columnWidth),
                        monsterCard.LifeValue
                    ));
                }
                else
                {
                    Console.Write("".PadRight(columnWidth - 4));
                }
            }
            Console.WriteLine();
            for (int i = 0; i < hand.Size; ++i)
            {
                ICard card = hand[i];
                Console.ForegroundColor = ColorDefault;
                if (card is IMonsterCard c0 && c0.IsSummonable(gameState))
                {
                    Console.ForegroundColor = ColorSelectable;
                }
                else if (card is ISpellCard c1 && c1.IsCastable(gameState))
                {
                    Console.ForegroundColor = ColorSelectable;
                }
                Console.Write(string.Format(
                    "Id: {0}{1}".PadRight(columnWidth),
                    idPrefix,
                    i
                ));
            }
        }

        public static void PrintBoard(ICardCollection board, int idPrefix)
        {
            Console.ForegroundColor = ColorMana;
            for (int i = 0; i < board.MaxSize; ++i)
            {
                ICard? card = board[i];
                if (card != null)
                {
                    Console.Write(string.Format(
                    "Mana: {0:D2}".PadRight(columnWidth),
                    card.ManaValue
                ));
                }
                else
                {
                    Console.Write("".PadRight(columnWidth));
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ColorAttack;
            for (int i = 0; i < board.MaxSize; ++i)
            {
                ICard? card = board[i];
                if (card is IMonsterCard monsterCard)
                {
                    Console.Write(string.Format(
                        "Attack: {0:D2}".PadRight(columnWidth),
                        monsterCard.AttackValue
                    ));
                }
                else
                {
                    Console.Write("".PadRight(columnWidth));
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ColorLife;
            for (int i = 0; i < board.MaxSize; ++i)
            {
                ICard? card = board[i];
                if (card is IMonsterCard monsterCard)
                {
                    Console.Write(string.Format(
                        "Life: {0:D2}".PadRight(columnWidth),
                        monsterCard.LifeValue
                    ));
                }
                else
                {
                    Console.Write("".PadRight(columnWidth));
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = ColorDefault;
            for (int i = 0; i < board.MaxSize; ++i)
            {
                IMonsterCard? monsterCard = (IMonsterCard?)board[i];
                Console.ForegroundColor = monsterCard != null && monsterCard.IsReadyToAttack
                        ? ColorSelectable
                        : ColorDefault;
                if (monsterCard != null)
                {
                    Console.Write(string.Format(
                        "Id: {0}{1}".PadRight(columnWidth),
                        idPrefix,
                        i
                    ));
                }
                else
                {
                    Console.Write(string.Format(
                        "".PadRight(columnWidth),
                        i
                    ));
                }
            }
        }
    }
}
