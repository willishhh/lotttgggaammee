using System;
using System.ComponentModel.DataAnnotations;

namespace LotAPI.DataAccess.Models
{
    public class LotMaster
    {
        /// <summary>
        /// Master table id
        /// </summary>
        [Key]
        public string LotMasterId { get; set; }

        /// <summary>
        /// The total number of people that can be drawn
        /// </summary>
        public int TotalManCount { get; set; }

        /// <summary>
        /// Ceate date
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
