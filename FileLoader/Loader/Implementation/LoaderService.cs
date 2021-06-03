using FileLoader.Loader.Interface;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLoader.Loader.Implementation
{
    class LoaderService : ILoaderService
    {
        public void ProcessFile()
        {
            var dt = ReadCsvFile();
            Savedata(dt);
        }

        private DataTable ReadCsvFile()
        {

            DataTable dtCsv = new DataTable();
            string Fulltext;
            if (true)
            {
                string FileSaveWithPath = @"C:\\temp\Book1.csv";// Server.MapPath("\\Files\\Import" + System.DateTime.Now.ToString("ddMMyyyy_hhmmss") + ".csv");
               
                using (StreamReader sr = new StreamReader(FileSaveWithPath))
                {
                    while (!sr.EndOfStream)
                    {
                        Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                        string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                        for (int i = 0; i < rows.Count() - 1; i++)
                        {
                            string[] rowValues = rows[i].Split(','); //split each row with comma to get individual values  
                            {
                                if (i == 0)
                                {
                                    for (int j = 0; j < rowValues.Count(); j++)
                                    {
                                        dtCsv.Columns.Add(rowValues[j]); //add headers  
                                    }     

                                }
                                else
                                {
                                    DataRow dr = dtCsv.NewRow();
                                    for (int k = 0; k < rowValues.Count(); k++)
                                    {
                                        dr[k] = rowValues[k].ToString();
                                    }
                                    dtCsv.Rows.Add(dr); //add other rows  
                                }
                            }
                        }
                    }
                }
            }
            return dtCsv;
        }

        private void Savedata(DataTable dt)
        {
            string connectionString = GetConnectionString();
            // Open a connection to the AdventureWorks database.
            using (SqlConnection connection =
                       new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName =
                        "dbo.data";

                    try
                    {
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(dt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private string GetConnectionString()
        {
            //ConnectionStringSettingsCollection settings =
            // ConfigurationManager.ConnectionStrings;

            //if (settings != null)
            //{
            //    foreach (ConnectionStringSettings cs in settings)
            //    {
            //        Console.WriteLine(cs.Name);
            //        Console.WriteLine(cs.ProviderName);
            //        Console.WriteLine(cs.ConnectionString);
            //    }
            //}
            var connectionString = "data source=DESKTOP-BSAP295;initial catalog=salesdb;integrated security=True;";
            return connectionString;
        }
    }
}
