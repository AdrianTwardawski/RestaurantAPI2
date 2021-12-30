using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string messagee) : base(messagee)
        {

        }
    }
}
