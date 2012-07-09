﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Infrastructure.AttachmentSearching
{
    public class Attachments_QuickSearch : AbstractMultiMapIndexCreationTask<Attachments_QuickSearch.Projection>
    {
        //TODO: permitir indexar coleccion de key/values para metadatos (Por ejemplo los de los archivos de word)
        private const int PartialTextLength = 2000;

        public class Projection
        {
            public string AttachmentId { get; set; }
            public string[] FullText { get; set; }
            public string PartialText { get; set; }
            public string ContentType { get; set; }
            public string FileName { get; set; }
            public string RelatedEntityId { get; set; }
            public bool IsOrphan { get; set; }
            public bool HasText { get; set; }
            public string ContentExtractorConfigurationHash { get; set; }
        }

        public Attachments_QuickSearch()
        {
            AddMap<Attachment>(attachments =>
                from attachment in attachments
                select new
                {
                    AttachmentId = attachment.Id,
                    FullText = new string[] { attachment.PlainContent, attachment.FileName, attachment.ContentType },
                    PartialText = attachment.PlainContent.Length < PartialTextLength ? attachment.PlainContent : attachment.PlainContent.Substring(0, PartialTextLength),
                    ContentType = attachment.ContentType,
                    FileName = attachment.FileName,
                    RelatedEntityId = attachment.RelatedEntityId,
                    ContentExtractorConfigurationHash = attachment.ContentExtractorConfigurationHash,
                    IsOrphan = true,
                    HasText = !string.IsNullOrEmpty(attachment.PlainContent)
                });

            //TODO: hacer esto automático para cualquier entidad que tenga la propiedad AllAttachmentReferences
            AddMap<Employee>(employees =>
                from entity in employees
                from attachmentReference in entity.AllAttachmentReferences
                select new
                {
                    AttachmentId = attachmentReference.Id,
                    FullText = new string[] { entity.LastName, entity.FirstName, entity.Id, entity.Email, entity.Platform },
                    PartialText = (string) null,
                    ContentType = (string)null,
                    FileName = (string)null,
                    RelatedEntityId = (string)null,
                    ContentExtractorConfigurationHash = (string)null,
                    IsOrphan = false,
                    HasText = false
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                from attachmentReference in entity.AllAttachmentReferences
                select new
                {
                    AttachmentId = attachmentReference.Id,
                    FullText = new string[] { entity.LastName, entity.FirstName, entity.Id, entity.Email },
                    PartialText = (string)null,
                    ContentType = (string)null,
                    FileName = (string)null,
                    RelatedEntityId = (string)null,
                    ContentExtractorConfigurationHash = (string)null,
                    IsOrphan = false,
                    HasText = false
                });

            Reduce = docs => from doc in docs
                             group doc by doc.AttachmentId into g
                             select new
                             {
                                AttachmentId = g.Key,
                                FullText = g.SelectMany(x => x.FullText).ToArray(),
                                PartialText = g.Select(x => x.PartialText).FirstOrDefault(x => x != null),
                                ContentType = g.Select(x => x.ContentType).FirstOrDefault(x => x != null),
                                FileName = g.Select(x => x.FileName).FirstOrDefault(x => x != null),
                                RelatedEntityId = g.Select(x => x.RelatedEntityId).FirstOrDefault(x => x != null),
                                ContentExtractorConfigurationHash = g.Select(x => x.ContentExtractorConfigurationHash).FirstOrDefault(x => x != null),
                                IsOrphan = g.All(x => x.IsOrphan),
                                HasText = g.Any(x => x.HasText)
                             };

            Index(x => x.FullText, FieldIndexing.Analyzed);
            Index(x => x.PartialText, FieldIndexing.No);
        }
    }
}
