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

        // Display the dropdown with the list of student and their current GPA
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
                    ViewData["showStudentGpaList"] = p_output.Value.ToString();
                     
                    // Display the grade report based on selected student
                    var get_id = HttpContext.Request.Form["selectedStudentId"].ToString();
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

        // Display grade report table
        public void OnPostAccessGradeReport(String id) 
        { 
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE")) 
            { 
                con.Open(); 
                OracleCommand cmd = con.CreateCommand(); 
                cmd.CommandText = "STUDENT_REPORTS.get_student_grade_report"; 
                cmd.CommandType = CommandType.StoredProcedure; 
                try 
                {                     
                    OracleParameter displayTable = new OracleParameter();
                    displayTable.OracleDbType = OracleDbType.Varchar2;
                    displayTable.ParameterName = "p_output";
                    displayTable.Direction = ParameterDirection.Output;
                    displayTable.Size = 32767;
                    cmd.Parameters.Add("i_student_id", id);
                    cmd.Parameters.Add(displayTable); 
                    cmd.ExecuteNonQuery();
                    ViewData["showGradeReport"] = displayTable.Value.ToString();
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
