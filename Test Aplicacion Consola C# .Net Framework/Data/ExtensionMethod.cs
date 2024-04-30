
using System;
using Core.Dtos;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;
using System.Data;
using Microsoft.Win32;
using System.Text.Json;
using System.Reflection;
using System.Dynamic;
using System.CodeDom;

namespace Test_Aplicacion_Consola_C_.Net_Framework.Data
{
    public static class ExtensionMethod
    {
        public static int MultiplyBy(this int value, int val)
        {
            return value * val;
        }
        
        public static List<T> GetDtoUsingJson<T>(this DbDataReader value)
        {
            List<Dictionary<string, object>> listResult = new List<Dictionary<string, object>>();
            //object ob = (T)Activator.CreateInstance(typeof(T), new object[] {  });
            if (value.HasRows)
            {
                while (value.Read())
                {
                    var ptype = value.GetFieldType(0);
                    listResult.Add(Enumerable.Range(0, value.FieldCount).ToDictionary(value.GetName, value.GetValue));
                }
            }
            List<T> listDtos = new List<T>();
            foreach (var registro in listResult)
            {
                string aux = JsonSerializer.Serialize(registro);
                var report = JsonSerializer.Deserialize<T>(aux);
                listDtos.Add(report);

            }
            Console.WriteLine(value.ToString());
            return listDtos;
        }

        public static List<T> ConvertToDto<T>(this DbDataReader reader)
        {

            try
            {
                //object ob = (T)Activator.CreateInstance(typeof(T), new object[] {  });
                var classDtoName = typeof(T).Name;
                List<T> listDtos = new List<T>();
                //var metaDataList = new List<Dictionary<string, object>>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        object newDtoInstance = (T)Activator.CreateInstance(typeof(T), new object[] { });

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var readerFieldType = reader.GetFieldType(i);
                            var readerSqlFieldType = reader.GetDataTypeName(i);
                            var readerFielName = reader.GetName(i);
                            var newDtoPropertyInstance = newDtoInstance.GetType().GetProperties().FirstOrDefault(newDtoProperty => ValidateExistProperty(newDtoProperty, readerFielName));
                            if (newDtoPropertyInstance != null)
                            {
                                var newDtoPropertyType = newDtoPropertyInstance.PropertyType;
                                if (readerFieldType == newDtoPropertyType)
                                {
                                    newDtoPropertyInstance.SetValue(newDtoInstance, reader.GetValue(i));
                                }
                                else
                                {
                                    Program.log.Error($"Error: class property [{newDtoPropertyInstance.Name}] with property type [{newDtoPropertyType.Name}] \n" +
                                        $"incompatible to reader field [{readerFielName}] with field type [{readerFieldType.Name}]");
                                    Console.WriteLine($"Error: class property [{newDtoPropertyInstance.Name}] with property type [{newDtoPropertyType.Name}] \n" +
                                        $"incompatible to reader field [{readerFielName}] with field type [{readerFieldType.Name}]");
                                }
                            }
                            else
                            {
                                Program.log.Error($"Error: class [{classDtoName}] does not contains property called [{readerFielName}] ");
                                Console.WriteLine($"Error: class [{classDtoName}] does not contains property called [{readerFielName}] ");
                            }
                            /*dynamic fieldMetaData = new ExpandoObject();
                            var columnName = reader.GetName(i);
                            var value = reader[i];
                            var dotNetType = reader.GetFieldType(i);
                            var sqlType = reader.GetDataTypeName(i);
                            var specificType = reader.GetProviderSpecificFieldType(i);
                            fieldMetaData.columnName = columnName;
                            fieldMetaData.value = value;
                            fieldMetaData.dotNetType = dotNetType;
                            fieldMetaData.sqlType = sqlType;
                            fieldMetaData.specificType = specificType;
                            metaDataList.Add(fieldMetaData);*/
                        }
                        listDtos.Add((T)newDtoInstance);

                    }
                }
                else
                {
                    Program.log.Warn("No leads found in data base");
                    Console.WriteLine("No leads found in data base");
                }
                reader.Close();
                return listDtos;
            }
            catch (Exception ex)
            {
                Program.log.Error($"Error: exception was throw while converting reader to dto: " + ex.Message);
                Console.WriteLine($"Error: exception was throw while converting reader to dto: " + ex.Message);

                return new List<T>();
            }
        }

        public static bool ValidateExistProperty(PropertyInfo newDtoProperty, string readerFieldName)
        {
            object[] attribute = newDtoProperty.GetCustomAttributes(typeof(DbReaderMapAttribute), true);
            if (attribute.Length > 0)
            {
                DbReaderMapAttribute myAttribute = (DbReaderMapAttribute)attribute[0];
                return myAttribute.Name.ToLower() == readerFieldName.ToLower();
            }
            else
            {
                return newDtoProperty.Name.ToLower() == readerFieldName.ToLower();
            }
        }

        public static T CastTo<T>(this object originObjectToCast)// where T : new()
        {

            try
            {
                //object ob = (T)Activator.CreateInstance(typeof(T), new object[] {  });
                var classDtoName = typeof(T).Name;
                T newDto = (T)Activator.CreateInstance(typeof(T),new object[] {});//new T();
                //var metaDataList = new List<Dictionary<string, object>>();
                if (originObjectToCast != null)
                {
                   
                    foreach (var originObjectProp in originObjectToCast.GetType().GetProperties())
                    {
                        var originObjectFieldType = originObjectProp.PropertyType;
                        var originObjectFielName = string.Empty;
                        object[] objectAttributes = originObjectProp.GetCustomAttributes(typeof(DbReaderMapAttribute), true);
                        if (objectAttributes.Length > 0)
                        {
                            DbReaderMapAttribute objectAttribute = (DbReaderMapAttribute)objectAttributes[0];
                            originObjectFielName = objectAttribute.Name;
                        }
                        else
                        {
                            originObjectFielName = originObjectProp.Name;
                        }
                        var newDtoPropertyInstance = newDto.GetType().GetProperties().FirstOrDefault(newDtoProperty => ValidateExistProperty(newDtoProperty, originObjectFielName));
                        if (newDtoPropertyInstance != null)
                        {
                            var newDtoPropertyType = newDtoPropertyInstance.PropertyType;
                            if (newDtoPropertyInstance.PropertyType.IsClass && !newDtoPropertyInstance.PropertyType.FullName.StartsWith("System."))//&& newDtoPropertyInstance.PropertyType.Assembly.FullName == type.Assembly.FullName)
                            {
                                //if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
                                //    throw new ArgumentException("Type must be List<>, but was " + type.FullName, "someList");

                            }
                            else if (originObjectFieldType == newDtoPropertyType)
                            {
                                newDtoPropertyInstance.SetValue(newDto, originObjectProp.GetValue(originObjectToCast));
                            }
                            else
                            {
                                Program.log.Error($"Error: class property [{newDtoPropertyInstance.Name}] with property type [{newDtoPropertyType.Name}] \n" +
                                    $"incompatible to reader field [{originObjectFielName}] with field type [{originObjectFieldType.Name}]");
                                Console.WriteLine($"Error: class property [{newDtoPropertyInstance.Name}] with property type [{newDtoPropertyType.Name}] \n" +
                                    $"incompatible to reader field [{originObjectFielName}] with field type [{originObjectFieldType.Name}]");
                            }
                        }
                        else
                        {
                            Program.log.Error($"Error: class [{classDtoName}] does not contains property called [{originObjectFielName}] ");
                            Console.WriteLine($"Error: class [{classDtoName}] does not contains property called [{originObjectFielName}] ");
                        }
                    }
                        
                }
                else
                {
                    Program.log.Warn("El Objeto es nulo");
                    Console.WriteLine("El objeto es nulo");
                }

                return newDto;
            }
            catch (Exception ex)
            {
                Program.log.Error($"Error: exception was throw while converting reader to dto: " + ex.Message);
                Console.WriteLine($"Error: exception was throw while converting reader to dto: " + ex.Message);

                return (T)new object();
            }
        }

        //private static void ConvertObjectToObject(object fromObject, object toObject)
        //{
        //    foreach(var fromObjectProperty in fromObject.GetType().GetProperties())
        //    {
        //        var toObjectProperty = toObject.GetType().GetProperties().FirstOrDefault(toObjectPropertyIndex => ValidateExistProperty(toObjectPropertyIndex, fromObjectProperty.Name));
                
                
        //        var newDtoPropertyInstance = newDto.GetType().GetProperties().FirstOrDefault(newDtoProperty => ValidateExistProperty(newDtoProperty, originObjectFielName));
        //        if (newDtoPropertyInstance != null)
        //        {
        //            var newDtoPropertyType = newDtoPropertyInstance.PropertyType;
        //            if (newDtoPropertyInstance.PropertyType.IsClass && !newDtoPropertyInstance.PropertyType.FullName.StartsWith("System."))//&& newDtoPropertyInstance.PropertyType.Assembly.FullName == type.Assembly.FullName)
        //            {

        //            }
        //            else if (originObjectFieldType == newDtoPropertyType)
        //            {
        //                newDtoPropertyInstance.SetValue(newDto, originObjectProp.GetValue(originObjectToCast));
        //            }
        //            else
        //            {
        //                Program.log.Error($"Error: class property [{newDtoPropertyInstance.Name}] with property type [{newDtoPropertyType.Name}] \n" +
        //                    $"incompatible to reader field [{originObjectFielName}] with field type [{originObjectFieldType.Name}]");
        //                Console.WriteLine($"Error: class property [{newDtoPropertyInstance.Name}] with property type [{newDtoPropertyType.Name}] \n" +
        //                    $"incompatible to reader field [{originObjectFielName}] with field type [{originObjectFieldType.Name}]");
        //            }
        //        }
        //        else
        //        {
        //            Program.log.Error($"Error: class [{classDtoName}] does not contains property called [{originObjectFielName}] ");
        //            Console.WriteLine($"Error: class [{classDtoName}] does not contains property called [{originObjectFielName}] ");
        //        }
        //    }
        //}

        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
        public sealed class DbReaderMapAttribute : Attribute
        {
            public DbReaderMapAttribute(string name, Type type = null)
            {
                Name = name;
                Type = type;
            }
            public string Name { get; set; }
            public Type Type { get; set; }

        }

        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
