using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public interface IMentor
    {
        Task<List<Mentor>> GetAll(string? search);
        Task<Mentor?> GetById(int id);
        Task<Mentor> Create(MentorDTO dto);
        Task<Mentor?> Put(int id, MentorDTO dto);
        Task<Mentor?> DeleteById(int id);
        MentorDTO ReturnDTO(Mentor mentor);
    }
}