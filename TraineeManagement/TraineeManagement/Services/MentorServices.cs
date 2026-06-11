using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

namespace TraineeManagement.Services
{
    public class MentorServices : IMentor
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MentorServices> _logger;
        public MentorServices(AppDbContext context, ILogger<MentorServices> logger)
        {
            try
            {
                _context = context;
                _logger = logger;
                // seed data
                if (!_context.Mentors.Any())
                {
                    _context.Mentors.Add(new Mentor
                    {
                        Id = 1,
                        FirstName = "Zeus",
                        LastName = "Learning",
                        Email = "mentor1@zeuslearning.com",
                        Expertise = ["dotnet", "react"],
                        Status = MentorStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while seeding mentors.");
                throw new Exception($"Error while seeding mentor data.", ex);
            }
        }

        public async Task<List<Mentor>> GetAll(string? search)
        {
            try
            {
                List<Mentor> mentors = await _context.Mentors.ToListAsync();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    mentors = mentors.Where(m =>
                        m.FirstName.ToLower().Contains(search) ||
                        m.LastName.ToLower().Contains(search) ||
                        m.Email.ToLower().Contains(search) ||
                        m.Expertise.Any(e => e.ToLower().Contains(search)) ||
                        m.Status.ToString().ToLower().Contains(search)
                    // u.Role.ToLower().Contains(search)
                    ).ToList();
                }

                _logger.LogInformation($"Mentors fetched ({mentors.Count}) successfully.");
                return mentors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while fetching mentors.");
                throw new Exception($"Error while fetcing mentors.", ex);
            }
        }

        public async Task<Mentor?> GetById(int id)
        {
            try
            {
                Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == id);
                if(mentor == null)
                {
                    _logger.LogWarning($"Mentor not found!");
                    return null;
                }

                _logger.LogInformation($"Mentor with id {mentor.Id} fetched successfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while fetching mentor.");
                throw new Exception($"Error while fetching mentor wih id {id}.", ex);
            }
        }

        public async Task<Mentor> Create(MentorDTO dto)
        {
            try
            {
                Mentor mentor = new Mentor
                {
                    Id = _context.Mentors.ToArray().Length == 0 ? 1 : _context.Mentors.ToArray().Length + 1,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Expertise = dto.Expertise,
                    Status = dto.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Mentors.AddAsync(mentor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Mentor with id {mentor.Id} created successfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while creating mentor.");
                throw new Exception($"Error while creating mentor.", ex);
            }
        }

        public async Task<Mentor?> Put(int id, MentorDTO dto)
        {
            try
            {
                Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == id);
                if (mentor == null) return null;

                mentor.FirstName = dto.FirstName;
                mentor.LastName = dto.LastName;
                mentor.Email = dto.Email;
                mentor.Expertise = dto.Expertise;
                mentor.Status = dto.Status;
                mentor.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Mentor with id {mentor.Id} updated succssfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating mentor.");
                throw new Exception($"Error updating mentor with id {id}.", ex);
            }
        }

        public async Task<Mentor?> DeleteById(int id)
        {
            try
            {
                Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == id);
                if (mentor == null) return null;

                _context.Mentors.Remove(mentor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Mentor with id {mentor.Id} deleted successfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        "Error while deleting mentor.");
                throw new Exception($"Error deleting mentor with id {id}.", ex);
            }
        }

        public MentorDTO ReturnDTO(Mentor mentor)
        {
            try
            {
                MentorDTO dto = new()
                {
                    FirstName = mentor.FirstName,
                    LastName = mentor.LastName,
                    Email = mentor.Email,
                    Status = mentor.Status,
                    Expertise = mentor.Expertise
                };
                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while converting mentor to DTO.", ex);
            }
        }
    }
}