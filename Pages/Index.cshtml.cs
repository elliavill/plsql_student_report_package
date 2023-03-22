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

        public void OnPostAccessStudentList()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "STUDENT_REPORTS.get_student_gpa_list";
                cmd.CommandType = CommandType.StoredProcedure;

                // Add output parameter  
                OracleParameter p_output = new OracleParameter("p_output", OracleDbType.Varchar2);
                p_output.Direction = ParameterDirection.Output;
                p_output.Size = 32767;
                cmd.Parameters.Add(p_output);
                cmd.ExecuteNonQuery();

                // Get the result from the output parameter  
                string result = p_output.Value.ToString();
                ViewData["result"] = result;
            }
        }

        public void OnGetAccessGradeReport(int selectedStudentId)
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "STUDENT_REPORTS.get_student_grade_report";
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter i_student_id = new OracleParameter("i_student_id", OracleDbType.Int32);
                i_student_id.Value = selectedStudentId;
                cmd.Parameters.Add(i_student_id);

                OracleParameter p_report_date = new OracleParameter("p_report_date", OracleDbType.Date);
                p_report_date.Value = DateTime.Today;
                cmd.Parameters.Add(p_report_date);

                OracleParameter p_output = new OracleParameter("p_output", OracleDbType.Varchar2);
                p_output.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p_output);
                cmd.ExecuteNonQuery();

                // Get the table HTML from the output parameter and store it in a ViewData dictionary
                string tableHtml = p_output.Value.ToString();
                ViewData["tableHtml"] = tableHtml;
            }
        }
    }
}
