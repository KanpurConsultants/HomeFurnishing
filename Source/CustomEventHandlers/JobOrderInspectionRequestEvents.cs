﻿using Core.Common;
using CustomEventArgs;
using Data.Models;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEvents;
using JobOrderInspectionRequestDocumentEvents;

namespace Jobs.Controllers
{  
    public class JobOrderInspectionRequestEvents : JobOrderInspectionRequestDocEvents
    {
        //For Subscribing Events
        public JobOrderInspectionRequestEvents()
        {
            Initialized = true;           
        }

      

    }
}
