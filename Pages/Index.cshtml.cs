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

        public void OnPostAccessPackage(string studentId)
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "STUDENT_REPORTS.get_student_gpa_list";
                cmd.CommandType = CommandType.StoredProcedure;

                // Add input parameter 
                OracleParameter studentIdParam = new OracleParameter("p_student_id", OracleDbType.Varchar2);
                studentIdParam.Direction = ParameterDirection.Input;
                studentIdParam.Value = "123"; // replace with actual student ID 
                cmd.Parameters.Add(studentIdParam);

                // Add output parameter 
                OracleParameter outputParam = new OracleParameter("p_output", OracleDbType.Varchar2);
                outputParam.Direction = ParameterDirection.Output;
                outputParam.Size = 32767;
                cmd.Parameters.Add(outputParam);

                // Execute the stored procedure 
                cmd.ExecuteNonQuery();

                // Get the result from the output parameter 
                string result = outputParam.Value.ToString();

                // Do something with the result, such as display it in a view 
                ViewData["result"] = result;
            }
        }

        public void OnPostGetGpaList(string student_id)
        {
            //string output;
            //STUDENT_REPORTS.get_student_gpa_list(student_id, out output);
            //ViewData["result"] = output;
        }

        public void OnPostGetGradeReport(string student_id, DateTime report_date)
        {
            //string output;
            //STUDENT_REPORTS.get_student_grade_report(student_id, report_date, out output);
            //ViewData["result"] = output;
        }


    }
}
