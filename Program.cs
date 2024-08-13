using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
internal class Program
{
    public static Action<object> print = Console.WriteLine;
    public readonly static string connectionString = "data source=DESKTOP-97ENNMR;initial catalog=Leep;trusted_connection=true";
    private static void Main(string[] args)
    {
        //doWork();
        dapperWork();
    }

    public class Test
    {
        public int Id;
        public required string Email;
        public required string Name;
        public DateTime CreatedDate;
        public bool IsArchived;
    }

    private static void doWork()
    {
        var watch = new Stopwatch();
        var tests = new List<Test>();
        var sql = "select top(1000000) * from Test";

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        watch.Restart();
        using var command = new SqlCommand(sql, conn);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var test = new Test
            {
                Id = reader.GetInt32(0),
                Email = reader.GetString(1),
                CreatedDate = reader.GetDateTime(2),
                IsArchived = reader.GetBoolean(3),
                Name = reader.GetString(4)
            };
            tests.Add(test);
        }
        watch.Stop();
        print ($"took {watch.ElapsedMilliseconds/1000f} seconds");
    }
     
    private static IEnumerable<Test> dapperWork() 
    {
        var watch = new Stopwatch();
        var tests = new List<Test>();
        var sql = "select top(1000000) * from Test";
        using var conn = new SqlConnection(connectionString);
        conn.Open();
        watch.Restart();
        var items = conn.Query<Test>(sql, buffered: false);
        watch.Stop();
        print ($"took {watch.ElapsedMilliseconds/1000f} seconds - {items.Count()} - {items.Last().Id}");
        conn.Close();
        return items;
    }
}



