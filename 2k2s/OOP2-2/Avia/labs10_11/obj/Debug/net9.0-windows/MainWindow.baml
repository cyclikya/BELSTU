   M S B A M L   `   `   `  ����D  @labs10_11, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:labs10_11.ViewModelslabs10_11.ViewModels  5   	       labs10_11.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   1vm"clr-namespace:labs10_11.ViewModels     5   	     � Title$  Persons and Departments��5   	   $	��400��6)   $	��600��66   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0,0,0,10q�6.   .+�5      �� $��Имя:��6   $��Center=�6$   �� 5      $	��120��6   $	��5,0q�6"   ��6/   �� *NewPersonName+ �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors$
 True���� 5      $��Возраст:��6   $��Center=�6(   �� 5      $��50��6   $	��5,0q�6!   ��6.   �� *NewPersonAge+$ PropertyChanged=�$
 True���� 5      $��Добавить��6   $
��10,0q�6M   ��6(   �� *AddPersonCommand+6M   �� 5      $��Удалить��6   $
��10,0q�6O   ��6'   �� *DeletePersonCommand+6O   5      � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� $��0��6   .+�5      �� $��Persons:��6   $
��Bold"�6,   �� 5      ��6   �� *	Persons+ �� SelectedItem 5   
   �� *SelectedPerson+ �� Mode$ TwoWay=���5      �� 5      .R�5      �� $ 
Horizontal=�6)   .+�5   "   �� $	��100��6B   ��6,   �� *Name+6B   �� 5    "   $��40��6A   ��6,   �� *Age+6A   5!      5"      5$      5%      �� 5&      $��1��6   .+�5'      �� $��Departments:��6   $
��Bold"�60   �� 5(      ��6   �� *Departments+��5)      �� 5*      .R�5+      �� ��6(   �� *Title+5,      5.      5/      50      51      52      