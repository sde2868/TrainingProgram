using TraineeManagement.Models;

namespace TraineeManagement.Interfaces
{
    public interface IMentor
    {
        Task<PaginationDTO<MentorDTO>> GetAllMentors(int pageNumber = 1, int pageSize = 10, string? search = null, MentorStatus? status = null);
        Task<Mentor?> GetMentorById(int id);
        Task<Mentor> CreateMentor(MentorDTO dto);
        Task<Mentor?> UpdateMentor(int id, MentorDTO dto);
        Task<Mentor?> DeleteMentorById(int id);
        MentorDTO ReturnMentorDTO(Mentor mentor);
    }
}