   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   MainPgTitle  DepartureCity  ArrivalCity  Swap  RequiredSeats  Date 
 SortBy 	 Price  Baggage 		 Seats 	
 Total  Avalble  Company  ClearAll  BookingWinTitle  BookingTicket 
 Flight  	Departure  Destination  
SeatsCount  
TotalPrice  Book  Buy 
 Cancel  InvalidSeatCount  EnterValidSeatCount  OnlySeatsAvailable  UserNotFound  BookingSuccess  FlightNotFound  
AviaSystem  Account   Help ! AdminSettings " Flights # CurrentUser 
$ Author % Language 
& Welcom ' Registration 	( Login ) Password * RepeatPassword 
+ Client , Administrator - EnterAdminID . Register / 
HasAccount 
0 Error1 
1 Error2 
2 Error3 
3 Error4 4 LogInSystem 5 	NoAccount 6 ErrorLog 7 MyAccountTitle 8 
MyBookings 
9 Status : SeatsReserved ; CancelBooking < 
PayBooking = AddEditBookingWindow_Title > 	UserLabel ? FlightLabel @ BookingDateLabel A StatusLabel B StatusBooked C 
StatusPaid D StatusCancelled E SeatsReservedLabel F 
SaveButton G AddEditFlightWindow_Title H DepartureLabel I DestinationLabel J 	DateLabel K 	TimeLabel L AirlineLabel M 
PriceLabel N SeatsTotalLabel O SeatsAvailableLabel P BaggageInfoLabel Q AddEditUserWindow_Title R 
LoginLabel S PasswordLabel T 	RoleLabel U 	RoleAdmin V 
RoleClient W AdminPanel_Title X UsersTab Y 
FlightsTab Z BookingsTab [ 	AddButton \ 
EditButton ] DeleteButton ^ 
UndoButton _ 
RedoButton ` CloseButton a MyAccountPage_Title b BackToMainButton c Error_SelectUser d Error_SelectFlight e Error_SelectStatus f Error_InvalidSeatsFormat g Error_InvalidTimeFormat h Error_SelectDateTime i Error_InvalidPriceFormat !j Error_InvalidSeatsTotalFormat %k !Error_InvalidSeatsAvailableFormat l Error_SelectRole m PrimaryColor n SecondaryColor o BackgroundColor p 	TextColor q AccentColor r 
ErrorColor s PrimaryBrush  �� Color t SecondaryBrush u BackgroundBrush v 	TextBrush w AccentBrush x 
ErrorBrush �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value&    System.Windows.Controls.DataGrid(  "System.Windows.Controls.DatePicker�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .sys&clr-namespace:System;assembly=mscorlib  5      %�"  &	        &	 !     &	 H     &	 m     &	 �     &	 �     &	 �     &	 �     &	 
    &		 )    &	
 F    &	 c    &	 �    &	 �    &	 �    &	 �    &	 	    &	 (    &	 E    &	 `    &	 �    &	 �    &	 �    &	 �    &	     &	 +    &	 a    &	 �    &	 �    &	 �    &	 *    &	 L    &	  n    &	! �    &	" �    &	# �    &	$     &	% A    &	& a    &	'     &	( �    &	) �    &	* �    &	+ 
    &	, (    &	- M    &	. s    &	/ �    &	0 �    &	1     &	2 G    &	3 �    &	4 �    &	5 �    &	6     &	7 M    &	8 o    &	9 �    &	: �    &	; �    &	< �    &	= 	    &	> 7	    &	? T	    &	@ s	    &	A �	    &	B �	    &	C �	    &	D �	    &	E 
    &	F 9
    &	G U
    &	H |
    &	I �
    &	J �
    &	K �
    &	L �
    &	M     &	N :    &	O ^    &	P �    &	Q �    &	R �    &	S �    &	T     &	U ,    &	V I    &	W g    &	X �    &	Y �    &	Z �    &	[ �    &	\     &	]     &	^ ;    &	_ W    &	` s    &	a �    &	b �    &	c �    &	d     &	e 2    &	f a    &	g �    &	h �    &	i     &	j =    &	k p    &	l �    &	m �    &	n �    &	o     &	p 1    &	q P    &	r o    &	s �    7 m &	t �    7 n &	u �    7 o &	v �    7 p &	w �    7 q &	x     7 r '�     7 u 7 v 'N� �    7 u 7 v '�� [    7 s 7 w 7 t '��     7 v 'G�     7 v '��     7 v '��     7 v '�� �    7 v 'o�     7 s 'm� ~    '       7 v '  �    7 v '	� �    7 v ��5      	Main Page��60   ��5      Departure City:��69   ��5      Arrival City:��65   ��5      Swap��6$   ��5	      Required seats:��68   ��5
      	Date:��6&   ��5      Sort by:��6+   ��5      	Price��6&   ��5      Baggage��6*   ��5      	Seats��6&   ��5      	Total��6&   ��5      	Available��6,   ��5      Airline��6*   ��5      	Clear All��6-   ��5      Booking��62   ��5      Ticket booking��67   ��5      Flight:��6*   ��5      	From:��6+   ��5      To:��6+   ��5      Number of seats:��66   ��5      Total price:��63   ��5      Book��6$   ��5      Buy��6"   ��5      
Cancel��6(   ��5      Invalid seat count��6>   ��5       "Enter a valid number of seats.��6M   ��5!      Only {0} seats available.��6G   ��5"      User not found.��67   ��5#      -)Booking created successfully! Status: {0}��6S   ��5$      Error: Flight not found.��6B   ��5'      
AviaSystem��60   ��5(      
My Account��6-   ��5)      Help��6$   ��5*      Admin Settings��67   ��5+      All available flights:��6:   ��5,      Current User:��66   ��5-      *&Project created by Ugorenko V. R. 2025��6I   ��5.      Language��6,   ��5/      
Welcom��6(   ��52      Registration��64   ��53      
Login:��6(   ��54      	Password:��6-   ��55      Repeat password:��6:   ��56      
Client��6(   ��57      Administrator��66   ��58      Enter AdminID:��67   ��59      Register��6,   ��5:      ,(Already have an account? Click to log in��6N   ��5;      Enter login and password!��6;   ��5<      /+Login is already taken. Choose another one.��6M   ��5=      &"Passwords do not match. Try again.��6D   ��5>      "Access denied. Invalid AdminID��6@   ��5A      Log In System��64   ��5B      ,(Don't have an account? Click to register��6M   ��5C      Invalid login or password��6=   ��5F      
My Account��64   ��5G      My Bookings��61   ��5H      
Status��6(   ��5I      Seats Reserved��67   ��5J      
Cancel��6/   ��5K      Pay��6)   ��5N      Add/Edit Booking��6F   ��5O      	User:��6+   ��5P      Flight:��6/   ��5Q      Booking Date:��69   ��5R      Status:��6/   ��5S      
booked��6.   ��5T      paid��6*   ��5U      	cancelled��64   ��5V      Seats Reserved:��6=   ��5W      Save��6*   ��5Z      Add/Edit Flight��6D   ��5[      
Departure:��65   ��5\      Destination:��68   ��5]      	Date:��6+   ��5^      	Time:��6+   ��5_      Airline:��61   ��5`      
Price:��6-   ��5a      Seats Total:��67   ��5b      Seats Available:��6@   ��5c      Baggage Info:��6:   ��5f      Add/Edit User��6@   ��5g      
Login:��6-   ��5h      	Password:��63   ��5i      	Role:��6+   ��5j      	admin��6*   ��5k      
client��6,   ��5n      Admin Panel��67   ��5o      	Users��6)   ��5p      Flights��6-   ��5q      Bookings��6/   ��5r      Add��6(   ��5s      Edit��6*   ��5t      
Delete��6.   ��5u      Undo��6*   ��5v      Redo��6*   ��5w      	Close��6,   ��5z      
My Account��69   ��5{      Back to Main��68   ��5~      Please select a User.��6A   ��5      Please select a Flight.��6E   ��5�      Please select a Status.��6E   ��5�      "Invalid Seats Reserved Format.��6R   ��5�      1-Invalid Time Format. Please use HH:mm format.��6`   ��5�       Please select Date and Time.��6L   ��5�      Invalid Price Format.��6I   ��5�      Invalid Seats Total Format.��6T   ��5�      #Invalid Seats Available Format.��6\   ��5�      Please select a Role.��6A   ��5�      #333333��6*   ��5�      #555555��6,   ��5�      #f5f5f5��6-   ��5�      #808080��6'   ��5�      #444444��6)   ��5�      #d9534f��6(   �� 5�      8    6+   �� 5�      8    6-   �� 5�      8    6.   �� 5�      8    6(   �� 5�      8    6*   �� 5�      8    6)   �� 5�      " �6   .�5�   
   ��  � ��8   6'   �� 5�   
    � ��8  6'   �� 5�   
    � ��$ Segoe UI)�6'   5�      �� 5�      " N�6   .�5�   
   ��  �@N�
Background8   6'   �� 5�   
    �@N�
Foreground8  6'   �� 5�   
    �@N�
FontFamily$ Segoe UI)�6'   5�      �� 5�      " ��6   .�5�   
   ��  � ��8   6'   �� 5�   
    � ��
 �����6'   �� 5�   
    � ��
 ���� 6(   �� 5�   
    � ��$ 1q�6,   �� 5�   
    � ��$	 8 4q�6$   �� 5�   
    � ��$ 5q�6#   �� 5�   
    � ��$ SemiBold"�6'   �� 5�   
    � �� 5�      �� 5�      " ��6"   .R�5�      �� #��z��6   #��z��5�      #��z��5�      $ 4��5�      ._�5�      �� $��Center=�6+   $��Center=�6H   5�      5�      5�       5�   
   P� 5�       �@J�IsMouseOver$
 True��6-   . �5�      ��  � ��8  6/   5�      P� 5�       � ��$
 True��6+   . �5�      ��  � ��8  6/   5�      5�      �� 5�      " ��6   .�5�   
   ��  � ��
 �����6'   �� 5�   
    � ��8   6'   �� 5�   
    � �� �#cccccc6(   �� 5�   
    � ��$ 1q�6,   �� 5�   
    � ��$ 4q�6$   �� 5�   
    � ��$ 28��6&   5�      �� 5�      " G�6   .�5�   
   ��  � ��
 �����6'   �� 5�   
    � ��8   6'   �� 5�   
    � �� �#cccccc6(   �� 5�   
    � ��$ 1q�6,   �� 5�   
    � ��$ 4q�6$   �� 5�   
    � ��$ 28��6&   5�      �� 5�      " ��6   .�5�   
   ��  � ��
 �����6'   �� 5�   
    � ��8   6'   �� 5�   
    � �� �#cccccc6(   �� 5�   
    � ��$ 1q�6,   �� 5�   
    � ��$ 4q�6$   �� 5�   
    � ��$ 28��6&   5�      �� 5�      " ��6   .�5�   
   ��  � ��8   6'   �� 5�   
    � ��$ SemiBold"�6'   �� 5�   
    � ��$ 0 5 0 0q�6#   5�      �� 5�      " ��6   .�5�   
   ��  � ��8   6'   �� 5�   
    � ��$ 0 5 0 0q�6#   5�      �� 5�      " o�6   .�5�   
   ��  � ��8   6'   �� 5�   
    � ��
 �����6'   5�      �� 5�      " m�6   .�5�   
   ��  � ��
 ���� 6'   �� 5�   
    � ��
 �����6'   �� 5�   
    � ��$
 Bold"�6'   5�      �� 5�      "   6   .�5�   
   ��  � ��
 �����6'   �� 5�   
    � ��8   6'   �� 5�   
    � �� �#cccccc6(   �� 5�   
    � ��$ 1q�6,   �� 5   
    �@  RowBackground
 �����6*   �� 5  
     �@  AlternatingRowBackground �#f9f9f965   �� 5  
    �@  GridLinesVisibility$
 None=�60   �� 5  
    �@  HeadersVisibility$ Column=�6.   �� 5  
   % �@  HorizontalScrollBarVisibility$
 Auto=�6:   5     �� 5     "  6   .�5  
   ��  � ��
 �����6'   �� 5	  
    � ��8   6'   �� 5
  
    � �� �#cccccc6(   �� 5  
    � ��$ 1q�6,   5     �� 5     " 	�6   .�5  
   ��  � ��8   6'   �� 5  
    � ��$ 0 5 0 0q�6#   5     5     