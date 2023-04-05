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
                    string selectedCustomer = HttpContext.Request.Form["customerList"];
                    cmd.Parameters.Add("customerCode", SqlDbType.VarChar);
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

        public void OnPostGetOrderDetails(string getInvoiceNumber)
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
                    cmd.Parameters.Add("invoiceNumber", SqlDbType.VarChar);
                    cmd.Parameters["invoiceNumber"].Value = HttpContext.Request.Form["btnOrder"].ToString();

                    SqlDataAdapter oda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display all information
                    ViewData["showOrderDetails"] = ds.Tables[0];
                    OnPostGetOrdersForCustomer(selectedInvoice);
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


        // Show capacity in the dropdown list
        public void OnPostUpdateQuantity()
        {
            using (SqlConnection con = new SqlConnection("Data Source=aureliavill9010;Initial Catalog=cs306_villyani;Integrated Security=true;TrustServerCertificate=True;")) //catalog represent inenr part, once connected to server
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project5_UpdateOrderLine";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string selectedInvoice = HttpContext.Request.Form["btnQuantity"].ToString();

                    cmd.Parameters.Add("invoiceNumber", SqlDbType.Int);
                    cmd.Parameters["invoiceNumber"].Value = HttpContext.Request.Form["btnQuantity"].ToString();

                    cmd.Parameters.Add("lineNumber", SqlDbType.Int);
                    string lineNumber = HttpContext.Request.Form["lineNumber"];
                    string[] lineNumbers = lineNumber.Split(',');
                    for (int row = 0; row < lineNumbers.Length; row++)
                    {
                        cmd.Parameters["lineNumber"].Value = int.Parse(lineNumbers[row]);
                    }

                    cmd.Parameters.Add("newQuantity", SqlDbType.Decimal);
                    cmd.Parameters["newQuantity"].Value = decimal.Parse(HttpContext.Request.Form["quantityList"].ToString());
                    
                    OnPostGetOrderDetails(selectedInvoice);
                    cmd.ExecuteNonQuery();
                    
                }
                catch (SqlException ex)
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