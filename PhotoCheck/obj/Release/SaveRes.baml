   M S B A M L   `   `   `  ���� E  APhotoCheck, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:PhotoCheck
PhotoCheck      PhotoCheck.SaveRes   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:PhotoCheck       � Title$  SaveRes��$	��768��$
��1366��.���  � ColumnDefinitions �� $��* ��� $	��450 � � RowDefinitions �� $��* �.+��� -   $��SV_WaresList��$��0��$��0��$
��Auto=� K� ZIndex$ 2��.���� -   $��	WaresList������ .R��� 
�������$��0,0,0,1q�._�� $��10,0,0,0q� �� $��* ��� $��* �.+��� $��0��.+���  ��Name$ 	PhotoViev��$	��400��$��Center=�$��Center=����� *	photoPath+��  �� Orientation$ Vertical=�$��1��$��50��.+��� $��0q�$��1��$��24(� �� TextWrapping$
 Wrap=����� *	nameWares+�� $ 
Horizontal=�.+��� $��Код: ��$��24(��� $��0q�$��1��$��24(����� *	kodeWares+�� $ 
Horizontal=�.+��� $��Артикул: ��$��24(��� $��0q�$��1��$��24(����� *Articl+�� -   $/��)Копіювати фото в буфер��   yelowButton#��[  �� -   $/��)Копіювати фото на диск��#��[  �� $��1��.���� .+��� $��1�� �� Padding$ 10q�$��3q�
�������
�������$��0q� �� CornerRadius$ 10��	 J� Effect	 3  -System.Windows.Media.Effects.DropShadowEffect  
   ShadowDepth$
 0Q�   
BlurRadius$ 7Q�   Color$ #bfbfbf��._��� .+��� $��20(�$
��Wrap=�$��Center=�$��Шлях до фото���� $ 
Horizontal=�$��Center=�.+��� -    ��Name$ PathToPhotoTextBox��$��20(�$��30��$	��300���� -   $��2��$	��100��$��40��#��[  .��Змінити�� $��20(�$
��Wrap=�$��Center=�$=��7Шлях до ексель файла з газетки���� $ 
Horizontal=�$��Center=�.+��� -   $ PathToExelTextBox��$��20(�$��30��$	��300���� -   $��2��$	��100��$��40��#��[  .��Змінити�� $ 
Horizontal=�$��5q�$��Center=�$��Center=�.+��� $��20(�Номер колонки:�� -	   $ 
ExcelColum��$��30��$��30��$��20(�$��5���� -
   $��2��$	��150��$��40��$��Знайти фото������  �� BasedOn# [   �� 
TargetType" ��.���  �� Property � {� �� Value .  �� Triggers ��  �� Binding �� *	isExcelOk+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType$ WindowL� �� Value True.a���  � {� . ��  �� *	isExcelOk+ �� $ FindAncestor=�$ WindowL�	 False.a���  � {� .  �� $��1��$ 10q�$��3q�
�������
�������$��0q�$ 10��	   $
 0Q�$ 7Q�$ #bfbfbf��._��� .+��� $ Vertical=�$��Center=�.+��� $��20(�$��Код: ��$��Center=��� -   $ CodeWaresTextBox��$��20(�$��30��$	��300���� $ Vertical=�$��Center=�.+��� $��20(�$��Артикул: ��$��Center=��� -   $ ArtclWaresTextBox��$��20(�$��30��$	��300���� -    ��Name$ NameFindWaresTextBloc��$��20(�$
��Wrap=�$��Center=�$��5q��� $ 
Horizontal=�$��Center=�.+��� -   $��За кодом��$	��150��$��40��#��[  �� -   $��За артикулом��$	��150��$��40��#��[  �� $��1��$ 10q�$��3q�
�������
�������$��0q�$ 10��	   $
 0Q�$ 7Q�$ #bfbfbf��._��� $ Vertical=�$��Center=�.+��� $��20(�$D��>Вкажіть шлях куди копіювати фото: ��$��Center=��� $ 
Horizontal=�$��Center=�.+��� -   $ CopyPhotoPath��$��20(�$��30��$	��300��.��D:\�� -   $��2��$	��100��$��40��#��[  .��Змінити�� $��1��$ 10q�$��3q�
�������
�������$��0q�$ 10��	   $
 0Q�$ 7Q�$ #bfbfbf��._��� .+��� $��20(�$
��Wrap=�$��Center=�$
��Bold"�$<��6Копіювання і зміна назви фото���� $��20(�$
��Wrap=�$��Center=�$A��;Папка де фото названі артикулом:���� $ 
Horizontal=�$��Center=�.+��� -   $ TextBoxActclPath��$��20(�$��30��$	��300���� -   $��2��$	��100��$��40��#��[  .��Змінити�� $��20(�$
��Wrap=�$��Center=�$L��FПапка куди скопіювати і назвати кодом:���� $ 
Horizontal=�$��Center=�.+��� -   $ TextBoxCodePath��$��20(�$��30��$	��300���� -   $��2��$	��100��$��40��#��[  .��Змінити�� -   $��2��$	��200��$��40��$#��Скопіювати фото��  greenButton#��[ 