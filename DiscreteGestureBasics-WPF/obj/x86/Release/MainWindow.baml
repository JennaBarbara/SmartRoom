   M S B A M L   `   `   `  ���� E  ASmartSpace, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullo<clr-namespace:Microsoft.Samples.Kinect.DiscreteGestureBasics.Microsoft.Samples.Kinect.DiscreteGestureBasics  ?    9Microsoft.Samples.Kinect.DiscreteGestureBasics.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Nlocal<clr-namespace:Microsoft.Samples.Kinect.DiscreteGestureBasics     -     � Title$  ELEC 490 : SmartSpace��$	��700��$	��750�� � 	Resources    MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� DataTypeF   @Microsoft.Samples.Kinect.DiscreteGestureBasics.GestureResultView � RowDefinitions �� StringFormat��%�  &	        &	      &	 *     (�� ?     *, +)�� $ 	#ff6e6e6e���� $ 	#ff52318f���� $ 	#ff00BCF2���� "  .R�� $
��Auto��$��5q����� *	BodyColor+ �� $
��Auto ��� �� $
��Auto ��� $
��Auto �.+��� $��Stretch=�$��14(�$��Segoe UI)�$��SemiBold"�$��5q����� *	BodyIndex+$ Body Index: {0}���� $��Uniform=�$��1��$��5q����� *ImageSource+�� $��18(�$��Segoe UI)�$��SemiBold"�$��2��$��5 5 0 0q����� *grgr+$ Detected: {0}���� $��18(�$��Segoe UI)�$��SemiBold"�$��2��$��5 0 0 0q����� *
Confidence+$ Confidence: {0}���� $��18(�$��Segoe UI)�$��SemiBold"�$��3��$��5 0 0 0q����� *	Gesture+$ Gesture: {0}��.��� $��	10 0 10 0q� � ColumnDefinitions ��  �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��Right=�$��Bottom=�#��[  $��Segoe UI)�$��18(��� $�ELEC 490���� �� $�G���� $�roup 11��� -    �Name$ contentGrid��$��10,5,-10,37q�$��1�� �� $��1* ��� $��1* ��� $��2* � �� �� �� .+���  �� Orientation$ Vertical=�$��2��$��3��$��
20 10 0 10q�$��Center=�$��Center=�.+�(� -   	 (�Name$	 kinectBodyViewbox��.���� $��UniformToFill=����� *ImageSource+�� $��Kinect Event Detection��$
��Wrap=�$��5 10 5 5q�#��[  $��Segoe UI)�$��14(��� -   
 ��Name$
 CurrentState��$��State��$��2��$
��Left=�$��165.6,67.8,0,0q�$��2��$	��Top=�$	��100��$��36(�$��79���� -   $
 GestureTime��$��Timer��$��2��$
��Left=�$��168.6,151.8,0,-43.6q�$��2��$	��Top=�$��83��$	��183��$��24(��� -   $��	statusBar��$��2��$��Stretch=�$��Bottom=�
�������#��[  $��	0,0,0,0.4q�.@��� ���� *
StatusText+