using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Unilag_Medic.Data
{
    public class EntityConnection
    {
        public string ConnectionString = "Server=DESKTOP-FL7SFF4;Database=Unilag_Medic;User ID=sa;Password=ellnerd22;Trusted_Connection=True;";
        private SqlConnection connection;
        private string tableName;
        private int defaultSelectLength;
        private Dictionary<string, string> tableSchema;

        public static string ToJson(List<Dictionary<string, object>> list) //changed object to string for dict
        {
            string result = "";
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Dictionary<string, object>>)); //changed object to string for dict

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, list);
                result = Encoding.Default.GetString(ms.ToArray());
            }
            return result;
        }

        public EntityConnection(string tableName)
        {
            this.defaultSelectLength = 100;
            this.tableName = tableName;
            this.loadConnection();
            this.tableSchema = this.GetTableSchema();
        }
        
      

        internal SqlConnection loadConnection()
        {
            if(this.connection == null)
            {
                this.connection = new SqlConnection(this.ConnectionString);
            }
            else
            {
                this.connection.Close();
            }
            return this.connection;
        }

        private Dictionary<string, string> GetTableSchema()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string query = "select column_name, data_type from information_schema.columns where table_name='" + this.tableName + "'";
            this.connection.Open();
            SqlCommand command = new SqlCommand(query, this.connection);

            List<Dictionary<string, object>> tempResult = BaseSelect(query); //changed object to string for dict
            for (int i = 0; i < tempResult.Count; i++)
            {
                Dictionary<string, object> current = tempResult[i]; //changed object to string for dict
                result.Add(current["column_name"].ToString(), current["data_type"].ToString());
            }
            return result;

        }


        public EntityConnection(string tableName, int defaultlength):this(tableName)
        {
            this.defaultSelectLength = defaultlength;
        }

        //select all
        public List<Dictionary<string, object>> Select() 
        {
            string query = "select * from " + this.tableName + " ";
            return this.BaseSelect(query);
        }
        private List<Dictionary<string, object>> BaseSelect(string query) //changed object to string for dict
        {
            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();

            }
            SqlCommand command = new SqlCommand(query, this.connection);
            SqlDataReader reader = command.ExecuteReader();
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>(); //changed object to string for dict
            while (reader.Read())
            {
                Dictionary<string, object> tempResult = new Dictionary<string, object>(); // changed object to string for dict
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    tempResult.Add(reader.GetName(i), reader.GetValue(i).ToString());
                }
                result.Add(tempResult);
            }
            reader.Close();
            this.connection.Close();
            return result;
        }


        public List<Dictionary<string, object>> Select(int start, int length) //changed object to string for dict
        {
            string query = "select * from " + this.tableName + "  limit " + start + "," + length;
            return this.BaseSelect(query);
        }
        public List<Dictionary<string, object>> Select(int start) //changed object to string for dict
        {
            int length = this.defaultSelectLength;
            string query = "select * from " + this.tableName + "  limit " + start + "," + length;
            return this.BaseSelect(query);
        }

        private static string GetPlaceholder(string[] keys)
        {
            string result = "";
            for (int i = 0; i < keys.Length; i++)
            {
                string currentValue = "@" + keys[i];
                result += string.IsNullOrEmpty(result) ? currentValue : "," + currentValue;
            }
            return result;
        }
        ////add new column to tables
        //public bool AddColumn(string newColumn)
        //{
        //    this.connection.Open();
        //    string query = "alter table " + this.tableName + " add column_name, data_type from information.schema " + newColumn;
        //    SqlCommand command = new SqlCommand(query, this.connection);
            
        //}

        //insert method
        public bool Insert(Dictionary<string, string> content)
        {
            this.connection.Open();
            string[] keys = content.Keys.ToArray<string>();
            string placeholder = GetPlaceholder(keys);
            string query = " insert into " + this.tableName + "(" + implode(keys) + ") values (" + placeholder + ")";

            SqlCommand command = new SqlCommand(query, this.connection);
            for (int i = 0; i < keys.Length; i++)
            {
                string currentParam = "@" + keys[i];
                string currentValue = content[keys[i]].ToString();
                SqlDbType dbType = getColumnType(this.tableSchema[keys[i]]);
                SqlParameter tempParam = new SqlParameter(currentParam, dbType);
                tempParam.Value = wrapValue(currentValue, dbType);
                command.Parameters.Add(tempParam);
            }
            int n = command.ExecuteNonQuery();
            this.connection.Close();
            return n > 0;
            //connection.Open();
            //string query="insert into "+this.tableName

        }

        private object wrapValue(string currentValue, SqlDbType dbType)
        {
            if (dbType == SqlDbType.DateTime)
            {
                DateTime datetime = DateTime.Parse(currentValue);
                return datetime;
            }
            if (dbType == SqlDbType.Time)
            {
                TimeSpan datetime = TimeSpan.Parse(currentValue);
                return datetime;
            }
           
            return currentValue;
        }


        private SqlDbType getColumnType(string v)
        {
            v = v.ToLower();
            switch (v)
            {
                case "int":
                    return SqlDbType.Int;
                case "varchar":
                    return SqlDbType.VarChar;
                case "datetime":
                    return SqlDbType.DateTime;
                case "time":
                    return SqlDbType.Time;
                case "decimal":
                    return SqlDbType.Decimal;
                case "text":
                    return SqlDbType.Text;
                case "nvarchar":
                    return SqlDbType.NVarChar;
                default:
                    return SqlDbType.Udt;
            }
        }

        //to connect two joins into a single string
        private static string implode(string[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                string currentValue = array[i];
                result += string.IsNullOrEmpty(result) ? currentValue : "," + currentValue; //another way of writing if-then-else
            }
            return result;
        }

        //public string ResponseMsg(bool resp)
        //{
        //    if (resp == true)
        //    {
        //        string result = "Record successfully created!";
        //    }
        //    else
        //    {
        //        string result = "Error in saving record!";
        //    }

        //    return resp.ToString();
        //}

        //Select by parameter
        public List<Dictionary<string, object>> SelectByColumn(Dictionary<string, string> queryParam)
        {
            string query = "select * from " + this.tableName + " ";
            string param = "";
            string[] keys = queryParam.Keys.ToArray<string>();
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];
                string valueString = queryParam[currentKey];
                param += string.IsNullOrWhiteSpace(param) ? " where " : " and ";
                param += (currentKey + " = " + valueString);
            }
            return this.BaseSelect(query + param);
        }

        //Update by ID 
        public bool Update(int id, Dictionary<string, string> content)
        {
            this.connection.Open();
            string[] keys = content.Keys.ToArray<string>();
            string placeholder = GetPlaceholder(keys);
            string query = " update  " + this.tableName + " set ";
            SqlCommand command = new SqlCommand();
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];
                query += (i > 0 && keys.Length > 0 ? "," : "") + " " + currentKey + " = @" + currentKey;
                string currentParam = "@" + keys[i];
                string currentValue = content[keys[i]];
                SqlDbType dbType = getColumnType(this.tableSchema[keys[i]]);
                SqlParameter tempParam = new SqlParameter(currentParam, dbType);
                tempParam.Value = wrapValue(currentValue, dbType);
                command.Parameters.Add(tempParam);
            }
            string query2 = " where itbId =@itbId";

            command.CommandText = query + query2;
            SqlParameter parameter = new SqlParameter("@itbId", SqlDbType.Int);
            parameter.Value = id;
            command.Parameters.Add(parameter);
            command.Connection = this.connection;
            int n = command.ExecuteNonQuery();
            this.connection.Close();
            return n > 0;
            //connection.Open();
            //string query="insert into "+this.tableName
        }

        //Delete
        public bool Delete(Dictionary<string, string> queryParam)
        {
            this.connection.Open();
            string query = "delete from " + this.tableName + " ";
            string param = "";
            string[] keys = queryParam.Keys.ToArray<string>();
            for (int i = 0; i < keys.Length; i++)
            {
                string currentKey = keys[i];
                string valueString = queryParam[currentKey];
                param += string.IsNullOrWhiteSpace(param) ? " where " : " and ";
                param += (currentKey + " = " + valueString);
            }
            query += param;
            SqlCommand command = new SqlCommand(query, this.connection);
            int n = command.ExecuteNonQuery();
            this.connection.Close();
            return n > 0;
        }



    }


}
