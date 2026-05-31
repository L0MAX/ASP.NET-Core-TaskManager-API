using AutoMapper;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskDto>();
    }
}
