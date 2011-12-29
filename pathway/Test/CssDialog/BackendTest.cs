﻿// --------------------------------------------------------------------------------------------
#region // Copyright © 2011, SIL International. All Rights Reserved.
// <copyright file="BackendTest.cs" from='2011' to='2011' company='SIL International'>
//		Copyright © 2011, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// <author>Greg Trihus</author>
// <email>greg_trihus@sil.org</email>
// Last reviewed: 
// 
// <remarks>
// </remarks>
// --------------------------------------------------------------------------------------------

using System;
using System.Collections;
using NUnit.Framework;
using SIL.PublishingSolution;
using SIL.Tool;

namespace Test.CssDialog
{
    public class BackendTest
    {
        #region Setup
        [TestFixtureSetUp]
        protected void SetUp()
        {
            Common.Testing = true;
            Common.ProgInstall = PathwayPath.GetPathwayDir();
            Backend.Load(Common.ProgInstall);
        }
        #endregion Setup

        #region TearDown
        [TestFixtureTearDown]
        protected void TearDown()
        {
            Backend.Load(string.Empty);
            Common.ProgInstall = string.Empty;
            Common.SupportFolder = string.Empty;
        }
        #endregion TearDown

        [Test]
        [Category("SkipOnTeamCity")]
        public void GetExportTypeTest()
        {
            ArrayList exportType = Backend.GetExportType("Dictionary");
            ArrayList check = new ArrayList(exportType.Count);
            foreach (string item in exportType)
            {
                Assert.False(check.Contains(item));
                check.Add(item);
            }
        }
    }
}