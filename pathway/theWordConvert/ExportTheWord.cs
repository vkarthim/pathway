﻿// --------------------------------------------------------------------------------------
// <copyright file="ExportTheWord.cs" from='2012' to='2012' company='SIL International'>
//      Copyright © 2009, SIL International. All Rights Reserved.
//
//      Distributable under the terms of either the Common Public License or the
//      GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// <author>Greg Trihus</author>
// <email>greg_trihus@sil.org</email>
// Last reviewed:
// 
// <remarks>
// See: http://www.theword.net/
// </remarks>
// --------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using SIL.Tool;

namespace SIL.PublishingSolution
{
    public class ExportTheWord : IExportProcess
    {
        static int Verbosity = 0;
        protected static object ParatextData;
        protected static string Ssf;
        private static object _theWorProgPath;

        protected static readonly XslCompiledTransform TheWord = new XslCompiledTransform();

        public string ExportType
        {
            get
            {
                return "theWord/MySword";
            }
        }

        public bool Handle(string inputDataType)
        {
            return inputDataType.ToLower() == "scripture";
        }

        /// <summary>
        /// Entry point for TheWord export
        /// </summary>
        /// <param name="exportType">scripture / dictionary</param>
        /// <param name="publicationInformation">structure with other necessary information about project.</param>
        /// <returns></returns>
        public bool Launch(string exportType, PublicationInformation publicationInformation)
        {
            return Export(publicationInformation);
        }

        /// <summary>
        /// Entry point for TheWord converter
        /// </summary>
        /// <param name="projInfo">values passed including xhtml and css names</param>
        /// <returns>true if succeeds</returns>
        public bool Export(PublicationInformation projInfo)
        {
            if (projInfo == null) return false;
            bool success;
            var myCursor = Cursor.Current;
            var originalDir = Environment.CurrentDirectory;
            var inProcess = new InProcess(0, 7);
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                var codePath = Path.GetDirectoryName(assemblyLocation);
                Debug.Assert(codePath != null);
                Environment.CurrentDirectory = codePath;

                inProcess.Show();

                LoadXslt();
                inProcess.PerformStep();

                var exportTheWordInputPath = Path.GetDirectoryName(projInfo.DefaultCssFileWithPath);
                Debug.Assert(exportTheWordInputPath != null);

                LoadMetadata();
                inProcess.PerformStep();

                FindParatextProject();
                inProcess.PerformStep();

                var xsltArgs = LoadXsltParameters();
                inProcess.PerformStep();

                var otBooks = new List<string>();
                var ntBooks = new List<string>();
                CollectTestamentBooks(otBooks, ntBooks);
                inProcess.PerformStep();

                var output = GetSsfValue("//EthnologueCode", "zxx");
                var fullName = UsxDir(exportTheWordInputPath);
                LogStatus("Processing: {0}", fullName);

                var codeNames = new Dictionary<string, string>();
                var otFlag = OtFlag(fullName, codeNames, otBooks);
                inProcess.AddToMaximum(codeNames.Count*2);
                inProcess.PerformStep();

                var resultName = output + (otFlag ? ".ont" : ".nt");
                var resultFullName = Path.Combine(exportTheWordInputPath, resultName);
                ProcessAllBooks(resultFullName, otFlag, otBooks, ntBooks, codeNames, xsltArgs, inProcess);

                string theWordFullPath = Common.FromRegistry("TheWord");
                string tempTheWordCreatorPath = TheWordCreatorTempDirectory(theWordFullPath);
                xsltArgs.AddParam("refPref", "", "b");
                inProcess.PerformStep();

                var mySwordFullName = Path.Combine(tempTheWordCreatorPath, resultName);
                ProcessAllBooks(mySwordFullName, otFlag, otBooks, ntBooks, codeNames, xsltArgs, inProcess);

                var mySwordResult = ConvertToMySword(resultName, tempTheWordCreatorPath, exportTheWordInputPath);
                inProcess.PerformStep();

                Directory.Delete(tempTheWordCreatorPath, true);
                success = true;

                inProcess.Close();
                Environment.CurrentDirectory = originalDir;
                Cursor.Current = myCursor;

                success = ReportResults(resultFullName, mySwordResult, exportTheWordInputPath);

                Common.CleanupExportFolder(projInfo.DefaultXhtmlFileWithPath, ".tmp,.de", "layout", string.Empty);
                CreateRAMP(projInfo);
            }
            catch (Exception ex)
            {
                success = false;
                inProcess.PerformStep();
                inProcess.Close();
                Environment.CurrentDirectory = originalDir;
                Cursor.Current = myCursor;
                ReportFailure(ex);
            }
            return success;
        }

        private void CreateRAMP(PublicationInformation projInfo)
        {
            Ramp ramp = new Ramp();
            ramp.Create(projInfo.DefaultXhtmlFileWithPath, ".mybible,.nt");
        }

        private static void ReportFailure(Exception ex)
        {
            if (ex.Message.Contains("BookNames"))
            {
                MessageBox.Show("Please run the References basic check.", "theWord Export", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(ex.Message, "theWord Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool ReportResults(string resultFullName, string mySwordResult, string exportTheWordInputPath)
        {
            bool success;

            if (File.Exists(resultFullName))
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var theWordFolder = Path.Combine(Path.Combine(appData, "The Word"), "Bibles");
                if (RegistryHelperLite.RegEntryExists(RegistryHelperLite.TheWordKey, "RegisteredLocation", "",
                                                      out _theWorProgPath))
                {
                    if (!File.Exists((string) _theWorProgPath))
                    {
                        _theWorProgPath = null;
                    }
                }
                if (Directory.Exists(theWordFolder) && _theWorProgPath != null)
                {
                    ReportWhenTheWordInstalled(resultFullName, theWordFolder, mySwordResult, exportTheWordInputPath);
                }
                else
                {
                    ReportWhenTheWordNotInstalled(resultFullName, theWordFolder, mySwordResult, exportTheWordInputPath);
                }
                success = true;
            }
            else
            {
                MessageBox.Show("Failed Exporting TheWord Process.", "theWord Export", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                success = false;
            }
            return success;
        }

        private static void ReportWhenTheWordNotInstalled(string resultFullName, string theWordFolder, string mySwordResult, string exportTheWordInputPath)
        {
            string msgFormat = @"Do you want to open the folder with the results?

● Click Yes.

The folder with the ""{0}"" file ({2}) will open so you can manually copy it to {1}.

The MySword file ""{3}"" is also there so you can copy it to your Android device or send it to pathway@sil.org for uploading. 

● Click Cancel to do neither of the above.
";
            string resultName = Path.GetFileName(resultFullName);
            string resultDir = Path.GetDirectoryName(resultFullName);
            string msg = string.Format(msgFormat, resultName, theWordFolder, resultDir, Path.GetFileName(mySwordResult));
            DialogResult dialogResult = MessageBox.Show(msg, "theWord Export", MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                LaunchFileNavigator(exportTheWordInputPath);
            }
        }

        private static void ReportWhenTheWordInstalled(string resultFullName, string theWordFolder, string mySwordResult, string exportTheWordInputPath)
        {
            string msgFormat = @"Do you want to start theWord?

● Click Yes.

The program will copy the ""{0}"" file to {1} and start theWord. 

● Click No. 

The folder with the ""{0}"" file ({2}) will open so you can manually copy it to {1}.

The MySword file ""{3}"" is also there so you can copy it to your Android device or send it to pathway@sil.org for uploading. 

● Click Cancel to do neither of the above.
";
            string resultName = Path.GetFileName(resultFullName);
            string resultDir = Path.GetDirectoryName(resultFullName);
            string msg = string.Format(msgFormat, resultName, theWordFolder, resultDir, Path.GetFileName(mySwordResult));
            DialogResult dialogResult = MessageBox.Show(msg, "theWord Export", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                const bool overwrite = true;
                File.Copy(resultFullName, Path.Combine(theWordFolder, resultName), overwrite);
                Process.Start((string) _theWorProgPath);
            }
            else if (dialogResult == DialogResult.No)
            {
                LaunchFileNavigator(exportTheWordInputPath);
            }
        }

        private static void LaunchFileNavigator(string exportTheWordInputPath)
        {
            const bool noWait = false;
            SubProcess.Run(exportTheWordInputPath, Common.IsUnixOS() ? "nautilus" : "explorer.exe", exportTheWordInputPath, noWait);
        }

        protected static void LoadMetadata()
        {
            Param.LoadSettings();
            Param.SetValue(Param.InputType, "Scripture");
            Param.LoadSettings();
            //string layout = Param.GetItem("//settings/property[@name='LayoutSelected']/@value").Value;
        }

        protected static void LoadXslt()
        {
            var xsltSettings = new XsltSettings { EnableDocumentFunction = true };
            var inputXsl = Assembly.GetExecutingAssembly().GetManifestResourceStream("SIL.PublishingSolution.theWord.xsl");
            Debug.Assert(inputXsl != null);
            TheWord.Load(XmlReader.Create(inputXsl), xsltSettings, null);
        }

        protected static void CollectTestamentBooks(List<string> otBooks, List<string> ntBooks)
        {
            var xmlDoc = Common.DeclareXMLDocument(true);
            var vfs = new StreamReader("vrs.xml");
            xmlDoc.Load(vfs);
            vfs.Close();
            var codeNodes = xmlDoc.SelectNodes("//@code");
            Debug.Assert(codeNodes != null);
            var bkCount = 0;
            foreach (XmlNode node in codeNodes)
            {
                if (++bkCount <= 39)
                {
                    otBooks.Add(node.InnerText);
                }
                else
                {
                    ntBooks.Add(node.InnerText);
                }
            }
        }

        private void ProcessAllBooks(string fullName, bool otFlag, IEnumerable<string> otBooks, IEnumerable<string> ntBooks, Dictionary<string, string> codeNames, XsltArgumentList xsltArgs, InProcess inProcess)
        {
            LogStatus("Creating MySword: {0}", Path.GetFileName(fullName));
            // false = do not append but overwrite instead
            var sw = new StreamWriter(fullName, false, new UTF8Encoding(true));
            if (otFlag)
            {
                ProcessTestament(otBooks, codeNames, xsltArgs, sw, inProcess);
            }
            ProcessTestament(ntBooks, codeNames, xsltArgs, sw, inProcess);
            AttachMetadata(sw);
            sw.Close();
        }

        protected static void ProcessTestament(IEnumerable<string> books, Dictionary<string, string> codeNames, XsltArgumentList xsltArgs,
                                             StreamWriter sw, IInProcess inProcess)
        {
            foreach (string book in books)
            {
                if (codeNames.ContainsKey(book))
                {
                    LogStatus("Processing {0}", codeNames[book]);
                    TheWord.Transform(codeNames[book], xsltArgs, sw);
                    inProcess.PerformStep();
                }
                else
                {
                    LogStatus("Creating empty {0}", book);
                    var tempName = TempName(book);
                    TheWord.Transform(tempName, xsltArgs, sw);
                    File.Delete(tempName);
                }
            }
        }

        protected static string TempName(string book)
        {
            var tempName = Path.GetTempFileName();
            var tempStream = new StreamWriter(tempName);
            tempStream.Write(string.Format("<usx><book code= \"{0}\"/></usx>", book));
            tempStream.Close();
            return tempName;
        }

        protected static bool OtFlag(string fullName, Dictionary<string, string> codeNames, List<string> otBooks)
        {
            var codeStart = -1;
            var otFlag = false;
            foreach (string file in Directory.GetFiles(fullName, "*.usx"))
            {
                if (codeStart == -1)
                {
                    var fileDir = Path.GetDirectoryName(file);
                    Debug.Assert(fileDir != null);
                    codeStart = fileDir.Length + 1;
                }
                var curCode = file.Substring(codeStart, 3);
                codeNames[curCode] = file;
                if (otBooks.Contains(curCode))
                {
                    otFlag = true;
                }
            }
            return otFlag;
        }

        private static void FindParatextProject()
        {
            RegistryHelperLite.RegEntryExists(RegistryHelperLite.ParatextKey, "Settings_Directory", "", out ParatextData);
            var sh = new SettingsHelper(Param.DatabaseName);
            Ssf = sh.GetSettingsFilename();
        }

        private static string GetSsfValue(string xpath)
        {
            return GetSsfValue(xpath, null);
        }
        private static string GetSsfValue(string xpath, string def)
        {
            var node = Common.GetXmlNode(Ssf, xpath);
            return (node != null)? node.InnerText : def;
        }

        protected static XsltArgumentList LoadXsltParameters()
        {
            var xsltArgs = new XsltArgumentList();
            xsltArgs.AddParam("refPunc", "", GetSsfValue("//ChapterVerseSeparator", ":"));
            xsltArgs.AddParam("bookNames", "", GetBookNamesUri());
            return xsltArgs;
        }

        private static string GetBookNamesUri()
        {
            var myProj = Path.Combine((string) ParatextData, GetSsfValue("//Name"));
            return "file:///" + Path.Combine(myProj, "BookNames.xml");
        }

        private static string UsxDir(string exportTheWordInputPath)
        {
            var usxDir = Path.Combine(exportTheWordInputPath, "USX");
            if (!Directory.Exists(usxDir))
            {
                throw new FileNotFoundException("No USX folder");
            }
            return usxDir;
        }

        private string TheWordCreatorTempDirectory(string theWordFullPath)
        {
            var theWordDirectoryName = Path.GetFileNameWithoutExtension(theWordFullPath);
            Debug.Assert(theWordDirectoryName != null);
            var tempFolder = Path.GetTempPath();
            var folder = Path.Combine(tempFolder, theWordDirectoryName);
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            CopyTheWordFolderToTemp(theWordFullPath, folder);
            FolderTree.Copy(Path.Combine(folder, Directory.Exists(@"C:\Program Files (x86)") ? "x64" : "x32"), folder);
            return folder;
        }

        private void CopyTheWordFolderToTemp(string sourceFolder, string destFolder)
        {
            if (Directory.Exists(destFolder))
            {
                Common.DeleteDirectory(destFolder);
            }
            Directory.CreateDirectory(destFolder);
            FolderTree.Copy(sourceFolder, destFolder);
        }

        private static string ConvertToMySword(string resultName, string tempTheWordCreatorPath, string exportTheWordInputPath)
        {
            var myProc = Process.Start(new ProcessStartInfo
            {
                Arguments = resultName,
                FileName = "TheWordBible2MySword.exe",
                WorkingDirectory = tempTheWordCreatorPath,
                CreateNoWindow = true
            });
            myProc.WaitForExit();
            var mySwordFiles = Directory.GetFiles(tempTheWordCreatorPath, "*.mybible");
            var mySwordResult = "<No MySword Result>";
            if (mySwordFiles.Length >= 1)
            {
                var mySwordName = Path.GetFileName(mySwordFiles[0]);
                Debug.Assert(mySwordName != null);
                mySwordResult = Path.Combine(exportTheWordInputPath, mySwordName);
                File.Copy(mySwordFiles[0], mySwordResult);
            }
            return mySwordResult;
        }

        private void AttachMetadata(StreamWriter sw)
        {
            var format = @"id=W{0}
charset=0
lang={0}
font={9}
short.title={1}
title={2}
description={3} \
{4}
version.major=1
version.minor=0
version.date={5}
publisher={6}
publish.date={7}
author={6}
creator={8}
source={6}
about={2} \
<p>{4} \
<p>\
<p> . . . . . . . . . . . . . . . . . . .\
<p><b>Creative Commons</b> <i>Atribution-Non Comercial-No Derivatives 3.0</i>\
<p><font color=blue><i>http://creativecommons.org/licenses/by-nc-nd/3.0</i></font>\
";
            var langCode = GetSsfValue("//EthnologueCode", "zxx");
            const bool isConfigurationTool = false;
            var shortTitle = Param.GetTitleMetadataValue("Title", Param.GetOrganization(), isConfigurationTool);
            var FullTitle = GetSsfValue("//FullName", shortTitle);
            var description = Param.GetMetadataValue("Description");
            var copyright = Param.GetMetadataValue("Copyright Holder");
            var createDate = DateTime.Now.ToString("yyyy.M.d");
            var publisher = Param.GetMetadataValue("Publisher");
            var publishDate = createDate;
            var creator = Param.GetMetadataValue("Creator");
            var font = GetSsfValue("//DefaultFont", "Charis SIL");
            var myFormat = GetFormat("theWordFormat.txt", format);
            sw.Write(string.Format(myFormat, langCode, shortTitle, FullTitle, description, copyright, createDate, publisher, publishDate, creator, font));
        }

        private string GetFormat(string thewordformatTxt, string format)
        {
            var folder = Common.GetAllUserPath();
            var fullPath = Path.Combine(Path.Combine(folder, "Scripture"), thewordformatTxt);
            if (File.Exists(fullPath))
            {
                return FileData.Get(fullPath);
            }
            var sw = new StreamWriter(fullPath);
            sw.Write(format);
            sw.Close();
            return format;
        }

        static void LogStatus(string format, params object[] args)
        {
            if (Verbosity > 0)
            {
                Console.Write("# ");
                Console.WriteLine(format, args);
            }
        }

    }
}