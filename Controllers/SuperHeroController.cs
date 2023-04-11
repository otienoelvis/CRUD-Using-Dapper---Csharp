using Dapper;
using JasperTut.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace JasperTut.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SuperHeroController(IConfiguration  config) 
        { 
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHero()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var heroes = await SelectAllHeroes(connection);

            return Ok(heroes);
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetSuperHero(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstOrDefaultAsync<SuperHero>("select * from superheroes where id = @Id",
                new {Id = id});

            return Ok(hero);
        }
        
        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(
                "insert into superheroes(firstname, lastname, place) " +
                "values (@Firstname, @Lastname, @Place)", hero);
            return Ok(await SelectAllHeroes(connection));
        }
        
        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(
                "update superheroes set firstname=@FirstName, lastname=@LastName, place=@Place where id = @Id", hero);
            return Ok(await SelectAllHeroes(connection));
        }
        
        [HttpDelete]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync(
                "delete from superheroes where id = @Id", new {Id = heroId});
            return Ok(await SelectAllHeroes(connection));
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            var heroes = await connection.QueryAsync<SuperHero>("select * from superheroes");
            return heroes;
        }
    }
}
