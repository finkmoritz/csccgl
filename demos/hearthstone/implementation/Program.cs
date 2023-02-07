﻿using csbcgf;

namespace hearthstone
{
    class MainClass
    {
        private const string CommandSummon = "S";
        private const string CommandCast = "C";
        private const string CommandAttack = "A";
        private const string CommandEndTurn = "E";
        private const string CommandQuit = "Q";

        public static void Main(string[] args)
        {
            HearthstoneGame game = CreateGame();
            game.Start();

            string info = string.Empty;
            string input;
            do
            {
                Console.Clear();
                Console.WriteLine(info + "\n");
                ConsoleUtil.PrintGame(game);
                Console.WriteLine(GetOptions());
                input = Console.ReadLine() ?? "";
                info = ProcessInput(game, input);
            } while (input.ToUpper() != CommandQuit);
        }

        private static HearthstoneGame CreateGame()
        {
            HearthstoneGame game = new HearthstoneGame();
            for (int i = 0; i < 2; ++i)
            {
                IPlayer player = new Player();
                player.LifeValue = 5;
                player.LifeBaseValue = 5;

                ICardCollection deck = player.GetCardCollection(CardCollectionKeys.Deck);
                for (int n = 0; n < 3; ++n)
                {
                    deck.Add(new Wisp());
                    deck.Add(new ArgentSquire());
                    deck.Add(new Bananas());
                    deck.Add(new ManaWyrm());
                    deck.Add(new FarSight());
                    deck.Add(new KingMukla());
                }
                deck.Shuffle();

                game.AddPlayer(player);
            }

            foreach (HearthstonePlayer player in game.Players)
            {
                for (int i = 0; i < 3; ++i)
                {
                    player.DrawCard(game);
                }
            }

            return game;
        }

        private static string GetOptions()
        {
            string options = "Choose command:\n";
            options += CommandSummon + " <id>: Summon monster card from hand (with <id>) to the board\n";
            options += CommandCast + " <id> [<target_id>]: Cast spell card from hand (with <id>). Cast spell on target (with <target_id>) if the spell requires a target\n";
            options += CommandAttack + " <id> <target_id>: Attack monster card (with <target_id>) with monster card (with <id>)\n";
            options += CommandEndTurn + ": End Turn\n";
            options += CommandQuit + ": Quit\n";
            return options;
        }

        private static string ProcessInput(HearthstoneGame game, string input)
        {
            string output = string.Empty;
            try
            {
                HearthstonePlayer activePlayer = (HearthstonePlayer)game.ActivePlayer;

                string[] inputParams = input.Split(' ');
                switch (inputParams[0].ToUpper())
                {
                    case CommandSummon:
                        IMonsterCard monsterCard = (IMonsterCard)GetObjectById(game, inputParams[1]);
                        activePlayer.SummonMonster(game, monsterCard);
                        output = "Cast monster card";
                        break;
                    case CommandCast:
                        ISpellCard spellCard = (ISpellCard)GetObjectById(game, inputParams[1]);
                        activePlayer = (HearthstonePlayer)game.ActivePlayer;
                        if (spellCard is TargetlessSpellCard targetlessSpellCard)
                        {
                            activePlayer.CastSpell(game, targetlessSpellCard);
                            output = "Cast spell";
                        }
                        else if (spellCard is TargetfulSpellCard targetfulSpellCard)
                        {
                            ICharacter target = (ICharacter)GetObjectById(game, inputParams[2]);
                            activePlayer.CastSpell(game, targetfulSpellCard, target);
                            output = "Cast spell onto target";
                        }
                        break;
                    case CommandAttack:
                        IHearthstoneMonsterCard monster = (IHearthstoneMonsterCard)GetObjectById(game, inputParams[1]);
                        ICharacter targetCard = (ICharacter)GetObjectById(game, inputParams[2]);
                        monster.Attack(game, targetCard);
                        output = "Attacked";
                        break;
                    case CommandEndTurn:
                        game.NextTurn();
                        output = "Player ended turn.";
                        break;
                    case CommandQuit:
                        break;
                    default:
                        output = "Invalid command: " + input;
                        break;
                }
            }
            catch (Exception e)
            {
                output = e.Message + "\n" + e.StackTrace;
            }
            return output;
        }

        private static Object GetObjectById(IGame game, string id)
        {
            switch (int.Parse(id.Substring(0, 1)))
            {
                case 0:
                    return game.NonActivePlayers.First();
                case 1:
                    return game.NonActivePlayers.First().GetCardCollection(CardCollectionKeys.Hand)[int.Parse(id.Substring(1, 1))];
                case 2:
                    return game.NonActivePlayers.First().GetCardCollection(CardCollectionKeys.Board)[int.Parse(id.Substring(1, 1))]!;
                case 3:
                    return game.ActivePlayer.GetCardCollection(CardCollectionKeys.Board)[int.Parse(id.Substring(1, 1))]!;
                case 4:
                    return game.ActivePlayer.GetCardCollection(CardCollectionKeys.Hand)[int.Parse(id.Substring(1, 1))];
                case 5:
                    return game.ActivePlayer;
                default:
                    throw new Exception("Unparsable id: " + id);
            }
        }
    }
}