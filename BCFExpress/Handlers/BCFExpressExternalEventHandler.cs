using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BCFExpress.Bcf;
using BCFExpress.Bcf.Bcf2;
using BCFExpress.Helper;
using BCFExpress.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BCFExpress.Handlers
{
    public class BCFExpressExternalEventHandler : IExternalEventHandler
    {
        private UIApplication m_app;
        private readonly string addinDefinitionFile;
        public Document ActiveDoc { get; set; }
        public View3D BCFOrthoView { get; set; }
        public View3D BCFPersView { get; set; }
        public UIView BCFUIView { get; set; }
        public Request Request { get; } = new Request();
        public string IssueName { get; set; }
        public RevitComponent SelectedRevitComponent { get; set; }

        public string DatabaseFile { get; set; } = "";

        public BCFExpressExternalEventHandler(UIApplication uiapp)
        {
            m_app = uiapp;
            ActiveDoc = uiapp.ActiveUIDocument.Document;

        }
        public void Execute(UIApplication app)
        {

            var result = Request.Take();
            var selectedComponent = DB.SelectedComponent;
            if (null == selectedComponent) return;
            if (BCFUIView == null)
                SetViewPointView();
            ActiveDoc = app.ActiveUIDocument.Document;
            try
            {
                switch (result)
                {
                    case RequestId.ApplyViews:
                        // (Jinsol) When component selection changed
                        if (null != BCFOrthoView)
                        {
                            CleanViewSettings(BCFOrthoView, true, true, true);

                            SetDefaultView();
                            //if (DB.IsHighlightChecked) HighlightElements();
                            if (DB.IsIsolateChecked) IsolateElement(BCFOrthoView);
                            else if (DB.IsSectionBoxChecked) PlaceSectionBox(BCFOrthoView);
                        }
                        break;
                    //case RequestId.HighlightElement:
                    //    if (null != BCFOrthoView)
                    //    {
                    //        if (DB.IsHighlightChecked) HighlightElements();

                    //        CleanViewSettings(BCFOrthoView, true, false, false);
                    //    }
                    //    break;
                    case RequestId.IsolateElement:
                        if (null != BCFOrthoView)
                        {
                            CleanViewSettings(BCFOrthoView, false, true, true);
                            SetDefaultView();

                            if (DB.IsIsolateChecked) IsolateElement(BCFOrthoView);
                            else SetViewPointView();
                            if (DB.IsHighlightChecked) HighlightElements();
                        }
                        break;
                    case RequestId.PlaceSectionBox:
                        if (null != BCFOrthoView)
                        {
                            CleanViewSettings(BCFOrthoView, false, true, true);
                            SetDefaultView();

                            if (DB.IsSectionBoxChecked) PlaceSectionBox(BCFOrthoView);
                            else SetViewPointView();
                            if (DB.IsHighlightChecked) HighlightElements();

                        }
                        break;

                    case RequestId.SetViewPointView:
                        // (Jinsol) When markup topic changed
                        SetViewPointView();
                        break;
                    case RequestId.GrowSectionBox:
                        if (null != BCFOrthoView)
                        {
                            //CleanViewSettings(BCFOrthoView, false, true, false);
                            //SetDefaultView();

                            if (DB.IsSectionBoxChecked) GrowShrinkSectionBox(BCFOrthoView, true);
                            else SetViewPointView();
                        }

                        break;
                    case RequestId.ShrinkSectionBox:
                        if (null != BCFOrthoView)
                        {
                            CleanViewSettings(BCFOrthoView, false, true, false);
                            SetDefaultView();

                            if (DB.IsSectionBoxChecked) GrowShrinkSectionBox(BCFOrthoView, false);
                            else SetViewPointView();
                        }
                        break;
                    case RequestId.DuplicateView:
                        if (null != BCFOrthoView)
                        {
                            DuplicateView(BCFOrthoView);

                        }
                        break;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to execute the external event.\n" + ex.Message, "Execute Event", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DuplicateView(View3D bcfView)
        {
            try
            {
                Transaction tr = new Transaction(ActiveDoc, "Duplicate View");

                tr.Start();
                ViewDuplicateOption duplicateOption = ViewDuplicateOption.Duplicate;

                var duplicated = ActiveDoc.GetElement(bcfView.Duplicate(duplicateOption)) as View;
                duplicated.Name = $"{BCFExpress.Properties.Settings.Default.DefaultViewPrefix} - {IssueName}/{DB.SelectedComponent.RvtElement.Id} - " + DateTime.Now.ToString("dd/MM/yyyy");
                tr.Commit();
            }
            catch (Exception)
            {

            }

        }
        private void UnhideRevitLinks(View view , Document doc)
        {

            //find the linked files
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> elementIdSet =
              collector
              .OfCategory(BuiltInCategory.OST_RvtLinks)
              .OfClass(typeof(RevitLinkType))
              .ToElementIds();

            foreach (ElementId linkedFileId in elementIdSet)
                {
                    if (linkedFileId != null)
                    {
                        if (true == doc.GetElement(linkedFileId).IsHidden(view))
                        {
                            if (true == doc.GetElement(linkedFileId).CanBeHidden(view))
                            {
                                view.UnhideElements(elementIdSet);
                                doc.Regenerate();
                            }
                        }
                        else
                        {
                            view.HideElements(elementIdSet);
                        }
                    }
                }
               
           
        }

        private void GrowShrinkSectionBox(View3D bcfView, bool grow)
        {


            var selectedComponent = DB.SelectedComponent;
            if (null == selectedComponent) return;

            var factor = grow ? 1 : -1;

            using (var trans = new Transaction(ActiveDoc))
            {
                trans.Start("Grow Section Box");
                try
                {

                    var boundingBox = bcfView.GetSectionBox();
                    var minXYZ = boundingBox.Min;
                    var maxXYZ = boundingBox.Max;


                    var offsetBox = new BoundingBoxXYZ();
                    offsetBox.Min = new XYZ(minXYZ.X - factor, minXYZ.Y - factor, minXYZ.Z - factor);
                    offsetBox.Max = new XYZ(maxXYZ.X + factor, maxXYZ.Y + factor, maxXYZ.Z + factor);

                    var element = selectedComponent.RvtElement;
                    if (element != null)
                    {
                        var transform = selectedComponent.TransformValue;
                        var eleBoundingBox = element.get_BoundingBox(null);
                        var eleMinXYZ = transform.OfPoint(eleBoundingBox.Min);
                        var eleMaxXYZ = transform.OfPoint(eleBoundingBox.Max);

                        // check if the bounding box is smaller than the element itself, if so, set the section box to the element's bounding box
                        if (eleMinXYZ.X < offsetBox.Min.X || eleMinXYZ.Y < offsetBox.Min.Y ||
                            eleMinXYZ.Z < offsetBox.Min.Z || offsetBox.Max.X < eleMaxXYZ.X ||
                            offsetBox.Max.Y < eleMaxXYZ.Y || offsetBox.Max.Z < eleMaxXYZ.Z)
                        {
                            offsetBox = eleBoundingBox;
                        }
                    }

                    bcfView.SetSectionBox(offsetBox);
                    bcfView.GetSectionBox().Enabled = true;

                    //BCFUIView.ZoomAndCenterRectangle(offsetBox.Min, offsetBox.Max);
                    //BCFUIView.Zoom(0.8);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to set sectionbox.\n" + ex.Message, "Set Section Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    trans.RollBack();
                }
            }
        }

        private void HighlightElements()
        {
            if (null == DB.SelectedComponent) return;

            try
            {
                var uidoc = new UIDocument(ActiveDoc);
                var selectedIds = new List<ElementId>();
                selectedIds.Add(DB.SelectedComponent.ElementId);
                uidoc.Selection.SetElementIds(selectedIds);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bcfView"></param>
        private void IsolateElement(View3D bcfView)
        {
            PlaceSectionBox(bcfView);
            var selectedComponent = DB.SelectedComponent;
            if (null == selectedComponent) return;

            using (var trans = new Transaction(ActiveDoc))
            {
                trans.Start("Isolate View");
                try
                {

                    ICollection<ElementId> elements = new List<ElementId>();
                    elements.Add(selectedComponent.ElementId);

                    var element = selectedComponent.RvtElement;
                    if (element != null)
                    {
                        var boundingBox = element.get_BoundingBox(null);
                        if (boundingBox != null)
                        {
                            BCFUIView.ZoomAndCenterRectangle(boundingBox.Max, boundingBox.Min);
                            BCFUIView.Zoom(.6);

                        }
                    }
                    bcfView.UnhideElements(elements);
                    bcfView.IsolateElementTemporary(selectedComponent.ElementId);
                    bcfView.ConvertTemporaryHideIsolateToPermanent();
                    //bcfView.IsSectionBoxActive = false;
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.RollBack();
                }
            }
        }
        /// <summary>
        /// Return a unit vector in the specified direction.
        /// </summary>
        /// <param name="angleHorizD">Angle in XY plane 
        /// in degrees</param>
        /// <param name="angleVertD">Vertical tilt between 
        /// -90 and +90 degrees</param>
        /// <returns>Unit vector in the specified 
        /// direction.</returns>
        private XYZ VectorFromHorizVertAngles(
          double angleHorizD,
          double angleVertD)
        {
            // Convert degreess to radians.

            double degToRadian = Math.PI * 2 / 360;
            double angleHorizR = angleHorizD * degToRadian;
            double angleVertR = angleVertD * degToRadian;

            // Return unit vector in 3D

            double a = Math.Cos(angleVertR);
            double b = Math.Cos(angleHorizR);
            double c = Math.Sin(angleHorizR);
            double d = Math.Sin(angleVertR);

            return new XYZ(a * b, a * c, d);
        }
        private bool SetViewPointView()
        {
            var updated = false;
            try
            {
                //refresh background view
                if (null == BCFPersView)
                {
                    BCFPersView = CreateDefaultPersView();
                }
                if (null == BCFOrthoView)
                {
                    BCFOrthoView = CreateDefaultOrthoView();
                }

                SetDefaultView();
                VisualizationInfo visInfo = null;
                Version2_1.VisualizationInfo visInfo2_1 = null;
                if (DB.SelectedMarkup != null)
                {
                    visInfo = DB.SelectedMarkup.Viewpoints.FirstOrDefault().VisInfo;
                    visInfo2_1 = DB.SelectedMarkup.Viewpoints.FirstOrDefault().VisInfo2_1;
                }

                if (visInfo != null)
                {
                    if (DB.RvtComponents.Count > 0)
                    {
                        //create orthoview to iterate elements
                        CleanViewSettings(BCFOrthoView, true, true, true);

                        BCFUIView = FindDefaultUIView(BCFOrthoView);

                        if (visInfo.IsPersepective)
                        {
                            updated = SetOrthogonalView(BCFOrthoView, visInfo.PerspectiveCamera);
                        }
                        else
                        {
                            updated = SetOrthogonalView(BCFOrthoView, visInfo.OrthogonalCamera);
                        }
                    }
                    else
                    {
                        if (visInfo.IsPersepective && null != BCFPersView)
                        {
                            //PerspectiveCamera
                            CleanViewSettings(BCFPersView, true, true, true);

                            updated = SetPerspectiveView(BCFPersView, visInfo.PerspectiveCamera);
                            m_app.ActiveUIDocument.ActiveView = BCFPersView;
                            BCFUIView = FindDefaultUIView(BCFPersView);
                        }
                        else if (!visInfo.IsPersepective && null != BCFOrthoView)
                        {
                            //OrthogonalCamera
                            CleanViewSettings(BCFOrthoView, true, true, true);

                            BCFUIView = FindDefaultUIView(BCFOrthoView);

                            updated = SetOrthogonalView(BCFOrthoView, visInfo.OrthogonalCamera);
                        }
                    }
                }
                else if (visInfo2_1 != null)
                {
                    if (DB.RvtComponents != null)
                    {
                        //create orthoview to iterate elements
                        CleanViewSettings(BCFOrthoView, true, true, true);

                        BCFUIView = FindDefaultUIView(BCFOrthoView);

                        if (true)
                        {
                            updated = SetOrthogonalView(BCFOrthoView, visInfo2_1.PerspectiveCamera);
                        }
                        else
                        {
                            updated = SetOrthogonalView(BCFOrthoView, visInfo2_1.OrthogonalCamera);
                        }
                    }
                    else
                    {
                        if (null != BCFPersView)
                        {
                            //PerspectiveCamera
                            CleanViewSettings(BCFPersView, true, true, true);

                            updated = SetPerspectiveView(BCFPersView, visInfo2_1.PerspectiveCamera);
                            m_app.ActiveUIDocument.ActiveView = BCFPersView;
                            BCFUIView = FindDefaultUIView(BCFPersView);
                        }
                        else if (null != BCFOrthoView)
                        {
                            //OrthogonalCamera
                            CleanViewSettings(BCFOrthoView, true, true, true);

                            BCFUIView = FindDefaultUIView(BCFOrthoView);

                            updated = SetOrthogonalView(BCFOrthoView, visInfo2_1.OrthogonalCamera);
                        }
                    }
                }

                m_app.ActiveUIDocument.RefreshActiveView();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return updated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bcfView"></param>
        private void PlaceSectionBox(View3D bcfView)
        {
            var selectedComponent = DB.SelectedComponent;
            if (null == selectedComponent) return;

            using (var trans = new Transaction(ActiveDoc))
            {
                trans.Start("Set Section Box");
                try
                {
                    UnhideRevitLinks(bcfView, m_app.ActiveUIDocument.Document);

                    var element = selectedComponent.RvtElement;
                    if (null != element)
                    {
                        var transform = selectedComponent.TransformValue;
                        BoundingBoxXYZ boundingBox = null;
                        try
                        {

                            boundingBox = element.get_BoundingBox(null);
                            if (boundingBox == null) throw new Exception() ;

                        }
                        catch (Exception)
                        {

                            MessageBox.Show("This element can't be presented as it not model element\nand not have a physical geometry.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                            trans.RollBack();
                        }

                        var minXYZ = transform.OfPoint(boundingBox.Min);
                        var maxXYZ = transform.OfPoint(boundingBox.Max);
                        var offsetBox = new BoundingBoxXYZ();
                        offsetBox.Min = new XYZ(minXYZ.X - 3, minXYZ.Y - 3, minXYZ.Z - 3);
                        offsetBox.Max = new XYZ(maxXYZ.X + 3, maxXYZ.Y + 3, maxXYZ.Z + 3);

                        // Create a BoundingBoxIntersectsFilter to filter elements within the specified bounding box
                        BoundingBoxIntersectsFilter boundingBoxFilter = new BoundingBoxIntersectsFilter(new Outline(boundingBox.Min, boundingBox.Max));

                        // Create a FilteredElementCollector with the specified filter
                        var elementCollector = new FilteredElementCollector(ActiveDoc)
                            .WherePasses(boundingBoxFilter).ToElementIds();

                        //get section box 
                        var fec = new FilteredElementCollector(ActiveDoc);
                        var sections = fec.OfCategory(BuiltInCategory.OST_SectionBox).WhereElementIsNotElementType().ToElementIds();

                        if (elementCollector.Count != 0 && sections.Count != 0)
                        {
                            bcfView.UnhideElements(elementCollector);
                            bcfView.UnhideElements(sections);
                        }

                        bcfView.SetSectionBox(offsetBox);

                        bcfView.GetSectionBox().Enabled = true;
                        //
                        XYZ eye = XYZ.Zero;

                        XYZ forward = VectorFromHorizVertAngles(
                          -45, -30);

                        XYZ up = VectorFromHorizVertAngles(
                         -45, -30 + 90);

                        bcfView.SetOrientation(new ViewOrientation3D(eye, up, forward));


                        BCFUIView.ZoomAndCenterRectangle(offsetBox.Min, offsetBox.Max);
                        BCFUIView.Zoom(0.5);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to set sectionbox.\n" + ex.Message, "Set Section Box", MessageBoxButton.OK, MessageBoxImage.Warning);
                    trans.RollBack();
                }
            }
        }
        public bool SetDefaultView()
        {
            var result = false;
            try
            {
                if (null != BCFOrthoView)
                {
                    m_app.ActiveUIDocument.ActiveView = BCFOrthoView;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return result;
        }

        private bool SetOrthogonalView(View3D bcfView, OrthogonalCamera camera)
        {
            var result = false;
            try
            {
                var zoom = camera.ViewToWorldScale.ToFeet();
                var direction = RevitUtils.GetRevitXYZ(camera.CameraDirection);
                var upVector = RevitUtils.GetRevitXYZ(camera.CameraUpVector);
                var viewPoint = RevitUtils.GetRevitXYZ(camera.CameraViewPoint);
                var orientation = RevitUtils.ConvertBasePoint(ActiveDoc, viewPoint, direction, upVector, true);

                using (var trans = new Transaction(ActiveDoc))
                {
                    trans.Start("Set Orientation");
                    try
                    {
                        bcfView.SetOrientation(orientation);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.RollBack();
                        var message = ex.Message;
                    }
                }

                var m_xyzTl = bcfView.Origin.Add(bcfView.UpDirection.Multiply(zoom)).Subtract(bcfView.RightDirection.Multiply(zoom));
                var m_xyzBr = bcfView.Origin.Subtract(bcfView.UpDirection.Multiply(zoom)).Add(bcfView.RightDirection.Multiply(zoom));
                BCFUIView.ZoomAndCenterRectangle(m_xyzTl, m_xyzBr);

                result = true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// Orthogonal View by Perspective Camera.
        /// </summary>
        /// <param name="bcfView"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        private bool SetOrthogonalView(View3D bcfView, PerspectiveCamera camera)
        {
            var result = false;
            try
            {
                var direction = RevitUtils.GetRevitXYZ(camera.CameraDirection);
                var upVector = RevitUtils.GetRevitXYZ(camera.CameraUpVector);
                var viewPoint = RevitUtils.GetRevitXYZ(camera.CameraViewPoint);
                var orientation = RevitUtils.ConvertBasePoint(ActiveDoc, viewPoint, direction, upVector, true);

                using (var trans = new Transaction(ActiveDoc))
                {
                    trans.Start("Set Orientation");
                    try
                    {
                        bcfView.SetOrientation(orientation);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.RollBack();
                    }
                }

                SetViewPointBoundingBox(bcfView);
                result = true;
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }

        private bool SetPerspectiveView(View3D bcfView, PerspectiveCamera camera)
        {
            var result = false;
            try
            {
                var zoom = camera.FieldOfView;
                var direction = RevitUtils.GetRevitXYZ(camera.CameraDirection);
                var upVector = RevitUtils.GetRevitXYZ(camera.CameraUpVector);
                var viewPoint = RevitUtils.GetRevitXYZ(camera.CameraViewPoint);
                var orientation = RevitUtils.ConvertBasePoint(ActiveDoc, viewPoint, direction, upVector, true);


                using (var trans = new Transaction(ActiveDoc))
                {
                    trans.Start("Set Orientation");
                    try
                    {
                        if (bcfView.CanResetCameraTarget()) bcfView.ResetCameraTarget();
                        bcfView.SetOrientation(orientation);
                        if (bcfView.get_Parameter(BuiltInParameter.VIEWER_BOUND_ACTIVE_FAR).HasValue)
                        {
                            var m_farClip = bcfView.get_Parameter(BuiltInParameter.VIEWER_BOUND_ACTIVE_FAR);
                            m_farClip.Set(0);
                        }

                        bcfView.CropBoxActive = true;
                        bcfView.CropBoxVisible = true;

                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.RollBack();
                    }
                }
                result = true;
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bcfView"></param>
        private void SetViewPointBoundingBox(View3D bcfView)
        {
            if (DB.RvtElements.Count == 0) return;

            var offsetBoundingBox = new BoundingBoxXYZ();
            using (var trans = new Transaction(ActiveDoc))
            {
                trans.Start("Set BoundingBox");
                try
                {
                    double minX = 0;
                    double minY = 0;
                    double minZ = 0;
                    double maxX = 0;
                    double maxY = 0;
                    double maxZ = 0;
                    var firstLoop = true;

                    foreach (var rvtComp in DB.RvtComponents)
                    {
                        var element = rvtComp.RvtElement;
                        if (null != element)
                        {
                            var bb = element.get_BoundingBox(null);
                            if (null != bb)
                            {
                                var transform = rvtComp.TransformValue;

                                var minXYZ = transform.OfPoint(bb.Min);
                                var maxXYZ = transform.OfPoint(bb.Max);

                                if (firstLoop)
                                {
                                    minX = minXYZ.X;
                                    minY = minXYZ.Y;
                                    minZ = minXYZ.Z;
                                    maxX = maxXYZ.X;
                                    maxY = maxXYZ.Y;
                                    maxZ = maxXYZ.Z;
                                    firstLoop = false;
                                }
                                else
                                {
                                    if (minX > minXYZ.X) { minX = minXYZ.X; }
                                    if (minY > minXYZ.Y) { minY = minXYZ.Y; }
                                    if (minZ > minXYZ.Z) { minZ = minXYZ.Z; }
                                    if (maxX < maxXYZ.X) { maxX = maxXYZ.X; }
                                    if (maxY < maxXYZ.Y) { maxY = maxXYZ.Y; }
                                    if (maxZ < maxXYZ.Z) { maxZ = maxXYZ.Z; }
                                }
                            }
                        }
                    }
                    offsetBoundingBox.Min = new XYZ(minX - 3, minY - 3, minZ - 3);
                    offsetBoundingBox.Max = new XYZ(maxX + 3, maxY + 3, maxZ + 3);

                    bcfView.SetSectionBox(offsetBoundingBox);
                    bcfView.GetSectionBox().Enabled = true;

                    BCFUIView.ZoomAndCenterRectangle(offsetBoundingBox.Min, offsetBoundingBox.Max);
                    BCFUIView.Zoom(0.8);

                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.RollBack();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bcfView"></param>
        /// <param name="removeHighlight"></param>
        /// <param name="removeIsolate"></param>
        /// <param name="removeSectionBox"></param>
        private void CleanViewSettings(View3D bcfView, bool removeHighlight, bool removeIsolate, bool removeSectionBox)
        {
            using (var trans = new Transaction(ActiveDoc))
            {
                trans.Start("Clean Views");
                try
                {
                    var uidoc = new UIDocument(ActiveDoc);
                    if (removeHighlight)
                    {
                        //remove selection
                        uidoc.Selection.SetElementIds(new List<ElementId>());
                    }

                    if (removeIsolate)
                    {
                        //remove isolation
                        if (bcfView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
                        {
                            bcfView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                        }
                    }

                    if (removeSectionBox)
                    {
                        //remove sectionbox
                        bcfView.GetSectionBox().Enabled = false;
                    }

                    //FilteredElementCollector fec = new FilteredElementCollector(ActiveDoc);
                    //var eles  = fec.OfClass(typeof(Element)).WhereElementIsNotElementType().Cast<Element>().ToList();


                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.RollBack();
                }
            }
        }

        private View3D CreateDefaultOrthoView()
        {
            View3D view3D = null;
            try
            {
                var viewName = $"{BCFExpress.Properties.Settings.Default.DefaultViewPrefix} - Orthogonal - " + Environment.UserName;
                var collector = new FilteredElementCollector(ActiveDoc);
                var view3ds = collector.OfClass(typeof(View3D)).ToElements().Cast<View3D>().ToList();
#if RELEASE2018
                var viewfound = from view in view3ds where view.IsTemplate == false && view.IsPerspective == false && view.ViewName == viewName select view;
#else
                var viewfound = from view in view3ds where view.IsTemplate == false && view.IsPerspective == false && view.Name == viewName select view;
#endif
                if (viewfound.Any())
                {
                    view3D = viewfound.First();
                }
                else
                {
                    var viewFamilyTypeId = GetViewFamilyTypeId();
                    if (viewFamilyTypeId != ElementId.InvalidElementId)
                    {
                        using (var trans = new Transaction(ActiveDoc))
                        {
                            trans.Start("Create View");
                            try
                            {
                                view3D = View3D.CreateIsometric(ActiveDoc, viewFamilyTypeId);
                                view3D.Name = viewName;
                                trans.Commit();
                            }
                            catch (Exception)
                            {
                                trans.RollBack();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return view3D;
        }

        private View3D CreateDefaultPersView()
        {
            View3D view3D = null;
            try
            {
                var viewName = $"{BCFExpress.Properties.Settings.Default.DefaultViewPrefix} - Perspective - " + Environment.UserName;
                var collector = new FilteredElementCollector(ActiveDoc);
                var view3ds = collector.OfClass(typeof(View3D)).ToElements().Cast<View3D>().ToList();
                //by the limitation of perspective view, create isometric instead
#if RELEASE2018
                var viewfound = from view in view3ds where view.IsTemplate == false && view.IsPerspective == false && view.ViewName == viewName select view;
#else
                var viewfound = from view in view3ds where view.IsTemplate == false && view.IsPerspective == false && view.Name == viewName select view;
#endif
                if (viewfound.Any())
                {
                    view3D = viewfound.First();
                }
                else
                {
                    var viewFamilyTypeId = GetViewFamilyTypeId();
                    if (viewFamilyTypeId != ElementId.InvalidElementId)
                    {
                        using (var trans = new Transaction(ActiveDoc))
                        {
                            trans.Start("Create View");
                            try
                            {
                                view3D = View3D.CreatePerspective(ActiveDoc, viewFamilyTypeId);
                                view3D.Name = viewName;
                                trans.Commit();
                            }
                            catch (Exception)
                            {
                                trans.RollBack();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return view3D;
        }

        private UIView FindDefaultUIView(View3D bcfView)
        {
            UIView uiview = null;
            try
            {
                var uidoc = new UIDocument(ActiveDoc);
                var uiviews = uidoc.GetOpenUIViews();
                var viewFound = from view in uiviews where view.ViewId == bcfView.Id select view;
                if (viewFound.Any())
                {
                    uiview = viewFound.First();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return uiview;
        }

        private ElementId GetViewFamilyTypeId()
        {
            var viewTypeId = ElementId.InvalidElementId;
            try
            {
                var collector = new FilteredElementCollector(ActiveDoc);
                var viewFamilyTypes = collector.OfClass(typeof(ViewFamilyType)).ToElements().Cast<ViewFamilyType>().ToList();
                var vTypes = from vType in viewFamilyTypes where vType.ViewFamily == ViewFamily.ThreeDimensional select vType;
                if (vTypes.Any())
                {
                    var vfType = vTypes.First();
                    viewTypeId = vfType.Id;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return viewTypeId;
        }

        public string GetName()
        {
            return "BCF Event Handler";
        }
    }
    public class Request
    {
        private int m_request = (int)RequestId.None;

        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }

        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
    public enum RequestId
    {
        None = 0,
        SetViewPointView = 1,
        ApplyViews = 2,
        HighlightElement = 3,
        IsolateElement = 4,
        PlaceSectionBox = 5,
        WriteParameters = 6,
        StoreToolSettings = 7,
        ExportImage = 8,
        GrowSectionBox = 9,
        ShrinkSectionBox = 10,
        DuplicateView = 11,
    }
}
