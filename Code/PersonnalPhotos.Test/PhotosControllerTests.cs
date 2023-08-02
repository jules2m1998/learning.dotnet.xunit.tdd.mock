using Core.Interfaces;
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

public class PhotosControllerTests
{
    private readonly Mock<IKeyGenerator> _keyGenerator;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly Mock<IPhotoMetaData> _photoMetaData;
    private readonly Mock<IFileStorage> _fileStorage;
    private readonly PhotosController _controller;

    public PhotosControllerTests()
    {
        _keyGenerator = new Mock<IKeyGenerator>();
        _photoMetaData = new Mock<IPhotoMetaData>();
        _fileStorage = new Mock<IFileStorage>();

        var session = Mock.Of<ISession>();
        session.Set("User", Encoding.UTF8.GetBytes("a@b.com"));

        var context = Mock.Of<HttpContext>(x => x.Session == session);
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        _controller = new PhotosController(_keyGenerator.Object, _httpContextAccessor.Object, _photoMetaData.Object, _fileStorage.Object);
    }

    [Fact]
    public async Task Upload_GivenFileName_ReturnsDisplayAction()
    {
        // Arrange
        var formFile = Mock.Of<IFormFile>();
        var model = Mock.Of<PhotoUploadViewModel>(m => m.File == formFile);
        var result = await _controller.Upload(model);

        // Act
        var iaction = Assert.IsAssignableFrom<IActionResult>(result);
        var typed = Assert.IsType<RedirectToActionResult>(iaction);

        // Assert
        Assert.NotNull(typed);
        Assert.Equal("display", typed.ActionName, ignoreCase: true);

    }
}
