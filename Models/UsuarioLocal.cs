using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Postsistem.Models
{
    public class UsuarioLocal
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int LocalId { get; set; }
        public IdentityUser User { get; set; }
        public Local Local { get; set; }


    }
}