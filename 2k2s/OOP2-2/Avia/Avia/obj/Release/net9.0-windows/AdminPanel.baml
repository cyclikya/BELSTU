   M S B A M L   `   `   `  ����   Avia    Avia.AdminPanel   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml        AdminPanel_Title  � Title#  �    � WindowState$ 	Maximized=� � WindowStartupLocation$ CenterScreen=����#f0f4f8 � Cursor$; 5https://ani.cursors-4u.net/cursors/cur-13/cur1162.ani�� � Icon$; 5https://cdn-icons-png.flaticon.com/512/744/744502.png��.���  � RowDefinitions �� $��* ��� $
��Auto �.+��� -    ��Name$ 
tabControl��$��0��$��10q�.@��� -   $��tabUsers��  UsersTab#���  .��&   System.Windows.Controls.DataGrid  -     Name$ dataGridUsers��   AutoGenerateColumns$
 True��$��10q��� -   $��
tabFlights��  
FlightsTab#���  .��  -   $ dataGridFlights��$
 True��$��10q��� -   $��tabBookings��  BookingsTab#���  .��  -   $ dataGridBookings��$
 True��$��10q��� $��1��	 �� Orientation$	 
Horizontal=�$��Center=�$��10q�.+��� -   
 ��Name$
 btnAdd��  	AddButton#���  $��80��$��5q��� -	   $
 btnEdit��  
EditButton#���  $��80��$��5q��� -
   $
 	btnDelete��  DeleteButton#���  $��80��$��5q��� -   $
 btnClose��  CloseButton#���  $��80��$��5q�