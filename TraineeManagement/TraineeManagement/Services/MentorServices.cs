using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;
using TraineeManagement.Interfaces;

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
                _logger.LogError(ex, "Error while seeding mentors.");
                throw new Exception($"Error while seeding mentor data.", ex);
            }
        }

        public async Task<PaginationDTO<MentorDTO>> GetAllMentors(int pageNumber = 1, int pageSize = 10, string? search = null, MentorStatus? status = null)
        {
            try
            {
                _logger.LogInformation($"GetAllMentors: Searching for {search} in all mentors.");
                var query = _context.Mentors.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(m =>
                        m.FirstName.ToLower().Contains(search) ||
                        m.LastName.ToLower().Contains(search) ||
                        m.Email.ToLower().Contains(search) ||
                        m.Expertise.Any(e => e.ToLower().Contains(search))
                    );
                }

                // Status filter
                if (status.HasValue)
                {
                    query = query.Where(m => m.Status.Equals(status.Value));
                }

                // Total count before pagination
                var totalRecords = await query.CountAsync();

                // Pagination using Skip and Take
                var mentors = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MentorDTO
                    {
                        // id = m.id,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Email = m.Email,
                        Expertise = m.Expertise,
                        Status = m.Status
                    })
                    .ToListAsync();

                _logger.LogInformation($"GetAllMentors: mentors fetched ({totalRecords}) successfully for search {search}.");
                return new PaginationDTO<MentorDTO>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    Data = mentors
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllMentors: error while fetching mentors for search {search}.");
                throw new Exception($"Error while fetcing mentors.", ex);
            }
        }

        public async Task<Mentor?> GetMentorById(int id)
        {
            try
            {
                _logger.LogInformation($"GetMentorById: fetching mentor by id: {id}.");
                Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == id);
                if(mentor == null)
                {
                    _logger.LogWarning($"GetMentorById: mentor by id {id} not found!");
                    return null;
                }

                _logger.LogInformation($"GetMentorById: mentor with id {mentor.Id} fetched successfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetMentorById: error while fetching mentor with id {id}.");
                throw new Exception($"Error while fetching mentor wih id {id}.", ex);
            }
        }

        public async Task<Mentor> CreateMentor(MentorDTO dto)
        {
            try
            {
                _logger.LogInformation($"CreateMentor: creating mentor.");
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

                _logger.LogInformation($"CreateMentor: mentor with id {mentor.Id} created successfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateMentor: error while creating mentor.");
                throw new Exception($"Error while creating mentor.", ex);
            }
        }

        public async Task<Mentor?> UpdateMentor(int id, MentorDTO dto)
        {
            try
            {
                _logger.LogInformation($"UpdateMentorById: updating mentor by id: {id}.");
                Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == id);
                if (mentor == null) return null;

                mentor.FirstName = dto.FirstName;
                mentor.LastName = dto.LastName;
                mentor.Email = dto.Email;
                mentor.Expertise = dto.Expertise;
                mentor.Status = dto.Status;
                mentor.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"UpdateMentor: mentor with id {mentor.Id} updated succssfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateMentorById: error while updating mentor with id {id}.");
                throw new Exception($"Error updating mentor with id {id}.", ex);
            }
        }

        public async Task<Mentor?> DeleteMentorById(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteMentorById: deleting mentor by id: {id}.");
                Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == id);
                if (mentor == null) return null;

                _context.Mentors.Remove(mentor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"DeleteMentor: mentor with id {mentor.Id} deleted successfully.");
                return mentor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteMentor: error while deleting mentor with id {id}.");
                throw new Exception($"Error deleting mentor with id {id}.", ex);
            }
        }

        public MentorDTO ReturnMentorDTO(Mentor mentor)
        {
            try
            {
                _logger.LogInformation($"ReturnLearningTaskDTO: converting mentor with id {mentor.Id} to DTO");
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
                _logger.LogError(ex, $"ReturnMentorDTO: error while converting mentor with id {mentor.Id} to DTO");
                throw new Exception("Error while converting mentor to DTO.", ex);
            }
        }
    }
}