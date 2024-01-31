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
using System.Threading;
using Version2_1;
using Component = BCFExpress.Bcf.Bcf2.Component;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Emit;
using Autodesk.Revit.DB.Architecture;
using static Autodesk.Internal.Windows.SwfMediaPlayer;
using System.Collections;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media;
using System.Drawing;
using Color = Autodesk.Revit.DB.Color;

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
        ObservableCollection<Tuple<string, ObservableCollection<Element>>> listOfMarkupElements;
        ObservableCollection<Tuple<string, ObservableCollection<RevitComponent>>> listOfMarkupComponentElements;
        ExternalEvent _ev;
        List<Element> RvtElements;
        List<RevitComponent> RvtComponents;
        BCFExpressExternalEventHandler m_handler;
        UIApplication m_application;
        public List<Result> results { get; set; }
        private static bool IsWindowOpen = false;
        private ObservableCollection<ObservableCollection<CategoryView>> CategoriesViews;

        public BCFUI(UIApplication _uiapp, ExternalEvent ev, BCFExpressExternalEventHandler bCFExpressExternalEventHandler)
        {
            InitializeComponent();
            listOfMarkupElements = new ObservableCollection<Tuple<string, ObservableCollection<Element>>>();
            listOfMarkupComponentElements = new ObservableCollection<Tuple<string, ObservableCollection<RevitComponent>>>();
            Loaded += MainWindow_Loaded;
            IsWindowOpen = true;
            _ev = ev;
            m_application = _uiapp;
            results = new List<Result>();

            m_handler = bCFExpressExternalEventHandler;
            HighlightRB.IsChecked = true;
            _document = _uiapp.ActiveUIDocument.Document;
            DataContext = _bcf;
            _bcf.UpdateDropdowns();

            LoadBcfBtn.Click += delegate
            {
                if (!_bcf.OpenFile()) return;

                CategoryList.ItemsSource = null;
                CategoriesViews = new ObservableCollection<ObservableCollection<CategoryView>>();

                listOfMarkupElements.Clear();
                listOfMarkupComponentElements.Clear();
                _document = _uiapp.ActiveUIDocument.Document;
                RvtElements = new List<Element>();
                RvtComponents = new List<RevitComponent>();

                DB.SelectedComponent = null;
                DB.SelectedCategories = null;
                DB.SelectedMarkup = null;
                var startTime = DateTime.Now;
                Debug.WriteLine($"start Time {startTime}");
                startTime = DateTime.Now;
                _bcf.UpdateDropdowns();
                Debug.WriteLine($"Total Seconds {(DateTime.Now - startTime).TotalSeconds}");
                startTime = DateTime.Now;


                Task.Run(() =>
                {
                    GetElement();

                    IssueList.Dispatcher.Invoke(() =>
                    {


                        LoadBcfBtn.IsEnabled = true;
                        //CancelBtn.IsEnabled = true;
                        if (_bcf.BcfFiles.Count != 0)
                        {
                            if (RvtElements.Count == 0)
                            {
                                Indexlbl.Content = '0';
                                TaskDialog.Show("Info... ", "The elements found in loaded BCF file doesn't match the opened revit file!");

                            }

                        }
                        IssueList.SelectedIndex = 0;

                    });
                });
                Debug.WriteLine($"Total Seconds {(DateTime.Now - startTime).TotalSeconds}");


            };
            IssueList.ItemsSource = Globals._Issues;

            //if (IssueList.Items.Count!=0)

            if (DB.RvtComponents != null && RvtElements != null)
            {
                DB.RvtElements.Clear();
                DB.RvtComponents.Clear();
            }
            DB.IsIsolateChecked = false;
            DB.IsSectionBoxChecked = false;


        }


        public string CountElements
        {

            get { return listOfMarkupElements.Count.ToString(); }
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Topmost = true; // Set the window to always be on top
        }



        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsWindowOpen = false; // Reset the flag
            DB.BCFUIs.Clear();
            DB.IsHighlightChecked = false;
        }
     
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            #region Navigation key triggers up and down

            if (SectionBoxRB.IsChecked ?? false || (IsolateRB.IsChecked ?? false))
            {
                if (e.Key == Key.Right)
                    forwardBtn_Click(forwardBtn, new RoutedEventArgs());

                else if (e.Key == Key.Left)
                    backBtn_Click(backBtn, new RoutedEventArgs());

                else if (e.Key == Key.Up)
                {
                    m_handler.Request.Make(RequestId.GrowSectionBox);
                    _ev.Raise();
                }
                else if (e.Key == Key.Down)
                {
                    m_handler.Request.Make(RequestId.ShrinkSectionBox);
                    _ev.Raise();
                }



            }

            #endregion


            #region Add Status Triggers (1,2,3)
            if (e.Key == Key.NumPad1 || e.Key == Key.D1)
                AddToResultFile(sender, e);
            else if (e.Key == Key.NumPad2 || e.Key == Key.D2)
                AddToResultFile(sender, e);

            else if (e.Key == Key.NumPad9 || e.Key == Key.D9)
            {
                //Duplicate View 
                if (IssueList.SelectedItem != null)
                {
                    m_handler.Request.Make(RequestId.DuplicateView);
                    m_handler.IssueName = (IssueList.SelectedItem as Markup).Topic.Title;
                    _ev.Raise();
                }

            }
            #endregion


            e.Handled = true;

        }



        private void IssueList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsWindowOpen)
            {
                _index = 1;
                //UpdateBackgroundView();
                Indexlbl.Content = _index;

                var selectedMarkup = (Markup)IssueList.SelectedItem;
                if (selectedMarkup != null)
                {

                    var MarkupName = selectedMarkup.Topic.Title.ToString();
                    if (listOfMarkupElements.Count > 0)
                    {
                        ObservableCollection<Element> RvtTargetElements = new ObservableCollection<Element>();
                        ObservableCollection<RevitComponent> RvtTargetComponents = new ObservableCollection<RevitComponent>();

                        try
                        {
                            RvtTargetElements = listOfMarkupElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                            RvtTargetComponents = listOfMarkupComponentElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                        }
                        catch (Exception)
                        {


                        }




                        if (IssueList.SelectedItems.Count != 0)
                        {
                            CategoryList.ItemsSource = CategoriesViews[IssueList.SelectedIndex];
                            Countlbl.DataContext = RvtTargetElements;

                            DB.SelectedMarkup = IssueList.SelectedItem as Markup;
                            if (RvtElements == null || RvtComponents == null) return;
                            DB.RvtComponents = RvtTargetComponents.ToList();
                            DB.RvtElements = RvtTargetElements.ToList();
                            //Countlbl.Content = RvtTargetElements.Count;
                            if (RvtComponents.Count != 0)
                            {
                                DB.SelectedComponent = RvtTargetComponents.ElementAt(0);

                            }

                            if (DB.SelectedComponent != null)
                                FamilyTypeLbl.Content = GetFamilyName(DB.SelectedComponent);

                            //FamilyTypeLbl.Foreground = DB.SelectedComponent.ColorBrush;
                        }
                        if (RvtComponents != null)
                        {
                            DB.SelectedComponent = RvtTargetComponents.FirstOrDefault();
                        }


                        if (checkIfAnyRadiobuttonChecked())
                        {
                            //if ((bool)HighlightRB.IsChecked)
                            //    m_handler.Request.Make(RequestId.HighlightElement);
                            if ((bool)IsolateRB.IsChecked)
                                m_handler.Request.Make(RequestId.IsolateElement);
                            else if ((bool)SectionBoxRB.IsChecked)
                                m_handler.Request.Make(RequestId.PlaceSectionBox);


                            if (RvtComponents != null && RvtTargetComponents.Count != 0)
                                DB.SelectedComponent = RvtTargetComponents.ElementAt(0);

                            _ev.Raise();
                        }
                        else
                        {
                            MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw Elements");
                        }
                    }
                }
            }
        }
        string GetFamilyName(RevitComponent revitComponent)
        {
            var rvtElement = revitComponent.RvtElement;
            var Family = rvtElement as FamilyInstance;
            if (Family != null)
            {
                var fSymbole = Family.Symbol as FamilySymbol;
                var Name = fSymbole.FamilyName;
                return $"{Name} : {revitComponent.ElementName}";

            }
            else
            {
                return $"{rvtElement.Category?.Name??""} : {revitComponent.ElementName}";
            }
        }

        private void CategoryView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                e.Handled = true; // Prevent default navigation behavior
            }
        }
        private void IssueView_KeyDown(object sender, KeyEventArgs e)
        {

            e.Handled = true; // Prevent default navigation behavior

        }

        /// <summary>
        /// This method appears to be part of a WPF application and is responsible for iterating through items in an IssueList collection and extracting information about elements and components. The purpose is likely to retrieve and process specific elements and components associated with each issue or markup.
        ///Steps:
        ///Iteration through Markup Items:
        ///The method starts by iterating through each Markup item in the IssueList.
        ///Markup seems to represent an issue or a mark on the model.
        ///Extracting Viewpoints:
        ///Within each Markup, it extracts the Viewpoints associated with it.
        ///Version-based Logic:
        ///The code contains version-specific logic, as indicated by the Globals.BcfFile.VersionId checks.
        ///Looping through Components:
        ///Depending on the version, it retrieves either Components or Components2_1.Selection from each Viewpoint.
        ///These components seem to represent elements or parts of the model relevant to the markup.
        ///Processing Components:
        ///For each component, it extracts the IfcGuid, which likely serves as a unique identifier for the component.
        ///It then searches for the corresponding Revit model element using GetElementByIfcGUID method.
        ///If the element is found, it creates a RevitComponent object and adds it to a collection.
        ///Handling Linked Documents:
        ///If the element is not found in the main document, it searches for it in linked documents using GetLinkDocuments.
        ///If found in a linked document, it creates a RevitComponent for the linked element and adds it to a collection.
        ///Updating UI and Collections:
        ///Throughout the loops, it updates the UI by invoking Dispatcher.Invoke to display the count of collected elements.
        ///After processing each set of components, it updates collections (listOfMarkupElements and listOfMarkupComponentElements).
        ///Important Considerations:
        ///The code appears to manage asynchronous operations(async keyword), which might be necessary for preventing UI freezing during long-running tasks.
        ///The method performs UI updates through the Dispatcher.Invoke to ensure thread-safe interaction with UI elements.
        ///The code handles different BCF file versions (2.0, 2.1, 3.0) and adapts the processing accordingly.
        ///The code seems to rely on various external methods(GetElementByIfcGUID, GetLinkDocuments, UpdateCategoryList) that are not provided in this snippet.Ensure these methods are defined elsewhere in your codebase.
        ///The snippet suggests the code's main function is to gather information about model elements and components associated with specific issues/markups. It's possible that the method could be further refactored and optimized based on specific use cases and requirements.
        /// </summary>

        public void GetElement()
        {

            try
            {

                Dispatcher.Invoke(() =>
                {
                    LoadBcfBtn.IsEnabled = false;


                });


                foreach (Markup mark in IssueList.Items.Cast<Markup>())
                {

                    ObservableCollection<Element> _RvtElements = new ObservableCollection<Element>();
                    ObservableCollection<RevitComponent> _RvtComponents = new ObservableCollection<RevitComponent>();
                    //list of category view 
                    ObservableCollection<CategoryView> categoriesView = new ObservableCollection<CategoryView>();
                    CategoriesViews.Add(categoriesView);

                    var viewPoints = mark.Viewpoints;

                    foreach (var viewPoint in viewPoints)
                    {
                        if (Globals.BcfFile.VersionId == "2.0")
                        {
                            //Thread.Sleep(1000);

                            ObservableCollection<Component> Components = viewPoint.VisInfo.Components;

                            foreach (var component in Components)
                            {
                                var ifcGuid = component.IfcGuid;
                                FilteredElementCollector collector = new FilteredElementCollector(_document);
                                var currentModelElement = GetElementByIfcGUID(_document, ifcGuid);

                                if (currentModelElement != null)
                                {
                                    RevitComponent RvtComponent = new RevitComponent(component, currentModelElement, null);

                                    _RvtElements.Add(currentModelElement);
                                    _RvtComponents.Add(RvtComponent);
                                    UpdateCategoryList(categoriesView, _RvtElements, mark.Topic.Title.ToString());
                                    //continue;
                                }


                                foreach (var linkedDoc in GetLinkDocuments(_document))
                                {
                                    var linkedElement = GetElementByIfcGUID(linkedDoc, ifcGuid);
                                    if (linkedElement != null)
                                    {
                                        RevitComponent RvtComponent = new RevitComponent(component, linkedElement, null); ;
                                        _RvtElements.Add(linkedElement);
                                        _RvtComponents.Add(RvtComponent);
                                        UpdateCategoryList(categoriesView, _RvtElements, mark.Topic.Title.ToString());
                                        break;
                                    }

                                }

                                RvtElements = _RvtElements.ToList();
                                RvtComponents = _RvtComponents.ToList();

                                listOfMarkupElements.Add(new Tuple<string, ObservableCollection<Element>>(mark.Topic.Title, _RvtElements));
                                listOfMarkupComponentElements.Add(new Tuple<string, ObservableCollection<RevitComponent>>(mark.Topic.Title, _RvtComponents));
                                //if (IssueList.SelectedIndex == null)
                                //{
                                //    IssueList.SelectedIndex = 0;

                                //}

                                //Dispatcher.Invoke(() =>
                                //{
                                //    Countlbl.Content = RvtElements.Count;
                                //});

                            }
                        }
                        else if (Globals.BcfFile.VersionId == "2.1" || Globals.BcfFile.VersionId == "3.0")
                        {
                            //Thread.Sleep(1000);
                            var Components2_1 = viewPoint.VisInfo2_1.Components;

                            foreach (var component in Components2_1.Selection)
                            {
                                var ifcGuid = component.IfcGuid;
                                FilteredElementCollector collector = new FilteredElementCollector(_document);
                                var currentModelElement = GetElementByIfcGUID(_document, ifcGuid);

                                if (currentModelElement != null)
                                {
                                    RevitComponent RvtComponent = new RevitComponent(component, currentModelElement, null); ;
                                    _RvtElements.Add(currentModelElement);
                                    _RvtComponents.Add(RvtComponent);
                                    UpdateCategoryList(categoriesView, _RvtElements, mark.Topic.Title.ToString());
                                    //Dispatcher.Invoke(() =>
                                    //{
                                    //    Countlbl.Content = RvtElements.Count;
                                    //});
                                    //continue;
                                }


                                foreach (var linkedDoc in GetLinkDocuments(_document))
                                {
                                    var linkedElement = GetElementByIfcGUID(linkedDoc, ifcGuid);
                                    if (linkedElement != null)
                                    {
                                        RevitComponent RvtComponent = new RevitComponent(component, linkedElement, null); ;
                                        _RvtElements.Add(linkedElement);
                                        _RvtComponents.Add(RvtComponent);
                                        UpdateCategoryList(categoriesView, _RvtElements, mark.Topic.Title.ToString());
                                        //Dispatcher.Invoke(() =>
                                        //{
                                        //    Countlbl.Content = RvtElements.Count;
                                        //});
                                        break;
                                    }

                                }

                                RvtElements.AddRange(_RvtElements.ToList());
                                RvtComponents.AddRange(_RvtComponents.ToList());

                                if (_RvtElements.Any())
                                    listOfMarkupElements.Add(new Tuple<string, ObservableCollection<Element>>(mark.Topic.Title, _RvtElements));
                                if (_RvtComponents.Any())
                                    listOfMarkupComponentElements.Add(new Tuple<string, ObservableCollection<RevitComponent>>(mark.Topic.Title, _RvtComponents));
                                //if (IssueList.SelectedIndex==null)
                                //{
                                //     IssueList.SelectedIndex = 0;

                                //}

                            }
                            //UpdateCategoryList(_RvtElements, mark.Topic.Title.ToString());


                        }
                    }


                }

            }
            catch (Exception)
            {


            }


        }
        private void UpdateCategoryList(ObservableCollection<CategoryView> categoriesView, ObservableCollection<Element> elements, string MarkupName)
        {
            var categories = elements.GroupBy(x => x.Category?.Name);
            if (categoriesView == null)
            {
                categoriesView = new ObservableCollection<CategoryView>();
                //CategoryList.ItemsSource = categoriesView;
            }
            if (categories != null)
                foreach (var cat in categories)
                {
                    CategoryView categoryView = null;
                    var target = categoriesView.FirstOrDefault(x => x.Name == (cat.Key ?? cat.FirstOrDefault().ToString().Split('.').Last()) && x.MarkupName == MarkupName);
                    if (target != null)
                    {

                        Dispatcher.Invoke(() =>
                        {
                            target.ELementsNumber = cat.Count();
                            categoryView = target;
                        });
                    }
                    else
                    {
                        categoryView = new CategoryView();
                        categoryView.MarkupName = MarkupName;
                        categoryView.Name = cat.Key ?? cat.FirstOrDefault().ToString().Split('.').Last();
                        categoryView.ELementsNumber = cat.Count();

                        Dispatcher.Invoke(() =>
                        {
                            categoriesView.Add(categoryView);
                        });
                    }
                    #region Old Code
                    //foreach (var result in results)
                    //{
                    //    if (result.Category != cat.Key) continue;

                    //    switch (result.IgnoreOrIssue)
                    //    {
                    //        case "Issue":
                    //            categoryView.Issue++;
                    //            break;
                    //        case "Ignore":
                    //            categoryView.Ignore++;
                    //            break;
                    //    }

                    //    categoryView.NotAddressed = cat.Count() - categoryView.Issue - categoryView.Ignore;
                    //    categoryView.ELementsNumber = cat.Count() - 0;

                    //} 
                    #endregion
                    categoryView.NotAddressed = cat.Count() - categoryView.Issue - categoryView.Ignore;
                    categoryView.ELementsNumber = cat.Count();





                }
        }

        public void FilterElement(String MarkupName)
        {

            //Countlbl.Content = Count;
            DB.SelectedMarkup = IssueList.SelectedItem as Markup;
            if (RvtElements == null || RvtComponents == null) return;
            DB.RvtComponents = RvtComponents;
            DB.RvtElements = RvtElements;
            if (RvtComponents.Count != 0)
            {
                DB.SelectedComponent = RvtComponents.ElementAt(0);

            }

        }


        public static List<Document> GetLinkDocuments(Document doc)
        {
            // Collect all RevitLinkInstance elements in the current document
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> linkInstances = collector.OfClass(typeof(RevitLinkInstance)).ToElements();
            if (linkInstances != null)
            {

                var links = new List<Document>();
                foreach (Element linkInstance in linkInstances)
                {
                    // Cast the element to RevitLinkInstance to access its properties
                    RevitLinkInstance revitLinkInstance = linkInstance as RevitLinkInstance;

                    // Get the linked document
                    Document linkedDoc = revitLinkInstance.GetLinkDocument();

                    if (linkedDoc != null)
                    {
                        links.Add(linkedDoc);
                    }
                }
                return links;
            }
            return null;
        }

        public static Element GetElementByIfcGUID(Document doc, string ifcGUID)
        {
            // Filter elements by the IFC GUID parameter
            FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            try
            {

                collector.WherePasses(new ElementParameterFilter(new FilterStringRule(new ParameterValueProvider(new ElementId(BuiltInParameter.IFC_GUID)), new FilterStringEquals(), ifcGUID)));

                var element = collector.FirstOrDefault();
                return element;

            }
            catch (Exception)
            {


            }
            // Return the first element found with the matching IFC GUID


            return null;
        }

        //public void UpdateBackgroundView()
        //{
        //    try
        //    {
        //        m_handler.Request.Make(RequestId.ApplyViews);

        //        _ev.Raise();
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ex.Message;
        //    }
        //}

        private void veryBackBtn_Click(object sender, RoutedEventArgs e)
        {

            var selectedMarkup = (Markup)IssueList.SelectedItem;
            if (selectedMarkup != null)
            {
                var MarkupName = selectedMarkup.Topic.Title.ToString();
                ObservableCollection<Element> RvtTargetElements = new ObservableCollection<Element>();
                ObservableCollection<RevitComponent> RvtTargetComponents = new ObservableCollection<RevitComponent>();

                try
                {
                    RvtTargetElements = listOfMarkupElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                    RvtTargetComponents = listOfMarkupComponentElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't Find Any Element in Revit model match in this Markup");
                    return;

                }
                if (IssueList.SelectedItems.Count == 0 && DB.RvtComponents != null)
                    DB.RvtComponents.Clear();
                else if (DB.RvtComponents != null)
                {
                    if (checkIfAnyRadiobuttonChecked())
                    {
                        if ((bool)HighlightRB.IsChecked)
                            m_handler.Request.Make(RequestId.HighlightElement);
                        if ((bool)IsolateRB.IsChecked)
                            m_handler.Request.Make(RequestId.IsolateElement);
                        else if ((bool)SectionBoxRB.IsChecked)
                            m_handler.Request.Make(RequestId.PlaceSectionBox);

                        if (DB.RvtComponents != null && DB.RvtComponents.Count != 0)
                        {
                            _index = 1;
                            Indexlbl.Content = _index;
                            int count = _index;

                            DB.SelectedComponent = RvtTargetComponents.ElementAt(count - 1);
                            DB.SelectedComponent.ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                            FamilyTypeLbl.Content = GetFamilyName(RvtTargetComponents[count - 1]);
                            if (RvtTargetComponents[count - 1].ColorBrush != null)
                            {
                                FamilyTypeLbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;
                                Indexlbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;


                            }
                            else
                            {
                                FamilyTypeLbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                                Indexlbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

                            }

                            _ev.Raise();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw  Elements");
                    }

                }
            }

        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {

            var selectedMarkup = (Markup)IssueList.SelectedItem;
            if (selectedMarkup != null)
            {
                var MarkupName = selectedMarkup.Topic.Title.ToString();
                ObservableCollection<Element> RvtTargetElements = new ObservableCollection<Element>();
                ObservableCollection<RevitComponent> RvtTargetComponents = new ObservableCollection<RevitComponent>();

                try
                {
                    RvtTargetElements = listOfMarkupElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                    RvtTargetComponents = listOfMarkupComponentElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                }
                catch (Exception)
                {

                    MessageBox.Show("Can't Find Any Element in Revit model match in this Markup");
                    return;
                }
                if (IssueList.SelectedItems.Count == 0 && DB.RvtComponents != null)
                    DB.RvtComponents.Clear();
                else if (DB.RvtComponents != null)
                {
                    if (checkIfAnyRadiobuttonChecked())
                    {
                        if ((bool)HighlightRB.IsChecked)
                            m_handler.Request.Make(RequestId.HighlightElement);
                        if ((bool)IsolateRB.IsChecked)
                            m_handler.Request.Make(RequestId.IsolateElement);
                        else if ((bool)SectionBoxRB.IsChecked)
                            m_handler.Request.Make(RequestId.PlaceSectionBox);


                        if (_index > 1 && _index != 0)
                        {
                            _index = _index - 1;
                            Indexlbl.Content = _index;
                            int count = _index;

                            DB.SelectedComponent = RvtTargetComponents.ElementAt(count - 1);
                            //DB.SelectedComponent.ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                            FamilyTypeLbl.Content = GetFamilyName(RvtTargetComponents[count - 1]);
                            if (RvtTargetComponents[count - 1].ColorBrush != null)
                            {
                                FamilyTypeLbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;
                                Indexlbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;


                            }
                            else
                            {
                                FamilyTypeLbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                                Indexlbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

                            }

                            _ev.Raise();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw  Elements");
                    }
                }
            }

        }



        private void veryForwardBtn_Click(object sender, RoutedEventArgs e)
        {

            var selectedMarkup = (Markup)IssueList.SelectedItem;
            if (selectedMarkup != null)
            {
                var MarkupName = selectedMarkup.Topic.Title.ToString();
                ObservableCollection<Element> RvtTargetElements = new ObservableCollection<Element>();
                ObservableCollection<RevitComponent> RvtTargetComponents = new ObservableCollection<RevitComponent>();

                try
                {
                    RvtTargetElements = listOfMarkupElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                    RvtTargetComponents = listOfMarkupComponentElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                }
                catch (Exception)
                {

                    MessageBox.Show("Can't Find Any Element in Revit model match in this Markup");
                    return;
                }
                if (IssueList.SelectedItems.Count == 0 && DB.RvtComponents != null)
                    DB.RvtComponents.Clear();
                else if (DB.RvtComponents != null)
                {
                    if (checkIfAnyRadiobuttonChecked())
                    {
                        if ((bool)HighlightRB.IsChecked)
                            m_handler.Request.Make(RequestId.HighlightElement);
                        if ((bool)IsolateRB.IsChecked)
                            m_handler.Request.Make(RequestId.IsolateElement);
                        else if ((bool)SectionBoxRB.IsChecked)
                            m_handler.Request.Make(RequestId.PlaceSectionBox);


                        _index = RvtTargetElements.Count();
                        Indexlbl.Content = _index;
                        int count = _index;

                        DB.SelectedComponent = RvtTargetComponents.ElementAt(count - 1);
                        //DB.SelectedComponent.ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                        FamilyTypeLbl.Content = GetFamilyName(RvtTargetComponents[count - 1]);
                        if (RvtTargetComponents[count - 1].ColorBrush != null)
                        {
                            FamilyTypeLbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;
                            Indexlbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;


                        }
                        else
                        {
                            FamilyTypeLbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                            Indexlbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

                        }

                        _ev.Raise();
                    }
                    else
                    {
                        MessageBox.Show("Please Choose One Option from This \n 1 - Highlight Element \n 2 - Isolate Element \n 3 - Make Section Box \nTo navigate Throw Elements");
                    }

                }
            }
        }

        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {

            var selectedMarkup = (Markup)IssueList.SelectedItem;
            if (selectedMarkup != null)
            {
                var MarkupName = selectedMarkup.Topic.Title.ToString();
                ObservableCollection<Element> RvtTargetElements = new ObservableCollection<Element>();
                ObservableCollection<RevitComponent> RvtTargetComponents = new ObservableCollection<RevitComponent>();

                try
                {
                    RvtTargetElements = listOfMarkupElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                    RvtTargetComponents = listOfMarkupComponentElements.Where(x => x.Item1 == MarkupName).FirstOrDefault().Item2;
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't Find Any Element in Revit model match in this Markup");
                    return;

                }
                if (IssueList.SelectedItems.Count == 0)
                    DB.RvtComponents.Clear();
                else if (DB.RvtComponents != null)
                {

                    if (checkIfAnyRadiobuttonChecked())
                    {
                        if ((bool)HighlightRB.IsChecked)
                            m_handler.Request.Make(RequestId.HighlightElement);
                        if ((bool)IsolateRB.IsChecked)
                            m_handler.Request.Make(RequestId.IsolateElement);
                        else if ((bool)SectionBoxRB.IsChecked)
                            m_handler.Request.Make(RequestId.PlaceSectionBox);


                        if (RvtComponents != null && _index != RvtTargetComponents.Count)
                        {
                            _index = _index + 1;
                            Indexlbl.Content = _index;
                            if (_index != 0)
                            {
                                int count = _index;
                                DB.SelectedComponent = RvtTargetComponents.ElementAt(count - 1);
                                //DB.SelectedComponent.ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                                FamilyTypeLbl.Content = GetFamilyName(RvtTargetComponents[count - 1]);
                                if (RvtTargetComponents[count - 1].ColorBrush != null)
                                {
                                    FamilyTypeLbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;
                                    Indexlbl.Foreground = RvtTargetComponents[count - 1].ColorBrush;


                                }
                                else
                                {
                                    FamilyTypeLbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                                    Indexlbl.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));

                                }

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

            #region Add Bounding Box Data 
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
            #endregion

            Result result = new Result
            {
                Category = DB.SelectedComponent.RvtElement.Category.Name,
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


            if (Flag == 0)
            {
                OverrideGraphicSettings overrideSettings = _document.ActiveView.GetElementOverrides(DB.SelectedComponent.RvtElement.Id);
                overrideSettings.SetProjectionLineColor(new Color(255, 0, 0)); // Red color
                overrideSettings.SetSurfaceForegroundPatternId(GetSolidFillPatternId(_document)); // Red color
                overrideSettings.SetSurfaceBackgroundPatternId(GetSolidFillPatternId(_document)); // Red color
                overrideSettings.SetSurfaceForegroundPatternColor(new Color(255, 0, 0)); // Red color
                overrideSettings.SetSurfaceBackgroundPatternColor(new Color(255, 0, 0)); // Red color
                _document.ActiveView.SetElementOverrides(DB.SelectedComponent.RvtElement.Id, overrideSettings);
                //_ev.Raise();
                DB.SelectedComponent.ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 0, 0));
                FamilyTypeLbl.Foreground = DB.SelectedComponent.ColorBrush;
                Indexlbl.Foreground = DB.SelectedComponent.ColorBrush;


            }
            else if (Flag == 1)
            {
                OverrideGraphicSettings overrideSettings = _document.ActiveView.GetElementOverrides(DB.SelectedComponent.RvtElement.Id);
                overrideSettings.SetProjectionLineColor(new Color(0, 255, 0));
                overrideSettings.SetSurfaceForegroundPatternId(GetSolidFillPatternId(_document)); // Red color
                overrideSettings.SetSurfaceBackgroundPatternId(GetSolidFillPatternId(_document)); // Red color
                overrideSettings.SetSurfaceForegroundPatternColor(new Color(0, 255, 0)); // Red color
                overrideSettings.SetSurfaceBackgroundPatternColor(new Color(0, 255, 0)); // Red color
                _document.ActiveView.SetElementOverrides(DB.SelectedComponent.RvtElement.Id, overrideSettings);
                //_ev.Raise();
                DB.SelectedComponent.ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 21, 175, 59));
                FamilyTypeLbl.Foreground = DB.SelectedComponent.ColorBrush;
                Indexlbl.Foreground = DB.SelectedComponent.ColorBrush;


            }




            #region Update Category List 
            var selectedMarkup = (Markup)IssueList.SelectedItem;
            var categories = listOfMarkupElements.Where(x => x.Item1 == selectedMarkup.Topic.Title).FirstOrDefault().Item2.GroupBy(x => x.Category?.Name ?? "No Name");
            //RvtElements = listOfMarkupElements.Where(_ => _.Item1 == selectedMarkup.Topic.Title).FirstOrDefault().Item2;
            //RvtComponents = listOfMarkupComponentElements.Where(_ => _.Item1 == selectedMarkup.Topic.Title).FirstOrDefault().Item2;


            //////////////////////////////////////////////////////
            var TargetCategoryview = CategoriesViews[IssueList.SelectedIndex];
            var categoriesView = new ObservableCollection<CategoryView>();
            foreach (var cat in categories)
            {
                var Issue = results.Where(x => x.IgnoreOrIssue.ToString() == "Issue" && x.Category == cat.Key);
                var Ignore = results.Where(x => x.IgnoreOrIssue == "Ignore" && x.Category == cat.Key);

                CategoryView categoryView = new CategoryView() { Name = cat.Key, ELementsNumber = cat.Count(), Issue = Issue.Count(), Ignore = Ignore.Count(), NotAddressed = cat.Count() - Issue.Count() - Ignore.Count() };
                categoriesView.Add(categoryView);
            }

            CategoryList.ItemsSource = categoriesView;
            #endregion



        }
        // Function to get the ElementId of the Solid fill pattern
        private ElementId GetSolidFillPatternId(Document doc)
        {
            // Retrieve the solid fill pattern element
            FilteredElementCollector patternCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(FillPatternElement));

            foreach (FillPatternElement fillPatternElement in patternCollector)
            {
                FillPattern fillPattern = fillPatternElement.GetFillPattern();
                if (fillPattern.IsSolidFill)
                {
                    return fillPatternElement.Id;
                }
            }

            return ElementId.InvalidElementId; // If solid fill pattern is not found
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

        }
        private void HighlightRB_Unchecked(object sender, RoutedEventArgs e)
        {
            DB.IsHighlightChecked = (bool)HighlightRB.IsChecked;
        }

        private void IsolateRB_Checked(object sender, RoutedEventArgs e)
        {
            DB.IsIsolateChecked = (bool)IsolateRB.IsChecked;
            m_handler.Request.Make(RequestId.IsolateElement);
            if (IssueList.Items.Count != 0)
            {
                _ev.Raise();

            }
        }

        private void SectionBoxRB_Checked(object sender, RoutedEventArgs e)
        {
            DB.IsSectionBoxChecked = (bool)SectionBoxRB.IsChecked;
            m_handler.Request.Make(RequestId.PlaceSectionBox);
            if (IssueList.Items.Count != 0)
            {
                _ev.Raise();
            }
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



    }
    enum Status
    {
        Issue, Ignore, ResultMode_vs_IssueMode
    }
    class CategoryView : INotifyPropertyChanged
    {
        public string MarkupName { get; set; }
        public string Name { get; set; }



        private int elementNumber;
        public int ELementsNumber
        {
            get { return elementNumber; }
            set
            {
                elementNumber = value;
                onPropertyChanged();
            }
        }

        private int issue;
        public int Issue
        {
            get { return issue; }
            set
            {
                issue = value;
                onPropertyChanged();
            }
        }

        private int ignore;
        public int Ignore
        {
            get { return ignore; }
            set
            {
                ignore = value;
                onPropertyChanged();
            }
        }

        private int notAddressed;
        public int NotAddressed
        {
            get { return notAddressed; }
            set
            {
                notAddressed = value;
                onPropertyChanged();
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        public void onPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
