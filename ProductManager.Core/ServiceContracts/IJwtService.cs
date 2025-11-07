using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManager.Core.Domain.Entities;
using ProductManager.Core.DTOs.UserDTOs;

namespace ProductManager.Core.ServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse CreateToken(User user);
    }
}
