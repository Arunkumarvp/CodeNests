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
    public interface IFormatterServices
    {
        Task<ValidationDto> JsonValidate(BlobDto jsonDto);
        Task<ValidationDto> Save(BlobDto jsonDto, ObjectId workSpace, ObjectId user);
        Task<BlobDto> GetJsonDataLegacyUser(ObjectId workSpace, ObjectId user);
       
    }
}
