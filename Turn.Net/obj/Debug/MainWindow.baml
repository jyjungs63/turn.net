   M S B A M L   `   `   `  ����C  ?Turn.Net, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Turn.NetTurn.Net  5   	       Turn.Net.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Turn.Net     5   	     � Title$&   Turnning Circle Test Application��5   	   $��688.876��62   $��1124.81��6C    � WindowStyle$ 
ToolWindow=�6S   ��5	      �� 5
   
    �� EndPoint$ 0.5,1&�6    �� 
StartPoint$ 0.5,0&�6/   .��5      �  � Color$ Black��6    � Offset$ 0Q�6)   � 5      $ 	#FF0C0A0A��6   $ 1Q�6-   5      .��5      � $��0,0,-0.925,1.377q�6   #��� ��6%   .+�5   
   �� $��0,6,0,0q�6    �� Orientation$ Vertical=�6&   $��Right=�6=   $	��170��6Y   $	��421��6e   $	��Top=�6r   #��� ��6�   .+�5      �� -   5         ��Name$ button��5      $��
Open & Run��6%   $��Bottom=�6>   $��0,0,0.264,0q�6Y   ���	#FF222222$��32��6�   
�������6�   �� 5      -   5         ��Name$ comboBox��5      ���	#FF0C2A96���	#FF382323���	#FF020101	 �� SelectedIndex$	 0��6o   $��0,0,0.264,0q�6�   {�.  .@�5      �� -   5        
 ��Name$
 comboBoxItem��5      $��Port Drawing��65   $
��Left=�6L   $	��150��6g   ���	#FF0A07076s   �� 5      -   5        $
 comboBoxItem1��5      $��Starboard Drawing��66   $
��Left=�6R   $	��150��6m   ���	#FFF0DADA6y   �� 5      -   5        $
 comboBoxItem2��5      $��Port+Starboard��66   $
��Left=�6O   $	��150��6j   ���	#FFF0DADA6v   5      �� 5      $��Select Option��6   #��� ��6,   #��� ��6�   #��� ��6�   	� 5      -   5         	�Name$ radioButton��5      $��Tunning��6/    d� 	IsChecked$
 TrueY�6A   
�������6R   	� 5      -   5        $ radioButton1��5      $��Other��60   $ FalseY�6A   
�������6S   �� 5      $��Tunning Angle��6   ���	#FFF5EBEB6-   �� 5      -   5         ��Name$ textBoxAngle��5      $��30 Deg��6,   �� 5      -	   5         ��Name$ label��5      $��Title��6#   
���   ����	#FFF5EBEB$��0,0,0.264,0q�6]   �� 5      -
   5        $ textBox��5      $��23��6'    �� TextWrapping$
 Wrap=�63   $��0,0,-0.736,0q�6i   �� 5      -   5        $ button_Copy2��5      $
��Exit��6+   $��Bottom=�6:   $��0,0,0.264,0q�6U   ���	#FF9D2515$��33��6�   ���	#FF1032496�   5!      (  "System.Windows.Controls.WebBrowser  5"   
   -   5          Name$ Browser1��5"      $��0,0,175,135q�6'   �� 5#   
   -   5         ��Name$ richTextBox��5#      $	��135��6+   $��Bottom=�68   $��	0,0,175,0q�6S   ���	#FF0A1110���	#FFE8D4D4. �5$      /� 5%      5&      5'      