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
            OnPostGetCustomersWithOrders(null);
        }

        public void OnPostGetCustomersWithOrders(string selectedValue)
        {
            using (SqlConnection con = new SqlConnection("Data Source=AURELIAVILL9010;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetCustomersWithOrders";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> customerList = new List<SelectListItem>();
                    while (reader.Read())
                    {
                        string customerInfo = reader.GetInt32(0).ToString() + " " +
                                              reader.GetString(1) + " " +
                                              reader.GetString(2);
                        SelectListItem customer = new SelectListItem();
                        customer.Text = customerInfo;
                        customer.Value = reader["CUS_CODE"].ToString();
                        if (customer.Value == selectedValue)
                        {
                            customer.Selected = true;
                        }
                        customerList.Add(customer);
                    }
                    ViewData["showCustomerList"] = customerList;
                }
                catch (SqlException ex)
                {
                    ViewData["showCustomerList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public void OnPostGetOrderDetails()
        {
            using (SqlConnection con = new SqlConnection("Data Source=AURELIAVILL9010;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetOrderDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string selectedValue = HttpContext.Request.Form["customerList"];
                    cmd.Parameters.Add("invoiceNumber", SqlDbType.Int);
                    cmd.Parameters["invoiceNumber"].Value = 1001; // INV_NUMBER

                    // Execute the stored procedure
                    SqlDataAdapter oda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display all information
                    ViewData["showInstructorInformation"] = ds.Tables[0];
                    OnPostGetCustomersWithOrders(selectedValue);
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

        public void OnPostGetOrdersForCustomer()
        {
            using (SqlConnection con = new SqlConnection("Data Source=AURELIAVILL9010;Initial Catalog=cs306_villyani;Integrated Security=true; TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetOrdersForCustomer";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string selectedValue = HttpContext.Request.Form["customerList"];
                    cmd.Parameters.Add("customerCode", SqlDbType.Int);
                    cmd.Parameters["customerCode"].Value = selectedValue;

                    // Execute the stored procedure
                    //SqlDataAdapter oda = new SqlDataAdapter(cmd);
                    //DataSet ds = new DataSet();
                    //oda.Fill(ds);

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> orderList = new List<SelectListItem>();
                    while (reader.Read())
                    {
                        string customerInfo = reader.GetInt32(0).ToString() + " " +
                                              reader.GetInt32(1).ToString() + " " +
                                              reader.GetDateTime(2).ToString();
                        SelectListItem order = new SelectListItem();
                        order.Text = customerInfo;
                        order.Value = reader["CUS_CODE"].ToString();
                        if (order.Value == selectedValue)
                        {
                            order.Selected = true;
                        }
                        orderList.Add(order);
                    }
                    //ViewData["showCustomerList"] = customerList;
                    ViewData["showOrderForCustomer"] = orderList;
                    OnPostGetCustomersWithOrders(selectedValue);

                }
                catch (OracleException ex)
                {
                    ViewData["showOrderForCustomer"] = ex.Message;
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
                    
                    //OnPostGetCustomersWithOrders();
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
