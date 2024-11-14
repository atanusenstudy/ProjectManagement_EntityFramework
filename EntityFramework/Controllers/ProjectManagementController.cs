using CORE.Model;
using DAL.Data;
using DAL.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectManagementController : ControllerBase
    {
        private readonly ProjectManagementDbContext _projectContext;

        public ProjectManagementController(ProjectManagementDbContext project)
        {
            _projectContext = project;
        }
        // ----------- Project Endpoints -----------
        #region

        [HttpGet("projects")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _projectContext.Projects.ToListAsync();
        }

        [HttpGet("projects/{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _projectContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return project;
        }

        [HttpPost("projects")]
        public async Task<ActionResult<Project>> CreateProject(ProjectRequest project)
        {
            var data = new Project()
            {
                Title = project.Title,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Tickets = []
            };
            _projectContext.Projects.Add(data);
            await _projectContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = data.ProjectId }, data);
        }

        [HttpPut("projects/{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            _projectContext.Entry(project).State = EntityState.Modified;

            try
            {
                await _projectContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        [HttpDelete("projects/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _projectContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _projectContext.Projects.Remove(project);
            await _projectContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _projectContext.Projects.Any(e => e.ProjectId == id);
        }

        #endregion


        // ----------- Ticket Endpoints -----------
        #region
        [HttpGet("tickets")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            return await _projectContext.Tickets
                .Include(t => t.Employee)
                .Include(t => t.Project)
                .ToListAsync();
        }

        [HttpGet("tickets/{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            var ticket = await _projectContext.Tickets
                .Include(t => t.Employee)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        [HttpPost("tickets")]
        public async Task<ActionResult<Ticket>> CreateTicket(int employeeId, int projectId, string description)
        {
            var employee = await _projectContext.Employees.FindAsync(employeeId);
            var project = await _projectContext.Projects.FindAsync(projectId);

            if (employee == null || project == null)
            {
                return NotFound("Employee or Project not found");
            }

            var ticket = new Ticket
            {
                Description = description,
                AssignedDate = DateTime.Now,
                EmployeeId = employeeId,
                ProjectId = projectId
            };

            _projectContext.Tickets.Add(ticket);
            await _projectContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.TicketId }, ticket);
        }

        [HttpPut("tickets/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TicketUpdate updateDto)
        {
            var ticket = await _projectContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            // Update only specific fields from the DTO
            ticket.ProjectId = updateDto.ProjectId;
            ticket.EmployeeId = updateDto.EmployeeId;
            ticket.Description = updateDto.Description;
            ticket.AssignedDate = DateTime.Now;

            try
            {
                await _projectContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
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


        [HttpDelete("tickets/{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _projectContext.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _projectContext.Tickets.Remove(ticket);
            await _projectContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("EmployeeTickets/{employeeId}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByEmployee(int employeeId)
        {
            var tickets = await _projectContext.Tickets
                .Where(t => t.EmployeeId == employeeId)
                .Include(t => t.Project)
                .ToListAsync();

            if (!tickets.Any())
            {
                return NotFound("No Tickets found for this employee.");
            }

            return tickets;
        }

        [HttpGet("ProjectTickets/{projectId}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByProject(int projectId)
        {
            var tickets = await _projectContext.Tickets
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Employee)
                .ToListAsync();

            if (!tickets.Any())
            {
                return NotFound("No Tickets found for this project.");
            }

            return tickets;
        }

        private bool TicketExists(int id)
        {
            return _projectContext.Tickets.Any(e => e.TicketId == id);
        }
        #endregion
    }
}
