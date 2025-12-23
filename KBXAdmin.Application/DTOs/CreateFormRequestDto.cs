using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBXAdmin.Application.DTOs
{
    public sealed class CreateFormRequest
    {
        public string Title { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
