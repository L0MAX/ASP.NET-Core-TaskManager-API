using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Services;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<TaskDto>>> GetTasks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] TaskItemStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize is < 1 or > 100) pageSize = 10;

        var result = await _taskService.GetTasksAsync(pageNumber, pageSize, status, priority, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetTask(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);
        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.CreateTaskAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateTaskAsync(id, request, cancellationToken);
        return Ok(task);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        await _taskService.DeleteTaskAsync(id, cancellationToken);
        return NoContent();
    }
}
