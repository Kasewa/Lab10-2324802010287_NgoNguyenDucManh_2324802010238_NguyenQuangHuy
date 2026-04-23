using ASC.Web.Configuration;
using ASC.Web.Controllers;
using ASC.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ASC.Tests
{
    // Naming convention: ControllerName_ActionName_TestCondition_Test
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<IOptions<ApplicationSettings>> _mockOptions;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockOptions = new Mock<IOptions<ApplicationSettings>>();
            _mockOptions.Setup(x => x.Value).Returns(new ApplicationSettings
            {
                ApplicationTitle = "Test Application"
            });
        }

        private HomeController CreateController()
        {
            var controller = new HomeController(_mockLogger.Object, _mockOptions.Object);

            // Inject FakeSession into HttpContext
            var fakeSession = new FakeSession();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = fakeSession;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public void HomeController_Index_ViewResultNotNull_Test()
        {
            var controller = CreateController();
            var result = controller.Index() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public void HomeController_Index_ModelNull_Test()
        {
            var controller = CreateController();
            var result = controller.Index() as ViewResult;
            Assert.Null(result?.ViewData.Model);
        }

        [Fact]
        public void HomeController_Index_NoValidationErrors_Test()
        {
            var controller = CreateController();
            controller.Index();
            Assert.True(controller.ModelState.IsValid);
        }
        [Fact]
        public void HomeController_Index_Session_Test()
        {
            var controller = CreateController();
            controller.Index();
            var sessionData = controller.HttpContext.Session.GetObjectFromJson<ApplicationSettings>("Test");
            Assert.NotNull(sessionData);
        }
    }
}
