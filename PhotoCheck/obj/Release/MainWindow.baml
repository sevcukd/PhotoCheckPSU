   M S B A M L   `   `   `  ���� E  APhotoCheck, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:PhotoCheck
PhotoCheck      PhotoCheck.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:PhotoCheck       � Title$  
MainWindow��$	��768��$
��1366�� � 	Resources  �� 
TargetType �� Property �� Value �� 
TargetType �� 
Background � Cursor �� Bullet	 �� CornerRadius
 �� Triggers P� Property P� Value �� 
TargetName��%�  '	�       �� " 	�.���  � �� �� " 	�.R��� 
 �����$
 Hand�� � $��20��$��20��.+��� $��
RadioOuter��
������ 
���   �$��2q�$	 2���� $��	RadioMark��$	 0��$��4q�
���  ��$y�Hidden=�._��� $��3,1,0,0q�
���   �$��Calibri)�$��20(��� 
 P�  �@d�	IsChecked$
 trueY�. ��� $ 	RadioMark�� � y�$ Visible=��� $ 
RadioOuter�� � �� �	#FFADADAD.���  � ColumnDefinitions �� $��* ��� $	��450 � � RowDefinitions �� $��* �.+��� -   $��SV_WaresList��$��0��$��0��$
��Auto=� K� ZIndex$ 2��.���� -   $��	WaresList������ .R��� 
�������$��0,0,0,1q�._�� $��10,0,0,0q� �� $��* ��� $��* �.+��� $��0��.+���  ��Name$ 	PhotoViev��$	��400��$��Center=�$��Center=����� *photo+��  �� Orientation$ Vertical=�$��1��$��50��.+��� $��0q�$��1��$��24(�$
��Wrap=����� *	nameWares+�� $ 
Horizontal=�.+��� $��Код: ��$��24(��� $��0q�$��1��$��24(����� *	kodeWares+�� $ 
Horizontal=�.+��� $��Артикул: ��$��24(��� $��0q�$��1��$��24(����� *Articl+	� -   $��20(�$��Залишити фото�� 	� 	GroupName �� *	kodeWares+	� -   $��20(�$��Невірне фото�� �� *	kodeWares+	� -   $��20(�$��Невірний код�� �� *	kodeWares+	� -   $��20(�$��Пропустити�� �� *	kodeWares+� $��0��$��1�� ��  �� $��50 ��� $��* ��� $
��1.3* �.+��� $��18(�$
��Wrap=�$��Center=�$��Center=�$Z��TОберіть групу по якій буде проводитись відбір��$
��Bold"��� $��1�� �� Padding$ 10q�$��3q�
�������
�������$��0q�$	 10�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     ShadowDepth$ 0Q�   
BlurRadius$ 7Q�   Color$ #bfbfbf��._��� $��1��$��0��$
��Auto=�$ 2��.���� -   $��DirectionList������ .R��� $��3q�
�������
�������$��0q�$	 10��   $ 0Q�$ 7Q�$ #bfbfbf��._�� $��10,0,0,0q�.+��� $ Vertical=�.+�	� -   $ radioListDirection��$��18(����� *name+� $��0��$��2��.+��� .+��� $��1��$ 10q�$��3q�
�������
�������$��0q�$	 10��   $ 0Q�$ 7Q�$ #bfbfbf��._��� .+��� $��18(�$
��Wrap=�$��Center=�$
��Bold"�$@��:Оберіть основиний каталог фото:���� $ 
Horizontal=�$��Center=�.+��� -	    ��Name$ PathPhotoTextBox��$��20(�$��30��$	��300���� -
   $��2��$	��100��$��40��   yelowButton#��[  .��Змінити�� -   $��2��$	��150��$��40��#��[  .��Знайти фото�� $��1��$ 10q�$��3q�
�������
�������$��0q�$	 10��   $ 0Q�$ 7Q�$ #bfbfbf��._��� .+��� $��18(�$
��Wrap=�$��Center=�$
��Bold"�$U��OОберіть куди перемістити затверджені фото:���� $ 
Horizontal=�$��Center=�.+��� -   $ PathToPhotoTextBox��$��20(�$��30��$	��300���� -   $��2��$	��100��$��40��#��[  .��Змінити�� -   $��2��$	��150��$��40��  greenButton#��[ .��Підтвердити�� $��1��$ 10q�$��3q�
�������
�������$��0q�$	 10��   $ 0Q�$ 7Q�$ #bfbfbf��._��� .+��� $��18(�$
��Wrap=�$��Center=�$
��Bold"�$��Для дизайнера���� -   #��[ $��Знайти фото��$	��200��$��40��$��10q�