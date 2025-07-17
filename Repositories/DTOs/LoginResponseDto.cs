using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViroCureBLL.DTOs
{
    public class LoginResponseDto
    {
        public string Message { get; set; }
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}
