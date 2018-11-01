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

        public IEnumerable<ProdElement> GetAll(int? startIndex = null, int? endIndex = null)
        {
            string query = string.Empty;
            var essentialQuery =  // Informations Product
                                  "SELECT [keyId], [lang], [id], [web_offer], [stock_quantity], [pre_order],                                    " +
                                  "[is_for_sale] , [in_stock], [brand], [cat], [pId], [merchantRef],                                            " +
                                  // Informations Price
                                  "[price.curr] as 'curr', [price.buynow] as 'buynow', [price.rrp] as 'rrp',                                    " +
                                  "[price.store] as 'store', [price.basePrice] as 'basePrice',                                                  " +
                                  // Informations Text
                                  "[text.name] as 'name',                                                                                       " +
                                  // Informations Uri
                                  "[uri.awTrack] as 'awTrack', [uri.alternateImageTwo] as 'alternateImageTwo',                                  " +
                                  "[uri.alternateImageThree] as 'alternateImageThree', [uri.awImage] as 'awImage',                              " +
                                  "[uri.awThumb] as 'awThumb', [uri.mImage] as 'mImage', [uri.mLink] as 'mLink',                                " +
                                  // Informations Vertical
                                  "[vertical.id] as 'id', [vertical.name] as 'name', [vertical.availability] as 'availability',                 " +
                                  "[vertical.departureDate] as 'departureDate', [vertical.destinationCity] as 'destinationCity',                " +
                                  "[vertical.destinationCountry] as 'destinationCountry', [vertical.destinationRegion] as 'destinationRegion',  " +
                                  "[vertical.destinationType] as 'destinationType', [vertical.duration] as 'duration',                          " +
                                  "[vertical.hotelAddress] as 'hotelAddress', [vertical.latitude] as 'latitude',                                " +
                                  "[vertical.longitude] as 'longitude', [vertical.returnDate] as 'returnDate',                                  " +
                                  "[vertical.roomType] as 'roomType', [vertical.startingFromPrice] as 'startingFromPrice',                      " +
                                  "[vertical.travelRating] as 'travelRating', [vertical.travelType] as 'travelType'                             ";

            if (startIndex.HasValue && endIndex.HasValue)
            {
                query = $"{essentialQuery} FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY KeyId) AS RowNum FROM Prod) " +
                              $"AS OrderedTable WHERE OrderedTable.RowNum BETWEEN {startIndex} AND {endIndex}";
            }
            else
            {
                query = essentialQuery + " FROM [Prod]";
            }

            return connection.Query<ProdElement, PriceElement, TextElement, UriElement, VerticalElement, ProdElement>
                                        (query, (product, price, text, uri, vertical) =>
                                        {
                                            product.price = price;
                                            product.text = text;
                                            product.uri = uri;
                                            product.vertical = vertical;
                                            return product;
                                        }, splitOn: "curr, name, awTrack, id");
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

        public int GetCount()
        {
            return connection.ExecuteScalar<int>("Select count(*) From Prod");
        }
    }
}