using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KBXAdmin.Application.DTOs
{
    public sealed class AutoSaveRequest
    {
        public JsonElement SchemaJson { get; set; } = default!;
    }
    public class SaveFormRequest
    {
        public JsonElement SchemaJson { get; set; }
    }
}
