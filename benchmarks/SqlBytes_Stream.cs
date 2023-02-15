using System.Data.SqlTypes;
using System.Data;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using UkrGuru.SqlJson;
using BenchmarkDotNet.Disassemblers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.PortableExecutable;
using Microsoft.IdentityModel.Tokens;
using System.IO;

[MemoryDiagnoser]
public class SqlBytes_Stream
{
    [GlobalSetup]
    public static void GlobalSetup()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    #region SqlHelper_SqlBytes_Stream
    private class ProductPhoto
    {
        public string LargePhotoFileName { get; set; }
        public byte[] LargePhoto { get; set; }
    }

    [Benchmark]
    public string SqlJson_SqlBytes_Stream()
    {
        string result = null;

        int ProductPhotoID = 77;

        result = GetStream()?.Length.ToString();

        return result;

        static Stream? GetStream()
        {
            Stream stream = null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT LargePhoto FROM Production.ProductPhoto WHERE ProductPhotoID = 77";

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                return reader.GetSqlBytes(0).Stream;
                            }
                        }
                    }
                }
            }

            return stream;
        }
    }
    #endregion

    #region AdoNet_SqlBytes_Stream
    [Benchmark]
    public string AdoNet_SqlBytes_Stream()
    {
        string result = null;

        int ProductPhotoID = 77; 

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = connection.CreateCommand();
            SqlDataReader reader = null;

            command.CommandText =
                "SELECT LargePhotoFileName, LargePhoto "
                + "FROM Production.ProductPhoto "
                + "WHERE ProductPhotoID=@ProductPhotoID";
            command.CommandType = CommandType.Text;

            SqlParameter paramID = new SqlParameter("@ProductPhotoID", SqlDbType.Int);
            paramID.Value = ProductPhotoID;
            command.Parameters.Add(paramID);
            connection.Open();

            string photoName = null;

            reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    photoName = reader.GetString(0);

                    if (!reader.IsDBNull(1))
                    { 
                        SqlBytes bytes = reader.GetSqlBytes(1);

                        result = bytes.Stream.Length.ToString();

                        //using (var stream = bytes.Stream)
                        //using (var md5 = MD5.Create())
                        //{
                        //    var hash = md5.ComputeHash(stream);
                        //    result = Convert.ToBase64String(hash);
                        //}
                    }
                }
            }

            if (reader != null) reader.Dispose();

            return result;
        }
    }
    #endregion


    static private string ConnectionString => "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=AdventureWorks2019;Integrated Security=SSPI";
}
