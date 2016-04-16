// -----------------------------------------------------------------------
// <copyright file="BaseBulkLoad.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Core.Interface;

namespace Services.BulkLoad
{
    public class BaseBulkLoad
    {
        #region Fields
        protected readonly ILogger logger;
        private readonly string[] ColumnNames;
        #endregion

        #region Constructors
        public BaseBulkLoad(ILogger logger, string[] columnNames)
        {
            if( columnNames.Length == 0 )
                throw new ArgumentNullException("columnNames", "Bulk Load must have column names");

            ColumnNames = columnNames;
            this.logger = logger;
        }
        #endregion Constructors

        public DataTable ConfigureDataTable()
        {
            var dt = new DataTable();

            for (int i = 0; i < ColumnNames.Length; i++)
            {
                dt.Columns.Add(new DataColumn());
                dt.Columns[i].ColumnName = ColumnNames[i];
            }
            return dt;
        }

        public bool BulkCopy<T>(DataTable dt, String context)
        {
            string connString = ConfigurationManager.ConnectionStrings[context].ConnectionString;

            string tableName = "Options_" + dt.Rows[0].ItemArray[0].ToString(); // typeof(T).Name;
            bool success = false;

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connString))
            {
                for (int i = 0; i < ColumnNames.Length; i++)
                    bulkCopy.ColumnMappings.Add(i, ColumnNames[i]);

                bulkCopy.BulkCopyTimeout = 600; // in seconds 
                bulkCopy.DestinationTableName = tableName;
                try
                {
                    bulkCopy.WriteToServer(dt);
                    success = true;
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("BaseBulkLoad - BulkCopy<{0}> Bulk load error: {1}", tableName, ex.Message));
                }

                bulkCopy.Close();
            }
            return success;
        }
    }
}