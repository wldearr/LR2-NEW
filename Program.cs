using System;
using System.Collections.Generic;

namespace ConsoleApplication2
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var UserOne = Creator.CreateClassicAccount("Kolia");
            var UserTwo = Creator.CreateVipAccount("Olia");
            var GameOne = Creator.PlayGameWithRating(UserOne, UserTwo, 10);
            var GameTwo = Creator.PlayGameWithoutRating(UserOne, UserTwo);
            GameOne.PlayGame();
            GameTwo.PlayGame();
            Console.Write(UserOne.GetHistory());
            Console.Write(UserTwo.GetHistory());
            Console.Write(UserOne.PlayerInformation());
            Console.Write(UserTwo.PlayerInformation());
        }
        public class SetHistory
        {
            public int Rating { get; }
            public string Status { get; }
            public string OpponentName { get; }
            public int Index { get; }

            public SetHistory(int rating, string status, string opponentName, int index)
            {
                Rating = rating;
                Status = status;
                OpponentName = opponentName;
                Index = index;
            }
        }
        public class ClassicAccount
        {
            protected List<SetHistory> MyHistory = new List<SetHistory>();
            
            public string UserName { get; }
            public string UserAccount { get; }
            public int CurrentRating { get; set; }
            public ClassicAccount(string userName)
            {
                UserName = userName;
                UserAccount = GetType().Name;
                CurrentRating = 500;
            }
            public virtual void WinGame(string opponentName, BaseGame baseGame)
            {
                CurrentRating += baseGame.GameRating;
                SetHistory game = new SetHistory(baseGame.GameRating, "Winner", opponentName, 1);
                MyHistory.Add(game);
            }
            public virtual void LoseGame(string opponentName, BaseGame baseGame)
            {
                if (CurrentRating - baseGame.GameRating < 1)
                {
                    throw new InvalidOperationException($"The {UserName} lost");
                }
                CurrentRating -= baseGame.GameRating;
                SetHistory game = new SetHistory(baseGame.GameRating, "Loser", opponentName, 1);
                MyHistory.Add(game);
            }
            public string GetHistory()
            {
                Console.WriteLine($"History games player - {UserName}:");
                var report = new System.Text.StringBuilder();
                int Index = 0;
                for (var index = 0; index < MyHistory.Count; index++)
                {
                    var game = MyHistory[index];
                    Index += game.Index;
                    report.AppendLine(
                        $"GAME: {Index}|{UserName} vs {game.OpponentName}|Rating: {game.Rating}|Status for {UserName}: {game.Status}");
                }
                return report.ToString();
            }
            public string PlayerInformation()
            {
                Console.WriteLine("Information about player:");
                var report = new System.Text.StringBuilder();
                report.AppendLine($"Player: {UserName}, Rating: {CurrentRating}, Account: {UserAccount}");
                return report.ToString();
            }
        }
        
        public class VipAccount: ClassicAccount
        {
            public VipAccount(string userName) : base(userName)
            {
            }
            public override void WinGame(string opponentName, BaseGame baseGame)
            {
                CurrentRating += baseGame.GameRating * 2;
                SetHistory game = new SetHistory(baseGame.GameRating * 2, "Winner(VIP)", opponentName, 1);
                MyHistory.Add(game);
            }
            public override void LoseGame(string opponentName, BaseGame baseGame)
            {
                if (CurrentRating - baseGame.GameRating < 1)
                {
                    throw new InvalidOperationException($"The {UserName} lost");
                }
                CurrentRating -= baseGame.GameRating / 2;
                SetHistory game = new SetHistory(baseGame.GameRating / 2, "Loser(VIP)", opponentName, 1);
                MyHistory.Add(game);
            }
        }
        public abstract class BaseGame
        {
            protected readonly ClassicAccount PlayerOne;
            protected readonly ClassicAccount PlayerTwo;
            public int GameRating { get; }
            protected BaseGame(ClassicAccount playerOne, ClassicAccount playerTwo, int gameRating)
            {
                if (gameRating <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(gameRating), "The rating must be greater than 0");
                }
                PlayerOne = playerOne;
                PlayerTwo = playerTwo;
                GameRating = gameRating;
            }
            protected BaseGame(ClassicAccount playerOne, ClassicAccount playerTwo)
            {
                PlayerOne = playerOne;
                PlayerTwo = playerTwo;
                GameRating = 0;
            }
            public abstract void PlayGame();
        }
        public class GameWithRating: BaseGame
        {
            public GameWithRating(ClassicAccount playerOne, ClassicAccount playerTwo, int gameRating) : base(playerOne, playerTwo, gameRating)
            {
            }
            public int Result { get; set; }
            public override void PlayGame()
            {
                Random rnd = new Random();
                Result = rnd.Next(1, 3);
                switch (Result)
                {
                    case 1:
                        PlayerOne.WinGame(PlayerTwo.UserName, this);
                        PlayerTwo.LoseGame(PlayerOne.UserName, this);
                        break;
                    case 2:
                        PlayerTwo.WinGame(PlayerOne.UserName, this);
                        PlayerOne.LoseGame(PlayerTwo.UserName, this);
                        break;
                }
            }
        } 
        public class GameWithoutRating: BaseGame
        {
            public GameWithoutRating(ClassicAccount playerOne, ClassicAccount playerTwo) : base(playerOne, playerTwo)
            { 
            }
            public int Result { get; set; }
            public override void PlayGame()
            {
                Random rnd = new Random();
                Result = rnd.Next(1, 3);
                switch (Result)
                {
                    case 1:
                        PlayerOne.WinGame(PlayerTwo.UserName, this);
                        PlayerTwo.LoseGame(PlayerOne.UserName, this);
                        break;
                    case 2:
                        PlayerTwo.WinGame(PlayerOne.UserName, this);
                        PlayerOne.LoseGame(PlayerTwo.UserName, this);
                        break;
                }
            }
        }
        public class Creator
        {
            public static BaseGame PlayGameWithRating(ClassicAccount playerOne, ClassicAccount playerTwo, int gameRating)
            {
                return new GameWithRating(playerOne, playerTwo, gameRating);
            }
            public static BaseGame PlayGameWithoutRating(ClassicAccount playerOne, ClassicAccount playerTwo)
            {
                return new GameWithoutRating(playerOne, playerTwo);
            }
            public static ClassicAccount CreateClassicAccount(string userName)
            {
                return new ClassicAccount(userName);
            }
            public static VipAccount CreateVipAccount(string userName)
            {
                return new VipAccount(userName);
            }
        }
    }
}
