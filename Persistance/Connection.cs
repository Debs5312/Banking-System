namespace Persistance
{
    public class Connection
    {
        public string ConnectionString { get; set; }
        public Connection()
        {
            ConnectionString = "Server=localhost,1433;database=BankingDBStore;UID=sa;PWD=Jyoti@1234;MultipleActiveResultSets=True;TrustServerCertificate=True";
        }

    }
}