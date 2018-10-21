using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess
{
    public class DaoProduct : ConnectionAccess, ICrud<ProdElement>
    {
        #region Instance

        private static readonly DaoProduct instance = new DaoProduct();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DaoProduct()
        {
        }

        private DaoProduct()
        {
            SetConnection();
        }

        public static DaoProduct Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion Instance

        public void Delete(int key)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public void DeleteByExpression(string expression)
        {
            throw new NotImplementedException();
        }

        public ProdElement Get(int key)
        {
            return connection.Query<ProdElement>("Select * From prod Where id = " + key).Single(); ;
        }

        public IEnumerable<ProdElement> GetAll()
        {
            return connection.Query<ProdElement>("select * from prod");
        }

        public IEnumerable<ProdElement> GetByExpression(string expression)
        {
            return connection.Query<ProdElement>("Select * From prod Where " + expression);
        }

        public int Insert(ProdElement entity)
        {
            throw new NotImplementedException();
        }

        public int Insert(ProdElement entity, IDbTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void Update(ProdElement entity)
        {
            throw new NotImplementedException();
        }

        public void BulkCopy(DataTable dataTable, int merchantKeyId, IDbTransaction transaction)
        {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(ConnectionAccess.Instance, SqlBulkCopyOptions.KeepIdentity, transaction as SqlTransaction))
            {
                dataTable.Columns.Add("merchantRef");

                foreach (DataColumn item in dataTable.Columns)
                {
                    sqlBulkCopy.ColumnMappings.Add(item.ColumnName, "[" + item.ColumnName + "]");
                }
                dataTable.AsEnumerable().ToList().ForEach(r => r["merchantRef"] = merchantKeyId);
                sqlBulkCopy.DestinationTableName = "Prod";
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }

        public DataTable GetDataTableFromTable(DataTable productsTable, int? startIndex, int? endIndex = null)
        {
            string query = "SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY KeyId) AS RowNum FROM Prod) " +
                          $"AS OrderedTable WHERE OrderedTable.RowNum BETWEEN {startIndex} AND {endIndex}";
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, connection))
            {
                sqlDataAdapter.Fill(productsTable);
            }
            return productsTable;
        }

        public DataTable GetDataTableFromTable(DataTable productsTable, int? limit = null)
        {
            string query = "Select" + (limit == null ? string.Empty : " TOP " + limit) + " * From Prod";
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, connection))
            {
                sqlDataAdapter.Fill(productsTable);
            }
            return productsTable;
        }

        public int GetCount()
        {
            return connection.ExecuteScalar<int>("Select count(*) From Prod");
        }
    }
}