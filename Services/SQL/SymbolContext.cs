using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Services.SQL
{
    public static class SymbolContext
    {
        private static string connString = ConfigurationManager.ConnectionStrings["OptionContext"].ConnectionString;

        public static List<String> GetSymbolList()
        {
            List<String> symbols = new List<string>();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT Symbol FROM SymbolDetails WHERE HasOptions = 1 ORDER BY Symbol", connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            symbols.Add(reader["Symbol"].ToString());
                        }
                    } // reader closed and disposed up here

                } // command disposed here

            } //connection closed and disposed here

            return symbols;
        }
    }
}
