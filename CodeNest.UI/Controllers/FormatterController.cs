// ***********************************************************************************************
//
//  (c) Copyright 2023, Computer Task Group, Inc. (CTG)
//
//  This software is licensed under a commercial license agreement. For the full copyright and
//  license information, please contact CTG for more information.
//
//  Description: Sample Description.
//
// ***********************************************************************************************

using CodeNest.BLL.Service;
using CodeNest.DTO.Models;
using CodeNest.UI.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
namespace CodeNest.UI.Controllers
{
    public class FormatterController : Controller
    {
        private readonly IFormatterServices _formatterServices;
        private readonly IWorkspaceService _workspaceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJsonService _jsonService;

        public FormatterController(IFormatterServices formatterServices, IWorkspaceService workspaceService, IHttpContextAccessor httpContextAccessor, IJsonService jsonService)
        {
            _formatterServices = formatterServices;
            _workspaceService = workspaceService;
            _httpContextAccessor = httpContextAccessor;
            _jsonService = jsonService;
        }

        public async Task<IActionResult> Formatter(ObjectId userId, ObjectId? workSpaceId)
        {
            List<WorkspacesDto> workspaces = await _workspaceService.GetWorkspaces(userId);
            ObjectId workspaceObjectId = workSpaceId == null || workSpaceId == ObjectId.Empty
                ? workspaces[0].Id : workSpaceId.Value;

            List<BlobDto> blobsList = await _jsonService.GetJson(workspaceObjectId);

            UserWorkspaceFilesDto userWorkspace = new()
            {
                UserId = userId,
                WorkspaceId = workspaceObjectId,
                Workspaces = workspaces,
                BlobsList = blobsList
            };

            FormatterViewModel viewModel = new()
            {
                UserWorkspaceFiles = userWorkspace,
                Blob = new BlobDto() // Initialize with an empty BlobDto or any default value
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> JsonFormatter([FromBody] BlobDto blobDto)
        {
            if (blobDto == null)
            {
                return Json(new { success = false, message = "Invalid JSON data." });
            }

            ValidationDto result = await _formatterServices.JsonValidate(blobDto);
            if (result.IsValid)
            {
                return Json(new { success = true, message = result.Message, output = result.Blobs.Output });
            }

            return Json(new { success = false, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> SaveJson(UserWorkspaceFilesDto userWorkspaceDetail, string? Name, string? Description)
        {
            string? userId = HttpContext.Session.GetString("userId");
            string? workspaceId = HttpContext.Session.GetString("workspaceId");

            if (workspaceId == null)
            {
                WorkspacesDto workspace = new()
                {
                    Name = Name,
                    Description = Description,
                };

                WorkspacesDto result = await _workspaceService.CreateWorkspace(workspace, new ObjectId(userId));
                if (result != null)
                {
                    string workSpaceId = result.Id.ToString();
                    if (_httpContextAccessor.HttpContext != null)
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("workspaceId", workSpaceId);
                    }

                    workspaceId = HttpContext.Session.GetString("workspaceId");
                }
            }

            ValidationDto jsonResult = await _formatterServices.Save(userWorkspaceDetail.Blob, new ObjectId(workspaceId), ObjectId.Parse(userId));
            if (jsonResult.IsValid)
            {
                TempData["Success"] = jsonResult.Message;
                return RedirectToAction("Formatter", "Formatter");
            }

            return View("Formatter");
        }
    }
}
