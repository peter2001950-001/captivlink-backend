using Captivlink.Application.Users.Commands.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivlink.ApplicationTests.Users.Commands.Handlers
{
    public class UpdatePersonDetailsCommandHandlerTests
    {
        private readonly UpdatePersonDetailsCommandHandler _handler;

        public UpdatePersonDetailsCommandHandlerTests()
        {
            // implement constructor

        }
        // Implement unit tests will all scenarios for UpdatePersonDetailsCommandHandler
        [TestMethod]
        public void GivenNotFoundUser_ShouldReturnValidationProblem()
        {
        }   

        
    }
}