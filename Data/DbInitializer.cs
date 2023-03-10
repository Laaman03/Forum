using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace Forum.Data
{
    public class DbInitializer
    {
        public static void InitializeDB(ApplicationDbContext context, string initializerPassword)
        {
            context.Database.EnsureCreated();
            InitializeUsers(context, initializerPassword);
            InitializePosts(context);
        }
        private static void InitializeUsers(ApplicationDbContext context, string initializerPassword)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any())
            {
                return;
            }
            var users = new User[]
            {
                new() {UserName = "admin", JoinDate = DateTime.UtcNow},
                new() {UserName = "participant-1", JoinDate = DateTime.UtcNow},
                new() {UserName = "participant-2", JoinDate = DateTime.UtcNow},
            };

            foreach (var user in users)
            {
                context.Users.Add(user);
            }

            context.SaveChanges();
            foreach (var user in users)
            {
                var hash = BC.HashPassword(initializerPassword);
                var auth = new UserAuthentication
                {
                    UserName = user.UserName,
                    PasswordHash = hash,
                };
                context.UserAuthentication.Add(auth);
            }

            context.SaveChanges();
        }

        private static void InitializePosts(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Posts.Any())
            {
                return;
            }

            var user1 = context.Users.Include(u => u.Posts).First(u => u.ID== 1);

            if (user1 is null)
            {
                return;
            }

            for (var i = 0; i < 10; i++)
            {
                var title = RandWords(10);
                var content = RandWords(20);
                user1.Posts.Add(new Post { Title = title, Content = content, CreateDate = DateTime.UtcNow });
            }
            context.SaveChanges();
        }

        private static readonly string[] wordList =
        {
            "overcharged",
            "entoiling",
            "forspoke",
            "unscrawled",
            "depreciant",
            "motorcycling",
            "remindal",
            "unrepulsing",
            "becharmed",
            "bedlamized",
            "thiocyanic",
            "folliculina",
            "trigonella",
            "xiphisterna",
            "amoke",
            "martyrology",
            "outarde",
            "miscomputing",
            "chemotherapeutics",
            "plumbean",
            "maravedis",
            "arsphenamine",
            "farcinoma",
            "backbiter",
            "nonutilization",
            "unexpedient",
            "alaite",
            "burn",
            "authorize",
            "empoisonment",
            "counterpane",
            "colitic",
            "pseudohallucination",
            "punan",
            "jaggeries",
            "disamis",
            "northeast",
            "kinoplasm",
            "delays",
            "unpredaciousness"
        };
        private static readonly Random random = new();

        private static string RandWords(int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var word = wordList[random.Next(wordList.Length)];
                sb.Append(word);
                sb.Append(' ');
            }
            return sb.ToString();
        }

    }
}
