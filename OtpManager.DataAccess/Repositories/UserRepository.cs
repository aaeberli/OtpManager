using OtpManager.Common.Abstract;
using OtpManager.DataAccess.Abstract;
using OtpManager.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtpManager.DataAccess.Repositories
{

    public class UserRepository : RepositoryBase<User>, IRepository<User>
    {
        public UserRepository(IDataAccessAdapter dataAccessAdapter)
            : base(dataAccessAdapter)
        {

        }

    }
}

