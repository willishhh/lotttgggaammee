using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LotAPI.DataAccess.Models
{
    public class LotAwardList
    {

        /// <summary>
        /// Master table id
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public string LotMasterId { get; set; }

        /// <summary>
        /// Non-significant number
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public int Seq { get; set; }

        /// <summary>
        /// Award name
        /// </summary>
        public string AwardName { get; set; }

        /// <summary>
        /// Number of prizes available for drawing
        /// </summary>
        public int AwardManCount { get; set; }

        /// <summary>
        /// Whether the prize logo is drawn, 1: not drawn, 2: drawn
        /// </summary>
        public string Status { get; set; }
    }
}
