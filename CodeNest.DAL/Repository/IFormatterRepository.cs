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

namespace CodeNest.DAL.Repository
{
    public interface IFormatterRepository
    {
       Task<BlobDto> GetBlob(ObjectId blobId);
        Task<BlobDto> Update(BlobDto blobDto, ObjectId blobID, ObjectId userId);
    }
}
