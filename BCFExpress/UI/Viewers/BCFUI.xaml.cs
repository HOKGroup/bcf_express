using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BCFExpress.Bcf;
using BCFExpress.Bcf.Bcf2;
using BCFExpress.Data;

using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.UI.Selection;
using BCFExpress.Util;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using BCFExpress.Handlers;
using System;
using System.Windows.Input;
using HOK.SmartBCF.BCFWriter;
using Microsoft.Win32;
using BCFExpress.Data.Utils;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Serialization;
using Result = BCFExpress.Bcf.Bcf2.Result;
using Transform = BCFExpress.Bcf.Bcf2.Transform;

namespace BCFExpress.UI.Viewers
{
    /// <summary>
    /// Interaction logic for BCFUI.xaml
    /// </summary>
    public partial class BCFUI : Window
    {
        //my data context
        private readonly BcfContainer _bcf = new BcfContainer();
        Document _document;
        int _index = 1;
        List<Element> listOfElementsForSelectedIssues = new List<Element>();
        ExternalEvent _ev;
        List<Element> RvtElements = null;
        List<RevitComponent> RvtComponents = null;
        BCFExpressExternalEventHandler m_handler;
        UIApplication m_application;
        public List<Result> results { get; set; }


        public BCFUI(UIApplication _uiapp, ExternalEvent ev, BCFExpressExternalEventHandler bCFExpressExternalEventHandler)
        {

            InitializeComponent();
            _ev = ev;
            m_application = _uiapp;
            results = new List<Result>();
            m_handler = bCFExpressExternalEventHandler;
            HighlightRB.IsChecked = true;
            _document = _uiapp.ActiveUIDocument.Document;
            DataContext = _bcf;
            _bcf.UpdateDropdowns();
            LoadBcfBtn.Click += delegate { _bcf.OpenFile(); _bcf.UpdateDropdowns(); };
            IssueList.ItemsSource = Globals._Issues;
            if (DB.RvtComponents != null && RvtElements != null)
            {

                DB.RvtElements.Clear();
                DB.RvtComponents.Clear();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
             
            #region Navigation key triggers up and down

            if (SectionBoxRB.IsChecked?? false)
            {
                if (e.Key == Key.Up)
                    forwardBtn_Click(forwardBtn, new RoutedEventArgs());

                else if (e.Key == Key.Down)
                    backBtn_Click(backBtn, new RoutedEventArgs());

                else if (e.Key == Key.Right)
                {
                    m_handler.Request.Make(RequestId.GrowSectionBox);
                    _ev.Raise();
                }
                else if (e.Key == Key.Left)
                {
                    m_handler.Request.Make(RequestId.ShrinkSectionBox);
                    _ev.Raise();
                }



            }

            #endregion


            #region Add Status Triggers (1,2,3)
            else if (e.Key == Key.NumPad1 || e.Key == Key.D1)
                AddToResultFile(sender, e);
            else if (e.Key == Key.NumPad2 || e.Key == Key.D2)
                AddToResultFile(sender, e);
            else if (e.Key == Key.NumPad3 || e.Key == Key.D3)
                AddToResultFile(sender, e);
            #endregion


            e.Handled = true;

        }



        private void IssueList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _index = 1;
            //UpdateBackgroundView();
            Indexlbl.Content = _index;
            listOfElementsForSelectedIssues.Clear();


            if (IssueList.SelectedItems.Count != 0)
            {
                foreach (Markup item in IssueList.SelectedItems)
                {
                    var viewPoints = item.Viewpoints;
                    foreach (var viewPoint in viewPoints)
                    {
                        var Components = viewPoint.VisInfo.Components;
                        GetElementFromComponents(Components, _document, out RvtElements, out RvtComponents);
                        listOfElementsForSelectedIssues.AddRange(RvtElements);

                    }
                }
                var categories = listOfElementsForSelectedIssues.GroupBy(x => x.Category.Name);
                List<CategoryView> categoriesView = new List<CategoryView>();
                foreach (var cat in categories)
                {
                    CategoryView categoryView = new CategoryView() { Name = cat.Key, ELementsNumber = cat.Count() };
                    categoriesView.Add(categoryView);
                }
                CategoryList.ItemsSource = categoriesView;
                Countlbl.Content = listOfElementsForSelectedIssues.Count;
                DB.SelectedMarkup = IssueList.SelectedItem as Markup;
                if (RvtElements != null && RvtComponents != null)
                {
                    DB.RvtComponents = RvtComponents;
                    DB.RvtElements = RvtElements;

                }

            }
            DB.SelectedComponent = RvtComponents.FirstOrDefault();
            if (checkIfAnyRadiobuttonChecked())
            {
                if ((bool)HighlightRB.IsChecked)
                    m_handler.Request.Make(RequestId.HighlightElement);
                else if ((bool)IsolateRB.IsChecked)
                    m_handler.Request.Make(RequestId.IsolateElement);
                else if ((bool)SectionBoxRB.IsChecked)
                    m_handler.Request.Make(RequestId.PlaceSectionBox);



                DB.SelectedComponent = RvtComponents.ElementAt(0);
                _ev.Raise();
            }
            else
            {
                MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw Elements");
            }
        }

        public static void GetElementFromComponents(ObservableCollection<Component> components, Document _document, out List<Element> elements, out List<RevitComponent> RvtComponents)
        {
            elements = new List<Element>();
            RvtComponents = new List<RevitComponent>();
            foreach (var component in components)
            {
                var ifcGuid = component.IfcGuid;
                FilteredElementCollector collector = new FilteredElementCollector(_document);
                var Elements = collector.WhereElementIsNotElementType().Where(X => X.Category != null).Where(x => x.LevelId != new ElementId(-1)).ToList();
                if (GetLinkDocuments(_document) != null)
                {
                    FilteredElementCollector linkcollector = new FilteredElementCollector(GetLinkDocuments(_document));
                    var linkElements = linkcollector.WhereElementIsNotElementType().Where(X => X.Category != null).Where(x => x.LevelId != new ElementId(-1)).ToList();
                    if (linkElements != null)
                        Elements.AddRange(linkElements);
                }
                foreach (var element in Elements)
                {
                    if (element != null)
                    {
                        var parameter = element.get_Parameter(BuiltInParameter.IFC_GUID);
                        if (parameter != null && parameter.HasValue)
                        {
                            if (component.IfcGuid == parameter.AsValueString())
                            {
                                RevitComponent RvtComponent = new RevitComponent(component, element, null); ;
                                elements.Add(element);
                                RvtComponents.Add(RvtComponent);
                            }
                        }
                    }

                }
            }


        }
        public static Document GetLinkDocuments(Document doc)
        {
            // Collect all RevitLinkInstance elements in the current document
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> linkInstances = collector.OfClass(typeof(RevitLinkInstance)).ToElements();

            foreach (Element linkInstance in linkInstances)
            {
                // Cast the element to RevitLinkInstance to access its properties
                RevitLinkInstance revitLinkInstance = linkInstance as RevitLinkInstance;

                // Get the linked document
                Document linkedDoc = revitLinkInstance.GetLinkDocument();

                if (linkedDoc != null)
                {
                    return linkedDoc;
                }
            }
            return null;
        }



        public void UpdateBackgroundView()
        {
            try
            {
                m_handler.SetDefaultView();

                //DB.IsHighlightChecked = false;
                //DB.IsIsolateChecked = false;
                //DB.IsSectionBoxChecked = false;


                //m_handler.Request.Make(RequestId.SetViewPointView);
                _ev.Raise();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        private void veryBackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IssueList.SelectedItems.Count == 0 && DB.RvtComponents != null)
                DB.RvtComponents.Clear();
            else if (DB.RvtComponents != null)
            {
                if (checkIfAnyRadiobuttonChecked())
                {
                    if ((bool)HighlightRB.IsChecked)
                        m_handler.Request.Make(RequestId.HighlightElement);
                    else if ((bool)IsolateRB.IsChecked)
                        m_handler.Request.Make(RequestId.IsolateElement);
                    else if ((bool)SectionBoxRB.IsChecked)
                        m_handler.Request.Make(RequestId.PlaceSectionBox);

                    if (DB.RvtComponents != null && DB.RvtComponents.Count != 0)
                    {
                        _index = 1;
                        Indexlbl.Content = _index;
                        int count = _index;

                        DB.SelectedComponent = RvtComponents.ElementAt(count - 1);
                        _ev.Raise();
                    }
                }
                else
                {
                    MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw  Elements");
                }

            }

        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IssueList.SelectedItems.Count == 0 && DB.RvtComponents != null)
                DB.RvtComponents.Clear();
            else if (DB.RvtComponents != null)
            {
                if (checkIfAnyRadiobuttonChecked())
                {
                    if ((bool)HighlightRB.IsChecked)
                        m_handler.Request.Make(RequestId.HighlightElement);
                    else if ((bool)IsolateRB.IsChecked)
                        m_handler.Request.Make(RequestId.IsolateElement);
                    else if ((bool)SectionBoxRB.IsChecked)
                        m_handler.Request.Make(RequestId.PlaceSectionBox);


                    if (_index > 1 && _index != 0)
                    {
                        _index = _index - 1;
                        Indexlbl.Content = _index;
                        int count = _index;
                        DB.SelectedComponent = RvtComponents.ElementAt(count - 1);

                        _ev.Raise();
                    }
                }
                else
                {
                    MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw  Elements");
                }
            }

        }



        private void veryForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IssueList.SelectedItems.Count == 0 && DB.RvtComponents != null)
                DB.RvtComponents.Clear();
            else if (DB.RvtComponents != null)
            {
                if (checkIfAnyRadiobuttonChecked())
                {
                    if ((bool)HighlightRB.IsChecked)
                        m_handler.Request.Make(RequestId.HighlightElement);
                    else if ((bool)IsolateRB.IsChecked)
                        m_handler.Request.Make(RequestId.IsolateElement);
                    else if ((bool)SectionBoxRB.IsChecked)
                        m_handler.Request.Make(RequestId.PlaceSectionBox);


                    _index = listOfElementsForSelectedIssues.Count;
                    Indexlbl.Content = _index;
                    int count = _index;
                    DB.SelectedComponent = RvtComponents.ElementAt(count - 1);
                    _ev.Raise();
                }
                else
                {
                    MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw Elements");
                }

            }
        }

        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IssueList.SelectedItems.Count == 0 && DB.RvtComponents != null)
                DB.RvtComponents.Clear();
            else if (DB.RvtComponents != null)
            {

                if (checkIfAnyRadiobuttonChecked())
                {
                    if ((bool)HighlightRB.IsChecked)
                        m_handler.Request.Make(RequestId.HighlightElement);
                    else if ((bool)IsolateRB.IsChecked)
                        m_handler.Request.Make(RequestId.IsolateElement);
                    else if ((bool)SectionBoxRB.IsChecked)
                        m_handler.Request.Make(RequestId.PlaceSectionBox);


                    if (RvtComponents != null && _index != listOfElementsForSelectedIssues.Count)
                    {
                        _index = _index + 1;
                        Indexlbl.Content = _index;
                        if (_index != 0)
                        {
                            int count = _index;
                            DB.SelectedComponent = RvtComponents.ElementAt(count - 1);
                            _ev.Raise();
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw Elements");
                }
            }
        }

        public void ResultBtn_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer resultSerializer = new XmlSerializer(typeof(ResultContainer));
            ProgressManager.StepForward();
            //string topicDirectory = System.IO.Path.Combine(_bcf.BcfFiles.FirstOrDefault().TempPath, "Result");
            //Directory.CreateDirectory(topicDirectory);
            // Create the "results" folder within the topic directory
            //string resultsFolder = System.IO.Path.Combine(_bcf.BcfFiles.FirstOrDefault().TempPath, new Guid().ToString());
            //Directory.CreateDirectory(resultsFolder);
            ResultContainer resultContainer = new ResultContainer();
            resultContainer.Results = results;
            string resultFilePath = System.IO.Path.Combine(_bcf.BcfFiles.FirstOrDefault().TempPath, $"result_{DB.SelectedComponent.RvtElement.UniqueId}.xml");
            using (FileStream stream = new FileStream(resultFilePath, FileMode.Create))
            {
                resultSerializer.Serialize(stream, resultContainer);
                stream.Close();
            }

        }

        /// <summary>
        /// Add Result file to the bcf file using 1,2,3  keys 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddToResultFile(object sender, RoutedEventArgs e)
        {
            var newE = (KeyEventArgs)e;

            #region This Flag To check wich key the user clicked 
            int Flag = 0;
            if (newE.Key == Key.NumPad1 || newE.Key == Key.D1)
                Flag = 0;
            else if (newE.Key == Key.NumPad2 || newE.Key == Key.D2)
                Flag = 1;
            else if (newE.Key == Key.NumPad3 || newE.Key == Key.D3)
                Flag = 2;

            #endregion
            var bb = DB.SelectedComponent.RvtElement.get_BoundingBox(_document.ActiveView);
            BcfBoundingBoxXYZ bcfBoundingBoxXYZ = null;
            if (bb != null)
            {
                bcfBoundingBoxXYZ = new BcfBoundingBoxXYZ();
                bcfBoundingBoxXYZ.Enabled = bb.Enabled;
                bcfBoundingBoxXYZ.Max = new Max(bb.Max.X, bb.Max.Y, bb.Max.Z);
                bcfBoundingBoxXYZ.Min = new Min(bb.Min.X, bb.Min.Y, bb.Min.Z);
                Origin origin = new Origin(bb.Transform.Origin.X, bb.Transform.Origin.Y, bb.Transform.Origin.Z);
                BasisX basex = new BasisX(bb.Transform.BasisX.X, bb.Transform.BasisX.Y, bb.Transform.BasisX.Z);
                BasisY basey = new BasisY(bb.Transform.BasisY.X, bb.Transform.BasisY.Y, bb.Transform.BasisY.Z);
                BasisZ basez = new BasisZ(bb.Transform.BasisZ.X, bb.Transform.BasisZ.Y, bb.Transform.BasisZ.Z);
                bcfBoundingBoxXYZ.Transform = new Transform(origin, basex, basey, basez);

            }

            Result result = new Result
            {
                SelectedElementRevitUniqueID = DB.SelectedComponent.RvtElement.UniqueId,
                IssueElementUniqueID = DB.SelectedComponent.Guid,
                IssueElementIFCGUID = DB.SelectedComponent.IfcGuid,
                SelectedElementRevitVersionGUID = DB.SelectedComponent.RvtElement.VersionGuid.ToString(),
                SelectedElementBoundingBox = bcfBoundingBoxXYZ,
                IgnoreOrIssue = $"{(Status)Flag}",
                DateTimeStamp = DateTime.Now, // You can use any desired date/time here.
                RevitUserName = m_application.Application.Username // Replace with the actual Revit user name.
            };
            if (results.Any(x => x.IssueElementIFCGUID == result.IssueElementIFCGUID))
                results.Where(x => x.IssueElementIFCGUID == result.IssueElementIFCGUID).FirstOrDefault().IgnoreOrIssue = $"{(Status)Flag}";
            else
                results.Add(result);

        }
        static bool checkIfAnyRadiobuttonChecked()
        {
            if (DB.IsHighlightChecked || DB.IsIsolateChecked || DB.IsSectionBoxChecked)
                return true;
            return false;
        }
        private void HighlightRB_Checked(object sender, RoutedEventArgs e)
        {
            DB.IsHighlightChecked = (bool)HighlightRB.IsChecked;
            m_handler.Request.Make(RequestId.HighlightElement);
        }

        private void IsolateRB_Checked(object sender, RoutedEventArgs e)
        {
            DB.IsIsolateChecked = (bool)IsolateRB.IsChecked;
            m_handler.Request.Make(RequestId.IsolateElement);
        }

        private void SectionBoxRB_Checked(object sender, RoutedEventArgs e)
        {
            DB.IsSectionBoxChecked = (bool)SectionBoxRB.IsChecked;
            m_handler.Request.Make(RequestId.PlaceSectionBox);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            ResultBtn_Click(sender, e);
            _bcf.SaveFile(_bcf.BcfFiles.FirstOrDefault());
        }

        private void LoadBcfBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadBcfBtn.IsEnabled = false;
        }
        //public void ConvertBCFExecuted(object param)
        //{
        //    try
        //    {
        //        if (bcfFiles.Count > 0)
        //        {

        //            //select primary file info
        //            BCFZIP combinedBCF = CombineBCF(bcfFiles, 0);
        //            if (null != combinedBCF)
        //            {

        //                SaveFileDialog saveDialog = new SaveFileDialog();
        //                saveDialog.Title = "Save BCF";
        //                saveDialog.DefaultExt = ".bcfzip";
        //                saveDialog.Filter = "BCF (.bcfzip)|*.bcfzip";
        //                saveDialog.OverwritePrompt = true;
        //                if ((bool)saveDialog.ShowDialog())
        //                {
        //                    string bcfPath = saveDialog.FileName;
        //                    bool saved = BCFWriter.BCFWriter.Write(bcfPath, combinedBCF);
        //                    if (saved)
        //                    {
        //                        MessageBox.Show(bcfPath + "\n has been successfully saved!!", "BCF Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;
        //    }
        //}
    }
    enum Status
    {
        Issue, Ignore, ResultMode_vs_IssueMode
    }
    class CategoryView
    {
        public string Name { get; set; }
        public int ELementsNumber { get; set; }
    }
}
