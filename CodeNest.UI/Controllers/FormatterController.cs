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
using CodeNest.DAL.Models;
using CodeNest.DTO.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
namespace CodeNest.UI.Controllers
{
    public class FormatterController : Controller
    {
        private readonly IFormatterServices _formatterServices;
        private readonly IWorkspaceService _workspaceService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FormatterController(IFormatterServices formatterServices, IWorkspaceService workspaceService, IHttpContextAccessor httpContextAccessor)
        {
            _formatterServices = formatterServices;
            _workspaceService = workspaceService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Formatter() 
        {
            string? userId = HttpContext.Session.GetString("userId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            WorkspacesDto workSpacesNew = new();
            List<WorkspacesDto> workspaces = await _workspaceService.GetWorkspaces(new ObjectId(userId));
            WorkspacesDto latestWorkspace = workspaces.OrderByDescending(w => w.CreatedOn).FirstOrDefault();

            BlobDto blobdata = await _formatterServices.GetJsonDataLegacyUser(latestWorkspace.Id, new ObjectId(userId));

            FormatterViewDto? result = new()
            {
                Input = blobdata?.Input,
                Output = blobdata?.Output,
                Type= blobdata?.Type,
            };
            CompositeViewModel compositeViewModel = new()
            {
                FormatterView = result,
                Workspaces = workSpacesNew,
                WorkspacesList= workspaces

            };
            return View(compositeViewModel);
        } 

        [HttpPost]
        public async Task<IActionResult>Formatter(BlobDto? jsonDto)
        {
            if (jsonDto == null)
            {
                TempData["Error"] = "Invalid JSON data.";
                return View(new BlobDto()); // Return an empty BlobDto
            }

            ValidationDto result = await _formatterServices.JsonValidate(jsonDto);
            if (result.IsValid)
            {
                TempData["Success"] = result.Message;
                return View(result.Blobs); // Pass the validated BlobDto to the view
            }

            TempData["Error"] = result.Message;
            return View(result.Blobs); // Pass the original BlobDto to the view
        }

        [HttpPost]
        public async Task<IActionResult> SaveJson(BlobDto jsonDto, string? Name, string? Description)
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

            ValidationDto jsonResult = await _formatterServices.Save(jsonDto, new ObjectId(workspaceId), ObjectId.Parse(userId));
            if (jsonResult.IsValid)
            {
                TempData["Success"] = jsonResult.Message;
               return RedirectToAction("Formatter", "Formatter");
            }

            return View("Formatter");
        }
    }
}
