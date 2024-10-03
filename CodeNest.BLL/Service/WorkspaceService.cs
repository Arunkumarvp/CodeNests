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
using MongoDB.Driver;
namespace CodeNest.BLL.Service
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly MangoDbService _mongoService;
        private readonly IMapper _mapper;
        public WorkspaceService(MangoDbService mongoService, IMapper mapper)
        {
            _mongoService = mongoService;
            _mapper = mapper;
        }

        public async Task<bool> CreateWorkspace(WorkspacesDto workspacesDto)
        {
            Workspaces result = await _mongoService.WorkSpaces.Find(x => x.Name == workspacesDto.Name).FirstOrDefaultAsync();

            if (result != null)
            {
                return false;
            }
            try
            {
                await _mongoService.WorkSpaces.InsertOneAsync(_mapper.Map<Workspaces>(workspacesDto));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
