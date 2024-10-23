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

using CodeNest.DTO.Models;
using MongoDB.Bson;

namespace CodeNest.BLL.Service
{
    public interface IJsonService
    {

        Task<ValidationDto> Validate(BlobDto jsonDto);
        Task<ValidationDto> Save(BlobDto jsonDto, ObjectId workspaceId, ObjectId userId, string filename);
        Task<List<BlobDto>> GetJson(ObjectId workspaceId);
    }
}
