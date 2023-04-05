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
            OnPostGetCustomersWithOrders(0);
        }

        public void OnPostGetCustomersWithOrders(int selectedValue)
        {
            using (SqlConnection con = new SqlConnection("Data Source=aureliavill9010;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
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
                        if (customer.Value == selectedValue.ToString())
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
            using (SqlConnection con = new SqlConnection("Data Source=aureliavill9010;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetOrderDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string selectedInvoice = HttpContext.Request.Form["orderList"];
                    if (Int32.TryParse(HttpContext.Request.Form["btnOrder"].ToString(), out int invoiceNumber))
                    {
                        cmd.Parameters.Add("invoiceNumber", SqlDbType.Int);
                        cmd.Parameters["invoiceNumber"].Value = invoiceNumber;

                        // Execute the stored procedure
                        SqlDataAdapter oda = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        oda.Fill(ds);

                        // Display all information
                        ViewData["showOrderDetails"] = ds.Tables[0];
                        OnPostGetOrdersForCustomer(selectedInvoice);
                    }
                    else
                    {
                        ViewData["showOrderDetails"] = "Invalid invoice number";
                    }
                }
                catch (OracleException ex)
                {
                    ViewData["showOrderDetails"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public void OnPostGetOrdersForCustomer(string selectedOrder)
        {
            using (SqlConnection con = new SqlConnection("Data Source=aureliavill9010;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_GetOrdersForCustomer";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    int selectedCustomer = Convert.ToInt32(HttpContext.Request.Form["customerList"]);
                    cmd.Parameters.Add("customerCode", SqlDbType.Int);
                    cmd.Parameters["customerCode"].Value = selectedCustomer;

                    // Execute the stored procedure
                    SqlDataAdapter oda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display all information
                    ViewData["showOrderForCustomer"] = ds.Tables[0];
                    OnPostGetCustomersWithOrders(selectedCustomer);
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

        public void OnPostUpdateQuantity(int quantity, int lineNumber)
        {
            using (SqlConnection con = new SqlConnection("Data Source=aureliavill9010;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;"))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_UpdateOrderLine";
                cmd.CommandType = CommandType.StoredProcedure;

                if (int.TryParse(Request.Form["invoiceNumber"], out int invoiceNumber))
                {
                    int selectedCustomer = Convert.ToInt32(HttpContext.Request.Form["customerList"]);
                    cmd.Parameters.Add("invoiceNumber", SqlDbType.Int);
                    cmd.Parameters["invoiceNumber"].Value = invoiceNumber;

                    cmd.Parameters.Add("lineNumber", SqlDbType.Int);
                    cmd.Parameters["lineNumber"].Value = lineNumber;

                    cmd.Parameters.Add("newQuantity", SqlDbType.Int);
                    cmd.Parameters["newQuantity"].Value = quantity;

                    try
                    {
                        cmd.ExecuteNonQuery();

                        // Reload the order details and customer orders tables
                        OnPostGetCustomersWithOrders(selectedCustomer);
                        OnPostGetOrderDetails();
                        OnPostGetOrdersForCustomer(invoiceNumber.ToString());
                    }
                    catch (SqlException ex)
                    {
                        // Handle the exception
                        ViewData["showOrderDetails"] = ex.Message;
                    }
                }
                else
                {
                    // Handle the case where the "invoiceNumber" value is not a valid integer
                    ViewData["showOrderDetails"] = "Invalid invoice number.";
                }
            }
        }

    }
}