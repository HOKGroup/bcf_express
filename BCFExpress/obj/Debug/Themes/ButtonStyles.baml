   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      E ABCFExpress, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:BCFExpress.ThemesBCFExpress.Themes 5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   MainMenuButton �� 
TargetType"   BCFExpress.Themes.ButtonIcon �� Property  PrimaryDark �� Value  SecondaryDark  BackgroundDark  CanSaveEnablerStyle �� BasedOn �� Triggers �� Binding �� ElementName	 �� Path
 �� Value  IssuesSelectedEnablerStyle  ImageNullSource P� Property P� Value  AddViewCommentStyle  AddCommentEnablerStyle �� Empty 	 AddIssueWin 
 AnnotateSnapshot  FieldSelectedEnablerStyle  ViewSelectedEnablerStyle  ProjectSelectedEnablerStyle # ProjectSelectedGridEnablerStyle   CommentsSelectedEnablerStyle  IssueCountEnablerStyle  Standardbutton �� 
TargetType �� Padding �� CornerRadius  OverlayButton �� RelativeSource �� AncestorType  CloseButton  
IconXClose  ChangeFieldButton ^� 
Conditions �� Binding �� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      8d2http://schemas.microsoft.com/expression/blend/2008  5      Bmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      1system&clr-namespace:System;assembly=mscorlib  5      *themesclr-namespace:BCFExpress.Themes  5        5
      �� 5      $ 
Icons.xaml?�6   �� 5      $ Colors.xaml?�6   %  &	        7  7  7  7  &	 )    7   &	 �    &	 �    &	     7 �7  &	 �    7 �&		 �    &	
 �    &	 ,    &	 �    &	 �    &	 5    &	 �    &	 �	    &	 4
    &	 p    &	 �    7  7  &	 8    7 ��� 5      "   6!   �5      �� 5       �@  	IconColor8   6$   �� 5       �@  HighlightColor8  6)   �� 5       �@  
IsVertical . 6%   �� 5       � ��$ 0q�6*   �� 5       �@  DisabledIconColor8  6,   �� 5       � ��8  6%   5      �� 5      8   "   5        5!      �� 5"       6   �� $ BcfTabControl��$	 SelectedIndex�
 -16V   .a�5#   
   ��  � {� .  6&   5$   	   5&      �� 5(      " ��6-    5)      �� 5*       6   �� $ 	IssueList��$	 SelectedItems.Count�
 06X   .a�5+   
   ��  � {� .  6&   5,   	   5.      �� 50      " ��6"    51      P� 52       � �� 6"   Z� . �53   
   ��  � y�$ 	Collapsed=�6'   54   	   56      �� 58      8   " ��5;      �5<      �� 5=       � ��8  6&   �� 5>       � ��$ Stretch=�6.   �� 5?       � ��$ Stretch=�6,   5A      �� 5C      8   " ��5F       5G      �� 5H       6   �� $ 	IssueList��$	 SelectedItems.Count�
 06X   .a�5I   
   ��  � {� .  6&   5J   	   �� 5K      #
 Z 6N    6   �� $ TextBoxComment��$
	 Text�.a�5L   
   ��  � {� .  6&   5M   	   5O      �� 5Q      " ��6    5R      �� 5S      #
 Z 6H    6   �� $ TitleBox��$
	 Text�.a�5T   
   ��  � {� .  6&   5U   	   5W      �� 5Y      " ��6#    5Z      �� 5[       6   �� $ SnapshotImg��$	 Source�
 6M   Z� .a�5\   
   ��  � {� .  6&   5]   	   5_      �� 5a      " ��6,    5b      �� 5c       6   �� $ 	FieldList��$	 SelectedItems.Count�
 06X   .a�5d   
   ��  � {� .  6&   5e   	   5g      �� 5i      " ��6+    5j      �� 5k       6   �� $ ViewpointList��$	 SelectedItems.Count�
 06\   .a�5l   
   ��  � {� .  6&   5m   	   5o      �� 5q      " ��6.    5r      �� 5s       6   �� $ ProjectCombo��$	 SelectedIndex�
 -16U   .a�5t   
   ��  � {� .  6&   5u   	   5w      �� 5y      " �62    5z      �� 5{       6   �� $ ProjectCombo��$	 SelectedIndex�
 -16U   .a�5|   
   ��  � {� .  6&   5}   	   5      �� 5�      " ��6/    5�      �� 5�       6   �� $ CommentList��$	 SelectedItems.Count�
 06Z   .a�5�   
   ��  � {� .  6&   5�   	   5�      �� 5�      " ��6)    5�      �� 5�       6   �� $ 	IssueList��$	 Items.Count�
 06P   .a�5�   
   ��  � {� .  6&   5�   	   5�      �� 5�      " d�6!   .�5�      ��  � �� 5�      �� 5�   
   " d�6   .R�5�      �� $��border��5�      $ 2q�5�      
������ 
�������$��0q�5�      $ 0��5�      ._�5�      �� $��content��5�      $
��Left=�5�      $��Center=�5�      5�      5�      5�      5�      �� 5�      "   6    �5�      �� 5�       �@  
IsVertical . 6%   �� 5�       �@  IconMarginVertical$ 0q�6-   �� 5�       � ��$ 0q�6!   �� 5�       � ��$ 0q�6"   �� 5�       � ��$ 0q�6*   �� 5�       �@  IconSizeVertical$ 12��6+   �� 5�       � ��$	 Top=�6,   �� 5�       �@  	IconColor
 �����6$   �� 5�       � ��$ Right=�6.   �� 5�       � y�$ 	Collapsed=�6%    5�      �� 5�       6   ��  �� $
 GridL�$	 IsMouseOver�
 True6l   .a�5�   
   ��  � y�$ Visible=�6'   5�   	   5�      �� 5�      8   "   5�      �5�      �� 5�       �@  IconPath8  6#   5�      �� 5�      8   " ��5�       5�      P� 5�       � ��# Z 6#   . �5�   
   ��  � �� �#EEEEEE6'   5�   	   P� 5�       � �� 6#   Z� . �5�   
   ��  � �� �#EEEEEE6'   5�   	   ^� 5�       5�   
   �� 5�       6   �� $	 Value.isRequired� True6@   �� 5�      # Z 6=    6   �� $	 Value.display�6=   .0�5�   
   ��  � ��
 �z���6'   �� 5�   
    �@�ToolTip$ Required Field��6$   5�   	   ^� 5�       5�   
   �� 5�       6   �� $	 Value.isRequired� True6@   �� 5�       6   �� $	 Value.display� 6=   Z� .0�5�   
   ��  � ��
 �z���6'   �� 5�   
    �@�ToolTip$ Required Field��6$   5�   	   5�      5�      