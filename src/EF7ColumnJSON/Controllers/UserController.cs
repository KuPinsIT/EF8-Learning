using ConsoleApp1.DataAccess;
using EF7ColumnJSON.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EF7JSONColumns.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private ApplicationDbContext _dbContext;

        public UserController(ILogger<UserController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<User>?> Get()
        {
            return await _dbContext.Users
                .ToListAsync();
        }

        [HttpGet("{id}/post")]
        public async Task<Post> GetEnhPosts(int id)
        {
            var query = _dbContext.Users.AsNoTracking()
                .Include(a => a.Posts)
                .ThenInclude(post => post.Comments)
                .Where(u => u.Id == id).Select(p => p.Posts[0]);

            var queryString = query.ToQueryString();


                return await query.SingleOrDefaultAsync();
        }

        [HttpGet("{id}")]
        public async Task<User?> Get(int id)
        {
            return await _dbContext.Users
                .Include(a => a.Posts)
                .ThenInclude(post => post.Comments)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        [HttpGet("City/{city}")]
        public async Task<User?> Get(string city)
        {
            var query = _dbContext.Users
                .Include(a => a.Posts)
                .ThenInclude(post => post.Comments)
                .Where(a => a.Address.City == city);

            var queryString = query.ToQueryString();


            return await query.FirstOrDefaultAsync();
        }

        [HttpPut("{id}")]
        public async Task Update(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            user.Address.City = "HCM";
            user.Address.Street = "Nguyen Thi Minh Khai";

            _dbContext.SaveChanges();
        }

    }
}