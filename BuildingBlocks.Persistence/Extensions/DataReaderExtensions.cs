using BuildingBlocks.ComponentModel;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace BuildingBlocks.Persistence.Extensions
{
    public static class DataReaderExtensions
    {
        public static object GetValue(this IDataReader reader, string columnName)
        {
            try
            {
                if (reader != null && !reader.IsClosed && StringExtensions.HasValue(columnName))
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    return reader.GetValue(ordinal);
                }
            }
            catch (Exception ex)
            {
                ex.Dump();
            }
            return null;
        }

        public static IEnumerable<T> MapSequence<T>(this IDataReader reader, params string[] fieldsToSkip) where T : new()
        {
            Guard.AgainstNull(reader, nameof(reader));
            while (reader.Read())
            {
                yield return reader.Map<T>(fieldsToSkip);
            }
        }

        public static IEnumerable<object> MapSequence(this IDataReader reader, params string[] fieldsToSkip)
        {
            Guard.AgainstNull(reader, nameof(reader));
            while (reader.Read())
            {
                yield return reader.Map(fieldsToSkip);
            }
        }

        public static T Map<T>(this IDataReader reader, params string[] fieldsToSkip) where T : new()
        {
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("Data reader cannot be used because it's already closed");
            }

            var instance = new T();
            MapObject(reader, instance, fieldsToSkip);
            return instance;
        }

        public static object Map(this IDataReader reader, params string[] fieldsToSkip)
        {
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("Data reader cannot be used because it's already closed");
            }

            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            MapDictionary(reader, expandoObject, fieldsToSkip);
            return expandoObject;
        }

        public static void Map(this IDataReader reader, object instance, params string[] fieldsToSkip)
        {
            Guard.AgainstNull(instance, nameof(instance));

            if (reader.IsClosed)
            {
                throw new InvalidOperationException("Data reader cannot be used because it's already closed");
            }

            if (instance is IDictionary<string, object> dictionaryInstance)
            {
                MapDictionary(reader, dictionaryInstance, fieldsToSkip);
            }
            else
            {
                MapObject(reader, instance, fieldsToSkip);
            }
        }

        private static void MapObject(IDataReader reader, object instance, params string[] fieldsToSkip)
        {
            var properties = FastProperty.GetProperties(instance.GetType(), PropertyCachingStrategy.Cached);
            if (properties.Count == 0) return;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);

                if (properties.ContainsKey(fieldName) && !fieldsToSkip.Contains(fieldName))
                {
                    var property = properties[fieldName];
                    if (property != null && property.Property.CanWrite)
                    {
                        object value = reader.GetValue(i);
                        property.SetValue(instance, value == DBNull.Value ? null : value);
                    }
                }
            }
        }

        private static void MapDictionary(IDataReader reader, IDictionary<string, object> instance, params string[] fieldsToSkip)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);

                if (!fieldsToSkip.Contains(fieldName))
                {
                    object value = reader.GetValue(i);
                    instance[fieldName] = value == DBNull.Value ? null : value;
                }
            }
        }
    }
}
