using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class PostRedis
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int LikesPost { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
