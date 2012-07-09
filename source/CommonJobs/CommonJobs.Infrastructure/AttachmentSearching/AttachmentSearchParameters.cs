﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.AttachmentSearching
{
    public class AttachmentSearchParameters : BaseSearchParameters
    {
        //TODO: include in UI
        public bool SearchOnlyInFileName { get; set; } 
        public bool IncludeFilesWithoutText { get; set; } 
        public OrphansMode Orphans { get; set; }
    }
}
