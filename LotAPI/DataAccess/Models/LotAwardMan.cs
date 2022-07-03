using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LotAPI.DataAccess.Models
{
    public class LotAwardMan
    {
        /// <summary>
        /// Master table id
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public string LotMasterId { get; set; }

        /// <summary>
        /// Winner No
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public int AwardManNumber { get; set; }

        /// <summary>
        /// Award No, corresponding to LOTAWARDLIST.SEQ field
        /// </summary>
        public int LotAwardListSeq { get; set; }
    }
}
