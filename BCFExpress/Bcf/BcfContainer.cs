﻿using BCFExpress.Bcf.Bcf2;
using BCFExpress.Data;
using BCFExpress.Data.Utils;
using BCFExpress.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using Version2_1;
using Version = BCFExpress.Bcf.Bcf2.Version;
using VisualizationInfo = BCFExpress.Bcf.Bcf2.VisualizationInfo;

namespace BCFExpress.Bcf
{
    //this class reference from <<<<bcfier>>>
    /// <summary>
    /// Model View, binds to the tab control, contains the BCf files
    /// and the main methods as save, open...
    /// </summary>
    public class BcfContainer : INotifyPropertyChanged
    {
        private ObservableCollection<BcfFile> _bcfFiles { get; set; }
        private int selectedReport { get; set; }

        public BcfContainer()
        {
            BcfFiles = new ObservableCollection<BcfFile>();
        }


        public ObservableCollection<BcfFile> BcfFiles
        {
            get
            {
                return _bcfFiles;
            }

            set
            {
                _bcfFiles = value;
                NotifyPropertyChanged("BcfFiles");
            }
        }

        public int SelectedReportIndex
        {
            get
            {
                return selectedReport;
            }

            set
            {
                selectedReport = value;
                NotifyPropertyChanged("SelectedReportIndex");
            }
        }


        public void NewFile()
        {
            BcfFiles.Add(new BcfFile());
            SelectedReportIndex = BcfFiles.Count - 1;
        }
        public void SaveFile(BcfFile bcf)
        {
            SaveBcfFile(bcf);
        }
        public void MergeFiles(BcfFile bcf)
        {
            var bcffiles = OpenBcfDialog();
            if (bcffiles == null)
                return;
            bcf.MergeBcfFile(bcffiles);
        }

        public void OpenFile(string path)
        {
            var newbcf = OpenBcfFile(path);
            BcfOpened(newbcf);
        }
        public bool OpenFile()
        {
            var bcffiles = OpenBcfDialog();

            if (bcffiles == null)
                return false;
            foreach (var bcffile in bcffiles)
            {
                if (bcffile == null)
                    continue;
                BcfOpened(bcffile);
            }
            return true;
        }

        private void BcfOpened(BcfFile newbcf)
        {
            if (newbcf != null)
            {
                BcfFiles.Add(newbcf);
                SelectedReportIndex = BcfFiles.Count - 1;
                if (newbcf.Issues.Any())
                    newbcf.SelectedIssue = newbcf.Issues.First();

                Globals.BcfFile = newbcf;

                foreach (var issue in newbcf.Issues)
                {
                    if (!Globals.OpenStatuses.Contains(issue.Topic.TopicStatus))
                        Globals.OpenStatuses.Add(issue.Topic.TopicStatus);
                    //Globals._Issues.Add(issue);

                    if (!Globals.OpenTypes.Contains(issue.Topic.TopicType))
                        Globals.OpenTypes.Add(issue.Topic.TopicType);
                }

            }
        }

        public void CloseFile(BcfFile bcf)
        {
            try
            {
                _bcfFiles.Remove(bcf);
                Utils.DeleteDirectory(bcf.TempPath);
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }

        }

        /// <summary>
        /// Removes all old statuses and types from the collection and adds the new ones, except for the selected one
        /// This avoids having blank fields in case an existing value is removed
        /// could probably be optimized
        /// </summary>
        public void UpdateDropdowns()
        {
            Globals._Issues.Clear();
            if (BcfFiles.Count > 1)
            {
                BcfFiles.FirstOrDefault().Issues.Clear();
                BcfFiles.Remove(_bcfFiles[0]);

            }
            try
            {
                Globals.SetStatuses(UserSettings.Get("Stauses"));
                Globals.SetTypes(UserSettings.Get("Types"));

                foreach (var bcf in BcfFiles)
                {
                    foreach (var issue in bcf.Issues)
                    {
                        Globals._Issues.Add(issue);
                        var oldStatus = issue.Topic.TopicStatus;
                        var oldType = issue.Topic.TopicType;

                        //status
                        for (int i = issue.Topic.TopicStatusesCollection.Count - 1; i >= 0; i--)
                        {
                            if (issue.Topic.TopicStatusesCollection[i] != oldStatus)
                                issue.Topic.TopicStatusesCollection.RemoveAt(i);
                        }
                        foreach (var status in Globals.AvailStatuses)
                        {
                            if (status != oldStatus || !issue.Topic.TopicStatusesCollection.Contains(status))
                                issue.Topic.TopicStatusesCollection.Add(status);
                        }
                        //type
                        for (int i = issue.Topic.TopicTypesCollection.Count - 1; i >= 0; i--)
                        {
                            if (issue.Topic.TopicTypesCollection[i] != oldType)
                                issue.Topic.TopicTypesCollection.RemoveAt(i);
                        }
                        foreach (var type in Globals.AvailTypes)
                        {
                            if (type != oldType || !issue.Topic.TopicTypesCollection.Contains(type))
                                issue.Topic.TopicTypesCollection.Add(type);
                        }
                    }
                }

            }

            catch
            {
                //suppress error
            };

        }




        #region private methods
        /// <summary>
        /// Prompts a dialog to select one or more BCF files to open
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<BcfFile> OpenBcfDialog()
        {
            try
            {
                var openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
                openFileDialog1.Filter = "BIM Collaboration Format (*.bcfzip,*.bcf)|*.bcfzip;*.bcf";
                openFileDialog1.DefaultExt = ".bcfzip,.bcf";
                openFileDialog1.Multiselect = true;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.CheckPathExists = true;
                var result = openFileDialog1.ShowDialog(); // Show the dialog.

                if (result == true) // Test result.
                {
                    return openFileDialog1.FileNames.Select(OpenBcfFile).ToList();
                }
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return null;
        }

        /// <summary>
        /// Logic that extracts files from a bcfzip and deserializes them
        /// </summary>
        /// <param name="bcfzipfile">Path to the .bcfzip file</param>
        /// <returns></returns>
        private static BcfFile OpenBcfFile(string bcfzipfile) /*  ----------------------------------------------------------------------------------  */
        {
            DB.SelectedBCFZip = bcfzipfile;
            var bcffile = new BcfFile();
            try
            {
                if (!File.Exists(bcfzipfile) || (!String.Equals(Path.GetExtension(bcfzipfile), ".bcfzip", StringComparison.InvariantCultureIgnoreCase) && !String.Equals(Path.GetExtension(bcfzipfile), ".bcf", StringComparison.InvariantCultureIgnoreCase)))
                    return bcffile;


                bcffile.Filename = Path.GetFileNameWithoutExtension(bcfzipfile);
                bcffile.Fullname = bcfzipfile;

                using (ZipArchive archive = ZipFile.OpenRead(bcfzipfile))
                {
                    archive.ExtractToDirectory(bcffile.TempPath);
                }

                var dir = new DirectoryInfo(bcffile.TempPath);

                var projectFile = Path.Combine(bcffile.TempPath, "project.bcfp");
                if (File.Exists(projectFile))
                {
                    var project = DeserializeProject(projectFile);
                    var g = Guid.NewGuid();
                    Guid.TryParse(project.Project.ProjectId, out g);
                    bcffile.ProjectId = g;
                }

                //get File Version 
                var versionFile = Path.Combine(bcffile.TempPath, "bcf.version");
                if (File.Exists(versionFile))
                {
                    var version = DeserializeVersion(versionFile);
                    var versionid = version.VersionId;
                    bcffile.VersionId = versionid;

                }





                //ADD ISSUES FOR EACH SUBFOLDER

                foreach (var folder in dir.GetDirectories())
                {
                    //An issue needs at least the markup file
                    var markupFile = Path.Combine(folder.FullName, "markup.bcf");
                    if (!File.Exists(markupFile))
                        continue;

                    var bcfissue = DeserializeMarkup(markupFile);


                    if (bcfissue == null)
                        continue;

                    //Is a BCF 2 file, has multiple viewpoints
                    if (bcfissue.Viewpoints != null && bcfissue.Viewpoints.Any())
                    {
                        foreach (var viewpoint in bcfissue.Viewpoints)
                        {
                            string viewpointpath = Path.Combine(folder.FullName, viewpoint.Viewpoint);
                            if (File.Exists(viewpointpath))
                            {
                                switch (bcffile.VersionId)
                                {
                                    case "3.0":
                                        viewpoint.VisInfo2_1 = DeserializeViewpoint2_1(viewpointpath);
                                        break;
                                    case "2.1":
                                        viewpoint.VisInfo2_1 = DeserializeViewpoint2_1(viewpointpath);
                                        break;
                                    case "2.0":
                                        viewpoint.VisInfo = DeserializeViewpoint(viewpointpath);
                                        break;
                                    default:
                                        break;
                                }
                                //deserializing the viewpoint into the issue
                                viewpoint.SnapshotPath = Path.Combine(folder.FullName, viewpoint.Snapshot);
                            }
                        }
                    }
                    //Is a BCF 1 file, only one viewpoint
                    //there is no Viewpoints tag in the markup
                    //update it to BCF 2
                    else
                    {
                        bcfissue.Viewpoints = new ObservableCollection<ViewPoint>();
                        string viewpointFile = "";
                        var exeFiles = Directory.EnumerateFiles(folder.FullName, "*", SearchOption.AllDirectories).Where(s => s.EndsWith(".bcfv"));

                        switch (bcffile.VersionId)
                        {
                            case "3.0":
                                viewpointFile = exeFiles.FirstOrDefault();
                                break;
                            case "2.1":
                                viewpointFile = Path.Combine(folder.FullName, "viewpoint.bcfv");
                                break;
                            case "2.0":
                                viewpointFile = Path.Combine(folder.FullName, "viewpoint.bcfv");
                                break;

                        }

                        if (File.Exists(viewpointFile))
                        {
                            bcfissue.Viewpoints.Add(new ViewPoint(true)
                            {

                                VisInfo = DeserializeViewpoint(viewpointFile),
                                VisInfo2_1 = DeserializeViewpoint2_1(viewpointFile),
                                SnapshotPath = Path.Combine(folder.FullName, "snapshot.png"),
                            });
                            //update the comments
                            if (bcfissue.Comment.Count != 0)
                                foreach (var comment in bcfissue.Comment)
                                {
                                    comment.Viewpoint = new CommentViewpoint();
                                    comment.Viewpoint.Guid = bcfissue.Viewpoints.First().Guid;
                                }
    

                        }
                    }
                    bcfissue.Comment = new ObservableCollection<Comment>(bcfissue.Comment.OrderBy(x => x.Date));
                    try
                    {
                        bcfissue.Viewpoints = new ObservableCollection<ViewPoint>(bcfissue.Viewpoints.OrderBy(x => x.Index));
                    }
                    catch { }
                    //register the collectionchanged events,
                    //it is needed since deserialization overwrites the ones set in the constructor
                    bcfissue.RegisterEvents();
                    //ViewComment stuff
                    bcffile.Issues.Add(bcfissue);
                }
                try
                {
                    bcffile.Issues = new ObservableCollection<Markup>(bcffile.Issues.OrderBy(x => x.Topic.Index));
                }
                catch { }
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return bcffile;
        }

        /// <summary>
        /// Serializes to a bcfzip and saves it to disk
        /// </summary>
        /// <param name="bcffile"></param>
        /// <returns></returns>
        private static bool SaveBcfFile(BcfFile bcffile)
        {
            try
            {
                if (bcffile.Issues.Count == 0)
                {
                    MessageBox.Show("The current BCF Report is empty.", "No Issue", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (!Directory.Exists(bcffile.TempPath))
                    Directory.CreateDirectory(bcffile.TempPath);
                // Show save file dialog box
                string name = !string.IsNullOrEmpty(bcffile.Filename)
                    ? bcffile.Filename
                    : "New BCF Report";
                string filename = SaveBcfDialog(name + " Updated");

                // Process save file dialog box results
                if (string.IsNullOrWhiteSpace(filename))
                    return false;
                var bcfProject = new ProjectExtension
                {
                    Project = new Project
                    {
                        Name = string.IsNullOrEmpty(bcffile.ProjectName) ? bcffile.Filename : bcffile.ProjectName,
                        ProjectId = bcffile.ProjectId.Equals(Guid.Empty) ? Guid.NewGuid().ToString() : bcffile.ProjectId.ToString()
                    },
                    ExtensionSchema = ""

                };
                var bcfVersion = new Bcf2.Version { VersionId = bcffile.VersionId, DetailedVersion = bcffile.VersionId };

                var serializerP = new XmlSerializer(typeof(ProjectExtension));
                Stream writerP = new FileStream(Path.Combine(bcffile.TempPath, "project.bcfp"), FileMode.Create);
                serializerP.Serialize(writerP, bcfProject);
                writerP.Close();

                var serializerVers = new XmlSerializer(typeof(Bcf2.Version));
                Stream writerVers = new FileStream(Path.Combine(bcffile.TempPath, "bcf.version"), FileMode.Create);
                serializerVers.Serialize(writerVers, bcfVersion);
                writerVers.Close();

                var serializerV = new XmlSerializer(typeof(VisualizationInfo));
                var serializerV2 = new XmlSerializer(typeof(Version2_1.VisualizationInfo));
                var serializerM = new XmlSerializer(typeof(Markup));

                var i = 0;
                foreach (var issue in bcffile.Issues)
                {
                    //set topic index
                    issue.Topic.Index = i;
                    issue.Topic.IndexSpecified = true;
                    i++;

                    // serialize the object, and close the TextWriter
                    string issuePath = Path.Combine(bcffile.TempPath, issue.Topic.Guid);
                    if (!Directory.Exists(issuePath))
                        Directory.CreateDirectory(issuePath);

                    //set viewpoint index
                    for (var l = 0; l < issue.Viewpoints.Count; l++)
                    {
                        issue.Viewpoints[l].Index = l;
                        issue.Viewpoints[l].IndexSpecified = true;
                    }

                    //BCF 1 compatibility
                    //there needs to be a view whose viewpoint and snapshot are named as follows and not with a guid
                    //uniqueness is still guarenteed by the guid field
                    if (issue.Viewpoints.Any() && (issue.Viewpoints.Count == 1 || issue.Viewpoints.All(o => o.Viewpoint != "viewpoint.bcfv")))
                    {
                        if (File.Exists(Path.Combine(issuePath, issue.Viewpoints[0].Viewpoint)))
                            File.Delete(Path.Combine(issuePath, issue.Viewpoints[0].Viewpoint));
                        issue.Viewpoints[0].Viewpoint = "viewpoint.bcfv";
                        if (File.Exists(Path.Combine(issuePath, issue.Viewpoints[0].Snapshot)))
                            File.Move(Path.Combine(issuePath, issue.Viewpoints[0].Snapshot), Path.Combine(issuePath, "snapshot.png"));
                        issue.Viewpoints[0].Snapshot = "snapshot.png";
                    }
                    //serialize markup with updated content
                    Stream writerM = new FileStream(Path.Combine(issuePath, "markup.bcf"), FileMode.Create);
                    serializerM.Serialize(writerM, issue);
                    writerM.Close();
                    //serialize views
                    foreach (var bcfViewpoint in issue.Viewpoints)
                    {
                        Stream writerV = new FileStream(Path.Combine(issuePath, bcfViewpoint.Viewpoint), FileMode.Create);
                        switch (bcffile.VersionId)
                        {
                            case "3.0":
                                serializerV2.Serialize(writerV, bcfViewpoint.VisInfo2_1);

                                break;
                            case "2.1":
                                serializerV2.Serialize(writerV, bcfViewpoint.VisInfo2_1);

                                break;
                            case "2.0":
                                serializerV.Serialize(writerV, bcfViewpoint.VisInfo);

                                break;

                        }
                        writerV.Close();
                    }


                }

                //overwrite, without doubts
                if (File.Exists(filename))
                    File.Delete(filename);

                //added encoder to address backslashes issue #11
                //issue: https://github.com/teocomi/BCFier/issues/11
                //ref: http://stackoverflow.com/questions/27289115/system-io-compression-zipfile-net-4-5-output-zip-in-not-suitable-for-linux-mac
                ZipFile.CreateFromDirectory(bcffile.TempPath, filename, CompressionLevel.Optimal, false, new ZipEncoder());                               ///////// zipping hereerererererererererererere

                //Open browser at location
                Uri uri2 = new Uri(filename);
                string reportname = Path.GetFileName(uri2.LocalPath);

                if (File.Exists(filename))
                {
                    string argument = @"/select, " + filename;
                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
                bcffile.HasBeenSaved = true;
                bcffile.Filename = reportname;
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return true;
        }

        /// <summary>
        /// Prompts a the user to select where to save the bcfzip
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string SaveBcfDialog(string filename)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save as BCF report file (.bcfzip)",
                FileName = filename,
                DefaultExt = ".bcfzip",
                Filter = "BIM Collaboration Format (*.bcfzip)|*.bcfzip"
            };

            //if it goes fine I return the filename, otherwise empty
            var result = saveFileDialog.ShowDialog();
            return result == true ? saveFileDialog.FileName : "";
        }

        private static VisualizationInfo DeserializeViewpoint(string path)
        {
            VisualizationInfo output = null;
            try
            {

                using (var viewpointFile = new FileStream(path, FileMode.Open))
                {
                    var serializerS = new XmlSerializer(typeof(VisualizationInfo));
                    output = serializerS.Deserialize(viewpointFile) as VisualizationInfo;
                }
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return output;
        }
        private static Version2_1.VisualizationInfo DeserializeViewpoint2_1(string path)
        {
            Version2_1.VisualizationInfo output = null;
            try
            {

                using (var viewpointFile = new FileStream(path, FileMode.Open))
                {
                    var serializerS = new XmlSerializer(typeof(Version2_1.VisualizationInfo));
                    output = serializerS.Deserialize(viewpointFile) as Version2_1.VisualizationInfo;
                }
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return output;
        }
        private static Markup DeserializeMarkup(string path)
        {
            Markup output = null;
            try
            {
                using (var markupFile = new FileStream(path, FileMode.Open))
                {
                    var serializerM = new XmlSerializer(typeof(Markup));
                    output = serializerM.Deserialize(markupFile) as Markup;
                }
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return output;
        }

        private static ProjectExtension DeserializeProject(string path)
        {
            ProjectExtension output = null;
            try
            {
                using (var markupFile = new FileStream(path, FileMode.Open))
                {
                    var serializerM = new XmlSerializer(typeof(ProjectExtension));
                    output = serializerM.Deserialize(markupFile) as ProjectExtension;
                }
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return output;
        }
        private static Version DeserializeVersion(string path)
        {
            Version output = null;
            try
            {
                using (var versionFile = new FileStream(path, FileMode.Open))
                {
                    var serializerM = new XmlSerializer(typeof(Version));
                    output = serializerM.Deserialize(versionFile) as Version;
                }
            }
            catch (System.Exception ex1)
            {
                MessageBox.Show("exception: " + ex1);
            }
            return output;
        }



        #endregion

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    class ZipEncoder : UTF8Encoding
    {
        public ZipEncoder()
        {

        }
        public override byte[] GetBytes(string s)
        {
            s = s.Replace("\\", "/");
            return base.GetBytes(s);
        }
    }
}
