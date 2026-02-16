using childspace_backend.Models.DTOs;

namespace childspace_backend.Repositories
{
    public interface ITrialRequestRepository
    {
        Task<IEnumerable<TrialRequestDto>> GetAllAsync();

        Task<TrialRequestDto?> GetByIdAsync(Guid id);

        Task<TrialRequestDto> CreateAsync(TrialRequestCreateDto dto);

        Task<TrialRequestDto?> UpdateAsync(Guid id, TrialRequestUpdateDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}
