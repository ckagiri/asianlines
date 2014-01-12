using System;
using System.Collections.Generic;

namespace Ligi.Core.DataAccess
{
    public interface IFixtureBookiesDao
    {
        List<Guid> GetBookies(Guid fixtureId);
    }
}
