﻿using Snail.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.DTO
{
    /// <summary>
    ///  todo:IPagenation默认实现
    /// </summary>
    public class KeyQueryDto:IPagination
    {
        public string KeyWord { get; set; }
        public int PageSize { get;set;}
        public int PageIndex { get;set;}
    }
}
