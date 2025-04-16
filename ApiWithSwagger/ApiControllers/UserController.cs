using ApiWithSwagger.Data;
using ApiWithSwagger.Data.Models;
using ApiWithSwagger.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nelibur.ObjectMapper;

namespace ApiWithSwagger.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository _repo;

        public UserController(IRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get User by userId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _repo.FindById(id);
            if (user != null)
            {
                var userToReturn = TinyMapper.Map<UserDto>(user);
                return Ok(userToReturn);
            }

            return Ok(new UserDto());
        }

        /// <summary>
        /// Get list of users, input is an array of user emails
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetListUser([FromBody] List<string> emails)
        {
            var users = await _repo.GetManyUsers(emails);
            //var usersToReturn = users.Select(x => TinyMapper.Map<UserDto>(x)).ToList();
            var usersToReturn = TinyMapper.Map<List<UserDto>>(users);

            return Ok(usersToReturn);
        }

        /// <summary>
        /// Get an user with Deposit items
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetUserWithItems([FromBody]string email)
        {
            var user = await _repo.GetUserWithItems(email);
            if (user is null)
                return NoContent();
            var userToReturn = TinyMapper.Map<UserDto>(user);

            return Ok(userToReturn);
        }

        /// <summary>
        /// Add an User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddUser([FromBody] UserDto input)
        {
            var user = await _repo.GetUser(input.Email);

            if (user != null)
                return Ok("User already exist");

            var userToAdd= TinyMapper.Map<User>(input);
            _repo.Add(userToAdd);
            if(await _repo.SaveAll())
                return Ok();

            return BadRequest("Operation Failed!");
        }

        /// <summary>
        /// Update an User
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto input)
        {
            var user = await _repo.GetUser(input.Email);
            if (user != null)
            {
                TinyMapper.Map(input, user);
                _repo.Update(user);
                if (await _repo.SaveAll())
                    return Ok();
                else
                    return BadRequest("Operation Failed");
            }

            return BadRequest("User Not Found");
        }
    }
}
