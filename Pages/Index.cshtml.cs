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
                try
                {
                    OracleParameter p_output = new OracleParameter();
                    p_output.OracleDbType = OracleDbType.Varchar2;
                    p_output.Size = 32767; 
                    p_output.ParameterName = "p_output";
                    p_output.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(p_output);
                    cmd.ExecuteNonQuery();
                    
                    // Get the result from the output parameter  
                    string result = p_output.Value.ToString();
                    ViewData["showStudentGpaList"] = result;
                    var get_id = HttpContext.Request.Form["selectedStudentId"];
                    OnPostAccessGradeReport(get_id);
                } 
                catch (OracleException ex)
                {
                    ViewData["showStudentGpaList"] = ex.Message;
                } 
                finally
                {
                    con.Close();
                }
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
                try
                {
                    cmd.Parameters.Add("i_student_id", OracleDbType.Int32).Value = 123;                    
                    //cmd.Parameters.Add("i_student_id", i_student_id);
                  
                    OracleParameter p_output = new OracleParameter();
                    p_output.ParameterName = "p_output";
                    p_output.OracleDbType = OracleDbType.Varchar2;
                    p_output.Direction = ParameterDirection.Output;
                    p_output.Size = 32767;
                    cmd.Parameters.Add(p_output);

                    cmd.ExecuteNonQuery();
                    string result = cmd.Parameters["p_output"].Value.ToString();
                    ViewData["showGradeReport"] = result;
                }
                catch (OracleException ex)
                {
                    ViewData["showGradeReport"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
