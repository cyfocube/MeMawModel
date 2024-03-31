
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Linq;
    using System.IO;
    using System.Data.SqlClient;
    using System.Security.Cryptography;
    using System.Reflection.Emit;
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Numerics;
    using System.Security.AccessControl;
    using System.Xml;
    using System.Xml.Linq;

namespace MeMawProject
{
        class Program
        {
            private static string _sqlConnectionString;
            static void Main(string[] args)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("MeMawConfiguration.json");

                var configuration = builder.Build();
                _sqlConnectionString = configuration["ConnectionStrings"];
                var items = File.ReadAllLines("./int_rent_predicted.csv").
                    Skip(1)
                    .Select(line => line.Split(","))
            .Select(i => new int_rent_modelObject
            {
                Place = Convert.ToString(i[0]),
                State = Convert.ToString(i[1]),
                Placeid = Convert.ToString(i[2]),
                Predicted = float.Parse(i[3]),
               


            });

                using (var connection = new SqlConnection(_sqlConnectionString))
                {
                    connection.Open();
                    var insert_inc_rent_predicted = @"INSERT INTO MeMawBestKnowBest.dbo.Inc_Rent_Model VALUES (
                          @Place, @State, @Placeid, @Predicted );";
                          

                    var selectCommand = "SELECT COUNT (*) from MeMawBestKnowBest.dbo.Inc_Rent_Model";
                    var selectSqlCommand = new SqlCommand(selectCommand, connection);
                    var results = (int)selectSqlCommand.ExecuteScalar();
                    if (results > 0)
                    {
                        var deleteCommand = "DELETE FROM  MeMawBestKnowBest.dbo.Inc_Rent_Model";
                        var deleteSqlCommand = new SqlCommand(deleteCommand, connection);
                        deleteSqlCommand.ExecuteNonQuery();

                    }


                    foreach (var item in items)
                    {
                        var command = new SqlCommand(insert_inc_rent_predicted, connection);
                        command.Parameters.AddWithValue("@Place", item.Place);
                        command.Parameters.AddWithValue("@State", item.State);
                        command.Parameters.AddWithValue("@Placeid", item.Placeid);
                        command.Parameters.AddWithValue("@Predicted", item.Predicted);
                        command.ExecuteNonQuery();

                    }

                }
            }
            public static float Parse(string value)
            {
                return float.TryParse(value, out float parsedValue) ? parsedValue : default(float);
            }
        }

}




