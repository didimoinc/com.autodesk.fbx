// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;
using System.Collections.Generic;

namespace UnitTests
{
    public class FbxDocumentInfoTest : Base<FbxDocumentInfo>
    {
        private static Dictionary<string, string> m_dataValues = null;

        protected Dictionary<string, string> dataValues {
        	get {
        		if (m_dataValues == null) {
        			m_dataValues = new Dictionary<string, string> ()
        			{
						{ "title",      ".YvH5peIJMdg" },
						{ "subject",    "lmESAM8Fe3HV" },
						{ "author",     "hLsYMCqUekvr" },
						{ "revision",   "SknI2x=Ncp5P" },
						{ "keywords",   "netJRGcb8alS" },
						{ "comment",    ".0pzL-twb6mx" },
					};
        		}
        		return m_dataValues;
        	}
        }

        public static FbxDocumentInfo InitDocumentInfo (FbxDocumentInfo docInfo, Dictionary<string, string> values)
        {
            docInfo.mTitle = values ["title"];
            docInfo.mSubject = values ["subject"];
            docInfo.mAuthor = values ["author"];
            docInfo.mRevision = values ["revision"];
            docInfo.mKeywords = values ["keywords"];
            docInfo.mComment = values ["comment"];

            return docInfo;
        }

        public static void CheckDocumentInfo (FbxDocumentInfo docInfo, Dictionary<string, string> values)
        {
        	Assert.AreEqual (docInfo.mTitle, values ["title"]);
        	Assert.AreEqual (docInfo.mSubject, values ["subject"]);
        	Assert.AreEqual (docInfo.mAuthor, values ["author"]);
        	Assert.AreEqual (docInfo.mRevision, values ["revision"]);
        	Assert.AreEqual (docInfo.mKeywords, values ["keywords"]);
        	Assert.AreEqual (docInfo.mComment, values ["comment"]);
        }
        
        [Test]
        public void TestDocumentInfo ()
        {
            using (FbxDocumentInfo docInfo = CreateObject()) {

                CheckDocumentInfo (InitDocumentInfo (docInfo, this.dataValues), this.dataValues);
            }
        }
    }
}
