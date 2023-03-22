using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Package.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPostAccessPackage()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "STUDENT_REPORTS.get_student_gpa_list";
                cmd.CommandType = CommandType.StoredProcedure;

                // Add output parameter  
                OracleParameter outputParam = new OracleParameter("p_output", OracleDbType.Varchar2);
                outputParam.Direction = ParameterDirection.Output;
                outputParam.Size = 32767;
                cmd.Parameters.Add(outputParam);

                // Execute the stored procedure  
                cmd.ExecuteNonQuery();

                // Get the result from the output parameter  
                string result = outputParam.Value.ToString();
                ViewData["result"] = result;
            }
        }

        public void OnPostAccessGradeReport(string i_student_id)
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "STUDENT_REPORTS.get_student_grade_report";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_student_id", OracleDbType.Int32).Value = i_student_id;
                cmd.Parameters.Add("p_report_date", OracleDbType.Date);
                OracleParameter o_output = new OracleParameter("p_output", OracleDbType.Varchar2);
                o_output.Direction = ParameterDirection.Output;
                o_output.Size = 32767;
                cmd.Parameters.Add(o_output);
            
                cmd.ExecuteNonQuery();
                string result = cmd.Parameters["p_output"].Value.ToString();
                ViewData["showGradeReport"] = result;
            }
        }
    }
}
