using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoBoard.Models
{
    public class RegisterationResult
    {
        #nullable enable
        public ErrorInfo? error { get; set; }
        
        public string? device_key { get; set; }
        #nullable disable
    }
}
