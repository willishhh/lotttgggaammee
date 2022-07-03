using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LotAPI.Business.Interface;
using LotAPI.DataAccess.IsDbContext;
using LotAPI.DataAccess.Models;
using LotAPI.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace LotAPI.Business
{
    public class PrizeDrawLogic : IPrizeDrawLogic
    {
        private readonly BasicDbContext _dbConn;
        private readonly string _connStr; 

        public PrizeDrawLogic(BasicDbContext basicDbContext, IConfiguration configuration)
        {
            _dbConn = basicDbContext;
            _connStr = configuration["ConnectionStrings:OracleConnection"];
        }

        public List<LotMaster> GetAllLotMaster()
        {
            return _dbConn.LotMaster.ToList();
        }

        public LotGameHistoryRes GetLotGameHistory(LotGameListReq requst)
        {
            string lotMasterId = requst.LotMasterId;
            var lotGameHistories = new List<LotGameHistory>();
            var lotGameHistory = new LotGameHistory();

            var lotAwardLists = _dbConn.LotAwardList.Where(x => x.LotMasterId == lotMasterId && x.Status == "2").ToList();

            foreach(var item in lotAwardLists)
            {
                var lotAwardMan = _dbConn.LotAwardMan.Where(x => x.LotMasterId == lotMasterId && x.LotAwardListSeq == item.Seq).ToList();

                lotGameHistory = new LotGameHistory()
                {
                    LotAwardName = item.AwardName,
                    LotAwardMan = lotAwardMan
                };
                lotGameHistories.Add(lotGameHistory);
            }

            var response = new LotGameHistoryRes()
            {
                LotMasterId = lotMasterId,
                LotGameHistory = lotGameHistories
            };

            return response;
        }

        public GenerateRewardDataRes CeateNewLotGame(GenerateRewardDataReq requst)
        {
            using (var tran = _dbConn.Database.BeginTransaction())
            {
                try
                {
                    int totalManCount = requst.TotalManCount;
                    var lotAwardData = requst.LotAwardData;
                    string lotMasterId = "G" + DateTime.Now.ToString("yyyyMMddHHmmssff");
                    var response = new GenerateRewardDataRes()
                    {
                        LotMasterId = lotMasterId
                    };
                    var lotAwardLists = new List<LotAwardList>();
                    int seq = 1;

                    #region 產生主表
                    _dbConn.LotMaster.Add(new LotMaster()
                    {
                        LotMasterId = lotMasterId,
                        TotalManCount = totalManCount,
                        CreateDate = DateTime.Now
                    });
                    _dbConn.SaveChanges();
                    #endregion

                    #region 產生獎項紀錄
                    foreach (var lad in lotAwardData)
                    {
                        lotAwardLists.Add(new LotAwardList()
                        {
                            LotMasterId = lotMasterId,
                            Seq = seq,
                            AwardName = lad.AwardName,
                            AwardManCount = lad.AwardManCount,
                            Status = "1"
                        });

                        seq++;
                    }
                    _dbConn.LotAwardList.AddRange(lotAwardLists);
                    _dbConn.SaveChanges();
                    #endregion

                    tran.Commit();
                    return response;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
        }

        public LotGameListRes LotGameList(LotGameListReq requst)
        {
            var conn = new OracleConnection(_connStr);
            conn.Open();
            using (var bulkCopy = new OracleBulkCopy(_connStr, OracleBulkCopyOptions.UseInternalTransaction))
            {
                try
                {
                    bulkCopy.BatchSize = 100000;
                    bulkCopy.BulkCopyTimeout = 260;
                    bulkCopy.DestinationTableName = "LOTAWARDMAN";    //使用oraclebulk要指定表

                    string lotMasterId = requst.LotMasterId;
                    int num = 1; 
                    var nums = new List<int>();
                    var lotAwardManList = new List<LotAwardMan>();
                    var response = new LotGameListRes();

                    //尚未抽出的
                    var awardsNotYetDrawn = _dbConn.LotAwardList.Where(x => x.Status == "1" && x.LotMasterId == lotMasterId).OrderBy(t => t.Seq).FirstOrDefault();

                    bulkCopy.BatchSize = awardsNotYetDrawn.AwardManCount;   

                    //全抽完了
                    if (awardsNotYetDrawn == null)
                    {
                        response.LotMasterId = lotMasterId;
                        response.Message = "DONE";
                        return response;
                    }

                    //需要抽的人數
                    var lotMaster = _dbConn.LotMaster.FirstOrDefault(x => x.LotMasterId == lotMasterId);

                    //已抽出人
                    var awardManNumberList = _dbConn.LotAwardMan.Where(x => x.LotMasterId == lotMasterId).Select(t => t.AwardManNumber).ToList();

                    //篩選出可抽人數
                    for (int i = 0; i < awardsNotYetDrawn.AwardManCount; i++)
                    {
                        num = new Random().Next(1, lotMaster.TotalManCount+1); //產生編號
                        if (awardManNumberList.Contains(num) || nums.Contains(num))
                        {
                            i--;
                            continue;
                        }
                        nums.Add(num);

                      
                        lotAwardManList.Add(new LotAwardMan()
                        {
                            LotMasterId = lotMasterId,
                            LotAwardListSeq = awardsNotYetDrawn.Seq, 
                            AwardManNumber = num
                        });
                    }

                    //解析成DataTable
                    var lotData = ListToDataTable(lotAwardManList);

                    //新增資料至LOTAWARDMAN
                    bulkCopy.WriteToServer(lotData);

                    //更新已抽出狀態
                    awardsNotYetDrawn.Status = "2";
                    _dbConn.SaveChanges();

                    response.LotMasterId = lotMasterId;
                    response.LotAwardName = awardsNotYetDrawn.AwardName;
                    response.LotAwardMan = lotAwardManList;
                    response.Message = "OK";

                    conn.Close();
                    return response;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 將list轉換成DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(IList list)
        {
            var result = new System.Data.DataTable();
            if (list.Count > 0)
            {
                var propertys = list[0].GetType().GetProperties();
                foreach (var pi in propertys)
                {
                    var colType = pi.PropertyType;
                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    result.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
}
