using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public void OnPostAccessInstructorList()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "project4.get_instructor_list";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                try
                {
                    OracleParameter p_output = new OracleParameter();
                    p_output.OracleDbType = OracleDbType.RefCursor;
                    p_output.ParameterName = "p_output";
                    p_output.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(p_output);

                    OracleDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> instructorList = new List<SelectListItem>();
                    // Get the instructor name and display it with thei id, salutation, first name, and last name
                    while (reader.Read())
                    {
                        string instructorName = reader.GetInt32(0).ToString() + " " +
                                                reader.GetString(1) + " " + 
                                                reader.GetString(2) + " " +
                                                reader.GetString(3);
                        // Populate the existing instructor
                        SelectListItem instructor = new SelectListItem();
                        instructor.Text = instructorName;
                        instructor.Value = reader.GetInt32(0).ToString();
                        //instructor.Selected = true;
                        instructorList.Add(instructor);
                    }
                    ViewData["showInstructorList"] = instructorList;
                    reader.Close();
                }
                catch (OracleException ex)
                {
                    ViewData["showInstructorList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
