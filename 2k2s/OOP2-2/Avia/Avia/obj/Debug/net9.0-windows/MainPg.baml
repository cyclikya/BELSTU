   M S B A M L   `   `   `  ����  Avia    Avia.MainPg   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5         MainPgTitle  N� Title#  �   5       � Cursor$; 5https://ani.cursors-4u.net/cursors/cur-13/cur1162.ani��5       � 	Resources 5        SwapButtonDemoStyle �� 
TargetType �� Property �� Value �� Triggers ^� 
Conditions �� Binding	 �� ElementName
 �� Value��%^  &	       �� 5   
   " ��6,   .�5      ��  � ��
 �   �6+    5	      ^� 5
       5      �� 5       6$   �� *SelectedItem+$	 DepartureCityComboBox��
 6h   Z� �� 5       6$   �� *SelectedItem+$	 DestinationCityComboBox��
 6j   Z� .0�5      ��  � ��
 �����63   5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ��  �� Orientation$ Vertical=�6   $��10q�6,   .+�5      �� $ 
Horizontal=�6   $��0,0,0,5q�62   .+�5      ��   DepartureCity#���  6   $��Center=�6C   $��0,0,5,0q�6^   �� 5      -   5         ��Name$ DepartureCityComboBox��5      $	��150��6:   $��0,0,10,0q�6F   5      �� 5         ArrivalCity#���  6   $��Center=�6A   $��0,0,5,0q�6\   �� 5!      -   5        $ DestinationCityComboBox��5!      $	��150��6<   $��0,0,10,0q�6H   5"      �� 5#      -   5         ��Name$ ButtonForTrigger��5#        Swap#���  5$      $��80��5&      #��[ 5'      5(      �� 5*      $ 
Horizontal=�6   $��0,0,0,5q�62   .+�5+      ��   RequiredSeats#���  6   $��Center=�6C   $��0,0,5,0q�6^   �� 5,      -   5         ��Name$ RequiredSeatsTextBox��5,      $��50��68   $��0,0,10,0q�6C   5-      �� 5.        Date#���  6   $��Center=�6:   $��10,0,5,0q�6U   (  "System.Windows.Controls.DatePicker  5/      -   5          Name$ FlightDatePicker��5/      $	��120��67   $��0,0,10,0q�6C   50      51      �� 53      $ 
Horizontal=�6   $��0,0,0,5q�62   .+�54      ��  
 SortBy#���  6   $��Center=�6<   $��0,0,5,0q�6W   �� 55      -   5        $ SortByComboBox��55      $	��120��63   .@�57      ��  	 Price#���  6#   �� 58       	 Baggage#��� 	 6#   �� 59       	
 Seats#��� 
 6#   5:      5;      �� 5=      -   5          ClearAll#���  5=      $	��100��5?      $��Right=�5@      $��0,10,0,0q�5A      5B      �� 5D   
   $��1��6   $
��Auto=�6$   .��5E      �� -   5         ��Name$ FlightsPanel��5E      $��10q�6/   5F      5G      5H      