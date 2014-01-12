using System;

namespace Ligi.Core.Commands.Contracts
{
    public class AddBookie : ICommand
    {
        public AddBookie()
        {
            Id = Guid.NewGuid();
        }

        public AddBookie(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
        public Guid FixtureId { get; set; }
        public Guid BookieId { get; set; }
    }
}
