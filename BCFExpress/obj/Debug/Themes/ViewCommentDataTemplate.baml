   M S B A M L   `   `   `  ����E  ABCFExpress, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:BCFExpress.DataBCFExpress.Data  5      W0clr-namespace:BCFExpress.Data.AttachedProperties"BCFExpress.Data.AttachedProperties  5      5clr-namespace:BCFExpress.ThemesBCFExpress.Themes  5      Q-clr-namespace:BCFExpress.Data.ValueConvertersBCFExpress.Data.ValueConverters  5      /    )BCFExpress.Themes.ViewCommentDataTemplateR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source;   5BCFExpress.Data.ValueConverters.MultiCommandConverter   MultiCommandConverter3   -BCFExpress.Data.ValueConverters.DateConverter  DateConverter5   /BCFExpress.Data.ValueConverters.PathToImageConv  PathToImageConv  ViewCommentDataTemplate � RowDefinitions � Cursor � ToolTip �� Orientation �� 	Converter  BackgroundDark �� 
TargetType �� Property	 �� Value
 �� Triggers �� Binding �� Value"   BCFExpress.Themes.ButtonIcon  OverlayButton  
IconExpand   IconPath  CloseButton `� 	Converter �� Path �� ElementName ��Name �� ConverterParameter �� Padding:   4BCFExpress.Data.AttachedProperties.NavigationService   Text � ColumnDefinitions'   !BCFExpress.Themes.TextPlaceholder'   !BCFExpress.Themes.TextPlaceholder   KeyDown9   3BCFExpress.Data.AttachedProperties.CtrlEnterKeyDown   Command �� AcceptsReturn   Placeholder �� TextWrapping   CommandArgument  Name  AddCommentEnablerStyle 	 PrimaryLight   	IconColor   
IconMargin 
 IconSend �� Triggers  :� RoutedEvent  FadeInStoryboard   5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      &dataclr-namespace:BCFExpress.Data  5      :local0clr-namespace:BCFExpress.Data.AttachedProperties  5      *themesclr-namespace:BCFExpress.Themes  5      AvalueConverters-clr-namespace:BCFExpress.Data.ValueConverters  5        5	      �� 5
      $ ../Themes/Icons.xaml?�6   �� 5      $! ../Themes/ButtonStyles.xaml?�6   �� 5      $) #../Themes/VisibilityAnimations.xaml?�6   %;  &	        &	      &	 &     &	 9     7  7  7  7  7  7  7  7   7  7  7  7   7  7   7  7  7 	 7 
 7   7    5      6*     5      6"     5      6$   �� 5      .R�5      � $��Stretch=�6    5      �� 5   
   $
��Auto �6   �� 5   
   $
��Auto �6   �� 5   
   $
��Auto �6   .+�5      � .+�5   
   �� $��data:Commands.OpenView��5      $
 Hand��5      $ Open view »��5      ��5      �� *	Viewpoint+��5       �� 5!      .R�5#      � $	��150��6   $��Stretch=�6%   .+�5$      �� $��Center=�6   $ Vertical=�68   .+�5%      �� $��ViewpointImage��5&      $��0q�5'      $
��Fill=�5)      ��5(      �� *Viewpoint.SnapshotPath+8    5*      �� *Guid+5+      5,      5-      5/      �� 50   
   $��Stretch=�51      $��Center=�52      8�� $��20(�54      $
��Bold"�55      $��General Comments��56      ��57      �� 58      " ��6   .�59      ��  � y�$	 	Collapsed=�6-   
 5:      �� 5;       6   �� *Viewpoint.SnapshotPath+8   6u   Z� .a�5<      ��  � y�$	 Visible=�61   5=      5?      5A      �� 5B   
   $��Right=�6   $ 
Horizontal=�61   ��5C      �� 5D      " ��6   .�5E      ��  � y�$	 Visible=�6-   
 5F      �� 5G       6   �� *Viewpoint.SnapshotPath+8   6u   Z� .a�5H      ��  � y�$	 	Collapsed=�61   5I      5K      .+�5T        8�� $ ��data:Commands.OpenSnapshot��5V      8  $ Enlarge view��5Y      ��5W      �� *	Viewpoint+5Y        5[      8�� $��data:Commands.DeleteViews��5]      $ Delete view��5^      ��5_      `� 5`      8  .1�5a      �� $ 	Viewpoint�6   �� 5b      $ 	IssueList��6   $ SelectedItem�62   5c      5e      5f      5g   	   �� 5i      $ CommentList��5j   	   $��1��5k   	   $��2��5l   	   $��0,0,0,10q�5m   	   
������ $��0q�5o   	   ��.  $��Disabled=�5r   	   ��5p   	   �� *
Comments+��5s   
   �� 5t      .R�5v      �  5w      �� 5x      $
��Auto �6    �� 5y      $
��Auto �6    .+�5{      ]� $��0��6   .+�5|      �� $��Bottom=�5}      $
��Bold"�5~      ��5      �� *Author+�� 5�      $��Bottom=�5�      $��11(�5�      $��Italic$�5�      ��5�      �� *Date+8  $ relative�� 5�      �� *Date+8 	 5�        5�      $��0��5�      8��
 $"��data:Commands.DeleteComments��5�      $ Delete comment��5�      ��5�      `� 5�      8  .1�5�      �� $  �6   �� 5�      $ 	IssueList��6   $ SelectedItem�66   5�      5�      �� 5�      $��1��5�      $ 5q�5�      $��Stretch=�5�      $
��Wrap=�5�       5�      �� *
Comment1+5�      5�      5�      5�   	   � 5�      $��2��6   $��5,0,5,5q�6    5�   
   �� 5�      $
��Auto �6   �� 5�      $
��auto �6    5�   
   �� 5�      $��* �6   �� 5�      $
��Auto �6   .+�5�   
     $��TextBoxComment��5�       UIElement_OnKeyDown5�      $��1��5�      $��0��5�      8�� $��1,1,0,1q�5�      $ data:Commands.AddComment��5�       . $ Add a comment...��5�      $
 Wrap=�5�      .��6    5�      `� 5�      8  .1�5�      �� $ 	Viewpoint�6   �� 5�      $ 	IssueList��6   $ SelectedItem�60   �� 5�      $ TextBoxComment��6   $
 Text�65   5�      .��5�   4   5�      �� 5�   
   $��1��5�      $��1��5�      
�������8�� $��0,1,1,1q�5�      ._�5�        $ AddCommButton��5�      $��Bottom=�5�      8�� 
�������$��0q�5�      $��data:Commands.AddComment��5�      $��Right=�5�      8  $ 2,0,0,0q�5�      8  $ Add comment��5�      ��5�      `� 5�      8  .1�5�      �� $ 	Viewpoint�6   �� 5�      $ 	IssueList��6   $ SelectedItem�62   �� 5�      $ TextBoxComment��6   $
 Text�67   5�      5�      5�      5�   	   5�       5�      :� 5�      $  FrameworkElement.Loaded��6   .[�5�   
   �� 8�� 6   5�   	   5�      5�      