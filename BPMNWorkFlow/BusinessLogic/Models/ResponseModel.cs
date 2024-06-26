﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNWorkFlow.BusinessLogic.Models
{
    public class ResponseModel<TValue>
    {
        public TValue? Data { get; init; }

        public string? ErrorMessage { get; set; }

        public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    }
}
