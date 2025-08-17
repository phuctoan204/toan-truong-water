using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    public class ChatMessage
    {
            [Key]
            public int Id { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsActive { get; set; }
        }
    }
