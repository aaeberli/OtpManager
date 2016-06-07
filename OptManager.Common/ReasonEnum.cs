using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.Common
{
    public enum ReasonEnum
    {
        Generic,
        Error,
        ElementValidation,
        ElementDuplication,
        ElementPresence,
        UserIdLength,
    }
}
