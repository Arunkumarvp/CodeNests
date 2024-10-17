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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNest.DTO.Models
{
    public class CompositeViewModel
    {
        public FormatterViewDto? FormatterView { get; set; }
        public WorkspacesDto? Workspaces { get; set; }
        public List<WorkspacesDto> WorkspacesList { get; set; }
    }
}
