using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonalPhotos.Controllers;
using PersonalPhotos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnalPhotos.Test;

public class LoginsControllerTest
{
    private readonly LoginsController _controller;
    private readonly Mock<ILogins> _logins;
    private readonly Mock<IHttpContextAccessor> _accessor;
    public LoginsControllerTest()
    {
        _logins = new Mock<ILogins>();
        var session = Mock.Of<ISession>();
        var httpContext = Mock.Of<HttpContext>(x => x.Session == session);

        _accessor = new Mock<IHttpContextAccessor>();
        _accessor.Setup(x => x.HttpContext).Returns(httpContext);


        _controller = new LoginsController(_logins.Object, _accessor.Object);
    }

    [Fact]
    public void Index_GivenNotReturnUrl_ReturnLoginView()
    {
        var result = _controller.Index() as ViewResult;
        Assert.NotNull(result);
        Assert.Equal("login", result.ViewName, ignoreCase: true);
    }

    [Fact]
    public async Task Login_GivenModelStateInvalid_ReturnLoginView()
    {
        _controller.ModelState.AddModelError("Test", "Test");
        var result = await _controller.Login(Mock.Of<LoginViewModel>()) as ViewResult;
        Assert.NotNull(result);
        Assert.Equal("login", result.ViewName, ignoreCase: true);

    }

    [Fact]
    public async Task Login_GivenCorrectPassword_RedirectToDisplayAction()
    {
        const string pwd = "123";
        var modelView = Mock.Of<LoginViewModel>(x => x.Email == "a@b.c" && x.Password == pwd);
        var model = Mock.Of<User>(x => x.Password == pwd);

        _logins.Setup(l => l.GetUser(It.IsAny<string>())).ReturnsAsync(model);

        var result = await _controller.Login(modelView) as RedirectToActionResult;
        Assert.NotNull(result);
        Assert.Equal("Display", result.ActionName, ignoreCase: true);
        Assert.Equal("Photos", result.ControllerName, ignoreCase: true);
    }

    [Fact]
    public async Task Login_GivenInCorrectPassword_ReturnLoginView()
    {
        const string pwd = "123";
        var modelView = Mock.Of<LoginViewModel>(x => x.Email == "a@b.c" && x.Password == pwd);
        var model = Mock.Of<User>(x => x.Password == "tester");

        _logins.Setup(l => l.GetUser(It.IsAny<string>())).ReturnsAsync(model);

        var result = await _controller.Login(modelView) as ViewResult;
        Assert.NotNull(result);
        Assert.Equal("Login", result.ViewName, ignoreCase: true);
    }
}
