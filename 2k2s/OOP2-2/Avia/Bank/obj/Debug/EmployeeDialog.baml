   M S B A M L   `   `   `  ����  Bank    Bank.EmployeeDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Сотрудник��5   	   $	��300��6   $	��350��6(    � WindowStartupLocation$ CenterOwner=�64    � 
ResizeMode$ NoResize=�6X   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $	��110 �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0��6   $��Отдел:��6-   $��Center=�6>   �� 5   
   -   5        $��cmbDepartments��5   0   $��0��6   $��1��6    $��5q�6F    �� DisplayMemberPath$
 name��6Q    �� SelectedValuePath$ department_id��6j   �� 5   
   $��1��6   $��0��6   $��Имя:��6-   $��Center=�6<   �� 5   
   -   5        $��txtFirstName��5   /   $��1��6   $��1��6   $��5q�6C   �� 5   
   $��2��6   $��0��6   $��Фамилия:��6-   $��Center=�6@   �� 5   
   -   5        $��txtLastName��5   /   $��2��6   $��1��6   $��5q�6B   �� 5   
   $��3��6   $��0��6   $��Дата найма:��6-   $��Center=�6C   (  "System.Windows.Controls.DatePicker  5   
   -   5        $��
dpHireDate��5   2   $��3��6   $��1��6"   $��5q�6D   �� 5    
   $��4��6   $��0��6   $��	Фото:��6-   $��Center=�6=   �� 5!   
   -   5        $��btnLoadPhoto��5!   .   $��4��6   $��1��6   $!��Загрузить фото��6B   $��5q�6[   6f   �� 5#   
   -   5        $��imgPhoto��5#   A   $��5��6   $��0��6   $��3��6-   $	��100��6Q   $��5q�6^   $��Uniform=�6i   �� 5%   
   $��6��6   $��3��6"    �� Orientation$ 
Horizontal=�66   $��Right=�6O   $��0,10,0,0q�6k   .+�5&      �� -   5        $��75��5&      $	��5,0q�6    $��OK��6-    �� 	IsDefault . 6K   �� 5'      $��75��6   $	��5,0q�6    $��Отмена��6-   	 �� IsCancel	 . 6>   5(      5)      5*      