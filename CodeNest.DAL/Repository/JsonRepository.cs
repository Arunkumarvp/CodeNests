﻿// ***********************************************************************************************
//
//  (c) Copyright 2023, Computer Task Group, Inc. (CTG)
//
//  This software is licensed under a commercial license agreement. For the full copyright and
//  license information, please contact CTG for more information.
//
//  Description: Sample Description.
//
// ***********************************************************************************************

using AutoMapper;
using CodeNest.DAL.Context;
using CodeNest.DAL.Models;
using CodeNest.DTO.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeNest.DAL.Repository
{
    public class JsonRepository : IJsonRepository
    {
        private readonly MongoDbService _mongoDbService;
        private readonly IMapper _mapper;
        private readonly ILogger<JsonRepository> _logger;

        public JsonRepository(MongoDbService mongoDbService, IMapper mapper, ILogger<JsonRepository> logger)
        {
            _mongoDbService = mongoDbService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the list of JSON data for a given workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace identifier.</param>
        /// <returns>A list of JSON data.</returns>
        public async Task<List<BlobDto>> GetJsonList(ObjectId workspaceId)
        {
            _logger.LogInformation("GetJsonList: Retrieving JSON list for workspace.");

            try
            {
                List<BlobData> jsonData = await _mongoDbService.BlobDatas
                    .Find(x => x.Workspaces == workspaceId).ToListAsync();
                _logger.LogInformation("GetJsonList: Successfully retrieved JSON list.");
                return _mapper.Map<List<BlobDto>>(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetJsonList: An error occurred while retrieving JSON list.");
                throw;
            }
        }

        /// <summary>
        /// Saves the provided JSON data.
        /// </summary>
        /// <param name="jsonData">The JSON data to save.</param>
        /// <param name="workSpace">The workspace identifier.</param>
        /// <param name="user">The user identifier.</param>
        /// <returns>A boolean indicating whether the save operation was successful.</returns>
        public async Task<bool> SaveAsync(BlobDto jsonData, ObjectId workSpace, ObjectId user)
        {
            _logger.LogInformation("SaveAsync: Starting save operation.");

            try
            {
                Workspaces workspaceName = await _mongoDbService.WorkSpaces
                    .Find(x => x.Id == workSpace).FirstOrDefaultAsync();

                if (workspaceName == null)
                {
                    _logger.LogWarning("SaveAsync: Workspace not found.");
                    return false;
                }

                BlobData jsonUser = new()
                {
                    Name = workspaceName.Name,
                    Input = jsonData.Input,
                    Output = jsonData.Output,
                    Type = "Json",
                    Workspaces = workSpace,
                    CreatedBy = user,
                    CreatedOn = DateTime.UtcNow
                };

                await _mongoDbService.BlobDatas.InsertOneAsync(jsonUser);
                _logger.LogInformation("SaveAsync: Successfully saved JSON data.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveAsync: An error occurred while saving JSON data.");
                return false;
            }
        }

        public async Task<BlobDto> GetExistingBlobData(ObjectId WorkSpacesId, ObjectId user)
        {
            _logger.LogInformation("GetExistingBlobData: Retrieving latest workspace data for user.");
            _logger.LogDebug("GetExistingBlobData: Parameters - user: {User}, WorkSpacesId: {WorkSpacesId}", user, WorkSpacesId);

            try
            {
                BlobData userWorkspace = await _mongoDbService.BlobDatas
                    .Find(x => x.CreatedBy == user && x.Workspaces == WorkSpacesId)
                    .SortByDescending(x => x.CreatedOn)
                    .FirstOrDefaultAsync();

                if (userWorkspace == null)
                {
                    _logger.LogWarning("GetExistingBlobData: No data found for user: {User} in workspace: {WorkSpacesId}", user, WorkSpacesId);
                }
                else
                {
                    _logger.LogInformation("GetExistingBlobData: Data found for user: {User} in workspace: {WorkSpacesId}", user, WorkSpacesId);
                }

                return _mapper.Map<BlobDto>(userWorkspace);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetExistingBlobData: An error occurred while retrieving the latest workspace data.");
                throw;
            }
        }
    }
}
