using System.Collections.Generic;
using LotAPI.DataAccess.Models;
using LotAPI.Models;

namespace LotAPI.Business.Interface
{
    public interface IPrizeDrawLogic
    {
        List<LotMaster> GetAllLotMaster();
        LotGameHistoryRes GetLotGameHistory(LotGameListReq requst);
        GenerateRewardDataRes CeateNewLotGame(GenerateRewardDataReq requst);
        LotGameListRes LotGameList(LotGameListReq requst);
    }
}
