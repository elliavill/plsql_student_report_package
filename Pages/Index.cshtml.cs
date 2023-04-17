using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
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
            OnPostGetManagerList();
            OnPostGetEmployeeList();
            OnPostShowManagerDropdown();
        }

        public void OnPostGetManagerList()
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_villyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project6_getManagerList";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    // Execute the stored procedure
                    MySqlDataAdapter oda = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();

                    oda.Fill(ds);

                    // Show the table of the list of manager
                    ViewData["showManagerList"] = ds.Tables[0];
                }
                catch (MySqlException ex)
                {
                    ViewData["showManagerList"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }


        public void OnPostGetEmployeeList()
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_villyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project6_getAllEmployeeList";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    // Execute the stored procedure
                    MySqlDataAdapter oda = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);

                    // Display all information
                    ViewData["showAllEmployees"] = ds.Tables[0];
                    //OnPostGetManagerList(selectedCustomer);
                }
                catch (OracleException ex)
                {
                    ViewData["showAllEmployees"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public void OnPostShowManagerDropdown()
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_villyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project6_getAllEmployeeList";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    //string selectedManager = HttpContext.Request.Form["managerList"];
                    MySqlDataReader reader = cmd.ExecuteReader();
                    List<SelectListItem> managerList = new List<SelectListItem>();
                    while (reader.Read())
                    {
                        SelectListItem manager = new SelectListItem();
                        manager.Text = reader.GetInt32(0).ToString() + ' ' + reader.GetString(1); // Show the current manager full name
                        manager.Value = reader["CurrentManager"].ToString(); // Get the value of current manager
                        managerList.Add(manager);
                    }
                    ViewData["showManagerDropdown"] = managerList;
                }
                catch (MySqlException ex)
                {
                    ViewData["showManagerDropdown"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public void OnPostChangeManager(string employeeNumber, string managerNumber)
        {
            using (MySqlConnection con = new MySqlConnection("Server=csmysql;database=cs306_villyani;user id=CS306_Villyani;password=pcc138772"))
            {
                con.Open();
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"project6_updateNewManager";
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    cmd.Parameters.Add("@newReportsTo", MySqlDbType.Int32).Value = int.Parse(HttpContext.Request.Form["btnChangeManager"].ToString());
                    cmd.Parameters.Add("@empNum", MySqlDbType.Int32).Value = int.Parse(employeeNumber);
                    
                    cmd.ExecuteNonQuery();
                    OnPostGetManagerList();
                    OnPostGetEmployeeList();
                    OnPostShowManagerDropdown();
                }
                catch (MySqlException ex)
                {
                    ViewData["showUpdatedManager"] = ex.Message;
                }
                finally
                {
                    con.Close();
                }
            }
        }
    }
}