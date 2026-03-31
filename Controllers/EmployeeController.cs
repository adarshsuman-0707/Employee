using EmployeeManagement.Data;
using EmployeeManagement.Filters;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmployeeManagement.Controllers
{
    [SessionAuthorizationFilter]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int pageNumber = 1)
        {
            int pageSize = 5; 

            var employees = from e in _context.Employees
                            select e;

            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e =>
                    e.Name.Contains(search) ||
                    e.Email.Contains(search) ||
                    e.Department.Contains(search));
            }

            // total count
            int totalRecords = await employees.CountAsync();

            var pagedData = await employees
                .OrderBy(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(pagedData);
        }
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
       
        public async Task<IActionResult> Create(Employee employee)
         {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();

            }
            else
            {
                Employee employee = new Employee();
                using (var conn = _context.Database.GetDbConnection())
                {
                    conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "sp_GetEmployeeById";
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.CreateParameter();
                        param.ParameterName = "@Id";
                        param.Value = id;
                        cmd.Parameters.Add(param);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                employee.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                employee.Name = reader["Name"].ToString();
                                employee.Email = reader["Email"].ToString();
                                employee.Department = reader["Department"].ToString();
                                employee.Designation = reader["Designation"].ToString();
                                employee.Salary = Convert.ToDecimal(reader["Salary"]);
                                employee.Address = reader["Address"]?.ToString();
                                employee.City = reader["City"]?.ToString();
                            }
                        }

                    }
                }

                if (employee.Id == 0)
                    return NotFound();

                return View(employee);
            }

        }
        [HttpPost]

        public async Task<IActionResult> Edit(Employee model)
        {
            if (!ModelState.IsValid)
                return View(model);
    
            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "sp_UpdateEmployee";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Id", model.Id));
                    cmd.Parameters.Add(new SqlParameter("@Name", model.Name));
                    cmd.Parameters.Add(new SqlParameter("@Email", model.Email));
                    cmd.Parameters.Add(new SqlParameter("@Department", model.Department));
                    cmd.Parameters.Add(new SqlParameter("@Designation", model.Designation));
                    cmd.Parameters.Add(new SqlParameter("@Salary", model.Salary));
                    cmd.Parameters.Add(new SqlParameter("@Address", (object?)model.Address ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@City", (object?)model.City ?? DBNull.Value));

                    await cmd.ExecuteNonQueryAsync();
                }
                
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();

            }
            else
            {
                Employee employee = new Employee();
                using (var conn = _context.Database.GetDbConnection())
                {
                    conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "sp_GetEmployeeById";
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.CreateParameter();
                        param.ParameterName = "@Id";
                        param.Value = id;
                        cmd.Parameters.Add(param);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                employee.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                employee.Name = reader["Name"].ToString();
                                employee.Email = reader["Email"].ToString();
                                employee.Department = reader["Department"].ToString();
                                employee.Designation = reader["Designation"].ToString();
                                employee.Salary = Convert.ToDecimal(reader["Salary"]);
                                employee.Address = reader["Address"]?.ToString();
                                employee.City = reader["City"]?.ToString();
                            }
                        }

                    }
                }

                if (employee.Id == 0)
                    return NotFound();

                return View(employee);
            }

        }


        [HttpPost]
      
        public async Task<IActionResult> Deletes(int id)
        {
            if (id == 0)
            {
                return NotFound();

            }
            else
            {
               
                using (var conn = _context.Database.GetDbConnection())
                {
                    conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "sp_DeleteEmployee";
                        cmd.CommandType = CommandType.StoredProcedure;
                        
                        cmd.Parameters.Add(new SqlParameter("@id",id));
                       int count=await cmd.ExecuteNonQueryAsync();

                        if (count > 0)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    await conn.CloseAsync();
                }
                
            }
            return BadRequest();
        }





    }
}
