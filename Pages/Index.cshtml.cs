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
            OnPostAccessInstructorList();
        }

        // Display the dropdown with the list of instructors
        public void OnPostAccessInstructorList()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project4.get_instructor_list";
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

        // Display the information of course/section based on specific instructor.
        // Then, show the list of students who enrolled on that particular course/section
        public void OnPostGetInstructorInfo()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project4.get_instructor_info";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                try
                {
                    // Get the instructor id
                    OracleParameter getInstructorId = new OracleParameter();
                    getInstructorId.OracleDbType = OracleDbType.Int32;
                    getInstructorId.ParameterName = "p_instructor_id";
                    getInstructorId.Direction = ParameterDirection.Input;
                    getInstructorId.Value = Convert.ToInt32(HttpContext.Request.Form["instructorList"]);
                    cmd.Parameters.Add(getInstructorId);

                    // Get the section number associated with instructor id
                    OracleParameter getSectionId = new OracleParameter();
                    getSectionId.OracleDbType = OracleDbType.Int32;
                    getSectionId.ParameterName = "p_section_no";
                    getSectionId.Direction = ParameterDirection.Input;
                    getSectionId.Value = Convert.ToInt32(HttpContext.Request.Form["instructorList"]);
                    cmd.Parameters.Add(getSectionId);

                    // Get the result set and display each section separately
                    OracleParameter displayTable = new OracleParameter();
                    displayTable.OracleDbType = OracleDbType.RefCursor;
                    displayTable.ParameterName = "p_output";
                    displayTable.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(displayTable);

                    // Execute the stored procedure
                    OracleDataAdapter oda = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display all information
                    ViewData["showInstructorInformation"] = ds.Tables[0];
                    OnPostGetStudentInfo();
                    OnPostAccessInstructorList();
                    OnPostShowCapacity();
                }
                catch (OracleException ex)
                {
                    ViewData["showInstructorInformation"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public void OnPostGetStudentInfo()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project4.get_student_list";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                try
                {
                    // Get the section where the students enrolled in
                    OracleParameter getSectionId = new OracleParameter();
                    getSectionId.OracleDbType = OracleDbType.Int32;
                    getSectionId.ParameterName = "p_section_id";
                    getSectionId.Direction = ParameterDirection.Input;
                    getSectionId.Value = Convert.ToInt32(HttpContext.Request.Form["instructorList"]);
                    cmd.Parameters.Add(getSectionId);

                    // Get the result set and display students enrolled in that section
                    OracleParameter displayStudent = new OracleParameter();
                    displayStudent.OracleDbType = OracleDbType.RefCursor;
                    displayStudent.ParameterName = "p_output";
                    displayStudent.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(displayStudent);

                    // Execute the stored procedure
                    OracleDataAdapter oda = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display student information
                    ViewData["showStudentInformation"] = ds.Tables[0];
                }
                catch (OracleException ex)
                {
                    ViewData["showStudentInformation"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        // Show capacity in the dropdown list
        public void OnPostShowCapacity()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "PROJECT4.get_section_capacity";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                try
                {
                    OracleParameter getSectionId = new OracleParameter();
                    getSectionId.OracleDbType = OracleDbType.Int32;
                    getSectionId.ParameterName = "p_section_id";
                    getSectionId.Direction = ParameterDirection.Input;
                    cmd.Parameters.Add(getSectionId);

                    OracleDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> capacityList = new List<SelectListItem>();
                    while (reader.Read())
                    {
                        SelectListItem capacityNumber = new SelectListItem();
                        capacityNumber.Text = reader.GetString(0);
                        capacityNumber.Value = reader.GetString(0);
                        capacityNumber.Selected = true;
                        capacityList.Add(capacityNumber);
                    }

                    ViewData["capacity_number"] = capacityList;
                }
                catch (OracleException ex)
                {
                    ViewData["capacity_number"] = ex.Message;
                }
                con.Close();
            }
        }
    }
}
