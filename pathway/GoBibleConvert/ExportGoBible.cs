﻿// --------------------------------------------------------------------------------------
// <copyright file="ExportGoBible.cs" from='2009' to='2009' company='SIL International'>
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
// Go Bible export
// See: btai/Documentation/GoBible Documentation.docx
// See: http://www.crosswire.org/gobible/
// See: http://www.jolon.org
// See: http://en.wikipedia.org/wiki/Go_Bible
// See: http://gbcpreprocessor.codeplex.com
// See: http://code.google.com/p/gobible/wiki/GoBibleRoadmap
// Emulators (for testing): http://jolon.org/vanillaforum/comments.php?DiscussionID=57
// </remarks>
// --------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using SIL.Tool;
using System.Threading;

namespace SIL.PublishingSolution
{
    public class ExportGoBible : IExportProcess 
    {
        protected string processFolder;
        protected string restructuredFullName;
        protected string collectionFullName;
        protected string collectionName;
        protected static ProgressBar _pb;
        private const string RedirectOutputFileName = "Convert.log";
        List<string> DuplicateBooks;

        public string ExportType
        {
            get
            {
                return "GoBible";
            }
        }

        public bool Handle(string inputDataType)
        {
            bool returnValue = false;
            if (inputDataType.ToLower() == "scripture")
            {
                returnValue = true;
            }
            return returnValue;
        }

        /// <summary>
        /// Entry point for GoBible export
        /// </summary>
        /// <param name="exportType">scripture / dictionary</param>
        /// <param name="publicationInformation">structure with other necessary information about project.</param>
        /// <returns></returns>
        public bool Launch(string exportType, PublicationInformation publicationInformation)
        {
            //if (!Handle(exportType))
            //    return false;
            return Export(publicationInformation);
        }

        /// <summary>
        /// Entry point for GoBible converter
        /// </summary>
        /// <param name="projInfo">values passed including xhtml and css names</param>
        /// <returns>true if succeeds</returns>
        public bool Export(PublicationInformation projInfo)
        {
            bool success;
            //try
            //{
            var curdir = Environment.CurrentDirectory; // $(Desktop)\\Scripture
            //MessageBox.Show(string.Format("Preparing to convert {0} for cell phone", projInfo.DefaultXhtmlFileWithPath), "GoBible Export");
            var myCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            var inProcess = new InProcess(0, 5);
            inProcess.Show();
            inProcess.PerformStep();
            collectionName = GetCollectionName(projInfo);
            inProcess.PerformStep();
            Restructure(projInfo, inProcess);
            inProcess.PerformStep();
            if (!CreateCollection())
            {
                string errMsg = "The follow book names are duplicated:\r\n";
                foreach (string name in DuplicateBooks)
                    errMsg += "\t" + name + "\r\n";
                errMsg += "Please correct names in Translation Editor: File: Properties: Book Properties.";
                MessageBox.Show(errMsg, "GoBible Export Dpulicate Book(s) Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = myCursor;
                inProcess.Close();
                return false;
            }
            inProcess.PerformStep();
            BuildApplication();
            inProcess.PerformStep();
            inProcess.Close();
            Cursor.Current = myCursor;
            const int ExitOk = 0;
            if (SubProcess.ExitCode != ExitOk)
            {
                string msg = string.Format("The conversion has exited with an error. Do you want to display additional details on the error?");
                DialogResult dialogResult = MessageBox.Show(msg, "GoBible Export", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    string redirectDirectory = Path.GetDirectoryName(collectionFullName);
                    string redirectFullName = Path.Combine(redirectDirectory, RedirectOutputFileName);
                    StreamReader streamReader = new StreamReader(redirectFullName);
                    string errMsg = "";
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        if (!char.IsWhiteSpace(line[0]))
                            errMsg += line + "\r\n";
                    }
                    streamReader.Close();
                    MessageBox.Show(errMsg, "GoBible Export Error Details", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (projInfo.IsOpenOutput)
            {
                string result = Common.PathCombine(processFolder, collectionName + ".jar");
                string msg = string.Format("Please copy the file {0} to your phone", result);
                MessageBox.Show(msg, "GoBible Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Environment.CurrentDirectory = curdir;
            success = true;
            //}
            //catch (Exception ex)
            //{
            //    var msg = ex.Message;
            //    success = false;
            //}
            return success;
        }

        protected void Restructure(PublicationInformation projInfo, IInProcess inProcess)
        {
            // The first process for phone apps is to restructure the XHTML file that has been
            // exported from FieldWorks. This starts by identifying the XHTML file to
            // process. The process folder is "$(Local Settings)\\Temp\\Preprocess".

            // Merge the individual CSS stylesheets into one temporary stylesheet and
            // copy it to the process folder.
            GetMergedCSS(projInfo);
            // Preprocess the xhtml file to replace image names, and link to the merged css file.
            string processedXhtmlFile = GetProcessedXhtml(projInfo);

            Common.xsltProgressBar = inProcess.Bar();
            inProcess.AddToMaximum(Chapters(processedXhtmlFile));

            // Next, get the transformation (XSLT) file, which has been put in the folder
            // "C:\SIL\btai\PublishingSolution\PublishingSolutionExe\bin\Debug".
            // The path for the default XHTML file is "$(Desktop)\\Scripture".
            string cvFileName = Path.GetFileNameWithoutExtension(processedXhtmlFile) + "_cv";
            const string xsltName = "TE_XHTML-to-Phone_XHTML.xslt";
			string xsltFullName = Common.FromRegistry(xsltName);
            processFolder = Path.GetDirectoryName(processedXhtmlFile);

            // Transform the given XHTML file into a restructured version. Copy the results into
            // the project solution folder, "$(Desktop)\\Scripture".
            //Common.Xslt2Process(processedXhtmlFile, xsltFullName, "_cv.xhtml");
            Common.XsltProcess(processedXhtmlFile, xsltFullName, "_cv.xhtml");
            string temporaryCvFullName = Common.PathCombine(processFolder, cvFileName + ".xhtml");
            string projectPath = Path.GetDirectoryName(projInfo.DefaultXhtmlFileWithPath);
            restructuredFullName = Path.Combine(projectPath, cvFileName + ".xhtml");
            if (restructuredFullName != temporaryCvFullName)
                File.Copy(temporaryCvFullName, restructuredFullName, true);
        }

        protected int Chapters(string name)
        {
            if (!File.Exists(name))
                Thread.Sleep(300);
            XmlNodeList nodes = GetPathFromXhtml(name, "//xhtml:span[@class='Chapter_Number']");
            return nodes.Count;
        }

        /// <summary>
        /// Create collections files (using variables set up by Restructure
        /// </summary>
        protected bool CreateCollection()
        {
            var sourceText = Path.GetFileName(restructuredFullName);

            // Calculate book list
            XmlNodeList books = GetPathFromXhtml(restructuredFullName, "//xhtml:div[@class='scrBook']/@title");
            if (IsDuplicateBooks(books))
                return false;
            collectionFullName = Path.Combine(processFolder, "Collections.txt");

            // Set Default Collection Parameters
            var red = "false";
            var info = "Bible text exported from FieldWorks Translation Editor.";
			var iconFullName = Common.FromRegistry(Common.PathCombine("GoBible/GoBibleCore/Icon", "Icon.png"));

            // Load User Interface Collection Parameters
            Param.LoadSettings();
            Param.SetValue(Param.InputType, "Scripture");
            Param.LoadSettings();
            string layout = Param.GetItem("//settings/property[@name='LayoutSelected']/@value").Value;
            Dictionary<string, string> mobilefeature = Param.GetItemsAsDictionary("//stylePick/styles/mobile/style[@name='" + layout + "']/styleProperty");
            if (mobilefeature.ContainsKey("RedLetter") && mobilefeature["RedLetter"] == "Yes")
                red = "true";
            if (mobilefeature.ContainsKey("Information"))
                info = mobilefeature["Information"].Trim();
            if (mobilefeature.ContainsKey("Copyright"))
                info += " " + mobilefeature["Copyright"].Trim();
            if (mobilefeature.ContainsKey("Icon"))
                if (File.Exists(mobilefeature["Icon"]))
                    iconFullName = mobilefeature["Icon"];
            var iconDirectory = Path.GetDirectoryName(iconFullName);
            var iconName = Path.GetFileName(iconFullName);
            const bool overwrite = true;
            if (iconDirectory != processFolder)
                File.Copy(iconFullName, Path.Combine(processFolder, iconName), overwrite);

            // Write collection file
            TextWriter textWriter = new StreamWriter(collectionFullName);
            textWriter.WriteLine("Info: " + info);
            textWriter.WriteLine("Phone-Icon-Filepath: " + iconName); // path to 20x20 *.png icon file
            textWriter.WriteLine("RedLettering: " + red);  // relies on correct tag
            textWriter.WriteLine("Source-Text: " + sourceText);
            textWriter.WriteLine("Source-Format: xhtml_te");
            textWriter.WriteLine("Collection: " + collectionName);
            foreach (XmlNode xmlNode in books)
            {
                textWriter.WriteLine("Book: " + xmlNode.Value);
            }
            textWriter.Close();
            return true;
        }

        /// <summary>
        /// Returns the list of nodes selected by path in name
        /// </summary>
        /// <param name="name">xhtml file to query</param>
        /// <param name="path">XPath query</param>
        /// <returns>list of nodes resulting from applying Xpath path to file name</returns>
        protected XmlNodeList GetPathFromXhtml(string name, string path)
        {
            XmlDocument xmlDocument = new XmlDocument { XmlResolver = null };
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings {XmlResolver = null, ProhibitDtd = false};
            XmlReader xmlReader = XmlReader.Create(name, xmlReaderSettings);
            xmlDocument.Load(xmlReader);
            xmlReader.Close();
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNamespaceManager.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");
            return xmlDocument.SelectNodes(path, xmlNamespaceManager);
        }

        protected bool IsDuplicateBooks(XmlNodeList books)
        {
            DuplicateBooks = new List<string>();
            List<string> allBooks = new List<string>();
            foreach (XmlNode bookNode in books)
            {
                string bookName = bookNode.Value;
                if (allBooks.Contains(bookName))
                {
                    if (!DuplicateBooks.Contains(bookName))
                        DuplicateBooks.Add(bookName);
                }
                else
                {
                    allBooks.Add(bookName);
                }
            }
            return DuplicateBooks.Count > 0;
        }

        protected void BuildApplication()
        {
            const string Creator = "GoBibleCreator.jar";
            const string prog = "java.exe";
            var creatorPath = Common.PathCombine("GoBible", Creator);
			var creatorFullPath = Common.FromRegistry(creatorPath);
            var progFolder = SubProcess.GetLocation(prog);
            var progFullName = Common.PathCombine(progFolder, prog);
            var args = string.Format(@"-Xmx128m -jar ""{0}"" ""{1}""", creatorFullPath, collectionFullName);
            SubProcess.RedirectOutput = RedirectOutputFileName;
            SubProcess.Run(processFolder, progFullName, args, true);
        }

        /// <summary>
        /// Preprocess the xhtml file to replace image names, and link to the merged css file.
        /// </summary>
        /// <param name="projInfo">information about input data</param>
        /// <returns>path name to processed xthml file</returns>
        private string GetProcessedXhtml(PublicationInformation projInfo)
        {
            var result = projInfo.DefaultXhtmlFileWithPath;
            if (Path.GetFileNameWithoutExtension(result) == collectionName)
            {
                result = Common.PathCombine(Path.GetDirectoryName(projInfo.DefaultXhtmlFileWithPath),
                                                  collectionName + ".xhtml");
                File.Copy(projInfo.DefaultXhtmlFileWithPath, result, true);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projInfo"></param>
        /// <returns>mergedCSS</returns>
        private string GetMergedCSS(PublicationInformation projInfo)
        {
            PreExportProcess preProcessor = new PreExportProcess(projInfo);
            var mc = new MergeCss { OutputLocation = "Preprocess" };
            string mergedCSS = mc.Make(projInfo.DefaultCssFileWithPath, "Temp1.css");
            preProcessor.ReplaceStringInCss(mergedCSS);
            preProcessor.SetDropCapInCSS(mergedCSS);
            var savedCss = Common.PathCombine(Path.GetDirectoryName(projInfo.DefaultXhtmlFileWithPath),
                                              collectionName + ".css");
            File.Copy(mergedCSS, savedCss, true);
            return savedCss;
        }

        /// <summary>
        /// returns the project name from the path
        /// </summary>
        /// <param name="projInfo">data on project</param>
        /// <returns>Project Name</returns>
        protected string GetProjectName(IPublicationInformation projInfo)
        {
            var scrDir = Path.GetDirectoryName(projInfo.DefaultXhtmlFileWithPath);
            var projDir = Path.GetDirectoryName(scrDir);
            return Path.GetFileName(projDir);
        }

        /// <summary>
        /// Returns the book code from the input file
        /// </summary>
        /// <param name="projInfo">project file names</param>
        /// <returns>Book Code</returns>
        protected string GetBookCode(IPublicationInformation projInfo)
        {
            XmlNodeList codeNodes = GetPathFromXhtml(projInfo.DefaultXhtmlFileWithPath, "//xhtml:span[@class='scrBookCode']");
            if (codeNodes.Count == 0)
            {
                codeNodes = GetPathFromXhtml(projInfo.DefaultXhtmlFileWithPath, "//xhtml:span[@class='scrBookName']");
            }
            string result = "";
            if (codeNodes.Count > 0)
                result = codeNodes[0].InnerText;
            if (codeNodes.Count > 1)
                result += "_" + codeNodes[codeNodes.Count - 1].InnerText;
            result = Sanitize(result);
            return result;
        }

        protected string Sanitize(string result)
        {
            result.Trim();
            if (result.Contains(" "))
            {
                var newResult = "";
                foreach (char c in result)
                {
                    if (!char.IsWhiteSpace(c))
                        newResult = newResult + c;
                }
                result = newResult;
            }
            return result;
        }

        /// <summary>
        /// Construct collection name from project and book
        /// </summary>
        /// <param name="projInfo">project data</param>
        /// <returns>Collection Name</returns>
        protected string GetCollectionName(IPublicationInformation projInfo)
        {
            return GetProjectName(projInfo) + "_" + GetBookCode(projInfo);
        }
    }
}
