using OtpManager.Common.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.Common
{
    public class ApplicationRule
    {
        public bool Result { get; private set; }
        public ReasonEnum Reason { get; private set; }

        public static bool operator &(ApplicationRule a, bool b)
        {
            return a.Result && b;
        }
        public static bool operator &(bool a, ApplicationRule b)
        {
            return a && b.Result;
        }
        public static bool operator &(ApplicationRule a, ApplicationRule b)
        {
            return a.Result && b.Result;
        }

        public static bool operator |(ApplicationRule a, bool b)
        {
            return a.Result || b;
        }
        public static bool operator |(bool a, ApplicationRule b)
        {
            return a || b.Result;
        }
        public static bool operator |(ApplicationRule a, ApplicationRule b)
        {
            return a.Result || b.Result;
        }

        public static bool operator !(ApplicationRule a)
        {
            return !a.Result;
        }

        public static implicit operator bool(ApplicationRule a)
        {
            return a.Result;
        }

        public ApplicationRule(IApplicationServiceWithRules<ApplicationRule> service, bool result, ReasonEnum reason)
        {
            Result = result;
            Reason = reason;
            service.AddRule(this);
        }
    }
}
