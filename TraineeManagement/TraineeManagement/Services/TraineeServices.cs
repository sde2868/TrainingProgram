using TraineeManagement.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data;

namespace TraineeManagement.Services
{
    public class TraineeServices : ITrainee
    {
        private readonly AppDbContext _context;
        public TraineeServices(AppDbContext context)
        {
            _context = context;

            // seed data
            if (!_context.Trainees.Any())
            {
                _context.Trainees.Add(new Trainee
                {
                    id = 1,
                    FirstName = "Zeus",
                    LastName = "Learning",
                    Email = "zeuslearning@email.com",
                    TechStack = new string[] { "C#", "Dotnet" },
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                _context.SaveChanges();
            }
        }

        public async Task<List<Trainee>> GetAll(string? search)
        {
            var trainees = await _context.Trainees.ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                trainees = trainees.Where(t =>
                    t.FirstName.ToLower().Contains(search) ||
                    t.LastName.ToLower().Contains(search) ||
                    t.Email.ToLower().Contains(search) ||
                    t.TechStack.Any(ts => ts.ToLower().Contains(search))
                ).ToList();
            }

            return trainees;
        }

        public async Task<Trainee?> GetById(int id)
        {
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);
            return trainee;
        }
        
        public async Task<Trainee?> Create(TraineeDTO dto)
        {
            var trainee = new Trainee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                TechStack = dto.TechStack,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Trainees.AddAsync(trainee);

            _context.SaveChangesAsync();

            return trainee;
        }
        
        public async Task<Trainee?> Put(int id, TraineeDTO dto)
        {
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);

            if (trainee == null)
                return null;

            trainee.FirstName = dto.FirstName;
            trainee.LastName = dto.LastName;
            trainee.Email = dto.Email;
            trainee.Status = dto.Status;
            trainee.TechStack = dto.TechStack;
            trainee.UpdatedAt = DateTime.UtcNow;

            _context.SaveChangesAsync();

            return trainee;
        }

        public async Task<Trainee?> DeleteById(int id)
        {
            var trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.id == id);

            if (trainee == null)
                return null;

            _context.Trainees.Remove(trainee);

            _context.SaveChangesAsync();

            return trainee;
        }

        public TraineeDTO ReturnDTO(Trainee tr)
        {
            TraineeDTO dto = new TraineeDTO();
            dto.FirstName = tr.FirstName;
            dto.LastName = tr.LastName;
            dto.Email = tr.Email;
            dto.TechStack = tr.TechStack;
            dto.Status = tr.Status;
            return dto;
        }
    }
}