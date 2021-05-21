using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MMORPG.APIs;
using MMORPG.Filters;
using MMORPG.Types;
using MMORPG.Types.Player;

namespace MMORPG.Controllers {

    [ApiController]
    [Route("api/stat")]
    public class StatisticController : ControllerBase {

        private readonly IRepository Repository;

        public StatisticController(IRepository repo) {
            Repository = repo;
        }

        [HttpGet]
        [ExactQueryParam("request")]
        public async Task<long> GetStats([FromQuery]StatsRequest request) {
            return await Repository.GetStats(request);
        }
    }

}