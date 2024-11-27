using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;

namespace ReelBuy.Backend.UnitsOfWork.Implementations;

public class DepartmentsUnitOfWork : GenericUnitOfWork<Department>, IDepartmentsUnitOfWork
{
    private readonly IDepartmentsRepository _departmentsRepository;

    public DepartmentsUnitOfWork(IGenericRepository<Department> repository, IDepartmentsRepository departmentsRepository) : base(repository)
    {
        _departmentsRepository = departmentsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<Department>>> GetAsync() => await _departmentsRepository.GetAsync();

    public override async Task<ActionResponse<Department>> GetAsync(int id) => await _departmentsRepository.GetAsync(id);

    public async Task<IEnumerable<Department>> GetComboAsync() => await _departmentsRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Department>>> GetAsync(PaginationDTO pagination) => await _departmentsRepository.GetAsync(pagination);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _departmentsRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Department>> AddAsync(DepartmentDTO departmentDTO) => await _departmentsRepository.AddAsync(departmentDTO);

    public async Task<ActionResponse<Department>> UpdateAsync(DepartmentDTO departmentDTO) => await _departmentsRepository.UpdateAsync(departmentDTO);
}