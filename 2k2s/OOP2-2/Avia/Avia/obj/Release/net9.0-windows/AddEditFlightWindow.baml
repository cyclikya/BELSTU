   M S B A M L   `   `   `  ����   Avia    Avia.AddEditFlightWindow   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � WindowStartupLocation$  CenterScreen=�   AddEditFlightWindow_Title � Title# �   $	��480��$	��400�� � Cursor$; 5https://ani.cursors-4u.net/cursors/cur-13/cur1162.ani�� � Icon$; 5https://cdn-icons-png.flaticon.com/512/744/744502.png��.��� $��10q� � ColumnDefinitions �� $
��Auto ��� $��* � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��0��$��10q�  DepartureLabel#���  �� -    ��Name$ txtDeparture��$��0��$��1��$��10q��� $��1��$��0��$��10q�  DestinationLabel#���  �� -   $ txtDestination��$��1��$��1��$��10q��� $��2��$��0��$��10q�  	DateLabel#���  (  "System.Windows.Controls.DatePicker  -     Name$ datePickerDate��$��2��$��1��$��10q��� $��3��$��0��$��10q�  	TimeLabel#���  �� -   $ txtTime��$��3��$��1��$��10q��� $��4��$��0��$��10q�  AirlineLabel#���  �� -   $ 
txtAirline��$��4��$��1��$��10q��� $��5��$��0��$��10q�  
PriceLabel#���  �� -   $ txtPrice��$��5��$��1��$��10q��� $��6��$��0��$��10q�  SeatsTotalLabel#���  �� -   $ txtSeatsTotal��$��6��$��1��$��10q��� $��7��$��0��  SeatsAvailableLabel#���  �� -   $ txtSeatsAvailable��$��7��$��1���� $��8��$��0��$��10q� 	 BaggageInfoLabel#��� 	 �� -	   $ txtBaggageInfo��$��8��$��1��$��10q��� -
   $��9��$��1�� 
 
SaveButton#��� 
 $��Right=�$��0,10,0,0q�