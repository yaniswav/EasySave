<Window xmlns="https://github.com/avaloniaui"
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
x:Class="EasySave.FileTypeView"
Width="300" Height="200"
CanResize="False"
Title="Selectionner Fichier ou Dossier">
    <Window.Styles>
    <Style Selector="Button">
    <Setter Property="Background" Value="#857DFF" />
    <Setter Property="Margin" Value="10" />
    <Setter Property="Foreground" Value="White" />
    <Setter Property="FontWeight" Value="Bold" />
    <Setter Property="Width" Value="120" />
    <Setter Property="Height" Value="40" />
    <Setter Property="HorizontalContentAlignment" Value="Center" />
    <Setter Property="VerticalContentAlignment" Value="Center" />
    </Style>
    <Style Selector="Button:pointerover /template/ ContentPresenter#PART_ContentPresenter">
    <Setter Property="Background" Value="#6552d1"/>
    <Setter Property="Foreground" Value="white"/>
    </Style>
        
    </Window.Styles>
    <StackPanel Orientation="Horizontal" HorizontalAlignment="Center" VerticalAlignment="Center">
    <Button x:Name="SelectFolderButton" Content="Select Folder" Cursor="Hand" Click="SelectFolderButton_Click" Margin="10"/>
    <Button x:Name="SelectFileButton" Content="Select File" Cursor="Hand" Click="SelectFileButton_Click" Margin="10"/>
    </StackPanel>
    </Window>