   M S B A M L   `   `   `  ����   Avia    Avia.AddEditBookingWindow   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml        AddEditBookingWindow_Title  � Title#  �    � WindowStartupLocation$ CenterScreen=�$	��350��$	��400�� � Cursor$; 5https://ani.cursors-4u.net/cursors/cur-13/cur1162.ani�� � Icon$; 5https://cdn-icons-png.flaticon.com/512/744/744502.png��.��� $��10q� � ColumnDefinitions �� $
��Auto ��� $��* � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��10q�  	UserLabel#���  �� -    ��Name$ cmbUser��$��0��$��1��$��10q��� $��1��$��0��$��10q�  FlightLabel#���  �� -   $ 	cmbFlight��$��1��$��1��$��10q��� $��2��$��0��$��10q�  BookingDateLabel#���  (  "System.Windows.Controls.DatePicker  -     Name$ datePickerBookingDate��$��2��$��1��$��10q��� $��3��$��0��$��10q�  StatusLabel#���  �� -   $ 	cmbStatus��$��3��$��1��$��10q�.@���   StatusBooked#���  ��   
StatusPaid#���  ��   StatusCancelled#���  �� $��4��$��0��  SeatsReservedLabel#���  �� -    ��Name$ txtSeatsReserved��$��4��$��1��$��
10,5,10,10q��� -   $��5��$��1�� 	 
SaveButton#��� 	 $��Right=�$��0,10,0,0q�$
��3, 3q�