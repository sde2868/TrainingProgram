using TraineeManagement.Models;

namespace TraineeManagement.Services
{
    public class TraineeServices : ITrainee
    {
        private static List<Trainee> trainees = new List<Trainee>
        {
            new Trainee
            {
                id = 1,
                FirstName = "Zeus",
                LastName = "Learning",
                Email = "zeuslearning@email.com",
                TechStack = new String[] {"C#", "Dotnet"},
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        public List<Trainee> GetAll()
        {
            return trainees;
        }
        public Trainee GetById(int id)
        {
            var trainee = trainees.FirstOrDefault(t => t.id == id);
            return trainee;
        }
        public Trainee Create(TraineeDTO dto)
        {
            var newid = trainees.Any() ? trainees.Max(t => t.id) + 1 : 1;
            
            var trainee = new Trainee
            {
                id = newid,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                TechStack = dto.TechStack,
                Email = dto.Email,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            trainees.Add(trainee);
            return trainee;
        }
        public Trainee Put(int id, TraineeDTO dto)
        {
            var trainee = trainees.FirstOrDefault(t => t.id == id);
            if(trainee == null) return null;
            trainee.FirstName = dto.FirstName;
            trainee.LastName = dto.LastName;
            trainee.Email = dto.Email;
            trainee.Status = dto.Status;
            trainee.TechStack = dto.TechStack;
            trainee.UpdatedAt = DateTime.UtcNow;
            return trainee;
        }
        public Trainee DeleteById(int id)
        {
            var trainee = trainees.FirstOrDefault(t => t.id == id);
            if(trainee == null) return null;
            trainees.Remove(trainee);
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