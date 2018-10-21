using System.Data.SqlClient;

namespace DataAccess
{
    public abstract class ConnectionAccess
    {
        private string connectionString = @"Data Source=.\SQLExpress;Initial Catalog=XmlData;Integrated Security=True";
        public SqlConnection connection = null;
        public static SqlConnection Instance = null;

        public void SetConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(connectionString);
            }
            if (Instance == null)
            {
                Instance = connection;
            }
        }

        public static SqlTransaction SetTransaction()
        {
            if (Instance.State != System.Data.ConnectionState.Open)
            {
                Instance.Open();
            }
            return Instance.BeginTransaction();
        }
    }
}