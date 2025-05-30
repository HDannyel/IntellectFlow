using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellectFlow.Helpers
{
    public class DisciplineService
    {
        private readonly IntellectFlowDbContext _context;

        public DisciplineService(IntellectFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Discipline> CreateDiscipline(string name, string description)
        {
            var discipline = new Discipline
            {
                Name = name,
                Description = description
            };

            _context.Disciplines.Add(discipline);
            await _context.SaveChangesAsync();

            return discipline;
        }

        public async Task<IEnumerable<Discipline>> GetAllDisciplines()
        {
            return await _context.Disciplines.ToListAsync();
        }
    }
}