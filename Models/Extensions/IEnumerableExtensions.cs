using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Models.Extensions
{
    public static class IEnumerableExtensions
    {
        public static DataTable AsDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            PropertyDescriptorCollection propertiesOfObject = null;
            foreach (PropertyDescriptor prop in properties)
            {
                Type typ = prop.GetType();
                if (prop.PropertyType.IsSubclassOf(typeof(AbstractEntity)))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    var obj = assembly.CreateInstance(prop.ComponentType.Namespace + "." + prop.PropertyType.Name);
                    propertiesOfObject = TypeDescriptor.GetProperties(obj.GetType());
                    foreach (PropertyDescriptor currentProp in propertiesOfObject)
                    {
                        table.Columns.Add(prop.Name + "." + currentProp.Name, Nullable.GetUnderlyingType(currentProp.PropertyType) ?? currentProp.PropertyType);
                    }
                }
                else
                {
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.PropertyType.IsSubclassOf(typeof(AbstractEntity)))
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        var objj = assembly.CreateInstance(prop.ComponentType.Namespace + "." + prop.PropertyType.Name);
                        propertiesOfObject = TypeDescriptor.GetProperties(objj.GetType());
                        foreach (PropertyDescriptor currentProp in propertiesOfObject)
                        {
                            row[prop.Name + "." + currentProp.Name] = currentProp.GetValue(prop.GetValue(item)) ?? DBNull.Value;
                        }
                    }
                    else
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}