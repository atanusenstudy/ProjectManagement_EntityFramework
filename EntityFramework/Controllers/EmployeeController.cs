using CORE.Model;
using DAL.Data;
using DAL.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ProjectManagementDbContext _projectContext;

        public EmployeeController(ProjectManagementDbContext project)
        {
            _projectContext = project;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _projectContext.Employees.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _projectContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return employee;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(EmployeeRequest employee)
        {
            var data = new Employee()
            {
                Name = employee.Name,
                Position = employee.Position,
                Tickets = []
            };
            _projectContext.Employees.Add(data);
            await _projectContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = data.EmployeeId }, data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            _projectContext.Entry(employee).State = EntityState.Modified;

            try
            {
                await _projectContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _projectContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _projectContext.Employees.Remove(employee);
            await _projectContext.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _projectContext.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
