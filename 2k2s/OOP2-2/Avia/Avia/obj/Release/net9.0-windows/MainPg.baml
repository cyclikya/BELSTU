   M S B A M L   `   `   `  ����   Avia    Avia.MainPg   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml        MainPgTitle  N� Title#  �    N� 
Background �#f0f4f8 � Cursor$; 5https://ani.cursors-4u.net/cursors/cur-13/cur1162.ani��.���  � RowDefinitions �� $
��Auto ��� $��* �.+���  �� Orientation$ Vertical=�$��10q�.+��� $ 
Horizontal=�$��0,0,0,5q�.+���   DepartureCity#���  $��Center=�$��0,0,5,0q��� -    ��Name$ DepartureCityComboBox��$	��150��$��0,0,10,0q���   ArrivalCity#���  $��Center=�$��0,0,5,0q��� -   $ DestinationCityComboBox��$	��150��$��0,0,10,0q��� -     Swap#���  $��80�����#2c3e50
�������
������ �� $ 
Horizontal=�$��0,0,0,5q�.+���   RequiredSeats#���  $��Center=�$��0,0,5,0q��� -    ��Name$ RequiredSeatsTextBox��$��50��$��0,0,10,0q���   Date#���  $��Center=�$��10,0,5,0q�(  "System.Windows.Controls.DatePicker  -     Name$ FlightDatePicker��$	��120��$��0,0,10,0q��� $ 
Horizontal=�$��0,0,0,5q�.+���  
 SortBy#���  $��Center=�$��0,0,5,0q��� -   $ SortByComboBox��$	��120��.@���  	 Price#���  ��   Baggage#���  ��  		 Seats#��� 	 �� -    
 ClearAll#��� 
 $	��100��$��Right=�$��0,10,0,0q����#2c3e50
�������
������ �� $��1��$
��Auto=�.���� -    ��Name$ FlightsPanel��$��10q�