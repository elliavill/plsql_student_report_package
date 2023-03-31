using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
            OnPostGetCustomersWithOrders();
        }

        // Display the dropdown with the list of instructors
        public void OnPostGetCustomersWithOrders()
        {
            using (SqlConnection con = new SqlConnection("Data Source=cssqlserver;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetCustomersWithOrders";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> customerList = new List<SelectListItem>();
                    // Get the instructor name and display it with thei id, salutation, first name, and last name
                    while (reader.Read())
                    {
                        string customerInfo = reader.GetInt32(0).ToString() + " " +
                                              reader.GetString(1) + " " +
                                              reader.GetString(2);
                        SelectListItem customer = new SelectListItem();
                        customer.Text = customerInfo;
                        customer.Value = reader["CUS_CODE"].ToString();
                        // customer.Selected = true
                        customerList.Add(customer);
                    }
                    ViewData["showCustomerList"] = customerList;
                }
                catch (OracleException ex)
                {
                    ViewData["showCustomerList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        // Display the information of course/section based on specific instructor.
        // Then, show the list of students who enrolled on that particular course/section
        public void OnPostGetSectionInfo()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project4.get_section_info";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                try
                {
                    // Get the instructor id and section number
                    int instructorId = Convert.ToInt32(HttpContext.Request.Form["instructorList"]);
                    int sectionNo = Convert.ToInt32(HttpContext.Request.Form["sectionList"]);

                    // Get the instructor id
                    OracleParameter getInstructorId = new OracleParameter();
                    getInstructorId.OracleDbType = OracleDbType.Int32;
                    getInstructorId.ParameterName = "p_instructor_id";
                    getInstructorId.Direction = ParameterDirection.Input;
                    getInstructorId.Value = Convert.ToInt32(HttpContext.Request.Form["instructorList"]);
                    cmd.Parameters.Add(getInstructorId);

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
                    OnPostGetCustomersWithOrders();
                    //OnPostShowCapacity();
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
        public void OnPostUpdateCapacity()
        {
            using (OracleConnection con = new OracleConnection("User ID=cs306_avillyani;Password=StudyDatabaseWithDrSparks;Data Source=CSORACLE"))
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project4.update_section_capacity";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                try
                {
                    cmd.Parameters.Add("  ", HttpContext.Request.Form["updateCapacity"].ToString());
                    cmd.Parameters.Add("p_section_id", HttpContext.Request.Form["btnCapacity"].ToString());
                    cmd.ExecuteNonQuery();
                    OnPostGetSectionInfo();
                    OnPostGetStudentInfo();
                    OnPostGetCustomersWithOrders();
                }
                catch (OracleException ex)
                {
                    ViewData["showCapacityList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}
