using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
internal class Program
{
    public static Action<object> print = Console.WriteLine;
    public readonly static string connectionString = "data source=DESKTOP-97ENNMR;initial catalog=Leep;trusted_connection=true;";
    private static void Main(string[] args)
    {
        doWork();
        //doWorkAsync().Wait();
        dapperWork();
        //Task.WaitAll(dapperWorkAsync());
        //GetPlayersForTeamUsingDapper(1);
        //GetPlayersForTeam(1);
    }

    public static void GetPlayersForTeam(int teamId)
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        using(SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using(SqlDataAdapter adapter = new SqlDataAdapter("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE TeamId = @ID", conn))
            {
                adapter.SelectCommand.Parameters
                                     .Add(new SqlParameter("@ID", teamId));
                DataTable table = new DataTable();
                adapter.Fill(table);
            }
        }
        watch.Stop();
        print ($"took {watch.ElapsedMilliseconds/1000f} seconds");
    }
    public static void GetPlayersForTeamUsingDapper(int teamId)
    {
        Stopwatch watch = new Stopwatch();
        IEnumerable<Player> players;
        watch.Start();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            var items = conn.Query<Player>("SELECT * FROM Player WHERE TeamId = @ID", new { ID = teamId });
        }
        watch.Stop();
        print ($"took {watch.ElapsedMilliseconds/1000f} seconds");
    }

    private static List<Player> doWork()
    {
        var sql = "select top 1000 * from Player where TeamId = 1";
        var players = new List<Player>();
        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var watch = Stopwatch.StartNew();
        using var command = new SqlCommand(sql, conn);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var test = new Player
            {
                Id = reader.GetInt32(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                DayOfBirth = reader.GetDateTime(3),
                TeamId = reader.GetInt32(4)
            };
            players.Add(test);
        }
        watch.Stop();
        print ($"took {watch.ElapsedMilliseconds/1000f} seconds");
        return players;
    }
    private static async Task<List<Player>> doWorkAsync()
    {
        var sql = "SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE TeamId = 1";
        var players = new List<Player>();

        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        var watch = Stopwatch.StartNew();

        using var command = new SqlCommand(sql, conn);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var player = new Player
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                DayOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                TeamId = reader.GetInt32(reader.GetOrdinal("TeamId"))
            };
            players.Add(player);
        }

        watch.Stop();
        print($"took {watch.ElapsedMilliseconds / 1000f} seconds");
        return players;
    }

    private static IEnumerable<Player> dapperWork() 
    {
        var sql = "select top 1000 * from Player where TeamId = 1";
        using var conn = new SqlConnection(connectionString);
        conn.Open();
        var watch = Stopwatch.StartNew();
        var items = conn.Query<Player>(sql);
        watch.Stop();
        print ($"took {watch.ElapsedMilliseconds/1000f} seconds");
        return items;
    }
    private static async Task<IEnumerable<Player>> dapperWorkAsync()
    {
        var sql = "SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Player WHERE TeamId = 1";
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        var watch = Stopwatch.StartNew();
        var items = await conn.QueryAsync<Player>(sql);
        watch.Stop();

        print($"took {watch.ElapsedMilliseconds / 1000f} seconds");

        return items;
    }
    public class Player
    {
        public int Id;
        public required string FirstName;
        public required string LastName;
        public DateTime DayOfBirth;
        public required int TeamId;
    }

    public class Team
    {
        public int Id;
        public required string Name;
        public DateTime FoundingDate;
        public int SportId;
    }

    public class Sport {
        public int Id;
        public required string Name;
    }
    public class Test
    {
        public int Id;
        public required string Email;
        public required string Name;
        public DateTime CreatedDate;
        public bool IsArchived;
    }
}



