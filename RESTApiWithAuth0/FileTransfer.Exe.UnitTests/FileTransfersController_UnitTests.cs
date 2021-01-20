using FileTransfer.Exe.Controllers;
using FileTransfer.Exe.Transfer;
using FileTransfer.WebAPI.Definitions;
using FileTransfer.WebAPI.Definitions.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace FileTransfer.Exe.UnitTests
{
    [TestClass]
    public class FileTransfersController_UnitTests
    {
        [TestMethod]
        public void FileTransferCreateRequest_Test_ReturnsProperTransferID()
        {
            // Arrange
            string fileID = "FileID";
            Guid transferGUID = Guid.NewGuid();

            var mockIFileTransferService = new Mock<IFileTransferService>();
            mockIFileTransferService.Setup(m => m.TransferCreate(It.Is<FileTransferCreateDto<string>>(a => a.FileID == fileID)))
                .Returns(transferGUID);
            var controller = new FileTransfersController(null, mockIFileTransferService.Object);
            FileTransferCreateDto<string> request = new FileTransferCreateDto<string>() { FileID = "FileID" };

            // Act
            var response = controller.FileTransferCreateRequest(request) as ObjectResult;
            var value = response.Value as SingleResponse<Guid>;


            // Assert
            mockIFileTransferService.Verify(m => m.TransferCreate(It.Is<FileTransferCreateDto<string>>(a => a.FileID == fileID)), Times.Once);
            Assert.IsFalse(value.HasError);
            Assert.AreEqual(transferGUID, value.Model);
        }
    }
}
