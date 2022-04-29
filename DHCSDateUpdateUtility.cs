using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;

namespace DHCSDateUpdate
{
    public class DHCSDateUpdate 
    {
        public string DBBassConn { get; set; }
        public string DBPatsConn { get; set; }

        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
        
        public void Import()
        {
            LogWriter.LogMessageToFile("Get all CDCRs from import file.");
            //string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=c:\\DHCSDates\\Q42017Results.xlsx;Extended Properties=\"Excel 12.0;HDR=YES;\"";
            string CSVFileConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"text;HDR=Yes;FMT=Delimited\";", Path.GetDirectoryName(FilePath));
           
            try
            {
                DataTable dt = new DataTable();
                using (OleDbConnection con = new OleDbConnection(CSVFileConnectionString))
                {
                    con.Open();
                    var csvQuery = string.Format("select [CDCRNum],[Eligibility Date], [Y/N] from[{0}]", Path.GetFileName(FilePath));
                    using (OleDbDataAdapter da = new OleDbDataAdapter(csvQuery, con))
                    {
                        try {
                            da.Fill(dt);
                        }
                        catch(Exception ex)
                        {
                            LogWriter.LogMessageToFile(ex.Message);
                        }
                    }

                    dt.TableName = "DHCSTable";
                }

                LogWriter.LogMessageToFile("Get data Complated");

                if (UpdateDHCSDates(dt))
                {
                    LogWriter.LogMessageToFile("All CDCR Updated.");
                    return;
                }
                LogWriter.LogMessageToFile("Failed to update CDCRs.");
            }
            catch (Exception ex)
            {
                LogWriter.LogMessageToFile(ex.Message);
            }
        }

        private bool UpdateDHCSDates(DataTable dt)
        {
            try
            {
                //Update DHCSDates only for Medical
                using (SqlConnection cnz = new SqlConnection(DBBassConn))
                {
                    try
                    {
                        cnz.Open();
                        SqlCommand cmdzs = new SqlCommand();
                        SqlParameter parameter = new SqlParameter();
                        cmdzs.Connection = cnz;
                        cmdzs.Parameters.AddWithValue("@DHCStb", dt);
                        cmdzs.CommandText = "spUpdateDHCSDates";
                        cmdzs.CommandType = CommandType.StoredProcedure;
                        cmdzs.CommandTimeout = 300;
                        cmdzs.ExecuteNonQuery();
                        return true;
                    }
                    catch (SqlException err)
                    {
                        LogWriter.LogMessageToFile(err.Message);                      
                    }
                    finally
                    {
                        cnz.Close();                       
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogWriter.LogMessageToFile(ex.Message);
                return false;
            }
            
        }

        ~DHCSDateUpdate() { }
        
    }
}
