   M S B A M L   `   `   `  ����N  JRegisterWindow.xaml, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:CashRegisterWPFCashRegisterWPF  5   	   $    CashRegisterWPF.RegisterWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:CashRegisterWPF     5   	     � Title$  Регистрация��5   	   $	��250��6   $	��300��6*   .��5	      �� $��10q�6   .+�5
   
   �� $��Логин:��6   �� 5   
   -   5         ��Name$ txtUsername��5      $��0,0,0,10q�6'   �� 5   
   $��Пароль:��6   G� 5   
   -   5         G�Name$ txtPassword��5      $��0,0,0,10q�6+   �� 5   
   $*��$Подтвердите пароль:��6   G� 5   
   -   5        $ txtConfirmPassword��5      $��0,0,0,10q�62   �� 5   
   $��	Роль:��6   �� 5   
   -   5         ��Name$ cmbRole��5      $��0,0,0,10q�6$   .@�5      �� $��Кассир��6    �� 
IsSelected . 6,   �� 5      $ ��Администратор��6   5      �� 5   
   -   5        $&�� Зарегистрировать��5      $��30��6C   5      5      