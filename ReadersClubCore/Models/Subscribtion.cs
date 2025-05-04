using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadersClubCore.Models
{
    //Many to Many Relation between Channel and User
    public class Subscribtion: BaseEntity
    {
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
