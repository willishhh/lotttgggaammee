using System.Collections.Generic;
using LotAPI.DataAccess.Models;

namespace LotAPI.Models
{
    #region 產生資料
    public class GenerateRewardDataReq
    {
        /// <summary>
        /// The total number of people that can be drawn
        /// </summary>
        public int TotalManCount { get; set; }

        /// <summary>
        /// Reward record form
        /// </summary>
        public List<LotAwardData> LotAwardData { get; set; }
    }

    /// <summary>
    /// Reward record form
    /// </summary>
    public class LotAwardData
    {
        /// <summary>
        /// Award name
        /// </summary>
        public string AwardName { get; set; }

        /// <summary>
        /// Number of prizes available for drawing
        /// </summary>
        public int AwardManCount { get; set; }
    }

    public class GenerateRewardDataRes
    {
        /// <summary>
        /// Master table id
        /// </summary>
        public string LotMasterId { get; set; }
    }
    #endregion

    #region 開始抽
    public class LotGameListReq
    {
        /// <summary>
        /// Master table id
        /// </summary>
        public string LotMasterId { get; set; }
    }

    public class LotGameListRes
    {
        /// <summary>
        /// Master table id
        /// </summary>
        public string LotMasterId { get; set; }

        /// <summary>
        /// Name of winner
        /// </summary>
        public string LotAwardName { get; set; }

        /// <summary>
        /// LotAwardMan List
        /// </summary>
        public List<LotAwardMan> LotAwardMan { get; set; }

        /// <summary>
        /// Assign the value after completing the lottery
        /// </summary>
        public string Message { get; set; }
    }

    public class LotGameHistoryRes
    {
        /// <summary>
        /// Master table id
        /// </summary>
        public string LotMasterId { get; set; }

        public List<LotGameHistory> LotGameHistory { get; set; }
    }

    public class LotGameHistory
    {
        /// <summary>
        /// Name of winner
        /// </summary>
        public string LotAwardName { get; set; }

        /// <summary>
        /// LotAwardMan List
        /// </summary>
        public List<LotAwardMan> LotAwardMan { get; set; }
    }
    #endregion
}
