#pragma checksum "..\..\SaveRes.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2B8774FED718567954BAB9D977A5E1367E64B8711CA318AE75C0F35B56B7E587"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PhotoCheck;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PhotoCheck {
    
    
    /// <summary>
    /// SaveRes
    /// </summary>
    public partial class SaveRes : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 22 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer SV_WaresList;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl WaresList;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PathToPhotoTextBox;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PathToExelTextBox;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ExcelColum;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CodeWaresTextBox;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ArtclWaresTextBox;
        
        #line default
        #line hidden
        
        
        #line 135 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock NameFindWaresTextBloc;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CopyPhotoPath;
        
        #line default
        #line hidden
        
        
        #line 179 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBoxActclPath;
        
        #line default
        #line hidden
        
        
        #line 189 "..\..\SaveRes.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextBoxCodePath;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PhotoCheck;component/saveres.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SaveRes.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SV_WaresList = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 2:
            this.WaresList = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 5:
            this.PathToPhotoTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 76 "..\..\SaveRes.xaml"
            this.PathToPhotoTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.PathToPhotoCanged);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 78 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenToFilePath);
            
            #line default
            #line hidden
            return;
            case 7:
            this.PathToExelTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 85 "..\..\SaveRes.xaml"
            this.PathToExelTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ChangeExcelPath);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 87 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenToFilePathExel);
            
            #line default
            #line hidden
            return;
            case 9:
            this.ExcelColum = ((System.Windows.Controls.TextBox)(target));
            
            #line 94 "..\..\SaveRes.xaml"
            this.ExcelColum.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.NumColumChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 98 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.FindPhoto);
            
            #line default
            #line hidden
            return;
            case 11:
            this.CodeWaresTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 12:
            this.ArtclWaresTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 13:
            this.NameFindWaresTextBloc = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            
            #line 139 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.FindPhotoBuCode);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 142 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.FindPhotoByActcl);
            
            #line default
            #line hidden
            return;
            case 16:
            this.CopyPhotoPath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 17:
            
            #line 161 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenToFilePathSavePhoto);
            
            #line default
            #line hidden
            return;
            case 18:
            this.TextBoxActclPath = ((System.Windows.Controls.TextBox)(target));
            
            #line 179 "..\..\SaveRes.xaml"
            this.TextBoxActclPath.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextChangedActclPath);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 182 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClickPhotoPathArtcl);
            
            #line default
            #line hidden
            return;
            case 20:
            this.TextBoxCodePath = ((System.Windows.Controls.TextBox)(target));
            
            #line 189 "..\..\SaveRes.xaml"
            this.TextBoxCodePath.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextChangedCodePath);
            
            #line default
            #line hidden
            return;
            case 21:
            
            #line 192 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClickPhotoPathToCode);
            
            #line default
            #line hidden
            return;
            case 22:
            
            #line 197 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyAndRenamePhoto);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 3:
            
            #line 53 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyPhoto);
            
            #line default
            #line hidden
            break;
            case 4:
            
            #line 55 "..\..\SaveRes.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyPhotoToRepository);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

