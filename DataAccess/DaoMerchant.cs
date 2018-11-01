using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    public class DaoMerchant : ConnectionAccess, ICrud<MerchantElement>
    {
        #region Instance

        private static readonly DaoMerchant instance = new DaoMerchant();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DaoMerchant()
        {
        }

        private DaoMerchant()
        {
            SetConnection();
        }

        public static DaoMerchant Instance
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

        public MerchantElement Get(int key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MerchantElement> GetAll(int? startIndex, int? endIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MerchantElement> GetByExpression(string expression)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            return connection.ExecuteScalar<int>("Select count(*) From Merchant");
        }

        public int Insert(MerchantElement entity)
        {
            throw new NotImplementedException();
        }

        public int Insert(MerchantElement entity, IDbTransaction transaction)
        {
            return connection.ExecuteScalar<int>("Insert Into Merchant Values(@merchantId, @merchantName, @id); select Scope_Identity();", entity, transaction: transaction);
        }

        public void Update(MerchantElement entity)
        {
            throw new NotImplementedException();
        }
    }
}