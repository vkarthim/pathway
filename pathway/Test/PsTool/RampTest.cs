﻿// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. LGPL
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. LGPL
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: RampTest.cs
// Responsibility: Trihus
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using NMock2;
using NUnit.Framework;
using SIL.PublishingSolution;
using SIL.Tool;

namespace Test.PsTool
{
    /// ----------------------------------------------------------------------------------------
    /// <summary>
    /// Test functions of Ramp package preparation
    /// </summary>
    /// ----------------------------------------------------------------------------------------
    [TestFixture]
    public class RampTest: Ramp
    {
        #region setup
        private static Mockery mocks = new Mockery();
        private static string _inputPath;
        private static string _outputPath;
        private static string _expectedPath;

        [TestFixtureSetUp]
        public void Setup()
        {
            Common.ProgInstall = PathPart.Bin(Environment.CurrentDirectory, @"/../PsSupport");
            Common.SupportFolder = "";
            Common.ProgBase = Common.ProgInstall;
            string testPath = PathPart.Bin(Environment.CurrentDirectory, "/PsTool/TestFiles");
            _inputPath = Common.PathCombine(testPath, "InputFiles");
            _outputPath = Common.PathCombine(testPath, "output");
            _expectedPath = Common.PathCombine(testPath, "Expected");
            if (Directory.Exists(_outputPath))
                Directory.Delete(_outputPath, true);
            Directory.CreateDirectory(_outputPath);
        }

        [SetUp]
        public void TestSetup()
        {
            _fileList.Clear();
            _isoLanguageCode.Clear();
            _isoLanguageCodeandName.Clear();
            _isoLanguageScriptandName.Clear();
            LanguageIso.Clear();
            RampDescriptionHas = null;
            RampDescription = null;
            var settingsFolder = Path.Combine(Common.GetAllUserPath(), "Pathway");
            if (Directory.Exists(settingsFolder))
            {
                Directory.Delete(settingsFolder, true);
            }
        }
        #endregion setup

        [Test]
        public void SetRampDataModeTest()
        {
            _folderPath = FileInput(Path.Combine("rampInput", "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SetRampData();
            Assert.AreEqual("Text,Graphic", TypeMode);
        }

        [Test]
        public void SetRampDataAudienceTest()
        {
            _folderPath = FileInput(Path.Combine("rampInput", "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SetRampData();
            Assert.AreEqual("wider_audience", BroadType);
        }

        [Test]
        public void AddSubjLanguageTest()
        {
            SubjectLanguage = new List<string>();
            _folderPath = FileInput(Path.Combine("rampInput", "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            LoadLanguagefromXML();
            AddSubjLanguage(Common.GetLanguageCode(_folderPath, _projInputType, true));
            Assert.True(SubjectLanguage.Contains("ggo:Gondi, Southern"), "missing Gondwana");
        }

        [Test]
        public void SetRampDataSubjectLanguage2Test()
        {
            _folderPath = FileInput(Path.Combine("rampInput2", "sena3.xhtml"));
            _projInputType = "Dictionary";
            LoadLanguagefromXML();
            SetRampData();
            Assert.True(SubjectLanguage.Contains("seh:Sena"), "missing Sena");
        }

        [Test]
        public void AddLanguagIsoTest()
        {
            _folderPath = FileInput(Path.Combine("rampInput2", "sena3.xhtml"));
            _projInputType = "Dictionary";
            LoadLanguagefromXML();
            AddLanguageIso(Common.GetLanguageCodeList(_folderPath, _projInputType));
            Assert.AreEqual(3, LanguageIso.Count);
            Assert.True(LanguageIso.Contains("seh:Sena"), "missing Sena");
            Assert.True(LanguageIso.Contains("eng:English"), "missing English");
            Assert.True(LanguageIso.Contains("por:Portuguese"), "missing Portuguese");
        }

        [Test]
        public void AddLanguageScriptTest()
        {
            _folderPath = FileInput(Path.Combine("rampInput", "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            LoadLanguagefromXML();
            AddLanguageScript(Common.GetLanguageScriptList(_folderPath, _projInputType));
            Assert.True(LanguageScript.Contains("Telu:Telugu"), "missing Telugu");
        }

        [Test]
        [Category("SkipOnTeamCity")]
        public void AddContributorTest()
        {
            const string TestFolder = "rampInput";
            _folderPath = FileInput(Path.Combine(TestFolder, "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.True(Contributor.Contains("GOD,compiler"), "should be GOD!");
        }

        [Test]
        [Category("SkipOnTeamCity")]
        public void RampFormatExtentImagesTest()
        {
            const string TestFolder = "rampInput";
            _folderPath = FileInput(Path.Combine(TestFolder, "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.True(FormatExtentImages.Contains("2"), "should be 2!");
        }

        [Test]
        [Category("SkipOnTeamCity")]
        public void RampRelRequiresTest()
        {
            RelRequires = new List<string>();
            const string TestFolder = "rampInput";
            _folderPath = FileInput(Path.Combine(TestFolder, "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.True(RelRequires.Count == 4, "should be 4 fonts!");
        }

        [Test]
        [Ignore]
        [Category("SkipOnTeamCity")]
        public void RampRightsTest()
        {
            const string TestFolder = "rampInput";
            _folderPath = FileInput(Path.Combine(TestFolder, "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.True(Rights == "© 2013 SIL International®. DRAFT DOCUMENTPlease note that this document is not in its final form. This document was printed during the normal course of a review process by the creator or editor, and will likely be superceded in both content and format by a...", "CopyRight Information is wrong!");
        }

        [Test]
        [Category("SkipOnTeamCity")]
        public void RampCreatedOnTest()
        {
            const string TestFolder = "rampInput";
            _folderPath = FileInput(Path.Combine(TestFolder, "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.True(CreatedOn == DateTime.Now.ToString("r"), "Created on is incorrect!");
        }

        [Test]
        [Category("SkipOnTeamCity")]
        public void RampModifiedDateTest()
        {
            const string TestFolder = "rampInput";
            _folderPath = FileInput(Path.Combine(TestFolder, "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.True(ModifiedDate == DateTime.Now.ToString("yyyy-MM-dd"), "Modified on is incorrect!");
        }

        [Test]
        [Category("SkipOnTeamCity")]
        public void RampDescriptionTest()
        {
            const string TestFolder = "rampInput";
            _folderPath = FileInput(Path.Combine(TestFolder, "Gondwana Sample.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.AreEqual("Just for testing", RampDescription);
            Assert.AreEqual("Y", RampDescriptionHas);
        }

        [Test]
        public void RampDescriptionEmptyTest()
        {
            const string TestFolder = "rampInput2";
            _folderPath = FileInput(Path.Combine(TestFolder, "sena3.xhtml"));
            _projInputType = "Dictionary";
            SettingsInput(TestFolder);
            SetRampData();
            Assert.AreEqual(null, RampDescription);
            Assert.AreEqual(null, RampDescriptionHas);
        }

        #region Private Functions
        private static void SettingsInput(string TestFolder)
        {
            var settingsFolder = FileInput(Path.Combine(TestFolder, "Pathway"));
            if (Directory.Exists(settingsFolder))
            {
                FolderTree.Copy(settingsFolder, Common.GetAllUserPath());
            }
        }

        private static string FileProg(string fileName)
        {
            return Common.PathCombine(Common.GetPSApplicationPath(), fileName);
        }

        private static string FileInput(string fileName)
        {
            return Common.PathCombine(_inputPath, fileName);
        }

        private static string FileOutput(string fileName)
        {
            return Common.PathCombine(_outputPath, fileName);
        }

        private static string FileExpected(string fileName)
        {
            return Common.PathCombine(_expectedPath, fileName);
        }

        /// <summary>
        /// Create a simple PublicationInformation instance
        /// </summary>
        private static PublicationInformation GetProjInfo(string XhtmlName, string BlankName)
        {
            PublicationInformation projInfo = new PublicationInformation();
            File.Copy(FileInput(XhtmlName), FileOutput(XhtmlName), true);
            File.Copy(FileInput(BlankName), FileOutput(BlankName), true);
            projInfo.DefaultXhtmlFileWithPath = FileOutput(XhtmlName);
            projInfo.DefaultCssFileWithPath = FileOutput(BlankName);
            projInfo.IsOpenOutput = false;
            return projInfo;
        }
        #endregion PrivateFunctions
    }
}