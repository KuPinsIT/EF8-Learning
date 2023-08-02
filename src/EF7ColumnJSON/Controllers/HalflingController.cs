using ConsoleApp1.DataAccess;
using EF7ColumnJSON.Entities;
using EF7JSONColumns.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EF8Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HalflingController: ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private ApplicationDbContext _dbContext;

        public HalflingController(ILogger<UserController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get by level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpGet("level")]
        public async Task<ActionResult> GetLevel(int level)
        {
            var result = await _dbContext.Halflings
                .Where(halfling => halfling.PathFromPatriarch.GetLevel() == level)
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Get parent
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("ancestor")]
        public async Task<ActionResult> GetAncestor(string name)
        {
            var result = await _dbContext.Halflings.SingleOrDefaultAsync(
                    ancestor 
                        => ancestor.PathFromPatriarch 
                        == _dbContext.Halflings.Single(descendent => descendent.Name == name).PathFromPatriarch
                            .GetAncestor(1));

            return Ok(result);
        }

        /// <summary>
        /// Get children
        /// All entry has parent Id == parent of search entry 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("descendent")]
        public async Task<ActionResult> GetDescendent(string name)
        {
            var result = _dbContext.Halflings.Where(
                descendent 
                    => descendent.PathFromPatriarch.GetAncestor(1)
                       == _dbContext.Halflings.Single(ancestor => ancestor.Name == name).PathFromPatriarch);

            return Ok(result);
        }

        /// <summary>
        /// Get all parent
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("all-ancestor")]
        public async Task<ActionResult> FindAllAncestors(string name)
        {
            var result = _dbContext.Halflings.Where(
                    ancestor => _dbContext.Halflings
                        .Single(
                            descendent =>
                                descendent.Name == name
                                && ancestor.Id != descendent.Id)
                        .PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
                .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel());

            return Ok(result);
        }

        /// <summary>
        /// Get all parent
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("all-descendent")]
        public async Task<ActionResult> FindAllDescendents(string name)
        {
            var result = _dbContext.Halflings.Where(
                    descendent => descendent.PathFromPatriarch.IsDescendantOf(
                        _dbContext.Halflings
                            .Single(
                                ancestor =>
                                    ancestor.Name == name
                                    && descendent.Id != ancestor.Id)
                            .PathFromPatriarch))
                .OrderBy(descendent => descendent.PathFromPatriarch.GetLevel()).ToQueryString();

            return Ok();
        }

        /// <summary>
        /// Get common parent
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("common-ancestor")]
        public async Task<ActionResult> FindCommonAncestor(string firstEntryName, string secondEntryName)
        {
            var result = await _dbContext.Halflings
                .Where(
                    ancestor => 
                        _dbContext.Halflings.Single(first => first.Name == firstEntryName).PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch)
                        && _dbContext.Halflings.Single(second => second.Name == secondEntryName).PathFromPatriarch.IsDescendantOf(ancestor.PathFromPatriarch))
                .OrderByDescending(ancestor => ancestor.PathFromPatriarch.GetLevel())
                .FirstOrDefaultAsync();

            return Ok(result);
        }

        [HttpPut("re-parenting")]
        public async Task ReparentSubHierarchy(string oldEntryName, string newEntryName)
        {
            var oldEntry = _dbContext.Halflings.Single(first => first.Name == oldEntryName);
            var newEntry = _dbContext.Halflings.Single(second => second.Name == newEntryName);

            var oldEntryAncestor = oldEntry.PathFromPatriarch.GetAncestor(1);

            var firstAndDescendents = await _dbContext.Halflings.Where(
                    descendent => descendent.PathFromPatriarch.IsDescendantOf(
                        _dbContext.Halflings.Single(ancestor => ancestor.Name == oldEntryName).PathFromPatriarch))
                .ToListAsync();

            foreach (var descendent in firstAndDescendents)
            {
                descendent.PathFromPatriarch
                    = descendent.PathFromPatriarch.GetReparentedValue(
                        oldEntryAncestor, newEntry.PathFromPatriarch)!;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
