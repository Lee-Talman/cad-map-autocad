   M S B A M L   `   `   `  ����X  TCAD-MAP AutoCAD Plugin, Version=1.0.8501.18368, Culture=neutral, PublicKeyToken=null?$clr-namespace:CAD_MAP_AutoCAD_PluginCAD_MAP_AutoCAD_Plugin  5   	        CAD_MAP_AutoCAD_Plugin.GUI   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:CAD_MAP_AutoCAD_Plugin     5   	     � Title$E  ?CAD-MAP v3.0    |    Engineering Industries eXcellence (c) 2023��5   	   $	��720��6Q   $��1277.8��6^   .��5	      � $��0,0,2,0q�6   .+�5
   
   �� -   5         ��Name$ btnConnectDB��5
      $��
Connect DB��6'   $
��Left=�6<   $��1183,87,0,0q�6W   $	��Top=�6l   $��75��6�   ���	#FF0F0F0F��5      �� 5       �� Color# � ��6"   5      �� 5   
   -   5        $ 
btnClearDB��5      $��Clear DB��6%   $
��Left=�68   $��1023,87,0,0q�6S   $	��Top=�6h   $��75��6�   
���  ��
�������6�   �� 5   
   -   5         ��Name$ labelConnectionString��5      $5��/1. Enter the SQL connection string for your DB:��6/   $
��Left=�6i   $��
798,17,0,0q�6�   $	��Top=�6�   $��20(�6�   $��Italic$�6�   $	��460��6�   �� 5   
   -   5        $ btnUploadFile��5      $��Upload��6(   $
��Left=�69   $��365,245,0,0q�6T   $	��Top=�6i   $��75��6�   ��5      �� 5      # � ��6"   5      �� 5   
   -   5        $ labelUpload��5      $.��(3. Upload a validation file (.csv only):��6%   $
��Left=�6X   $��
10,233,0,0q�6s   $	��Top=�6�   $��20(�6�   $��Italic$�6�   $	��350��6�   �� 5   
   -   5        $ labelFunctions��5      $$��2. Select desired function(s):��6(   $
��Left=�6Q   $��798,138,0,0q�6l   $	��Top=�6�   $��20(�6�   $��Italic$�6�   $	��460��6�   �� 5   
   -   5        $ labelImportAll��5      $��Import All:��6(   $
��Left=�6>   $��798,185,0,0q�6Y   $	��Top=�6n   $	��160��6�   $
��Bold"�6�   $��20(�6�   �� 5   
   -   5         ��Name$ chkTexts��5      $��Texts��6%   $
��Left=�65   $��821,232,0,0q�6P   $	��Top=�6e   $��18(�6}   �� 5   
   -	   5        $ 	chkMTexts��5      $��MTexts��6&   $
��Left=�67   $��821,260,0,0q�6R   $	��Top=�6g   $��18(�6    J� RenderTransformOrigin$ 0.667,0.565&�6�   �� 5   
   -
   5        $ chkLines��5      $��Lines��6%   $
��Left=�65   $��910,233,0,0q�6P   $	��Top=�6e   $��18(�6}   �� 5   
   -   5        $ chkPolylines��5      $��	PolyLines��6)   $
��Left=�6=   $��910,261,0,0q�6X   $	��Top=�6m   $��18(�6�   �� 5   
   -   5        $ 	chkBlocks��5      $��Blocks��6&   $
��Left=�67   $��1010,233,0,0q�6R   $	��Top=�6h   $��18(�6�   �� 5   
   ���	#FFF4F4F5$
��Left=�6%   $��73��6@   $��805,227,0,0q�6L   
���   �$	��Top=�6p   $	��293��6�    K� ZIndex$ -1��6�   �� 5    
   -   5         ��Name$ inputConnectionString��5       $��Right=�61   $��23��6M   $��	0,59,10,0q�6Y    �� TextWrapping$
 Wrap=�6l   $J��DServer=LAT-ENGUSA\SQLEXPRESS;Database=CADDB;Trusted_Connection=True;��6�   $	��Top=�6�   $	��460��6�   6�   �� 5!   
   -   5        $ btnConfigureDB��5!      $��Configure DB��6)   $
��Left=�6@   $��1103,87,0,0q�6[   $	��Top=�6p   $��75��6�   ���	#FF0F0F0F��5"      �� 5#      # � ��6"   5%      �� 5&   
   -   5        $ btnStartCADMAP��5&      $��Start CAD-MAP��6)   $
��Left=�6A   $��798,619,0,0q�6\   $	��Top=�6q   $	��460��6�   $��60��6�   $��24(�6�   
���������5'      �� 5(      # � ��6"   5*      &   System.Windows.Controls.DataGrid  5+   
   -   5        	  Name$	 ImportDataGrid��5+      $
��Left=�6+   $	��404��6F   $��
10,275,0,0q�6S   $	��Top=�6g   $	��770��6   $ -2��6�   �� 5,   
   -   5        
 ��Name$
 INDX_SPLASH��5,      $
��Left=�6%   $	��200��6@   $��	10,22,0,0q�6M   $	��Top=�6`   $	��770��6x   $��INDX_SPLASH.jpg��6�   �� 5-   
   -   5        $ btnOverlapTest��5-      $��OverlapTest��6)   $
��Left=�6?   $��805,320,0,0q�6Z   $	��Top=�6o   $��75��6�   6�   �� 5.   
   -   5        $ 	chkGroups��5.      $��Groups��6&   $
��Left=�67   $��1010,261,0,0q�6R   $	��Top=�6h   $��18(�6�    d� 	IsChecked$
 TrueY�6�   5/      50      