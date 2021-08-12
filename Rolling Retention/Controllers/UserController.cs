using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rolling_Retention.Models;

namespace Rolling_Retention.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ContextDb _contextDb;

        public UserController(ContextDb contextDb)
        {
            _contextDb = contextDb;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _contextDb.Users.ToArrayAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            var result = _contextDb.Users.Add(new User()
            {
                DateRegistration = user.DateRegistration.Date,
                DateLastActivity = user.DateLastActivity.Date
            });
            await _contextDb.SaveChangesAsync();
            return Ok(result.Entity);
        }

        [HttpGet("GetRetention")]
        public async Task<IActionResult> GetRetention()
        {
            var startDay = new DateTime(2021, 08, 11);

            var RowData = _contextDb.Users
                .Where(x => x.DateRegistration == startDay)
                .Where(x => x.DateLastActivity <= startDay.AddDays(7))
                .GroupBy(x => x.DateLastActivity)
                .OrderBy(x => x.Key)
                .Select(x => new
                {
                    Date = x.Key,
                    Count = x.Count()
                })
                .Take(7);

            int totalUsers = _contextDb.Users.Where(x => x.DateRegistration == startDay).Count();

            Dictionary<DateTime, decimal> result = new Dictionary<DateTime, decimal>();

            int counter = 0;
            foreach (var item in RowData)
            {
                if (item.Date == startDay)
                {
                    result[item.Date] = 100;
                }
                else
                {
                    var summ = ((totalUsers - counter) / (totalUsers * 1m)) * 100m;
                    result[item.Date] = summ;
                }

                counter += item.Count;
            }

            decimal uncountedUsers = totalUsers - result[startDay];
            DateTime previousDay = startDay;
            for (int i = 7; i > 0; i--)
            {
                var day = startDay.AddDays(i);
                if (result.ContainsKey(day))
                {
                    previousDay = day;
                }
                else
                {
                    result.Add(day, result[previousDay]);
                }
            }

            return Ok(result.OrderBy(x => x.Key));
        }
    }
}