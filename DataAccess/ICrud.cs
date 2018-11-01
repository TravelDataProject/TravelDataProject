using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    internal interface ICrud<T>
    {
        int Insert(T entity);

        int Insert(T entity, IDbTransaction transaction);

        void Update(T entity);

        void Delete(int key);

        void DeleteByExpression(string expression);

        void DeleteAll();

        int GetCount();

        T Get(int key);

        IEnumerable<T> GetByExpression(string expression);

        IEnumerable<T> GetAll(int? startIndex, int? endIndex);
    }
}