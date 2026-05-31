using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Services;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:guid}/[controller]")]
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
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] TaskStatus? status = null,
        [FromQuery] TaskPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize is < 1 or > 100) pageSize = 10;

        var result = await _taskService.GetTasksAsync(
            projectId, pageNumber, pageSize, status, priority, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetTask(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.GetTaskByIdAsync(projectId, taskId, cancellationToken);
        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaskDto>> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var createRequest = request with { ProjectId = projectId };
        var task = await _taskService.CreateTaskAsync(createRequest, cancellationToken);
        return CreatedAtAction(nameof(GetTask), new { projectId, taskId = task.Id }, task);
    }

    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskDto>> UpdateTask(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateTaskAsync(projectId, taskId, request, cancellationToken);
        return Ok(task);
    }

    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTask(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        await _taskService.DeleteTaskAsync(projectId, taskId, cancellationToken);
        return NoContent();
    }
}
