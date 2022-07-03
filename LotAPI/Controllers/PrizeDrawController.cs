using System;
using LotAPI.Business.Interface;
using LotAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace LotAPI.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class PrizeDrawController : ControllerBase
    {
        private readonly IPrizeDrawLogic _prizeDrawLogic;

        public PrizeDrawController(IPrizeDrawLogic prizeDrawLogic)
        {
            _prizeDrawLogic = prizeDrawLogic;
        }

        /// <summary>
        /// Get all LotMaster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getAllLotMaster")]
        public IActionResult GetAllLotMaster()
        {
            try
            {
                var response = _prizeDrawLogic.GetAllLotMaster();
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// Get Lot Game History
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/getLotGameHistory")]
        public IActionResult GetLotGameHistory(LotGameListReq requst)
        {
            try
            {
                var response = _prizeDrawLogic.GetLotGameHistory(requst);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// create new lot game 
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ceateNewLotGame")]
        public IActionResult CeateNewLotGame(GenerateRewardDataReq requst)
        {
            try
            {
                var response = _prizeDrawLogic.CeateNewLotGame(requst);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// d
        /// </summary>
        /// <param name="requst"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/lotGameList")]
        public IActionResult LotGameList(LotGameListReq requst)
        {
            try
            {
                var response = _prizeDrawLogic.LotGameList(requst);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
