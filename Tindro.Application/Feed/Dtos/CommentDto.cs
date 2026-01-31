using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Feed.Dtos
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } 
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

}
